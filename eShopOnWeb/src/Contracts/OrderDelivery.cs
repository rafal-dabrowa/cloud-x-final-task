using Newtonsoft.Json;

namespace Contracts;

public record OrderDelivery
{
    public OrderDelivery(int orderId, ShippingAddress shippingAddress, IEnumerable<LineItem> lineItems, decimal finalPrice)
    {
        OrderId = orderId;
        ShippingAddress = shippingAddress;
        LineItems = lineItems;
        FinalPrice = finalPrice;
    }

    [JsonProperty("orderId")]
    public int OrderId { get; }
    [JsonProperty("shippingAddress")]
    public ShippingAddress ShippingAddress { get; }
    [JsonProperty("lineItems")]
    public IEnumerable<LineItem> LineItems { get; }
    [JsonProperty("finalPrice")]
    public decimal FinalPrice { get; }
}
