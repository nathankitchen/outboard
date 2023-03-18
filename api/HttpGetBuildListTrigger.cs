namespace Outboard.Api
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Security.Claims;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.Http;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using Outboard.Api.Data;
    using Outboard.Api.Resources;

    /// <summary>
    /// HTTP-based trigger used as an API for Outboard releases.
    /// </summary>
    public class HttpGetBuildListTrigger : HttpTrigger
    {
        /// <summary>
        /// Creates a new instance of a function to g
        /// </summary>
        public HttpGetBuildListTrigger(IConfiguration config, IDataStore dataStore) : base(config, dataStore)
        {
        }

        /// <summary>
        /// Handles requests to create a new deployment of a given build into an environment.
        /// </summary>
        /// <param name="request">Incoming HTTP request details.</param>
        /// <param name="identity">An identity.</param>
        /// <param name="log">An object for recording logs.</param>
        /// <returns>A JSON payload containing metadata about releases.</returns>
        [FunctionName("list-builds")]
        public async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.User, "get", Route = "builds")] HttpRequest request, ClaimsPrincipal identity, ILogger log)
        {
            ArgumentNullException.ThrowIfNull(request);
            ArgumentNullException.ThrowIfNull(identity);
            ArgumentNullException.ThrowIfNull(log);

            await Task.CompletedTask.ConfigureAwait(true);

            var config = new ConfigResource();
            this.Configuration.GetSection("outboard").Bind(config);

            log.LogInformation($"Getting metadata for {identity?.Identity?.Name} and {config.Environments.Count}");

            var trimmedConfig = new ConfigResource();

            foreach (var environment in config.Environments.Where(e => e.Roles.Contains("anonymous")))
            {
                trimmedConfig.Environments.Add(environment);
            }

            foreach (var product in config.Products.Where(p => p.Roles.Contains("anonymous")))
            {
                trimmedConfig.Products.Add(product);
            }

            foreach (var pathways in config.Pathways.Where(p => p.Roles.Contains("anonymous")))
            {
                trimmedConfig.Pathways.Add(pathways);
            }
    
            return Success(trimmedConfig);
        }
    }
}
