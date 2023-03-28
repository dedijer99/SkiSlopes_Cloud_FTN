using Common.Models;

namespace Common.Interfaces;

public interface ITableStorage
{
    Task<List<SkiSlopeState>> GetAllSkiSlopeStates();
}
