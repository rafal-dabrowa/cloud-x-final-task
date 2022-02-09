using System;
using Azure.Identity;
using Functions.Configurations;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(Functions.Startup))]
namespace Functions;

internal class Startup : FunctionsStartup
{
    public override void Configure(IFunctionsHostBuilder builder)
    {
        var functionConfigSection = builder.GetContext().Configuration.GetSection(CosmosConfiguration.CONFIG_NAME);
        builder.Services.Configure<CosmosConfiguration>(functionConfigSection);

        builder.Services.AddOptions<CosmosSecretConfiguration>().Configure(option => {
            option.PrimaryKey = builder.GetContext().Configuration.GetValue<string>("CosmosPrimaryKey");
        });

        builder.Services.AddOptions<StorageConfiguration>().Configure(option =>
        {
            option.StorageAccountConnectionString = builder.GetContext().Configuration.GetValue<string>("StorageAccountConnectionString");
            option.StorageAccountContainerName = builder.GetContext().Configuration.GetValue<string>("StorageAccountContainerName");
        });
    }

    public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
    {
        builder.ConfigurationBuilder
            .SetBasePath(Environment.CurrentDirectory)
            .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true);
        var builtConfig = builder.ConfigurationBuilder.Build();
        var keyVaultEndpoint = builtConfig["KeyVaultUri"];

        builder.ConfigurationBuilder
                .SetBasePath(Environment.CurrentDirectory)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddAzureKeyVault(new Uri(keyVaultEndpoint), new DefaultAzureCredential())
                .AddEnvironmentVariables()
            .Build();
    }
}
