namespace Outboard.Api.Resources    
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// An object representing a single named change in a release note.
    /// </summary>
    public class DeploymentResource : Resource
    {
        private string _id = string.Empty;

        /// <summary>
        /// A unique identifier for the environment. Should be in slug format.
        /// </summary>
        public string Id
        {
            get => _id;
            set => _id = ToSlug(value);
        }

        /// <summary>
        /// The environment that the build was deployed to.
        /// </summary>
        public EnvironmentResource Environment { get; set; }

        /// <summary>
        /// The build that was deployed.
        /// </summary>
        public BuildResource Build { get; set; }

        /// <summary>
        /// The date that the build was deployed.
        /// </summary>
        public DateTimeOffset DeployDate { get; set; }

        /// <summary>
        /// The approval associated with the deployment.
        /// </summary>
        public ApprovalResource Approval { get; set; } = new ApprovalResource();
    }
}