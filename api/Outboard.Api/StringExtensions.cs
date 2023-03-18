namespace Outboard.Api { 

    using System.Text;

    /// <summary>
    /// String extensions.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Converts a string to a slug string which is safe for use as a filename or URL.
        /// </summary>
        /// <param name="value">The value to convert to a slug.</param>
        /// <returns>The specified value but in lowercase, slug format.</returns>
        public static string ToSlug(this string value)
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

            // If the last thing we added was a dash, strip it. Slugs shouldn't
            // end in a dash.
            if (lastCharWasWhitespace)
            {
                bob.Remove(bob.Length - 1, 1);
            }

            return bob.ToString();
        }
    }
}
