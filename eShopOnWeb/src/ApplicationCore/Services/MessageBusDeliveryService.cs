using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Contracts;
using Microsoft.eShopWeb.ApplicationCore.Configuration;
using Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Microsoft.eShopWeb.ApplicationCore.Services;

public class MessageBusDeliveryService : IDeliveryService
{
    private readonly ServiceBusQueueConfiguration serviceBusQueueConfiguration;
    private readonly ILogger<MessageBusDeliveryService> logger;

    public MessageBusDeliveryService(IOptions<ServiceBusQueueConfiguration> serviceBusQueueConfigurationOptions, ILogger<MessageBusDeliveryService> logger)
    {
        this.serviceBusQueueConfiguration = serviceBusQueueConfigurationOptions.Value;
        this.logger = logger;
    }

    public Task CreateDeliveryFor(Order order)
    {
        var delivery = new OrderDelivery(order.Id, MapShippingAddress(order), MapLineItems(order), CalculateFinalPrice(order));

        var client = new ServiceBusClient(serviceBusQueueConfiguration.QueueConnectionString);
        var sender = client.CreateSender(serviceBusQueueConfiguration.QueueName);

        var message = new ServiceBusMessage(JsonConvert.SerializeObject(delivery));

        return sender.SendMessageAsync(message);
    }

    private static decimal CalculateFinalPrice(Order order)
    {
        return order.OrderItems.Sum(orderItem => orderItem.UnitPrice * orderItem.Units);
    }

    private static IEnumerable<LineItem> MapLineItems(Order order)
    {
        return order.OrderItems
            .Select(orderItem => new LineItem(
                orderItem.ItemOrdered.CatalogItemId,
                orderItem.ItemOrdered.ProductName,
                orderItem.ItemOrdered.PictureUri,
                orderItem.UnitPrice,
                orderItem.Units));
    }

    private static ShippingAddress MapShippingAddress(Order order)
    {
        return new ShippingAddress(
            order.ShipToAddress.Street,
            order.ShipToAddress.City,
            order.ShipToAddress.State,
            order.ShipToAddress.Country,
            order.ShipToAddress.ZipCode);
    }
}