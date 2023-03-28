using Common.Constants;
using Common.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System.Fabric;

namespace TableStorage;

public class TableRefresher : BackgroundService
{
    private static async Task AddToTableAsync(List<SkiSlopeStateTable> skiSlopeStates)
    {
        CloudStorageAccount storageAccount = CloudStorageAccount.Parse(TableStorageService.ConnectionString);
        CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
        CloudTable table = tableClient.GetTableReference("SkiSlopeState");
        await table.CreateIfNotExistsAsync();

        foreach (SkiSlopeStateTable skiSlopeState in skiSlopeStates)
            await table.ExecuteAsync(TableOperation.InsertOrReplace(skiSlopeState));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            FabricClient fabricClient = new();
            int partitionsNumber = (await fabricClient
                .QueryManager
                .GetPartitionListAsync(new Uri(ServiceFabricConstants.Persister))).Count;
            int index = 0;

            for (int i = 0; i < partitionsNumber; i++, index++)
            {
                IPersister proxy = ServiceProxy.Create<IPersister>(
                    new Uri(ServiceFabricConstants.Persister),
                    new ServicePartitionKey(index % partitionsNumber));

                List<Common.Models.SkiSlopeState> slopes = await proxy.GetAllSkiSlopeStatesReportedBeforeProvidedTimestampAsync(DateTime.UtcNow.AddSeconds(-30));

                if (slopes.Count != 0)
                    await AddToTableAsync(slopes
                        .Select(slope => new SkiSlopeStateTable(slope))
                        .ToList());
            }

            await Task.Delay(10000, stoppingToken);
        }
    }
}
