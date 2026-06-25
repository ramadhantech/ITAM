using ITAM.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OfficeOpenXml;

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
        public async Task<IActionResult> ByLocation(
            int? locationId,
            int? categoryId,
            int page = 1)
        {
            int pageSize = 10;

            var locations =
                await _serviceReport.GetAllLocationsAsync();

            var categories =
                await _serviceReport.GetAllCategoriesAsync();

            ViewBag.Locations = new SelectList(
                locations,
                "Id",
                "Name",
                locationId);

            ViewBag.Categories = new SelectList(
                categories,
                "Id",
                "Name",
                categoryId);

            var allData =
                await _serviceReport.GetAssetsByFilterAsync(
                    locationId,
                    categoryId);

            var totalItems = allData.Count;

            var data = allData
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalItems = totalItems;

            ViewBag.CurrentLocation = locationId;
            ViewBag.CurrentCategory = categoryId;

            return View(data);
        }

        [HttpGet]
        public async Task<IActionResult> ExportExcel(
      int? locationId,
      int? categoryId)
        {
            var data = await _serviceReport.GetAssetsByFilterAsync(
                locationId,
                categoryId);

            ExcelPackage.License.SetNonCommercialOrganization("ITAM");

            using var package = new ExcelPackage();

            var worksheet = package.Workbook.Worksheets.Add("Asset Report");

            worksheet.Cells[1, 1].Value = "Asset Tag";
            worksheet.Cells[1, 2].Value = "Asset Name";
            worksheet.Cells[1, 3].Value = "Category";
            worksheet.Cells[1, 4].Value = "Location";
            worksheet.Cells[1, 5].Value = "Status";
            worksheet.Cells[1, 6].Value = "Condition";

            int row = 2;

            foreach (var item in data)
            {
                worksheet.Cells[row, 1].Value = item.AssetTag;
                worksheet.Cells[row, 2].Value = item.AssetName;
                worksheet.Cells[row, 3].Value = item.Category?.Name;
                worksheet.Cells[row, 4].Value = item.Location?.Name;
                worksheet.Cells[row, 5].Value = item.Status;
                worksheet.Cells[row, 6].Value = item.Condition;

                row++;
            }

            worksheet.Cells.AutoFitColumns();

            var fileBytes = package.GetAsByteArray();

            return File(
                fileBytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"AssetReport_{DateTime.Now:yyyyMMddHHmmss}.xlsx");
        }
    }

}