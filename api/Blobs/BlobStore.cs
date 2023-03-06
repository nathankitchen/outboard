namespace Outboard.Api.Blobs    
{
    using Outboard.Api.Resources;

    /// <summary>
    /// An object representing a single named change in a release note.
    /// </summary>
    public class BlobStore : IBlobStore
    {
        private string _connection = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="product"></param>
        /// <param name="build"></param>
        public void SaveBuild(ProductResource product, BuildResource build)
        {
            this._connection = "a";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="deployment"></param>
        public void SaveDeployment(DeploymentResource deployment)
        {
            this._connection = "a";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="buildId"></param>
        public void GetBuild(string productId, string buildId)
        {
            this._connection = "a";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="productId"></param>
        public void GetBuildHistory(string productId)
        {
            this._connection = "a";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="environmentId"></param>
        public void GetDeploymentHistory(string productId, string environmentId)
        {
            this._connection = "a";
        }
    }
}