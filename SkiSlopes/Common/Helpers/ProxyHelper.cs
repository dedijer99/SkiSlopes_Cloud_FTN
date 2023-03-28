using System.Fabric;

namespace Common.Helpers;

public static class ProxyHelper
{
    public static async Task<int> GetPartitionsNumberByUri(string uri)
    {
        FabricClient fabricClient = new();
        int partitionsNumber = (await fabricClient
            .QueryManager
            .GetPartitionListAsync(new Uri(uri))).Count;
        return partitionsNumber;
    }
}
