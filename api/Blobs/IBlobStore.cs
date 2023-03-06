namespace Outboard.Api.Blobs    
{
    using Outboard.Api.Resources;

    /// <summary>
    /// An object representing a single named change in a release note.
    /// </summary>
    public interface IBlobStore
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="product"></param>
        /// <param name="build"></param>
        void SaveBuild(ProductResource product, BuildResource build);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="deployment"></param>
        void SaveDeployment(DeploymentResource deployment);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="buildId"></param>
        void GetBuild(string productId, string buildId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="productId"></param>
        void GetBuildHistory(string productId);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="environmentId"></param>
        void GetDeploymentHistory(string productId, string environmentId);
    }
}