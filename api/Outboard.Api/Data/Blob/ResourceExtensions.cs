namespace Outboard.Api.Data.Blob { 

    using System;
    using System.Globalization;
    using System.Linq;
    using Outboard.Api.Resources;

    /// <summary>
    /// Extensions for various resources to generate consistent paths from objects.
    /// </summary>
    public static class ResourceExtensions
    {
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
        /// <param name="product">The product that the build is for.</param>
        /// <param name="build">The build to get a data storage path from.</param>

        /// <returns>A path to store the build data.</returns>
        public static string GetDataPath(string product, string build)
        {
            ArgumentNullException.ThrowIfNull(build, nameof(build));
            ArgumentNullException.ThrowIfNull(product, nameof(product));

            var productSlug = product.ToSlug();

            return $"/{productSlug}/{BuildByIdFolder}/{build}/build.json";
        }

        /// <summary>
        /// Gets the path that should be used to store the build as a chronological date entry.
        /// This is important because we can't rely on alpha sort of build versions to give us
        /// the right chronological order, e.g., 1.25.1 and 1.100.3. So, we store a separate
        /// path for this.
        /// </summary>
        /// <param name="build">The build to get a data storage path from.</param>
        /// <param name="product">The product that the build is for.</param>
        /// <returns>A path to store a date entry for the build.</returns>
        public static string GetDateEntryPath(this BuildResource build, string product)
        {
            ArgumentNullException.ThrowIfNull(build, nameof(build));
            ArgumentNullException.ThrowIfNull(product, nameof(product));

            var productSlug = product.ToSlug();
            var buildDate = build.BuildDateUtc.UtcDateTime.ToString("yyyyMMddhhmmss", CultureInfo.InvariantCulture);
            return $"/{productSlug}/{BuildByDateFolder}/{buildDate}-{build.Version}";
        }

        /// <summary>
        /// Gets the path that should be used to store the release by product and environment.
        /// </summary>
        /// <param name="release">The release that should be recorded.</param>
        /// <returns>A path to store the release data at.</returns>
        public static string GetReleaseHistoryPath(this ReleaseResource release)
        {
            ArgumentNullException.ThrowIfNull(release, nameof(release));

            if (release.Builds.Count == 0) { throw new ArgumentException("Release must contain at least one build."); }

            var productSlug = release.Product.Id;
            var environmentSlug = release.EnvironmentId.ToSlug();

            var build = release.Builds.OrderByDescending(b => b.BuildDateUtc).First();
            var buildSlug = build.Version.ToSlug();
            var buildDate = build.BuildDateUtc.UtcDateTime.ToString("yyyyMMddhhmmss", CultureInfo.InvariantCulture);

            return $"/{productSlug}/environments/{environmentSlug}/{buildDate}-{buildSlug}.json";
        }

        /// <summary>
        /// Gets the path that should be used to store the latest release by product and environment.
        /// </summary>
        /// <param name="release">The release that should be recorded.</param>
        /// <returns>A path to store the release data at.</returns>
        public static string GetReleaseLatestPath(this ReleaseResource release)
        {
            ArgumentNullException.ThrowIfNull(release, nameof(release));

            if (release.Builds.Count == 0) { throw new ArgumentException("Release must contain at least one build."); }

            var productSlug = release.Product.Id;
            var environmentSlug = release.EnvironmentId.ToSlug();

            var build = release.Builds.OrderByDescending(b => b.BuildDateUtc).First();
            var buildSlug = build.Version.ToSlug();
            var buildDate = build.BuildDateUtc.UtcDateTime.ToString("yyyyMMddhhmmss", CultureInfo.InvariantCulture);

            return $"/{productSlug}/environments/{environmentSlug}/latest.json";
        }

        /// <summary>
        /// Gets the path that should be used to store the latest release by product and environment.
        /// </summary>
        /// <param name="release">The release that should be recorded.</param>
        /// <returns>A path to store the release data at.</returns>
        public static string GetBuildReleaseHistoryPath(this ReleaseResource release)
        {
            ArgumentNullException.ThrowIfNull(release, nameof(release));

            if (release.Builds.Count == 0) { throw new ArgumentException("Release must contain at least one build."); }

            var productSlug = release.Product.Id;
            var environmentSlug = release.EnvironmentId.ToSlug();

            var build = release.Builds.OrderByDescending(b => b.BuildDateUtc).First();
            var buildSlug = build.Version.ToSlug();
            var buildDate = build.BuildDateUtc.UtcDateTime.ToString("yyyyMMddhhmmss", CultureInfo.InvariantCulture);
            var deployDate = release.Deployment.DeployDate.UtcDateTime.ToString("yyyyMMddhhmmss", CultureInfo.InvariantCulture);

            return $"/{productSlug}/{BuildByIdFolder}/{buildSlug}/releases/{deployDate}-{environmentSlug}.json";
        }
    }
}
