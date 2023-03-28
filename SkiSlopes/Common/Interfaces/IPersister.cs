using Common.Models;
using Microsoft.ServiceFabric.Services.Remoting;

namespace Common.Interfaces;

public interface IPersister : IService
{
    Task AddSkiSlopeStateAsync(SkiSlopeState skiSlopeState);  
    Task<List<SkiSlopeState>> GetAllSkiSlopeStatesAsync();
    Task<List<SkiSlopeState>> GetAllSkiSlopeStatesReportedBeforeProvidedTimestampAsync(DateTime dateTime);
}
