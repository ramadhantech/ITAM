using ITAM.Data;
using ITAM.Models;
using Microsoft.EntityFrameworkCore;

namespace ITAM.Services
{
    public class ServiceReport
    {
        private readonly ItamDbContext _context;

        public ServiceReport(ItamDbContext context)
        {
            _context = context;
        }

        // =========================
        // LOKASI
        // =========================
        public async Task<List<Location>> GetAllLocationsAsync()
        {
            return await _context.Locations
                .AsNoTracking()
                .OrderBy(x => x.Name)
                .ToListAsync();
        }

        // =========================
        // KATEGORI
        // =========================
        public async Task<List<Category>> GetAllCategoriesAsync()
        {
            return await _context.Categories
                .AsNoTracking()
                .OrderBy(x => x.Name)
                .ToListAsync();
        }

        // =========================
        // UNTUK INSPECTION CONTROLLER
        // =========================
        public async Task<List<Asset>> GetAssetsByLocationIdAsync(int locationId)
        {
            return await _context.Assets
                .AsNoTracking()
                .Include(x => x.Category)
                .Include(x => x.Location)
                .Where(x =>
                    x.LocationId == locationId &&
                    !x.IsDeleted)
                .ToListAsync();
        }

        // =========================
        // UNTUK REPORT FILTER
        // =========================
        public async Task<List<Asset>> GetAssetsByFilterAsync(
            int? locationId,
            int? categoryId)
        {
            var query = _context.Assets
                .AsNoTracking()
                .Include(x => x.Category)
                .Include(x => x.Location)
                .Where(x => !x.IsDeleted)
                .AsQueryable();

            if (locationId.HasValue)
            {
                query = query.Where(x =>
                    x.LocationId == locationId.Value);
            }

            if (categoryId.HasValue)
            {
                query = query.Where(x =>
                    x.CategoryId == categoryId.Value);
            }

            return await query
                .OrderBy(x => x.AssetName)
                .ToListAsync();
        }
    }
}