using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Contracts;
using Functions.Configurations;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Functions
{
    public class DeliveryHandler
    {
        private readonly StorageConfiguration storageConfiguration;

        public DeliveryHandler(IOptions<StorageConfiguration> storageConfigurationOptions)
        {
            this.storageConfiguration = storageConfigurationOptions.Value;
        }

        [FunctionName("DeliveryHandler")]
        public Task Run([ServiceBusTrigger("%QueueName%", Connection = "QueueConnectionString")]string myQueueItem, ILogger log)
        {
            log.LogInformation($"C# ServiceBus queue trigger function processed message: {myQueueItem}");

            var delivery = JsonConvert.DeserializeObject<OrderDelivery>(myQueueItem);
            byte[] bytes = System.Text.Encoding.ASCII.GetBytes(myQueueItem);
            var stream = new MemoryStream(bytes);
            var client = CreateContainerClient();
            return client.UploadBlobAsync($"{delivery.OrderId}.json", stream);
        }

        private BlobContainerClient CreateContainerClient()
        {
            BlobServiceClient blobServiceClient = new BlobServiceClient(storageConfiguration.StorageAccountConnectionString);
            return blobServiceClient.GetBlobContainerClient(storageConfiguration.StorageAccountContainerName);
        }
    }
}