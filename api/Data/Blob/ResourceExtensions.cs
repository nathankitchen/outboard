namespace Outboard.Api.Data.Blob { 

    using System;
    using System.Globalization;
    using Outboard.Api.Resources;

    /// <summary>
    /// Extensions for various resources to generate consistent paths from objects.
    /// </summary>
    public static class ResourceExtensions
    {
        /// <summary>
        /// The conventional name for the folder where product information is stored.
        /// </summary>
        public const string ProductFolder = "products";

        /// <summary>
        /// The conventional name for the folder where build resources are stored by date.
        /// </summary>
        public const string BuildByDateFolder = "dates";

        /// <summary>
        /// The conventional name for the folder where build resources are stored by date.
        /// </summary>
        public const string BuildByIdFolder = "builds";

        /// <summary>
        /// Gets the path that should be used to store the data about the build.
        /// </summary>
        /// <param name="build">The build to get a data storage path from.</param>
        /// <param name="product">The product that the build is for.</param>
        /// <returns>The specified value but in lowercase, slug format.</returns>
        public static string GetDataPath(this BuildResource build, string product)
        {
            ArgumentNullException.ThrowIfNull(build, nameof(build));
            ArgumentNullException.ThrowIfNull(product, nameof(product));

            var productSlug = product.ToSlug();
            var buildSlug = build.Id.ToSlug();

            return $"/{ProductFolder}/{productSlug}/{BuildByIdFolder}/{buildSlug}.json";
        }

        /// <summary>
        /// Gets the path that should be used to store the build as a chronological date entry.
        /// This is important because we can't rely on alpha sort of build versions to give us
        /// the right chronological order, e.g., 1.25.1 and 1.100.3. So, we store a separate
        /// path for this.
        /// </summary>
        /// <param name="build">The build to get a data storage path from.</param>
        /// <param name="product">The product that the build is for.</param>
        /// <returns>The specified value but in lowercase, slug format.</returns>
        public static string GetDateEntryPath(this BuildResource build, string product)
        {
            ArgumentNullException.ThrowIfNull(build, nameof(build));
            ArgumentNullException.ThrowIfNull(product, nameof(product));

            var productSlug = product.ToSlug();
            var buildSlug = build.Id.ToSlug();
            var buildDate = build.BuildDate.UtcDateTime.ToString("yyyyMMddhhmmss", CultureInfo.InvariantCulture);
            return $"/{ProductFolder}/{productSlug}/{BuildByDateFolder}/{buildDate}-{buildSlug}.txt";
        }
    }
}
