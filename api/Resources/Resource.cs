using System;
using System.Text;

namespace Outboard.Api.Resources    
{
    /// <summary>
    /// An object representing a single named change in a release note.
    /// </summary>
    public class Resource
    {
        /// <summary>
        /// Converts a string to a slug string which is safe for use as a filename or URL.
        /// </summary>
        /// <param name="value">The value to convert to a slug.</param>
        /// <returns>The specified value but in lowercase, slug format.</returns>
        protected static string ToSlug(string value)
        {
            if (value == null) { return null; }

            var bob = new StringBuilder();
            bool lastCharWasWhitespace = true;

            foreach (char c in value)
            {
                if (char.IsWhiteSpace(c) || char.IsPunctuation(c))
                {
                    if (!lastCharWasWhitespace)
                    {
                        bob.Append('-');
                    }
                    lastCharWasWhitespace = true;
                }
                else if (char.IsLetterOrDigit(c))
                {
                    bob.Append(char.ToLowerInvariant(c));
                    lastCharWasWhitespace = false;
                }
            }

            return bob.ToString();
        }
    }
}