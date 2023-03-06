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
    /// HTTP-based trigger used as an API for Outboard releases.
    /// </summary>
    public static class PostBuildFunction
    {
        /// <summary>
        /// Handles requests to create a new build for a given product.
        /// </summary>
        /// <param name="request">Incoming HTTP request details.</param>
        /// <param name="product">The ID of the product to which this build relates.</param>
        /// <param name="log">An object for recording logs.</param>
        /// <returns>204 if successfully created.</returns>
        [FunctionName("note")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Admin, "post", Route = "build/{product:alpha}")] HttpRequest request, string product, ILogger log)
        {
            ArgumentNullException.ThrowIfNull(request);
            ArgumentNullException.ThrowIfNull(product);
            ArgumentNullException.ThrowIfNull(log);

            await Task.CompletedTask.ConfigureAwait(true);

            log.LogInformation($"Creating a new build for {product}");

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
