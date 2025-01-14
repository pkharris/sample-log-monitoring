﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Sample.Serilog.WebApi.Core.HealthCheck;

public class MyHealthCheck : IHealthCheck
{
    private readonly IMyCustomService dependency;

    public MyHealthCheck(IMyCustomService dependency)
    {
        this.dependency = dependency;
    }

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,CancellationToken cancellationToken = default(CancellationToken))
    {
        var result = dependency.IsHealthy() ? HealthCheckResult.Healthy() : HealthCheckResult.Unhealthy("Error loading dependencies.");
        return Task.FromResult(result);
    }
}

public interface IMyCustomService
{
    public bool IsHealthy();
}

public class MyCustomService : IMyCustomService
{
    public bool IsHealthy()
    {
        return new Random().NextDouble() > 0.6;
    }
}