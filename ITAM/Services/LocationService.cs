using ITAM.Data;
using ITAM.Models;
using Microsoft.EntityFrameworkCore;

namespace ITAM.Services
{
    public class LocationService
    {
        private readonly ItamDbContext _context;
        public LocationService(ItamDbContext context)
        {
            _context = context;
        }

        // 1. READ ALL (Dioptimasi agar muat menu Lokasi secepat kilat)
        public async Task<List<Location>> GetAllAsync()
        {
            return await _context.Locations
                .AsNoTracking() // Perbaikan 1: Matikan pelacakan memori RAM server agar query data tabel super ringan
                .Where(x => !x.IsDeleted)
                .ToListAsync();
        }

        // 2. READ BY ID (Dioptimasi untuk memuat data lama di form Edit secara instan)
        public async Task<Location?> GetById(int id)
        {
            return await _context.Locations
                .AsNoTracking() // Perbaikan 2: Menghemat resource server untuk pembacaan data tunggal
                .Where(x => x.Id == id && !x.IsDeleted)
                .FirstOrDefaultAsync();
        }

        // 3. CREATE (Tetap dipertahankan karena sudah benar)
        public async Task CreateAsync(Location request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var data = new Location
            {
                Name = request.Name
            };

            _context.Locations.Add(data);
            await _context.SaveChangesAsync();
        }

        // 4. UPDATE (Optimasi: Hanya jalankan SaveChanges jika data aktif ditemukan)
        public async Task UpdateAsync(int id, Location request)
        {
            var data = await _context.Locations.FindAsync(id);

            if (data != null && !data.IsDeleted)
            {
                data.Name = request.Name;
                await _context.SaveChangesAsync(); // Taruh di dalam blok IF agar hemat proses database
            }
        }

        // 5. DELETE (Tetap dipertahankan karena alur Soft Delete sudah tepat)
        public async Task DeleteAsync(int id)
        {
            var data = await _context.Locations.FindAsync(id);

            if (data != null && !data.IsDeleted)
            {
                data.IsDeleted = true;
                await _context.SaveChangesAsync();
            }
        }
    }
}
