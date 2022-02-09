using Newtonsoft.Json;

namespace Contracts;

public record LineItem
{
    public LineItem(int catalogItemId, string productName, string pictureUri, decimal unitPrice, int units)
    {
        CatalogItemId = catalogItemId;
        ProductName = productName;
        PictureUri = pictureUri;
        UnitPrice = unitPrice;
        Units = units;
    }

    [JsonProperty("catalogItemId")]
    public int CatalogItemId { get; private set; }

    [JsonProperty("productName")]
    public string ProductName { get; private set; }

    [JsonProperty("pictureUri")]
    public string PictureUri { get; private set; }

    [JsonProperty("unitPrice")]
    public decimal UnitPrice { get; private set; }

    [JsonProperty("units")]
    public int Units { get; private set; }
}
