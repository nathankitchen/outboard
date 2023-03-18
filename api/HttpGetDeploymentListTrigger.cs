namespace Outboard.Api
{
    using System;
    using System.Linq;
    using System.Net.Http;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.Http;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using Outboard.Api.Data;
    using Outboard.Api.Resources;

    /// <summary>
    /// HTTP-based trigger used as an API for Outboard releases.
    /// </summary>
    public class HttpGetDeploymentListTrigger : HttpTrigger
    {
        /// <summary>
        /// Creates a new instance of a function to get a list of deployments
        /// to a specific environment.
        /// </summary>
        public HttpGetDeploymentListTrigger(IConfiguration config, IDataStore dataStore) : base(config, dataStore)
        {
        }

        /// <summary>
        /// Handles requests to create a new deployment of a given build into an environment.
        /// </summary>
        /// <param name="request">Incoming HTTP request details.</param>
        /// <param name="principal">A principal with the identity of the user.</param>
        /// <param name="environmentId">The ID of an environment.</param>
        /// <param name="productId">The ID of the product to check deployments of.</param>
        /// <param name="log">An object for recording logs.</param>
        /// <returns>A JSON payload containing metadata about releases.</returns>
        [FunctionName("list-deployments")]
        public async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.User, "get", Route = "deployments/{environmentId}/{productId}")] HttpRequest request, ClaimsPrincipal principal, string environmentId, string productId, ILogger log)
        {
            ArgumentNullException.ThrowIfNull(request, nameof(request));
            ArgumentNullException.ThrowIfNull(principal, nameof(principal));
            ArgumentNullException.ThrowIfNull(environmentId, nameof(environmentId));
            ArgumentNullException.ThrowIfNull(productId, nameof(productId));
            ArgumentNullException.ThrowIfNull(log, nameof(log));

            await Task.CompletedTask.ConfigureAwait(true);

            var config = new ConfigResource();
            this.Configuration.GetSection("outboard").Bind(config);

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
