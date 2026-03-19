using RateLimiter.Reader.Domain.Sevices;

namespace ReaderService.Domain.Services;

public sealed class ReaderBootstrapper : IHostedService
{
    private readonly IRateLimitReader _reader;

    public ReaderBootstrapper(IRateLimitReader reader)
    {
        _reader = reader;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _reader.StartAsync();
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}