namespace Outboard.Api.Data    
{
    using System.Threading.Tasks;
    using Outboard.Api.Resources;

    /// <summary>
    /// A data access layer which could in theory be implemented by a range of backing data stores.
    /// </summary>
    public interface IDataStore
    {
        /// <summary>
        /// Save a build record for the specified product.
        /// </summary>
        /// <param name="productId">The product ID that the build is part of.</param>
        /// <param name="build">The build resource.</param>
        Task SaveBuild(string productId, BuildResource build);
    }
}