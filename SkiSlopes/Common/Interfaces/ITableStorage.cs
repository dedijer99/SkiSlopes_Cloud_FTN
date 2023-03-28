using Common.Models;
using Microsoft.ServiceFabric.Services.Remoting;

namespace Common.Interfaces;

public interface ITableStorage : IService
{
    Task<List<SkiSlopeState>> GetAllSkiSlopeStates();
}
