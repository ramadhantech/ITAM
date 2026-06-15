using ITAM.Data;
using ITAM.Dto;
using ITAM.Models;
using Microsoft.EntityFrameworkCore;

namespace ITAM.Services
{
    public class AssetService
    {
        private readonly ItamDbContext _context;
        private readonly AssetLogService _logService;

        public AssetService(ItamDbContext context, AssetLogService logService)
        {
            _context = context;
            _logService = logService;
        }

        /* =========================
           GET ALL
        ========================= */
        public async Task<List<Asset>> GetAllAsync(
            string search,
            int? categoryId,
            int? locationId)
        {
            var query = _context.Assets
                .AsNoTracking()
                .AsSplitQuery()
                .Include(a => a.Category)
                .Include(a => a.Location)
                .Include(a => a.User)
                .Include(a => a.ContractName)
                .ThenInclude(c => c.Vendor)
                .Where(x => !x.IsDeleted);

            // SEARCH
            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();

                query = query.Where(x =>
                    x.AssetName.ToLower().Contains(search) ||
                    x.AssetTag.ToLower().Contains(search) ||
                    x.AssetType.ToLower().Contains(search) ||
                    x.AssetSubtype.ToLower().Contains(search)); // TAMBAHAN: Bisa search berdasarkan subtype
            }

            // CATEGORY FILTER
            if (categoryId.HasValue && categoryId.Value > 0)
            {
                query = query.Where(x => x.CategoryId == categoryId.Value);
            }

            // LOCATION FILTER
            if (locationId.HasValue && locationId.Value > 0)
            {
                query = query.Where(x => x.LocationId == locationId.Value);
            }

            return await query.ToListAsync();
        }

        /* =========================
           READ BY ID
        ========================= */
        public async Task<Asset?> GetByIdAsync(int id)
        {
            return await _context.Assets
                .AsNoTracking()
                .Include(x => x.ContractName)
                .Where(x => x.Id == id && !x.IsDeleted)
                .FirstOrDefaultAsync();
        }

        /* =========================
           GET DETAILS
        ========================= */
        public async Task<Asset?> GetDetailsAsync(int id)
        {
            return await _context.Assets
                .AsNoTracking()
                .Include(a => a.Category)
                .Include(a => a.Location)
                .Include(a => a.User)
                .Include (a => a.ContractName)
                .Where(x => x.Id == id && !x.IsDeleted)
                .FirstOrDefaultAsync();
        }

       

        /* =========================
           CREATE
        ========================= */
        public async Task CreateAsync(CreateAssetDto request, int currentUserId)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            var user = await _context.Users
                 .AsNoTracking()
                 .FirstOrDefaultAsync(x => x.Id == request.UserId);

            if (user == null)
            {
                throw new Exception("PIC tidak ditemukan");
            }

            if (user.LocationId != request.LocationId)
            {
                throw new Exception("PIC harus berasal dari lokasi yang sama");
            }

            var asset = new Asset
            {
                AssetTag = request.AssetTag,
                AssetName = request.AssetName,
                CategoryId = request.CategoryId,
                ContractId = request.ContractId,
                SerialNumber = request.SerialNumber,

                // MAP FIELD BARU KE DATABASE
                AssetSubtype = request.AssetSubtype,

                LocationId = request.LocationId,
                UserId = request.UserId,
                AssetType = request.AssetType,
                Status = request.Status,
                Condition = request.Condition,
                Note = request.Note,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsDeleted = false,
                CreatedBy = currentUserId
            };

            _context.Assets.Add(asset);

            // PENTING: Kita panggil SaveChanges dulu agar objek asset mendapatkan ID dari database
            await _context.SaveChangesAsync();

            // CATAT LOG TAMBAH DATA: 1 Baris Timeline Terpadu
            await _logService.CreateLogAsync(new CreateAssetHistoryDto
            {
                AssetId = asset.Id,
                FieldName = "Registrasi Asset Baru",
                OldValue = "-",
                NewValue = $"Asset '{asset.AssetName}' Subtype [{asset.AssetSubtype}] dengan Tag [{asset.AssetTag}] berhasil didaftarkan.",
                ChangedBy = currentUserId
            });

