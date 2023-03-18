namespace Outboard.Api
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.Http;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    /// <summary>
    /// HTTP-based trigger used as an API for Outboard releases. This function handles
    /// requests for creating a new deployment of a given product build.
    /// </summary>
    /// <example>
    /// <c>POST deployments/{buildId}/{pathwayId}/{environmentId}</c>
    public class HttpPostDeploymentTrigger : HttpTrigger
    {
        /// <summary>
        /// Handles requests to create a new deployment of a given build into an environment.
        /// </summary>
        /// <param name="request">Incoming HTTP request details.</param>
        /// <param name="build">The ID of the build which is being deployed.</param>
        /// <param name="pathway">The ID of the pathway upon which this build is proceeding.</param>
        /// <param name="environment">The ID of the environment to which this build is deployed.</param>
        /// <param name="log">An object for recording logs.</param>
        /// <returns>204 if successfully created.</returns>
        [FunctionName("create-deployment")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Admin, "post", Route = "deployments/{buildId}/{pathwayId}/{environmentId}")] HttpRequest request, string build, string pathway, string environment, ILogger log)
        {
            ArgumentNullException.ThrowIfNull(request);
            ArgumentNullException.ThrowIfNull(build);
            ArgumentNullException.ThrowIfNull(pathway);
            ArgumentNullException.ThrowIfNull(environment);
            ArgumentNullException.ThrowIfNull(log);

            var utc = DateTimeOffset.UtcNow;

            log.LogInformation($"Creating a new deployment for {build} into {environment}");

            await Task.CompletedTask.ConfigureAwait(true);

            return Success("Ok");
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