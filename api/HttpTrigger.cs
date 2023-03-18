namespace Outboard.Api
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using Microsoft.Extensions.Configuration;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using Outboard.Api.Data;

    /// <summary>
    /// HTTP-based trigger used as an API for Outboard releases. This is an abstract class to
    /// do some of the foundational heavy-lifting required to return HTTP responses.
    /// </summary>
    public abstract class HttpTrigger
    {
        /// <summary>
        /// Creates a new instance of a function with config and blob store access.
        /// </summary>
        protected HttpTrigger(IConfiguration config, IDataStore dataStore)
        {
            this.DataStore = dataStore;
            this.Configuration = config;
        }

        /// <summary>
        /// A data storage capability.
        /// </summary>
        protected IDataStore DataStore { get; init; }

        /// <summary>
        /// Access to the function configuration.
        /// </summary>
        protected IConfiguration Configuration { get; init; }

        /// <summary>
        /// Creates a new successful response, returning the requested data.
        /// </summary>
        /// <param name="data">An object represnting a valid data response.</param>
        /// <returns>An HTTP response representing a successful response.</returns>
        protected static HttpResponseMessage Success(object data)
        {
            return GetResponse(HttpStatusCode.OK, data);
        }

        /// <summary>
        /// Creates a new Bad Request response, representing an invalid request.
        /// </summary>
        /// <param name="parameter">The name of the invalid parameter.</param>
        /// <param name="message">A descriptive message explaining why the parameter is invalid.</param>
        /// <returns>An HTTP response representing a Bad Request response.</returns>
        protected static HttpResponseMessage BadRequest(string parameter, string message)
        {
            return GetResponse(HttpStatusCode.BadRequest, new { Message = message, Parameter = parameter });
        }

        /// <summary>
        /// Creates a new Failure response, representing a server-side failure.
        /// </summary>
        /// <param name="message">A descriptive message explaining why the parameter is invalid.</param>
        /// <returns>An HTTP response representing a Bad Request response.</returns>
        protected static HttpResponseMessage Failure(string message)
        {
            return GetResponse(HttpStatusCode.InternalServerError, new { Message = message });
        }

        /// <summary>
        /// Creates a new response representing a resource which could not be found.
        /// </summary>
        /// <param name="message">A descriptive message explaining which resource could not be found.</param>
        /// <returns>An HTTP response representing a Not Found response.</returns>
        protected static HttpResponseMessage NotFound(string message)
        {
            return GetResponse(HttpStatusCode.NotFound, new { Message = message });
        }

        /// <summary>
        /// Gets an HTTP JSON response. 
        /// </summary>
        /// <param name="code">The HTTP status code.</param>
        /// <param name="data">An object representing the data to return as JSON.</param>
        /// <returns>An <c>HttpResponseMessage</c> retpresenting the required status and data.</returns>
        protected static HttpResponseMessage GetResponse(HttpStatusCode code, object data)
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
