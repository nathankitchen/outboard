namespace Outboard.Api.Config
{
    using System;
    using System.Collections.Generic;
    using Outboard.Api.Resources;

    /// <summary>
    /// An object representing a posted release note.
    /// </summary>
    public class OutboardConfig
    {
        /// <summary>
        /// A unique identifier for the build.
        /// </summary>
        public IEnumerable<ProductResource> Products { get; set; } = new List<ProductResource>();
    }
}