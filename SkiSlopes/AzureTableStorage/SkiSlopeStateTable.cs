using Common.Models;
using Microsoft.WindowsAzure.Storage.Table;

namespace AzureTableStorage;

public class SkiSlopeStateTable : TableEntity
{
    public Guid Id { get; set; }
    public string Place { get; set; }
    public DateTime Date { get; set; }
    public int Number { get; set; }
    public string Name { get; set; }
    public int Condition { get; set; }
    public string Details { get; set; }
    public string Temperature { get; set; }
    public string Clouds { get; set; }
    public string WindSpeed { get; set; }

    public SkiSlopeStateTable(SkiSlopeState slope)
    {
        PartitionKey = slope.Id.ToString();
        RowKey = slope.Id.ToString();
        Id = slope.Id;
        Place = slope.Place;
        Date = slope.Date;
        Number = slope.Number;
        Name = slope.Name;
        Condition = (int)slope.Condition;
        Details = slope.Details;
        Temperature = slope.Weather.Temperature;
        Clouds = slope.Weather.Clouds;
        WindSpeed = slope.Weather.WindSpeed;
    }

    public SkiSlopeStateTable()
    {
    }
}
