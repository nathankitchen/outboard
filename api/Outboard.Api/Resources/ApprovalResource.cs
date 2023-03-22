namespace Outboard.Api.Resources    
{
    using System;

    /// <summary>
    /// An object representing an approval, typically to deploy to a specific environment.
    /// </summary>
    public class ApprovalResource
    {
        /// <summary>
        /// A short note to accompany the approval (plaintext).
        /// </summary>
        public string Note { get; set; } = string.Empty;

        /// <summary>
        /// Indicates whether the approval was automated.
        /// </summary>
        /// <value>When <c>true</c> the approval was automated; otherwise, <c>false</c></value>
        public bool Automated { get; set; } = true;

        /// <summary>
        /// The date when the approval was made (UTC).
        /// </summary>
        public DateTimeOffset ApprovalDate { get; set; } = DateTimeOffset.UtcNow;
    }
}