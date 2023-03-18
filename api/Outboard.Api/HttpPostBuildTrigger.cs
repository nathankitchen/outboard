namespace Outboard.Api
{
    using System;
    using System.IO;
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
    /// requests for details about a a specific environment deployment.
    /// </summary>
    /// <example>
    /// <c>POST /builds/{product}</c>
    /// </example>
    public class HttpPostBuildTrigger : HttpTrigger
    {
        /// <summary>
        /// Creates a new instance of a function to g
        /// </summary>
        public HttpPostBuildTrigger(IConfiguration config, IDataStore dataStore) : base(config, dataStore)
        {
        }

        /// <summary>
        /// Handles requests to create a new build for a given product.
        /// </summary>
        /// <param name="request">Incoming HTTP request details.</param>
        /// <param name="productId">The ID of the product to which this build relates.</param>
        /// <param name="log">An object for recording logs.</param>
        /// <returns>204 if successfully created.</returns>
        [FunctionName("create-build")]
        public async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Admin, "post", Route = "build/{productId}")] HttpRequest request, string productId, ILogger log)
        {
            ArgumentNullException.ThrowIfNull(request, nameof(request));
            ArgumentNullException.ThrowIfNull(productId, nameof(productId));
            ArgumentNullException.ThrowIfNull(log, nameof(log));

            using var stream = new StreamReader(request.Body);
            string payload = await stream.ReadToEndAsync().ConfigureAwait(false);

            var build = JsonConvert.DeserializeObject<BuildResource>(payload);
            
            await this.DataStore.SaveBuild(productId, build).ConfigureAwait(false);

            return Created(build);
        }
    }
}
