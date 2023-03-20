namespace Outboard.Api
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.Http;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using Outboard.Api.Data;
    using Outboard.Api.Resources;

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
        /// <param name="productId">The ID of the product which is being deployed.</param>
        /// <param name="buildId">The ID of the build which is being deployed.</param>
        /// <param name="environmentId">The ID of the environment to which this build is deployed.</param>
        /// <param name="log">An object for recording logs.</param>
        /// <returns>204 if successfully created.</returns>
        [FunctionName("create-deployment")]
        public async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Admin, "post", Route = "deployments/{productId}/{buildId}/{environmentId}")] HttpRequest request, string productId, string buildId, string environmentId, ILogger log)
        {
            ArgumentNullException.ThrowIfNull(request);
            ArgumentNullException.ThrowIfNull(buildId);
            ArgumentNullException.ThrowIfNull(environmentId);
            ArgumentNullException.ThrowIfNull(log);

            var config = new ConfigResource();
            this.Configuration.GetSection("outboard").Bind(config);

            using var stream = new StreamReader(request.Body);
            string payload = await stream.ReadToEndAsync().ConfigureAwait(false);

            var deployment = JsonConvert.DeserializeObject<DeploymentResource>(payload);

            log.LogInformation($"Creating a new deployment for {productId} build {buildId} into {environmentId}");

            var build = await this.DataStore.LoadBuild(productId, buildId).ConfigureAwait(false);

            var product = config.Products.FirstOrDefault(p => p.Id == productId);

            var environments = config.Environments.Where(e => product.EnvironmentSequence.Contains(e.Id));

            var release = new ReleaseResource()
            {
                Builds = { build },
                Deployment = deployment,
                EnvironmentId = environmentId,
                Environments = environments.ToList(),
                Product = product
            };

            await this.DataStore.SaveRelease(release).ConfigureAwait(false);

            return Created("Ok");
        }
    }
}