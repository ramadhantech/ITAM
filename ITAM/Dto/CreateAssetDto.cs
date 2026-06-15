using System.ComponentModel.DataAnnotations;

namespace ITAM.Dto;

public class CreateAssetDto
{
    [Required(ErrorMessage = "Asset Tag wajib diisi.")]
    [StringLength(50, ErrorMessage = "Asset Tag tidak boleh lebih dari 50 karakter.")]
    public string AssetTag { get; set; } = string.Empty;

    [Required(ErrorMessage = "Nama Asset wajib diisi.")]
    [StringLength(255, ErrorMessage = "Nama Asset tidak boleh lebih dari 255 karakter.")]
    public string AssetName { get; set; } = string.Empty;

    public string? SerialNumber { get; set; }
    public int? VendorId { get; set; }
    public int? ContractId { get; set; }

    [Required(ErrorMessage = "Kategori wajib dipilih.")]
    [Range(1, int.MaxValue, ErrorMessage = "Kategori tidak valid.")]
    public int CategoryId { get; set; }

    // TAMBAHKAN VALIDASI SUBTYPE DI SINI
    [Required(ErrorMessage = "Subtype Perangkat wajib dipilih.")]
    [StringLength(100, ErrorMessage = "Subtype Perangkat tidak boleh lebih dari 100 karakter.")]
    public string AssetSubtype { get; set; } = string.Empty;

    [Required(ErrorMessage = "Lokasi wajib dipilih.")]
    [Range(1, int.MaxValue, ErrorMessage = "Lokasi tidak valid.")]
    public int LocationId { get; set; }

    [Required(ErrorMessage = "User wajib dipilih.")]
    [Range(1, int.MaxValue, ErrorMessage = "User tidak valid.")]
    public int UserId { get; set; }

    [Required(ErrorMessage = "Tipe Asset wajib diisi.")]
    public string AssetType { get; set; } = string.Empty;

    [Required(ErrorMessage = "Status wajib diisi.")]
    public string Status { get; set; } = string.Empty;

    [Required(ErrorMessage = "Kondisi wajib diisi.")]
    public string Condition { get; set; } = string.Empty;

    public string Note { get; set; } = string.Empty;
}
