using System.Fabric;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using Common.Interfaces;
using Common.Models;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Persister;

/// <summary>
/// An instance of this class is created for each service replica by the Service Fabric runtime.
/// </summary>
internal sealed class Persister : StatefulService, IPersister
{
    private const string AllSkiSlopesStates = "weatherSlopeStates";
    private IReliableDictionary2<Guid, SkiSlopeState> _skiSlopeStates;

    public Persister(StatefulServiceContext context)
        : base(context)
    { }

    public async Task AddSkiSlopeStateAsync(SkiSlopeState skiSlopeState)
    {
        _skiSlopeStates = await StateManager.GetOrAddAsync<IReliableDictionary2<Guid, SkiSlopeState>>(AllSkiSlopesStates);
        using ITransaction transaction = StateManager.CreateTransaction();
        skiSlopeState.Weather = GetWeatherForPlaceAsync(skiSlopeState.Place)!;
        await _skiSlopeStates.AddAsync(transaction, skiSlopeState.Id, skiSlopeState);
        await transaction.CommitAsync();
    }

    public async Task<List<SkiSlopeState>> GetAllSkiSlopeStatesReportedBeforeProvidedTimestampAsync(DateTime dateTime)
    {
        List<SkiSlopeState> skiSlopeStatesList = new();
        Microsoft.ServiceFabric.Data.IAsyncEnumerator<KeyValuePair<Guid, SkiSlopeState>> enumerator = await GetAsyncEnumerator();

        while (await enumerator.MoveNextAsync(default))
        {
            KeyValuePair<Guid, SkiSlopeState> current = enumerator.Current;
            if (current.Value.Date < dateTime && !current.Value.IsMigrated)
            {
                skiSlopeStatesList.Add(current.Value);
                current.Value.IsMigrated = true;
            }
        }

        return skiSlopeStatesList;
    }

    private async Task<Microsoft.ServiceFabric.Data.IAsyncEnumerator<KeyValuePair<Guid, SkiSlopeState>>> GetAsyncEnumerator()
    {
        _skiSlopeStates = await StateManager.GetOrAddAsync<IReliableDictionary2<Guid, SkiSlopeState>>(AllSkiSlopesStates);
        using ITransaction transaction = StateManager.CreateTransaction();
        Microsoft.ServiceFabric.Data.IAsyncEnumerable<KeyValuePair<Guid, SkiSlopeState>> skiSlopeStates = await _skiSlopeStates.CreateEnumerableAsync(transaction);
        Microsoft.ServiceFabric.Data.IAsyncEnumerator<KeyValuePair<Guid, SkiSlopeState>> enumerator = skiSlopeStates.GetAsyncEnumerator();

        return enumerator;
    }

    public async Task<List<SkiSlopeState>> GetAllSkiSlopeStatesAsync()
    {
        List<SkiSlopeState> skiSlopeStatesList = new();
        Microsoft.ServiceFabric.Data.IAsyncEnumerator<KeyValuePair<Guid, SkiSlopeState>> enumerator = await GetAsyncEnumerator();

        while (await enumerator.MoveNextAsync(default))
        {
            KeyValuePair<Guid, SkiSlopeState> current = enumerator.Current;
            skiSlopeStatesList.Add(current.Value);
        }

        return skiSlopeStatesList;
    }

    public static Weather? GetWeatherForPlaceAsync(string skiSlopePlace)
    {
        string url = string.Format("https://api.openweathermap.org/data/2.5/weather?q={0}&appid=ee89cb80a57b008a5ca9b94bd300f41b", skiSlopePlace);

        double temperature;
        double windspeed;
        double clouds;
        try
        {
            WebClient client = new();
            string content = client.DownloadString(url);

            JObject? obj = JsonConvert.DeserializeObject<JObject>(content);

            double tempInKelvin = double.Parse(obj["main"]["temp"].ToString());
            temperature = Math.Round(tempInKelvin - 273.15, 2);

            windspeed = double.Parse(obj["wind"]["speed"].ToString());
            clouds = double.Parse(obj["clouds"]["all"].ToString());

            return new Weather(temperature, clouds, windspeed);
        }
        catch
        {
            ServiceEventSource.Current.Message("Not connected to OpenWeather!");
            return null;
        }
    }

    /// <summary>
    /// Optional override to create listeners (e.g., HTTP, Service Remoting, WCF, etc.) for this service replica to handle client or user requests.
    /// </summary>
    /// <remarks>
    /// For more information on service communication, see https://aka.ms/servicefabricservicecommunication
    /// </remarks>
    /// <returns>A collection of listeners.</returns>
    protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
    {
        return this.CreateServiceRemotingReplicaListeners();
    }

    /// <summary>
    /// This is the main entry point for your service replica.
    /// This method executes when this replica of your service becomes primary and has write status.
    /// </summary>
    /// <param name="cancellationToken">Canceled when Service Fabric needs to shut down this service replica.</param>
    protected override async Task RunAsync(CancellationToken cancellationToken)
    {
        // TODO: Replace the following sample code with your own logic 
        //       or remove this RunAsync override if it's not needed in your service.

        var myDictionary = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, long>>("myDictionary");

        while (true)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var tx = this.StateManager.CreateTransaction())
            {
                var result = await myDictionary.TryGetValueAsync(tx, "Counter");

                ServiceEventSource.Current.ServiceMessage(this.Context, "Current Counter Value: {0}",
                    result.HasValue ? result.Value.ToString() : "Value does not exist.");

                await myDictionary.AddOrUpdateAsync(tx, "Counter", 0, (key, value) => ++value);

                // If an exception is thrown before calling CommitAsync, the transaction aborts, all changes are 
                // discarded, and nothing is saved to the secondary replicas.
                await tx.CommitAsync();
            }

            await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
        }
    }
}
