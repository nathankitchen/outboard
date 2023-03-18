namespace Outboard.Api
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.Http;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using Outboard.Api.Data;

    /// <summary>
    /// HTTP-based trigger used as an API for Outboard releases. This function handles
    /// requests for creating a new deployment of a given product build.
    /// </summary>
    /// <example>
    /// <c>POST deployments/{buildId}/{environmentId}</c>
    /// </example>
    public class HttpPostDeploymentTrigger : HttpTrigger
    {

        /// <summary>
        /// Creates a new instance of a function to store information about a deployment to a specific
        /// environment as part of a deployment pathway.
        /// </summary>
        public HttpPostDeploymentTrigger(IConfiguration config, IDataStore dataStore) : base(config, dataStore)
        {
        }
        /// <summary>
        /// Handles requests to create a new deployment of a given build into an environment.
        /// </summary>
        /// <param name="request">Incoming HTTP request details.</param>
        /// <param name="buildId">The ID of the build which is being deployed.</param>
        /// <param name="environmentId">The ID of the environment to which this build is deployed.</param>
        /// <param name="log">An object for recording logs.</param>
        /// <returns>204 if successfully created.</returns>
        [FunctionName("create-deployment")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Admin, "post", Route = "deployments/{buildId}/{environmentId}")] HttpRequest request, string buildId, string environmentId, ILogger log)
        {
            ArgumentNullException.ThrowIfNull(request);
            ArgumentNullException.ThrowIfNull(buildId);
            ArgumentNullException.ThrowIfNull(environmentId);
            ArgumentNullException.ThrowIfNull(log);

            var utc = DateTimeOffset.UtcNow;

            log.LogInformation($"Creating a new deployment for {buildId} into {environmentId}");

            await Task.CompletedTask.ConfigureAwait(true);

            return Created("Ok");
        }
    }
}