using Common.Models;
using Microsoft.ServiceFabric.Services.Remoting;

namespace Common.Interfaces;

public interface IPersister : IService
{
    public Task AddSkiSlopeState(SkiSlopeState skiSlopeState);  
}
