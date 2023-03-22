namespace Outboard.Api.Data.Blob
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;
    using Azure.Core;
    using Azure.Identity;
    using Azure.Storage.Blobs;
    using Azure.Storage.Blobs.Models;
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
        /// Gets a build record for the specified product.
        /// </summary>
        /// <param name="productId">The product ID that the build is part of.</param>
        /// <param name="buildVersion">The build to retrieve data for.</param>
        public async Task<BuildResource> LoadBuild(string productId, string buildVersion)
        { 
            ArgumentNullException.ThrowIfNull(productId, nameof(productId));
            ArgumentNullException.ThrowIfNull(buildVersion, nameof(buildVersion));

            var buildDataPath = ResourceExtensions.GetDataPath(productId, buildVersion);
            var blobClient = this.BlobContainer.GetBlobClient(buildDataPath);

            using var stream = await blobClient.OpenReadAsync().ConfigureAwait(false);
            using var reader = new StreamReader(stream);
            var payload = await reader.ReadToEndAsync().ConfigureAwait(false);

            return JsonConvert.DeserializeObject<BuildResource>(payload);
        }

        /// <summary>
        /// Saves the specified build to blob storage. We save this record twice: once in
        /// chronological order and second by ID.
        /// </summary>
        /// <param name="productId">The product ID this build is associated with.</param>
        /// <param name="build">The build resource data.</param>
        public async Task SaveBuild(string productId, BuildResource build)
        {
            ArgumentNullException.ThrowIfNull(productId, nameof(productId));
            ArgumentNullException.ThrowIfNull(build, nameof(build));

            var buildData = JsonConvert.SerializeObject(build, GetSerializerSettings());

            using var buildDataStream = new MemoryStream(Encoding.UTF8.GetBytes(buildData));
            using var buildIdStream = new MemoryStream(Encoding.UTF8.GetBytes(build.Version));

            var buildDatePath = build.GetDateEntryPath(productId);
            var buildDataPath = ResourceExtensions.GetDataPath(productId, build.Version);

            var blobHeaders = new BlobHttpHeaders() { ContentType = "application/json" };
            var metadata = new Dictionary<string, string>() {
                { "BuildVersion", build.Version },
                { "Product", productId }
            };

            var blobProductClient = this.BlobContainer.GetBlobClient(buildDataPath);
            var blobMetaClient = this.BlobContainer.GetBlobClient(buildDatePath);

            var uploadProduct = blobProductClient.UploadAsync(buildDataStream, metadata: metadata, httpHeaders: blobHeaders)
                .ContinueWith( (t) => {
                    blobProductClient.SetTagsAsync(metadata);
                }, TaskScheduler.Default);
            
            var uploadMeta = blobMetaClient.UploadAsync(buildIdStream, metadata: metadata, httpHeaders: blobHeaders)
                .ContinueWith( (t) => {
                    blobProductClient.SetTagsAsync(metadata);
                }, TaskScheduler.Default);
                
            await Task.WhenAll(uploadMeta, uploadProduct).ConfigureAwait(false);
        }

        /// <summary>
        /// Save a release record. Archives all relevant information as a snapshot
        /// to give a stable history.
        /// </summary>
        /// <param name="release">The release data to save.</param>
        public async Task SaveRelease(ReleaseResource release)
        { 
            ArgumentNullException.ThrowIfNull(release, nameof(release));

            var releaseData = JsonConvert.SerializeObject(release, GetSerializerSettings());

            var data = Encoding.UTF8.GetBytes(releaseData);
            using var buildHistoryDataStream = new MemoryStream(data);
            using var releaseLatestDataStream = new MemoryStream(data);
            using var releaseHistoryDataStream = new MemoryStream(data);

            var buildHistoryPath = release.GetBuildReleaseHistoryPath();
            var releaseLatestPath = release.GetReleaseLatestPath();
            var releaseHistoryPath = release.GetReleaseHistoryPath();

            var uploadBuildHistory = this.BlobContainer.UploadBlobAsync(buildHistoryPath, buildHistoryDataStream);
            var uploadReleaseLatest = this.BlobContainer.UploadBlobAsync(releaseLatestPath, releaseLatestDataStream);
            var uploadReleaseHistory = this.BlobContainer.UploadBlobAsync(releaseHistoryPath, releaseHistoryDataStream);

            await Task.WhenAll(uploadBuildHistory, uploadReleaseLatest, uploadReleaseHistory).ConfigureAwait(false);
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