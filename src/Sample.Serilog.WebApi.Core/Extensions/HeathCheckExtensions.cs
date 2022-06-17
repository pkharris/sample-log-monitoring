using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Sample.Serilog.WebApi.Core.HealthCheck;

namespace Sample.Serilog.WebApi.Core.Extensions;

public static class HeathCheckExtensions
{
    public static void AddHeathCheckApi(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHealthChecks()
            .AddCheck("Situation", () => HealthCheckResult.Unhealthy())
            .AddCheck<MyHealthCheck>("Dependencies")
            .AddSqlServer(
                configuration.GetConnectionString("DefaultConnection"), "SELECT 1;", "Sql Server", HealthStatus.Degraded, timeout: TimeSpan.FromSeconds(30), tags: new[] { "db", "sql", "sqlServer", })
            .AddRedis(
                configuration.GetConnectionString("RedisConnection"), "Redis", HealthStatus.Degraded, new[] { "redis", "cache" })
            .AddElasticsearch(
                configuration.GetConnectionString("Elasticsearch"), "ElasticSearch", HealthStatus.Degraded, new[] { "elastic", "search" });

        services.AddHealthChecksUI(config =>
        {
            config.SetEvaluationTimeInSeconds(5);
            config.AddHealthCheckEndpoint("Host External", ObtainHostNameApiHealthCheck());
            config.AddHealthCheckEndpoint("Application", $"http://sample-serilog:5001/hc");

            config.AddWebhookNotification("Slack Notification WebHook", "Your_Slack_WebHook_Uri_Goes_Here",
                "{\"text\": \"[[LIVENESS]] is failing with the error message : [[FAILURE]]\"}",
                "{\"text\": \"[[LIVENESS]] is recovered.All is up & running !\"}");

        }).AddInMemoryStorage();
    }

    public static void UseHealthCheckApi(this IApplicationBuilder app)
    {
        app.UseHealthChecksUI(config =>
        {
            config.UIPath = "/hc-ui";
        });
    }

    public static string ObtainHostNameApiHealthCheck()
    {
        var tt = Environment.GetEnvironmentVariable("HostNameHealthCheck") == null ? "/api/hc" : $"{Environment.GetEnvironmentVariable("HostNameHealthCheck")}/api/hc";
        return tt;
    }
}