namespace Outboard.Api.Data    
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Outboard.Api.Resources;

    /// <summary>
    /// A data access layer which could in theory be implemented by a range of backing data stores.
    /// </summary>
    public interface IDataStore
    {

        /// <summary>
        /// Gets a build record for the specified product.
        /// </summary>
        /// <param name="productId">The product ID that the build is part of.</param>
        /// <param name="buildVersion">The build to retrieve data for.</param>
        Task<BuildResource> LoadBuild(string productId, string buildVersion);

        /// <summary>
        /// Save a build record for the specified product.
        /// </summary>
        /// <param name="productId">The product ID that the build is part of.</param>
        /// <param name="build">The build resource.</param>
        Task SaveBuild(string productId, BuildResource build);

        /// <summary>
        /// Save a release record. Archives all relevant information as a snapshot
        /// to give a stable history.
        /// </summary>
        /// <param name="release">The release data to save.</param>
        Task SaveRelease(ReleaseResource release);
    }
}