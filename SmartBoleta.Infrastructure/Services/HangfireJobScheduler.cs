using Hangfire;
using SmartBoleta.Domain.Abstractions;
using SmartBoleta.Infrastructure.BackgroundJobs;

namespace SmartBoleta.Infrastructure.Services;

internal sealed class HangfireJobScheduler : IJobScheduler
{
    private readonly IBackgroundJobClient _backgroundJobClient;

    public HangfireJobScheduler(IBackgroundJobClient backgroundJobClient)
    {
        _backgroundJobClient = backgroundJobClient;
    }

    public void EnqueueOcrJob(Guid boletaId)
        => _backgroundJobClient.Enqueue<OcrBackgroundJob>(job => job.ProcesarAsync(boletaId));
}
