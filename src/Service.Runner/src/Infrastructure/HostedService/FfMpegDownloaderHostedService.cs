using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Xabe.FFmpeg;
using Xabe.FFmpeg.Downloader;

namespace Giantnodes.Service.Runner.Infrastructure.HostedService;

internal sealed class FfMpegDownloaderHostedService : IHostedService
{
    private readonly IHostApplicationLifetime _lifetime;
    private readonly ILogger<FfMpegDownloaderHostedService> _logger;

    public FfMpegDownloaderHostedService(
        IHostApplicationLifetime lifetime,
        ILogger<FfMpegDownloaderHostedService> logger)
    {
        _lifetime = lifetime;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            var path = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData, Environment.SpecialFolderOption.Create),
                "ffmpeg");

            _logger.LogInformation("FFmpeg path: {Path}", path);

            FFmpeg.SetExecutablesPath(path);
            await FFmpegDownloader.GetLatestVersion(FFmpegVersion.Official, path);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "failed to initialize FFmpeg executable, stopping application");
            _lifetime.StopApplication();
            throw;
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
