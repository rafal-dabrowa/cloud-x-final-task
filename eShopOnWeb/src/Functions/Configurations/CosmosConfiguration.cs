namespace Functions.Configurations;

public class CosmosConfiguration
{
    public const string CONFIG_NAME = "Cosmos";

    public string Endpoint { get; set; }
    public string DatabaseName { get; set; }
    public string CollectionName { get; set; }
}
