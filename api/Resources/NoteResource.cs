namespace Outboard.Api.Resources    
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// An object representing a single named change in a release note.
    /// </summary>
    public class NoteResource : Resource
    {
        private string _type = string.Empty;

        /// <summary>
        /// A unique identifier for the change, typically an ID from a work tracking system.
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// A short title of the change (plaintext).
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// A long description of the change (plaintext).
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// A longer description of the change in HTML format. This is designed to
        /// support "user guide" type long explanations of a system change.
        /// </summary>
        public string SupportingHtml { get; set; } = string.Empty;

        /// <summary>
        /// Indicates whether to highlight this change note.
        /// </summary>
        /// <value>When <c>true</c> the note should be highlighted; otherwise, <c>false</c></value>
        public bool Highlight { get; set; }

        /// <summary>
        /// A single word categorising the change as a type. Dashes are allowed.
        /// The string will be automatically converted if the wrong value is used.
        /// </summary>
        /// <example>bug</example>
        /// <example>change-request</example>
        /// <example>feature</example>
        public string Type
        {
            get => _type;
            set => _type = ToSlug(value);
        }
    }
}