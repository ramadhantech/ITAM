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

        // AMBIL SEMUA LOKASI
        public async Task<List<Location>> GetAllLocationsAsync()
        {
            return await _context.Locations
        .AsNoTracking()
        .OrderBy(x => x.Name)
        .ToListAsync();
        }

        // AMBIL ASSET BERDASARKAN LOKASI
        public async Task<List<Asset>> GetAssetsByLocationIdAsync(int locationId)
        {
            return await _context.Assets
                .AsNoTracking()
                .Include(x => x.Category)
                .Include(x => x.Location)
                .Where(x => x.LocationId == locationId && !x.IsDeleted)
                .ToListAsync();
        }
    }
}