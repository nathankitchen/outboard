namespace Outboard.Api.Resources    
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// An object representing a single named change in a release note.
    /// </summary>
    public class ProductResource : Resource
    {
        private string _id = string.Empty;

        /// <summary>
        /// A unique identifier for the product. Should be in slug format.
        /// </summary>
        public string Id
        {
            get => _id;
            set => _id = ToSlug(value);
        }

        /// <summary>
        /// A short name for the product family (plaintext).
        /// </summary>
        public string Family { get; set; } = string.Empty;

        /// <summary>
        /// A short name for the product (plaintext).
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// A longer description of the product (plaintext).
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// A list of the roles who are allowed to see the details of this product.
        /// </summary>
        public IList<string> Roles { get; } = new List<string>();
    }
}