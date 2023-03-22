namespace Outboard.Api.Resources    
{
    using System;

    /// <summary>
    /// An object representing a single named change in a release note.
    /// </summary>
    public class DeploymentResource
    {
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