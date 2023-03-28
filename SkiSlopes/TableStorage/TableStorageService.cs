using Common.Models;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace TableStorage;

public static class TableStorageService
{
    public const string ConnectionString = "UseDevelopmentStorage=true";
    public static async Task<List<SkiSlopeState>> GetAllSkiSlopeStates()
    {
        TableQuery<SkiSlopeStateTable> tableQuery = new();
        CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConnectionString);
        CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
        CloudTable table = tableClient.GetTableReference("SkiSlopeState");

        List<SkiSlopeState> skiSlopeStates = new();
        foreach (SkiSlopeStateTable entity in await table.ExecuteQuerySegmentedAsync(tableQuery, default))
        {
            var skiSlopeState = new SkiSlopeState(entity.Place, entity.Date, entity.Number, entity.Name, (Common.Enums.SkiSlopeCondition)entity.Condition, entity.Details);
            skiSlopeState.Weather = new Weather(double.Parse(entity.Temperature), double.Parse(entity.Clouds), double.Parse(entity.WindSpeed));
            skiSlopeStates.Add(skiSlopeState);
        }

        return skiSlopeStates;
    }
}
