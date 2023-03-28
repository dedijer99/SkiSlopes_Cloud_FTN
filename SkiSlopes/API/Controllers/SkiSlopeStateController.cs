using API.Models;
using Common.Constants;
using Common.Helpers;
using Common.Interfaces;
using Common.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using System.Fabric;

namespace API.Controllers;

public class SkiSlopeStateController : Controller
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

            var slopes = await proxy.GetAllSkiSlopeStatesAsync();

            skiSlopeStates.AddRange(slopes);
        }

        ViewBag.SkiSlopes = skiSlopeStates;
        return View();
    }

    [HttpPost("SkiSlopeState/Add")]
    public async Task<IActionResult> AddSkiSlopeStateAsync(SkiSlopeRequest request, CancellationToken cancellationToken)
    {
        int partitionsNumber = await ProxyHelper.GetPartitionsNumberByUri(ServiceFabricConstants.Persister);
        int index = 0;

        for (int i = 0; i < partitionsNumber; i++, index++)
        {
            IPersister proxy = ServiceProxy.Create<IPersister>(
                new Uri(ServiceFabricConstants.Persister), 
                new ServicePartitionKey(index % partitionsNumber));

            await proxy.AddSkiSlopeStateAsync(new SkiSlopeState(
                place:      request.Place, 
                date:       DateTime.UtcNow, 
                number:     request.Number, 
                name:       request.Name, 
                condition:  request.Condition, 
                details:    request.Details));
        }

        return RedirectToAction("Index");
    }
}
