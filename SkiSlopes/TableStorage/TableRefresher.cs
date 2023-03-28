using Common.Constants;
using Common.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System.Fabric;

namespace Persister.Storage
{
    public class TableRefresher : BackgroundService
    {
        public const string ConnectionString = "UseDevelopmentStorage=true";

        private static async Task AddToTableAsync(List<SkiSlopeState> skiSlopeStates)
        {
            TableQuery<SkiSlopeState> tableQuery = new TableQuery<SkiSlopeState>();

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConnectionString);
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference("SkiSlopeState");
            await table.CreateIfNotExistsAsync();

            foreach (SkiSlopeState entity in await table.ExecuteQuerySegmentedAsync(tableQuery, default))
            {
                Console.WriteLine("{0}, {1}\t{2}", entity.PartitionKey, entity.RowKey, entity.Place);
            }

            foreach (SkiSlopeState skiSlopeState in skiSlopeStates)
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

                    List<Common.Models.SkiSlopeState> slopes = await proxy.GetAllSkiSlopeStatesReportedBeforeProvidedTimestampAsync(DateTime.UtcNow.AddSeconds(30));

                    await AddToTableAsync(slopes
                        .Select(slope => new SkiSlopeState(slope))
                        .ToList());
                }

                await Task.Delay(10000, stoppingToken);
            }
        }
    }
}
