using System;
using System.Text;

namespace Outboard.Api.Resources    
{
    /// <summary>
    /// An object representing a single named change in a release note.
    /// </summary>
    public static class Resource
    {
        /// <summary>
        /// A regular expression for validating version formats.
        /// </summary>
        public const string VersionRegex = "^[a-z0-9]+(?:[\\.-][a-z0-9]+)*$";
    }
}