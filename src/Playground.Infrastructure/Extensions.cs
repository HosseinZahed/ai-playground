using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
using Playground.Infrastructure.Options;
using Playground.Infrastructure.Plugins;
using Playground.Infrastructure.Services;

namespace Playground.Infrastructure;

public static class Extensions
{
    public static TBuilder AddSemanticKernelService<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
    {
        // Azure OpenAI configuration
        builder.Services.AddOptions<AzureOpenAIOptions>()
            .Bind(builder.Configuration.GetSection(nameof(AzureOpenAIOptions.SectionName)));
        //.ValidateDataAnnotations()
        //.ValidateOnStart();

        var azureOpenAIOptions = builder.Configuration.GetSection(AzureOpenAIOptions.SectionName).Get<AzureOpenAIOptions>()
            ?? throw new InvalidOperationException($"Missing configuration section '{AzureOpenAIOptions.SectionName}'");

        // Chat completion service that kernels will use
        builder.Services.AddSingleton<IChatCompletionService>(sp =>
        {
            return new AzureOpenAIChatCompletionService(
                azureOpenAIOptions.ChatDeploymentName,
                azureOpenAIOptions.Endpoint,
                azureOpenAIOptions.ApiKey);
        });

        // Add Plug-ins
        builder.Services.AddSingleton<CurrentDateTimePlugin>();

        // Add Kernel
        builder.Services.AddKeyedTransient(Constants.PlaygroundKernel, (sp, key) =>
        {
            // Add ConsolePlugIns
            KernelPluginCollection pluginCollection = [];
            pluginCollection.AddFromObject(sp.GetRequiredService<CurrentDateTimePlugin>());

            // Create the Kernel
            return new Kernel(sp, pluginCollection);
        });

        // Add project services
        builder.Services.AddTransient<IAzureOpenAIService, AzureOpenAIService>();

        return builder;
    }
}
