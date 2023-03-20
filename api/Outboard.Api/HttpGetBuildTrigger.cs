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
    /// HTTP-based trigger used as an API for Outboard releases. This function handles
    /// requests for details about a a specific product build.
    /// </summary>
    /// <example>
    /// <c>GET /builds/{product}/{buildId}</c>
    /// </example>
    public class HttpGetBuildTrigger : HttpTrigger
    {
        /// <summary>
        /// Creates a new instance of a function to return details of a build.
        /// </summary>
        public HttpGetBuildTrigger(IConfiguration config, IDataStore dataStore) : base(config, dataStore)
        {
        }

        /// <summary>
        /// Returns a list of builds for a given product, starting with the most recent.
        /// </summary>
        /// <param name="request">Incoming HTTP request details.</param>
        /// <param name="principal">An identity.</param>
        /// <param name="productId">The ID of the product to get the build for.</param>
        /// <param name="buildId">The build ID of the product to get.</param>
        /// <param name="log">An object for recording logs.</param>
        /// <returns>A JSON payload containing metadata about releases.</returns>
        [FunctionName("read-build")]
        public async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.User, "get", Route = "builds/{productId}/{buildId}")] HttpRequest request, ClaimsPrincipal principal, string productId, string buildId, ILogger log)
        {
            ArgumentNullException.ThrowIfNull(request);
            ArgumentNullException.ThrowIfNull(principal);
            ArgumentNullException.ThrowIfNull(log);

            var config = new ConfigResource();
            this.Configuration.GetSection("outboard").Bind(config);

            var product = config.Products.FirstOrDefault(p => p.Id == productId);
            
            if (product == null)
            {
                return NotFound($"Requested product \"{productId}\" could not be found.");
            }

            //if (!product.Roles.Any(r => principal.IsInRole(r)))
            //{
            //    return NotFound($"Requested product \"{productId}\" could not be found.");
            //}

            var build = await this.DataStore.LoadBuild(productId, buildId).ConfigureAwait(false);

            return Success(build);
        }
    }
}
