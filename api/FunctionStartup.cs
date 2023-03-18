using System;
using System.IO;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Outboard.Api.Data;
using Outboard.Api.Data.Blob;

[assembly: FunctionsStartup(typeof(FunctionApp.Startup))]
namespace FunctionApp
{
    /// <summary>
    /// Function startup to load all services for dependency injection.
    /// </summary>
    public class Startup : FunctionsStartup
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder"></param>
        public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
        {
            _ = builder ?? throw new ArgumentNullException(nameof(builder));

            var context = builder.GetContext();

            builder.ConfigurationBuilder
                .AddJsonFile(Path.Combine(context.ApplicationRootPath, "appsettings.json"), optional: false, reloadOnChange: false)
                .AddJsonFile(Path.Combine(context.ApplicationRootPath, $"appsettings.{context.EnvironmentName}.json"), optional: true, reloadOnChange: false)
                .AddEnvironmentVariables();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder"></param>
        public override void Configure(IFunctionsHostBuilder builder)
        {
            _ = builder ?? throw new ArgumentNullException(nameof(builder));

            builder.Services.AddSingleton<IDataStore>((a) => 
            {
                var config = a.GetRequiredService<IConfiguration>();
                string connection = config.GetConnectionString("outboard-connection-blob");
                string container = config.GetConnectionString("outboard-connection-blob-container");
                return new BlobDataStore(connection, container); 
            });
        }
    }
}