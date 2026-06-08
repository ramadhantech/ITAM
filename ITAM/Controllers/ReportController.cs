using ITAM.Models;
using ITAM.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ITAM.Controllers
{
    public class ReportController : Controller
    {
        private readonly ServiceReport _serviceReport;

        public ReportController(ServiceReport serviceReport)
        {
            _serviceReport = serviceReport;
        }

        [HttpGet]
        public async Task<IActionResult> ByLocation(int? locationId)
        {
            var locations = await _serviceReport.GetAllLocationsAsync();

            ViewBag.Locations =
                new SelectList(locations, "Id", "Name", locationId);

            if (locationId.HasValue)
            {
                var data =
                    await _serviceReport.GetAssetsByLocationIdAsync(locationId.Value);

                return View(data);
            }

            return View(new List<Asset>());
        }
    }
}