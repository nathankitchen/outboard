namespace Outboard.Api.Resources    
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// An object representing a build.
    /// </summary>
    public class BuildResource
    {
        /// <summary>
        /// A unique identifier for the build, typically the concatenated version and product ID.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The version of the build. This should be unique for any given product.
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// The date and time when the build was completed.
        /// </summary>
        public DateTimeOffset BuildDate { get; set; }

        /// <summary>
        /// A set of release notes associated with this build.
        /// </summary>
        public IList<NoteResource> Changes { get; } = new List<NoteResource>();
    }
}