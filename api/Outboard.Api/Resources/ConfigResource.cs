namespace Outboard.Api.Resources    
{
    using System.Collections.Generic;

    /// <summary>
    /// An object representing a configuration.
    /// </summary>
    public class ConfigResource
    {
        /// <summary>
        /// A list of all the products that can be managed by Outboard.
        /// </summary>
        public IList<ProductResource> Products { get; } = new List<ProductResource>();

        /// <summary>
        /// A list of all the environments that can be managed by Outboard.
        /// </summary>
        public IList<EnvironmentResource> Environments { get; } = new List<EnvironmentResource>();
    }
}