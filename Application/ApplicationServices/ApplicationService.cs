using System.Text.Json;
using Infrastructure.Database;
using Application.Messaging;
using Application.Models;
using Domain.Entities;
using Infrastructure.Repositories.Applications;
using Infrastructure.Repositories.Users;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;

namespace Application.ApplicationServices;

public class ApplicationService : IApplicationService
{
    private readonly IApplicationRepository _repository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IConfiguration _configuration;
    private readonly TimeProvider _timeProvider;

    public ApplicationService(IApplicationRepository repository, IUserRepository userRepository, IUnitOfWork unitOfWork, IConfiguration configuration,
        TimeProvider timeProvider)
    {
        _repository = repository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _configuration = configuration;
        _timeProvider = timeProvider;
    }

    private DateTime Now => _timeProvider.GetUtcNow().UtcDateTime;

    public async Task<IReadOnlyList<ApplicationResponse>> GetUserApplicationsAsync(string email,
        CancellationToken cancellationToken = default)
    {
        var userId = await GetUserIdAsync(email, cancellationToken);
        var applications = await _repository.GetByUserIdAsync(userId, cancellationToken);
        return applications.Select(ApplicationResponse.From).ToList();
    }

    public async Task<ApplicationResponse> CreateAsync(string email, AddApplicationModel model,
        CancellationToken cancellationToken = default)
    {
        var userId = await GetUserIdAsync(email, cancellationToken);

        var application = LoanApplication.Create(
            userId,
            model.LoanType,
            model.Amount,
            NormalizeCurrency(model.Currency),
            model.Period,
            Now);

        await _repository.AddAsync(application, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApplicationResponse.From(application);
    }

    public async Task<ApplicationResponse> UpdateAsync(string email, int applicationId, UpdateApplicationModel model,
        CancellationToken cancellationToken = default)
    {
        var userId = await GetUserIdAsync(email, cancellationToken);
        var application = await _repository.GetByIdAsync(applicationId, cancellationToken);

        if (application is null || application.UserId != userId)
            throw new KeyNotFoundException($"Application with id {applicationId} was not found.");

        application.Update(model.LoanType, model.Amount, NormalizeCurrency(model.Currency), model.Period, Now);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApplicationResponse.From(application);
    }

    public async Task DeleteAsync(string email, int applicationId, CancellationToken cancellationToken = default)
    {
        var userId = await GetUserIdAsync(email, cancellationToken);
        var application = await _repository.GetByIdAsync(applicationId, cancellationToken);

        if (application is null || application.UserId != userId)
            throw new KeyNotFoundException($"Application with id {applicationId} was not found.");

        application.EnsureCanBeDeleted();
        _repository.Remove(application);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<ApplicationResponse> SendAsync(string email, int applicationId,
        CancellationToken cancellationToken = default)
    {
        var userId = await GetUserIdAsync(email, cancellationToken);
        var application = await _repository.GetByIdAsync(applicationId, cancellationToken);

        if (application is null || application.UserId != userId)
            throw new KeyNotFoundException($"Application with id {applicationId} was not found.");

        application.Send(Now);

        await _unitOfWork.ExecuteInTransactionAsync(async token =>
        {
            await _unitOfWork.SaveChangesAsync(token);
            await PublishToQueueAsync(ApplicationSentMessage.From(application), token);
        }, cancellationToken);

        return ApplicationResponse.From(application);
    }

    private async Task PublishToQueueAsync(ApplicationSentMessage message, CancellationToken cancellationToken)
    {
        var factory = new ConnectionFactory
        {
            HostName = _configuration["RabbitMq:HostName"]!,
            Port = int.Parse(_configuration["RabbitMq:Port"]!),
            UserName = _configuration["RabbitMq:UserName"]!,
            Password = _configuration["RabbitMq:Password"]!
        };

        var exchange = _configuration["RabbitMq:Exchange"]!;
        var queue = _configuration["RabbitMq:Queue"]!;
        var routingKey = _configuration["RabbitMq:RoutingKey"]!;

        await using var connection = await factory.CreateConnectionAsync(cancellationToken);

        await using var channel = await connection.CreateChannelAsync(
            new CreateChannelOptions(publisherConfirmationsEnabled: true, publisherConfirmationTrackingEnabled: true),
            cancellationToken);

        await channel.ExchangeDeclareAsync(exchange, ExchangeType.Direct, durable: true, cancellationToken: cancellationToken);
        await channel.QueueDeclareAsync(queue, durable: true, exclusive: false, autoDelete: false, cancellationToken: cancellationToken);
        await channel.QueueBindAsync(queue, exchange, routingKey, cancellationToken: cancellationToken);

        var body = JsonSerializer.SerializeToUtf8Bytes(message);
        var properties = new BasicProperties { Persistent = true, ContentType = "application/json" };

        await channel.BasicPublishAsync(exchange, routingKey, true, properties, body, cancellationToken);
    }

    private async Task<int> GetUserIdAsync(string email, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(email, cancellationToken)
                   ?? throw new UnauthorizedAccessException("User not found.");

        return user.Id;
    }

    private static string NormalizeCurrency(string currency) => currency.Trim().ToUpperInvariant();
}
