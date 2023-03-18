namespace Outboard.Api.Resources    
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// An object representing a single named change in a release note.
    /// </summary>
    public class EnvironmentResource : Resource
    {
        private string _id = string.Empty;

        /// <summary>
        /// A unique identifier for the environment. Should be in slug format.
        /// </summary>
        public string Id
        {
            get => _id;
            set => _id = value?.ToSlug();
        }

        /// <summary>
        /// A short name for the product (plaintext).
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// A short description for the environment (plaintext).
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// A list of the roles who are allowed to see the details of this environment.
        /// </summary>
        public IList<string> Roles { get; } = new List<string>();

        /// <summary>
        /// A link to the environment.
        /// </summary>
        public string Link { get; set; } = string.Empty;
    }
}