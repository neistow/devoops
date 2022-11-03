﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Sinks.Elasticsearch;

namespace DemoApp.Shared;

public static class HostBuilderExtensions
{
    public static IHostBuilder UseSerilogWithElastic(this IHostBuilder builder, string applicationName)
    {
        return builder.UseSerilog((ctx, cfg) =>
        {
            var normalizedAppName = applicationName.ToLower().Replace(".", "-");
            var environmentName = ctx.HostingEnvironment.EnvironmentName.ToLower().Replace(".", "-");

            cfg.ReadFrom.Configuration(ctx.Configuration)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.Elasticsearch(
                    new ElasticsearchSinkOptions(new Uri(ctx.Configuration.GetConnectionString("ElasticSearch")))
                    {
                        AutoRegisterTemplate = true,
                        AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv7,
                        IndexFormat = $"{normalizedAppName}-{environmentName}",
                        
                    });
        });
    }
}