using API.Models;
using Common.Interfaces;
using Common.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using System.Fabric;

namespace API.Controllers;

public class SkiSlopeStateController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    [HttpPost("SkiSlopeState/Add")]
    public async Task<IActionResult> AddSkiSlopeStateAsync(SkiSlopeRequest request)
    {
        FabricClient fabricClient = new();
        int partitionsNumber = (await fabricClient
            .QueryManager
            .GetPartitionListAsync(new Uri("fabric:/SkiSlopes/Persister"))).Count;
        int index = 0;

        for (int i = 0; i < partitionsNumber; i++, index++)
        {
            IPersister proxy = ServiceProxy.Create<IPersister>(
                new Uri("fabric:/SkiSlopes/Persister"), 
                new ServicePartitionKey(index % partitionsNumber));

            await proxy.AddSkiSlopeState(new SkiSlopeState(
                place:      request.Place, 
                date:       request.Date, 
                number:     request.Number, 
                name:       request.Name, 
                condition:  request.Condition, 
                details:    request.Details));
        }

        return View("Index");
    }
}
