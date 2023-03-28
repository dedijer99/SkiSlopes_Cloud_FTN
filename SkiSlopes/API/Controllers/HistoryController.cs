using Common.Constants;
using Common.Interfaces;
using Common.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using System.Fabric;

namespace API.Controllers;

public class HistoryController : Controller
{
    public async Task<IActionResult> Index()
    {
        List<SkiSlopeState> skiSlopeStates = new();

        ITableStorage proxy = ServiceProxy.Create<ITableStorage>(new Uri(ServiceFabricConstants.TableStorage));
        List<SkiSlopeState> slopes = await proxy.GetAllSkiSlopeStates();
        skiSlopeStates.AddRange(slopes);

        ViewBag.SkiSlopes = skiSlopeStates;
        return View();
    }
}
