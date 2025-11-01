using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Trace;


var builder = DistributedApplication.CreateBuilder(args);

// Add SQL Server resource and db 
var password = builder.AddParameter("sql-password", secret: true);
var sqlServer = builder.AddSqlServer("projecttemplate-db").WithLifetime(ContainerLifetime.Persistent);
var sqlServerIdentity = builder.AddSqlServer("projecttemplate-identity-db").WithLifetime(ContainerLifetime.Persistent);

    
var messageQueue = builder
    .AddRedis("projecttemplate-mq")
    .WithRedisInsight()
    .WithLifetime(ContainerLifetime.Persistent);

builder.Services.AddOpenTelemetry()
    .WithTracing(tracerProviderBuilder =>
    {
        tracerProviderBuilder
            .AddAspNetCoreInstrumentation()
            .AddEntityFrameworkCoreInstrumentation(options =>
            {
                options.SetDbStatementForText = true;
            })
            .AddOtlpExporter(); // Aspire dashboard listens on OTLP by default
    });


if (builder.ExecutionContext.IsRunMode)
{
    // Data volumes don't work on ACA for only add when running
    sqlServer.WithDataVolume();
    // rabbitmq.WithDataVolume();  
}

var api = builder.AddProject<Projects.ProjectTemplate_Api>("projecttemplate-api")
     .WithReference(sqlServer)
     .WaitFor(sqlServer)
     .WithReference(sqlServerIdentity)
     .WaitFor(sqlServerIdentity)
     .WithReference(messageQueue)
     .WaitFor(messageQueue);


builder.Build().Run();