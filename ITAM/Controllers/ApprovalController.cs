using ITAM.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ITAM.Controllers
{
    [Authorize]
    public class ApprovalController : Controller
    {
        private readonly InspectionService _service;

        public ApprovalController(
            InspectionService service)
        {
            _service = service;
        }

        /* =========================
           INDEX
        ========================= */

        public async Task<IActionResult> Index(int page = 1)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
                return Unauthorized();

            int currentUserId = int.Parse(userId);

            int pageSize = 10;

            var allData = await _service.GetPendingApprovalAsync(currentUserId);

            var totalItems = allData.Count;

            var items = allData
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalItems = totalItems;

            return View(items);
        }



        /* =========================
           DETAIL
        ========================= */

        public async Task<IActionResult> Detail(int id)
        {
            var data = await _service.GetByIdAsync(id);

            if (data == null)
            {
                return NotFound();
            }

            return View(data);
        }

        /* =========================
           APPROVE LOCATION
        ========================= */




        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveLocation(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
                return Unauthorized();

            await _service.ApproveByLocationAsync(id, int.Parse(userId));

            return RedirectToAction(nameof(Index));
        }
    }
}