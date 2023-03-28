using Common.Constants;
using Common.Interfaces;
using Common.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using System.Fabric;

namespace API.Controllers
{
    public class HistoryController : Controller
    {
        public async Task<IActionResult> Index()
        {
            List<SkiSlopeState> skiSlopeStates = new();

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

                var slopes = await proxy.GetAllHistoryOfSkiSlopeStatesAsync();

                skiSlopeStates.AddRange(slopes);
            }

            ViewBag.SkiSlopes = skiSlopeStates;
            return View();
        }
    }
}
