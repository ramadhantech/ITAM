using ITAM.Services;
using Microsoft.AspNetCore.Mvc;

namespace ITAM.Controllers
{
    public class AssetLogController : Controller
    {
        private readonly AssetLogService _logService;

        public AssetLogController(AssetLogService logService)
        {
            _logService = logService;
        }

        // 1. HALAMAN UTAMA (Daftar Riwayat Perubahan/Audit Log Seluruh Asset)
        [HttpGet]
        public async Task<IActionResult> Index(int page = 1)
        {
            int pageSize = 10;

            var logs = await _logService.GetAllAsync();

            var totalItems = logs.Count();

            var pagedData = logs
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalItems = totalItems;

            return View(pagedData);
        }

        // 2. HALAMAN UTAMA KUSTOM (Daftar Log Khusus Berdasarkan ID Asset Tertentu)
        // Berguna jika Anda ingin menampilkan riwayat log di bawah halaman Detail Asset
        [HttpGet]
        public async Task<IActionResult> AssetHistory(int id)
        {
            var logs = await _logService.GetByAssetIdAsync(id);
            return View("Index", logs); // Menggunakan View Index yang sama agar hemat kode
        }

        // 3. HALAMAN DETAIL LOG (Melihat rincian nilai lama vs nilai baru dari log spesifik)
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var log = await _logService.GetByIdAsync(id);
            if (log == null)
            {
                return NotFound();
            }
            return View(log);
        }

        // 4. AKSI HAPUS LOG (Konsep POST Ringkas: Langsung Hapus & Refresh Tampilan)
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _logService.DeleteLogAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
