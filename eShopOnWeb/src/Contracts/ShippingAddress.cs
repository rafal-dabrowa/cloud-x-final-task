using Newtonsoft.Json;

namespace Contracts;

public record ShippingAddress
{
    public ShippingAddress(string street, string city, string state, string country, string zipCode)
    {
        Street = street;
        City = city;
        State = state;
        Country = country;
        ZipCode = zipCode;
    }

    [JsonProperty("street")]
    public string Street { get; private set; }

    [JsonProperty("city")]
    public string City { get; private set; }

    [JsonProperty("state")]
    public string State { get; private set; }

    [JsonProperty("country")]
    public string Country { get; private set; }

    [JsonProperty("zipCode")]
    public string ZipCode { get; private set; }
}
