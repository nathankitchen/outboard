namespace Outboard.Api
{
    using System;
    using System.Linq;
    using System.Net.Http;
    using System.Security.Claims;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.Http;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using Outboard.Api.Data;
    using Outboard.Api.Resources;

    /// <summary>
    /// HTTP-based trigger used as an API for Outboard releases. This function handles
    /// requests for metadata about the current Outboard configuration, namely the environments,
    /// and products that the current user has access to.
    /// </summary>
    /// <example>
    /// <c>GET /meta</c>
    /// </example>
    public class HttpGetMetaTrigger : HttpTrigger
    {
        /// <summary>
        /// Creates a new instance of a function able to return metadata.
        /// </summary>
        public HttpGetMetaTrigger(IConfiguration config, IDataStore dataStore) : base(config, dataStore)
        {
        }

        /// <summary>
        /// Handles requests to create a new deployment of a given build into an environment.
        /// </summary>
        /// <param name="request">Incoming HTTP request details.</param>
        /// <param name="principal">A claims principal.</param>
        /// <param name="log">An object for recording logs.</param>
        /// <returns>A JSON payload containing metadata about releases.</returns>
        [FunctionName("read-meta")]
        public HttpResponseMessage Run(
            [HttpTrigger(AuthorizationLevel.User, "get", Route = "meta")] HttpRequest request, ClaimsPrincipal principal, ILogger log)
        {
            ArgumentNullException.ThrowIfNull(request);
            ArgumentNullException.ThrowIfNull(principal);
            ArgumentNullException.ThrowIfNull(log);

            var config = new ConfigResource();
            this.Configuration.GetSection("outboard").Bind(config);

            log.LogInformation($"Getting metadata for {principal?.Identity?.Name} and {config.Environments.Count}");

            var trimmedConfig = new ConfigResource();

            foreach (var environment in config.Environments.Where(e => e.Roles.Contains("anonymous")))
            {
                trimmedConfig.Environments.Add(environment);
            }

            foreach (var product in config.Products.Where(p => p.Roles.Contains("anonymous")))
            {
                trimmedConfig.Products.Add(product);
            }

            var metaResource = new
            {
                Identity = principal?.Identity?.Name ?? "anonymous",
                Roles = Array.Empty<string>(),
                Config = trimmedConfig
            };

            return Success(metaResource);
        }
    }
}
