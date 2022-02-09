using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Contracts;
using Microsoft.eShopWeb.ApplicationCore.Configuration;
using Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Microsoft.eShopWeb.ApplicationCore.Services;

public class HttpDeliveryService : IDeliveryService
{
    private readonly FunctionConfigurations functionConfigurations;

    public HttpDeliveryService(IOptions<FunctionConfigurations> options)
    {
        functionConfigurations = options.Value;
    }

    public Task CreateDeliveryFor(Order order)
    {
        var httpClient = new HttpClient();

        return httpClient.PostAsync(functionConfigurations.Url, SerializeContent(order));
    }

    private static StringContent SerializeContent(Order order)
    {
        var delivery = new OrderDelivery(
            order.Id,
            MapShippingAddress(order),
            MapLineItems(order),
            CalculateFinalPrice(order));

        return new StringContent(JsonConvert.SerializeObject(delivery), Encoding.UTF8, "applicaiton/json");
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
