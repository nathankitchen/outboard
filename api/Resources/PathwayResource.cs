namespace Outboard.Api.Resources    
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// An object representing a single named change in a release note.
    /// </summary>
    public class PathwayResource : Resource
    {
        private string _id = string.Empty;

        /// <summary>
        /// A unique identifier for the product. Should be in slug format.
        /// </summary>
        public string Id
        {
            get => _id;
            set => _id = value?.ToSlug();
        }

        /// <summary>
        /// A short name for the pathway (plaintext).
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// A longer description for the pathway (plaintext).
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// The ID of the product to which this pathway pertains.
        /// </summary>
        public string ProductId { get; set; } = string.Empty;

        /// <summary>
        /// A list of the roles who are allowed to see the details of this pathway.
        /// </summary>
        public IList<string> Roles { get; } = new List<string>();

        /// <summary>
        /// A list of sequential environment IDs on the way to production.
        /// </summary>
        public IList<string> Environments { get; } = new List<string>();
    }
}