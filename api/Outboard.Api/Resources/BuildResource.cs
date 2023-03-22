namespace Outboard.Api.Resources    
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    
    /// <summary>
    /// An object representing a build.
    /// </summary>
    public class BuildResource
    {
        /// <summary>
        /// The version of the build. This should be unique for any given product.
        /// </summary>
        [Required]
        [StringLength(30)]
        [RegularExpression(Resource.VersionRegex)]
        public string Version { get; set; }

        /// <summary>
        /// The date and time when the build was completed.
        /// </summary>
        [Required]
        public DateTimeOffset BuildDateUtc { get; set; }

        /// <summary>
        /// A set of change notes associated with this build.
        /// </summary>
        [Required]
        [MaxLength(100)]
        public IList<NoteResource> Changes { get; } = new List<NoteResource>();
    }
}