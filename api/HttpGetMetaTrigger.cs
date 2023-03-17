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
    using Outboard.Api.Blobs;
    using Outboard.Api.Resources;

    /// <summary>
    /// HTTP-based trigger used as an API for Outboard releases.
    /// </summary>
    public class HttpGetMetaTrigger
    {
        /// <summary>
        /// Creates a new instance of a function to g
        /// </summary>
        public HttpGetMetaTrigger(IConfiguration config, IBlobStore blobStore)
        {
            this.BlobStore = blobStore;
            this.Configuration = config;
        }


        private IBlobStore BlobStore { get; init; }
        private IConfiguration Configuration { get; init; }

        /// <summary>
        /// Handles requests to create a new deployment of a given build into an environment.
        /// </summary>
        /// <param name="request">Incoming HTTP request details.</param>
        /// <param name="identity">An identity.</param>
        /// <param name="log">An object for recording logs.</param>
        /// <returns>A JSON payload containing metadata about releases.</returns>
        [FunctionName("read-meta")]
        public async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.User, "get", Route = "meta")] HttpRequest request, ClaimsPrincipal identity, ILogger log)
        {
            ArgumentNullException.ThrowIfNull(request);
            ArgumentNullException.ThrowIfNull(identity);
            ArgumentNullException.ThrowIfNull(log);

            await Task.CompletedTask.ConfigureAwait(true);

            var config = new ConfigResource();
            this.Configuration.GetSection("outboard").Bind(config);

            log.LogInformation($"Getting metadata for {identity?.Identity?.Name} and {config.Environments.Count}");

            if (this.BlobStore == null)
            { 
                log.LogInformation($"No blob");
            }

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

        /// <summary>
        /// Creates a new successful response, returning the requested data.
        /// </summary>
        /// <param name="data">An object represnting a valid data response.</param>
        /// <returns>An HTTP response representing a successful response.</returns>
        public static HttpResponseMessage Success(object data)
        {
            return GetResponse(HttpStatusCode.OK, data);
        }

        /// <summary>
        /// Creates a new Bad Request response, representing an invalid request.
        /// </summary>
        /// <param name="parameter">The name of the invalid parameter.</param>
        /// <param name="message">A descriptive message explaining why the parameter is invalid.</param>
        /// <returns>An HTTP response representing a Bad Request response.</returns>
        public static HttpResponseMessage BadRequest(string parameter, string message)
        {
            return GetResponse(HttpStatusCode.BadRequest, new { Message = message, Parameter = parameter });
        }

        /// <summary>
        /// Gets an HTTP JSON response. 
        /// </summary>
        /// <param name="code">The HTTP status code.</param>
        /// <param name="data">An object representing the data to return as JSON.</param>
        /// <returns>An <c>HttpResponseMessage</c> retpresenting the required status and data.</returns>
        public static HttpResponseMessage GetResponse(HttpStatusCode code, object data)
        {
            ArgumentNullException.ThrowIfNull(data);

            var contractResolver = new DefaultContractResolver { NamingStrategy = new CamelCaseNamingStrategy() };

            var settings = new JsonSerializerSettings
            {
                ContractResolver = contractResolver,
                Formatting = Formatting.Indented
            };

            string payload = JsonConvert.SerializeObject(data, settings);

            return new HttpResponseMessage(code)
            {
                Content = new StringContent(payload, Encoding.UTF8, System.Net.Mime.MediaTypeNames.Application.Json)
            };
        }
    }
}
