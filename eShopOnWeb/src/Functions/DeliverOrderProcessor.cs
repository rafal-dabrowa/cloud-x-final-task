using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Contracts;
using Functions.Configurations;
using Microsoft.Extensions.Options;
using Microsoft.Azure.Documents.Client;
using System;
using Microsoft.Azure.Documents;
using System.Net;

namespace Functions;

public class DeliverOrderProcessor
{
    private readonly CosmosConfiguration cosmosConfiguration;
    private readonly CosmosSecretConfiguration cosmosSecretConfiguration;

    public DeliverOrderProcessor(
        IOptions<CosmosConfiguration> cosmosConfigurationOptions, 
        IOptions<CosmosSecretConfiguration> cosmosSecretConfigurationOptions)
    {
        this.cosmosConfiguration = cosmosConfigurationOptions.Value;
        this.cosmosSecretConfiguration = cosmosSecretConfigurationOptions.Value;
    }

    [FunctionName("DeliverOrderProcessor")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest request,
        ILogger log, ExecutionContext context)
    {
        log.LogInformation("C# HTTP trigger function DeliverOrderProcessor start processing a request for delivery.");

        var delivery = await GetDelivery(request);

        await Write(delivery);

        log.LogInformation("C# HTTP trigger function DeliverOrderProcessor processed a request for delivery."); ;

        return new OkObjectResult("OK");
    }

    private static async Task<OrderDelivery> GetDelivery(HttpRequest request)
    {
        var requestBody = await new StreamReader(request.Body).ReadToEndAsync();
        return JsonConvert.DeserializeObject<OrderDelivery>(requestBody);
    }

    public async Task Write(OrderDelivery delivery)
    {
        var client = new DocumentClient(new Uri(cosmosConfiguration.Endpoint), cosmosSecretConfiguration.PrimaryKey);

        try
        {
            await client.ReadDocumentAsync(
                UriFactory.CreateDocumentUri(cosmosConfiguration.DatabaseName, cosmosConfiguration.CollectionName, delivery.OrderId.ToString()),
                new RequestOptions { PartitionKey = new PartitionKey(delivery.OrderId) });
        }
        catch (DocumentClientException de)
        {
            if (de.StatusCode == HttpStatusCode.NotFound)
            {
                await client.CreateDocumentAsync(
                    UriFactory.CreateDocumentCollectionUri(cosmosConfiguration.DatabaseName, cosmosConfiguration.CollectionName),
                    delivery);
            }
            else
            {
                throw;
            }
        }
    }
}
