using ITAM.Data;
using ITAM.Dto;
using ITAM.Models;
using Microsoft.EntityFrameworkCore;

namespace ITAM.Services
{
    public class AssetLogService
    {
        private readonly ItamDbContext _context;

        public AssetLogService(ItamDbContext context)
        {
            _context = context;
        }

        // 1. READ ALL (Mendapatkan semua log riwayat di-mapping langsung ke Read DTO)
        public async Task<List<ResponseAssetLogDto>> GetAllAsync()
        {
            return await _context.AssetHistory
                .AsNoTracking() // Matikan tracking memori agar loading kencang
                .OrderByDescending(h => h.ChangedAt) // Urutkan dari log paling baru
                .Select(h => new ResponseAssetLogDto
                {
                    Id = h.Id,
                    FieldName = h.FieldName,
                    AssetId = h.AssetId,
                    AssetName = h.Asset.AssetName, // Ambil string nama langsung dari relasi
                    OldValue = h.OldValue,
                    NewValue = h.NewValue,
                    ChangedBy = h.ChangedBy,
                    ChangedByUserName = h.ChangedByUser.Name, // Ambil string nama user pengubah
                    ChangedAt = h.ChangedAt
                })
                .ToListAsync();
        }

        // 2. READ BY ASSET ID (Mendapatkan log riwayat khusus untuk satu aset tertentu - berguna untuk halaman detail aset)
        public async Task<List<ResponseAssetLogDto>> GetByAssetIdAsync(int assetId)
        {
            return await _context.AssetHistory
                .AsNoTracking()
                .Where(h => h.AssetId == assetId)
                .OrderByDescending(h => h.ChangedAt)
                .Select(h => new ResponseAssetLogDto
                {
                    Id = h.Id,
                    FieldName = h.FieldName,
                    AssetId = h.AssetId,
                    AssetName = h.Asset.AssetName,
                    OldValue = h.OldValue,
                    NewValue = h.NewValue,
                    ChangedBy = h.ChangedBy,
                    ChangedByUserName = h.ChangedByUser.Name,
                    ChangedAt = h.ChangedAt
                })
                .ToListAsync();
        }

        // 3. READ BY ID (Mendapatkan 1 data log spesifik berdasarkan ID log history)
        public async Task<ResponseAssetLogDto?> GetByIdAsync(int id)
        {
            return await _context.AssetHistory
                .AsNoTracking()
                .Where(h => h.Id == id)
                .Select(h => new ResponseAssetLogDto
                {
                    Id = h.Id,
                    FieldName = h.FieldName,
                    AssetId = h.AssetId,
                    AssetName = h.Asset.AssetName,
                    OldValue = h.OldValue,
                    NewValue = h.NewValue,
                    ChangedBy = h.ChangedBy,
                    ChangedByUserName = h.ChangedByUser.Name,
                    ChangedAt = h.ChangedAt
                })
                .FirstOrDefaultAsync();
        }

        // 4. CREATE LOG / LOGGING ACTION (Mencatat log perubahan baru menggunakan Create DTO)
        public async Task CreateLogAsync(CreateAssetHistoryDto request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            // Mapping dari DTO ke Model database asli
            var log = new AssetHistory
            {
                FieldName = request.FieldName,
                AssetId = request.AssetId,
                OldValue = request.OldValue,
                NewValue = request.NewValue,
                ChangedBy = request.ChangedBy,
                ChangedAt = DateTime.UtcNow // Tanggal perubahan disetel otomatis di server
            };

            _context.AssetHistory.Add(log);
            await _context.SaveChangesAsync();
        }

        // 5. DELETE LOG (Menghapus catatan log secara permanen dari database seandainya diperlukan)
        public async Task<bool> DeleteLogAsync(int id)
        {
            var data = await _context.AssetHistory.FindAsync(id);
            if (data == null)
            {
                return false;
            }

            _context.AssetHistory.Remove(data);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
