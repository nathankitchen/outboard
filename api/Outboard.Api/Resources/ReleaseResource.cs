namespace Outboard.Api.Resources    
{
    using System.Collections.Generic;

    /// <summary>
    /// An object representing a release. This snapshots all the associated resources
    /// to preserve an accurate indication of the whole configuration at the point
    /// of release, making it a stable archive/audit record.
    /// </summary>
    public class ReleaseResource : Resource
    {
        /// <summary>
        /// Gets or sets the ID of the environment that is being released to.
        /// </summary>
        public string EnvironmentId { get; init; }

        /// <summary>
        /// The product that this release relates to.
        /// </summary>
        public ProductResource Product { get; init; }

        /// <summary>
        /// The complete list of environments that this release is on a pathway for.
        /// </summary>
        public IList<EnvironmentResource> Environments { get; init; } = new List<EnvironmentResource>();

        /// <summary>
        /// The list of builds that this release includes.
        /// </summary>
        public IList<BuildResource> Builds { get; init; } = new List<BuildResource>();

        /// <summary>
        /// The deployment that this release relates to.
        /// </summary>
        public DeploymentResource Deployment { get; init; }
    }
}