            // Simpan data log sejarahnya
            await _context.SaveChangesAsync();
        }

        /* =========================
           UPDATE
        ========================= */
        public async Task<bool> UpdateAsync(int id, CreateAssetDto request, int currentUserId)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            var data = await _context.Assets.FindAsync(id);
            var user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.UserId);

            if (user == null)
            {
                throw new Exception("PIC tidak ditemukan");
            }

            if (user.LocationId != request.LocationId)
            {
                throw new Exception("PIC harus berasal dari lokasi yang sama");
            }

            if (data == null || data.IsDeleted) return false;


            // Wadah koleksi string untuk mendeteksi multi-perubahan
            var fieldNames = new List<string>();
            var oldValues = new List<string>();
            var newValues = new List<string>();

            if (data.AssetName != request.AssetName)
            {
                fieldNames.Add("Nama Asset");
                oldValues.Add(data.AssetName);
                newValues.Add(request.AssetName);
            }

            if (data.AssetTag != request.AssetTag)
            {
                fieldNames.Add("Asset Tag");
                oldValues.Add(data.AssetTag);
                newValues.Add(request.AssetTag);
            }

            // TAMBAHAN LOGIKA: Lacak Perubahan History untuk AssetSubtype
            if (data.AssetSubtype != request.AssetSubtype)
            {
                fieldNames.Add("Subtype Perangkat");
                oldValues.Add(string.IsNullOrEmpty(data.AssetSubtype) ? "-" : data.AssetSubtype);
                newValues.Add(request.AssetSubtype);
            }

            if (data.Status != request.Status)
            {
                fieldNames.Add("Status");
                oldValues.Add(data.Status);
                newValues.Add(request.Status);
            }

            if (data.Condition != request.Condition)
            {
                fieldNames.Add("Kondisi");
                oldValues.Add(data.Condition);
                newValues.Add(request.Condition);
            }

            if (data.Note != request.Note)
            {
                fieldNames.Add("Catatan");
                oldValues.Add(string.IsNullOrEmpty(data.Note) ? "-" : data.Note);
                newValues.Add(string.IsNullOrEmpty(request.Note) ? "-" : request.Note);
            }

            // JIKA ADA PERUBAHAN FIELD, KEMAS JADI SATU BARIS LOG SAJA
            if (fieldNames.Any())
            {
                await _logService.CreateLogAsync(new CreateAssetHistoryDto
                {
                    AssetId = id,
                    FieldName = string.Join(" | ", fieldNames),
                    OldValue = string.Join(" | ", oldValues),
                    NewValue = string.Join(" | ", newValues),
                    ChangedBy = currentUserId
                });
            }

            // Eksekusi perubahan ke objek database master
            data.AssetTag = request.AssetTag;
            data.AssetName = request.AssetName;
            data.CategoryId = request.CategoryId;
            data.SerialNumber = request.SerialNumber;
            // SIMPAN PERUBAHAN SUBTYPE KE DATABASE MASTER
            data.AssetSubtype = request.AssetSubtype;

            data.LocationId = request.LocationId;
            data.UserId = request.UserId;
            data.AssetType = request.AssetType;
            data.ContractId = request.ContractId;
            data.Status = request.Status;
            data.Condition = request.Condition;
            data.Note = request.Note;
            data.UpdatedAt = DateTime.UtcNow;

            // Simpan gabungan perubahan ke database (Hanya memicu 1 baris di tabel log)
            await _context.SaveChangesAsync();
            return true;
        }

      //Delete
      public async Task<bool> DeleteAsync(int id)
        {
            var data = await _context.Assets
                .FindAsync(id);

            if (data == null)
            {
                return false;
            }

            _context.Assets.Remove(data);
            await _context.SaveChangesAsync();
            return true;

        }

    }
}
