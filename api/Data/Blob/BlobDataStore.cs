namespace Outboard.Api.Data.Blob
{
    using System;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;
    using Azure.Core;
    using Azure.Identity;
    using Azure.Storage.Blobs;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using Outboard.Api.Resources;

    /// <summary>
    /// An object representing a single named change in a release note.
    /// </summary>
    public class BlobDataStore : IDataStore
    {
        /// <summary>
        /// Creates a new instance of an <see cref="IDataStore" /> which uses an Azure
        /// Blob store (a named container) to persist and retrieve data. This constructor
        /// is designed to connect with traditional shared key connection strings.
        /// </summary>
        /// <param name="connection">The connection string of the service.</param>
        /// <param name="container">The container name.</param>
        public BlobDataStore(string connection, string container) {

            //connectionTokenCredential credential = new DefaultAzureCredential();
            var service = new BlobServiceClient(connection);
            this.BlobContainer = service.GetBlobContainerClient(container);
        }

        /// <summary>
        /// Creates a new instance of an <see cref="IDataStore" /> which uses an Azure
        /// Blob store (a named container) to persist and retrieve data. This constructor
        /// is designed to use a system-assigned managed identity to connect to a blob
        /// store.
        /// </summary>
        /// <param name="accountUri">The URI of the service.</param>
        /// <param name="container">The container name.</param>
        public BlobDataStore(Uri accountUri, string container) {

            TokenCredential credential = new DefaultAzureCredential();
            var service = new BlobServiceClient(accountUri, credential);
            this.BlobContainer = service.GetBlobContainerClient(container);
        }

        /// <summary>
        /// A blob service ready to connect.
        /// </summary>
        protected BlobContainerClient BlobContainer { get; init; }

        /// <summary>
        /// Saves the specified build to blob storage. We save this record twice: once in
        /// chronological order and second by ID.
        /// </summary>
        /// <param name="productId">The product ID this build is associated with.</param>
        /// <param name="build">The build resource data.</param>
        public async Task SaveBuild(string productId, BuildResource build)
        {
            ArgumentNullException.ThrowIfNull(build, nameof(build));

            var buildData = JsonConvert.SerializeObject(build, GetSerializerSettings());

            using var buildDataStream = new MemoryStream(Encoding.Unicode.GetBytes(buildData));
            using var buildIdStream = new MemoryStream(Encoding.Unicode.GetBytes(build.Id));

            var buildDatePath = build.GetDateEntryPath(productId);
            var buildDataPath = build.GetDataPath(productId);

            var uploadProduct = this.BlobContainer.UploadBlobAsync(buildDataPath, buildDataStream);
            var uploadMeta = this.BlobContainer.UploadBlobAsync(buildDatePath, buildIdStream);

            await Task.WhenAll(uploadMeta, uploadProduct).ConfigureAwait(false);
        }

        private static JsonSerializerSettings GetSerializerSettings()
        { 
            var contractResolver = new DefaultContractResolver { NamingStrategy = new CamelCaseNamingStrategy() };

            return new JsonSerializerSettings
            {
                ContractResolver = contractResolver,
                Formatting = Formatting.Indented
            };
        }
    }
}