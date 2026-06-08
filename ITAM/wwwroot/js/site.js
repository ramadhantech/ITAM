// ============================================================================
// [GLOBAL CONFIGURATION] MATRIKS PEMETAAN SUBTYPE FORMULIR PERTAMINA
// ============================================================================
const subtypeMappingGlobal = {
    "3": ["Indoor", "Outdoor", "PTZ", "Explosion"], // CCTV
    "7": ["Radio BS", "Radio Mobil", "Radio IS", "Radio BS Marine", "Radio IS Marine"], // HT
    "8": ["Colour", "B/W"], // Printer
    "9": ["HP Explosion", "Tablet Explosion", "HP Satelite"], // Device Mobile
    "10": ["Switch", "Access Point", "PABX", "Tlp Analog", "Tlp Digital"], // Network
    "11": ["PC Desktop", "Laptop Type 2", "Laptop Type 3", "Mini PC"], // Komputer
    "12": ["TV", "Projector", "Camera", "LED", "Speaker", "Speaker Portable", "Mic Wireless", "Mic Meja", "Mixer", "Amplifier", "Pointer"] // Multimedia
};

// Variabel penampung indeks baris tabel dan data payload AJAX
let rowIndex = 0;
let allAssetsJson = [];


// ============================================================================
// [INITIALIZER LOGIC] BERJALAN OTOMATIS SAAT HALAMAN SELESAI DIMUAT
// ============================================================================
document.addEventListener("DOMContentLoaded", function () {

    // milik user asset lokasi
    

    // MIKIK ASSET MODUL: Event Listener Dropdown Kategori Halaman Create/Edit Asset
    // ------------------------------------------------------------------------
    const assetCategorySelect = document.getElementById('categorySelect');
    if (assetCategorySelect) {
        assetCategorySelect.addEventListener('change', function () {
            const subtypeSelect = document.getElementById('subtypeSelect');
            if (!subtypeSelect) return;

            const selectedText = this.options[this.selectedIndex].text.trim().toUpperCase();
            subtypeSelect.innerHTML = '<option value="">-- Pilih Subtype Perangkat --</option>';

            let mappedKey = "";
            if (selectedText.includes("CCTV")) mappedKey = "3";
            else if (selectedText.includes("HT") || selectedText.includes("TALKIE") || selectedText.includes("HANDY")) mappedKey = "7";
            else if (selectedText.includes("PRINT")) mappedKey = "8";
            else if (selectedText.includes("MOBILE") || selectedText.includes("DEVICE")) mappedKey = "9";
            else if (selectedText.includes("NET")) mappedKey = "10";
            else if (selectedText.includes("KOMP") || selectedText.includes("LAP")) mappedKey = "11";
            else if (selectedText.includes("MULTI")) mappedKey = "12";

            if (mappedKey && subtypeMappingGlobal[mappedKey]) {
                subtypeSelect.disabled = false;
                subtypeMappingGlobal[mappedKey].forEach(sub => {
                    const option = document.createElement('option');
                    option.value = sub;
                    option.textContent = sub;
                    subtypeSelect.appendChild(option);
                });
            } else {
                subtypeSelect.disabled = true;
            }
        });
    }

    // ------------------------------------------------------------------------
    // MILIK ASSET MODUL : FILTER USER BERDASARKAN LOKASI
    // ------------------------------------------------------------------------
    const assetLocationSelect =
        document.getElementById('locationSelect');

    if (assetLocationSelect) {

        assetLocationSelect.addEventListener(
            'change',
            async function () {

                const locationId = this.value;

                const userSelect =
                    document.getElementById('userSelect');

                if (!userSelect)
                    return;

                userSelect.innerHTML =
                    '<option value="">-- Pilih User --</option>';

                if (!locationId)
                    return;

                try {

                    const response =
                        await fetch(
                            `/Asset/GetUsersByLocation?locationId=${locationId}`
                        );

                    if (response.ok) {

                        const users =
                            await response.json();

                        users.forEach(user => {

                            const option =
                                document.createElement('option');

                            option.value = user.id;
                            option.textContent = user.name;

                            userSelect.appendChild(option);

                        });

                    }

                }
                catch (error) {

                    console.error(
                        "Gagal mengambil user lokasi:",
                        error
                    );

                }

            });
    }

    // Milik halaman createt inpection filter lokasi user
    $(document).ready(function () {

        $("#LocationId").change(function () {

            let locationId = $(this).val();

            $("#approvalUserSelect").empty();
            $("#approvalUserSelect").append(
                '<option value="">-- Pilih PIC Approval --</option>'
            );

            if (locationId) {

                $.get(
                    '/Inspection/GetUsersByLocation',
                    { locationId: locationId },
                    function (data) {

                        $.each(data, function (i, user) {

                            $("#approvalUserSelect").append(
                                `<option value="${user.id}">
                                ${user.name}
                             </option>`
                            );

                        });

                    }
                );

            }

        });

    });
    // ------------------------------------------------------------------------
    // MILIK INSPECTION MODUL: Event Listener Dropdown Lokasi Kantor (AJAX)
    // ------------------------------------------------------------------------
    const inspectionLocationSelect = document.getElementById('LocationId');
    if (inspectionLocationSelect) {
        inspectionLocationSelect.addEventListener('change', async function () {
            const locationId = this.value;
            const tbody = document.getElementById('detailBody');

            if (tbody) tbody.innerHTML = '';
            rowIndex = 0;
            allAssetsJson = [];

            if (!locationId) return;

            try {
                // Eksekusi AJAX Fetch API untuk mengambil data aset spesifik lokasi kantor tersebut
                const response = await fetch(`/Inspection/GetAssetsByLocation?locationId=${locationId}`);
                if (response.ok) {
                    allAssetsJson = await response.json();

                    // Otomatis generate baris isian pertama demi UX yang responsif
                    window.addRow();
                }
            } catch (error) {
                console.error("Gagal menarik payload inventaris aset:", error);
            }
        });
    }

    // Mengosongkan tabel detail pemeriksaan di awal halaman inspection dimuat
    const tbody = document.getElementById('detailBody');
    if (tbody) tbody.innerHTML = '';
});


// ============================================================================
// [INSPECTION MODUL ENGINE] DAFTAR FUNGSI KHUSUS HALAMAN CREATE INSPECTION REPORT
// ============================================================================

// 1. MILIK INSPECTION: Fungsi menambah baris dinamis baru ke tabel pemeriksaan
window.addRow = function () {
    const tbody = document.getElementById('detailBody');
    if (!tbody) return;

    const tr = document.createElement('tr');
    tr.id = `row-${rowIndex}`;

    tr.innerHTML = `
        <td>
            <select class="form-select form-select-sm category-filter cat-sel" onchange="window.onCategoryChange(${rowIndex})" required>
                <option value="">-- Kategori --</option>
                <option value="1">CCTV</option>
                <option value="2">HT</option>
                <option value="3">Printer</option>
                <option value="4">Device Mobile</option>
                <option value="5">Network</option>
                <option value="6">Komputer</option>
                <option value="7">Multimedia</option>
            </select>
        </td>
        <td>
            <select class="form-select form-select-sm subtype-filter sub-sel" id="subtype-${rowIndex}" onchange="window.onSubtypeChange(${rowIndex})" disabled required>
                <option value="">-- Subtype --</option>
            </select>
        </td>
        <td>
            <select name="Details[${rowIndex}].AssetId" id="asset-${rowIndex}" class="form-select form-select-sm ast-sel" disabled required>
                <option value="">-- Pilih Perangkat --</option>
            </select>
        </td>
        <td>
            <select name="Details[${rowIndex}].Condition" class="form-select form-select-sm cond-sel" required>
                <option value="Good">Good</option>
                <option value="Warning">Warning</option>
                <option value="Bad">Bad</option>
            </select>
        </td>
        <td>
            <select name="Details[${rowIndex}].Status" class="form-select form-select-sm stat-sel" required>
                <option value="Active">Active</option>
                <option value="Repair">Repair</option>
                <option value="Retired">Retired</option>
            </select>
        </td>
        <td>
            <input name="Details[${rowIndex}].Notes" class="form-control form-control-sm note-inp" placeholder="Catatan kondisi..." />
        </td>
        <td class="text-center">
            <button type="button" class="btn btn-outline-danger btn-sm delete-btn" onclick="window.removeRow(${rowIndex})">
                <i class="bi bi-trash"></i>
            </button>
        </td>
    `;

    tbody.appendChild(tr);
    rowIndex++;
}

// 2. MILIK INSPECTION: Fungsi merender opsi sub-tipe statis Pertamina saat dropdown Kategori diubah
window.onCategoryChange = function (index) {
    const row = document.getElementById(`row-${index}`);
    if (!row) return;

    const categorySelect = row.querySelector('.cat-sel');
    const subtypeSelect = document.getElementById(`subtype-${index}`);
    const assetSelect = document.getElementById(`asset-${index}`);

    const categoryText = categorySelect.options[categorySelect.selectedIndex].text.trim().toUpperCase();
    let mappedCategoryId = categorySelect.value;

    if (categoryText.includes("CCTV")) mappedCategoryId = "3";
    else if (categoryText.includes("HT"))  mappedCategoryId = "7";
    else if (categoryText.includes("PRINT")) mappedCategoryId = "8";
    else if (categoryText.includes("MOBILE") || categoryText.includes("DEVICE")) mappedCategoryId = "9";
    else if (categoryText.includes("NET")) mappedCategoryId = "10";
    else if (categoryText.includes("KOMP") || categoryText.includes("LAP")) mappedCategoryId = "11";
    else if (categoryText.includes("MULTI")) mappedCategoryId = "12";

    subtypeSelect.innerHTML = '<option value="">-- Subtype --</option>';
    assetSelect.innerHTML = '<option value="">-- Pilih Perangkat --</option>';
    assetSelect.disabled = true;

    if (mappedCategoryId && subtypeMappingGlobal[mappedCategoryId]) {
        subtypeSelect.disabled = false;
        subtypeSelect.setAttribute('data-db-cat-id', categorySelect.value);
        subtypeSelect.setAttribute('data-cat-text', categoryText);

        subtypeMappingGlobal[mappedCategoryId].forEach(sub => {
            const opt = document.createElement('option');
            opt.value = sub;
            opt.textContent = sub;
            subtypeSelect.appendChild(opt);
        });
    } else {
        subtypeSelect.disabled = true;
    }
}

// 3. MILIK INSPECTION: Fungsi menyaring data array memori browser dan memunculkan nama barang fisik
window.onSubtypeChange = function (index) {
    const subtypeSelect = document.getElementById(`subtype-${index}`);
    const assetSelect = document.getElementById(`asset-${index}`);
    if (!subtypeSelect || !assetSelect) return;

    const targetCatText = (subtypeSelect.getAttribute('data-cat-text') || "").toLowerCase().trim();
    const targetSubtype = subtypeSelect.value ? subtypeSelect.value.toString().trim().toLowerCase() : "";

    assetSelect.innerHTML = '<option value="">-- Pilih Perangkat --</option>';

    if (!targetSubtype || !targetCatText) {
        assetSelect.disabled = true;
        return;
    }

    // Proses komparasi teks nama kategori murni bebas eror ID database melompat
    const filteredAssets = allAssetsJson.filter(a => {
        const dbCatName = (a.categoryname || "").toString().trim().toLowerCase();
        const dbSubtype = (a.subtype || "").toString().trim().toLowerCase();
        return dbCatName.includes(targetCatText) && dbSubtype === targetSubtype;
    });

    if (filteredAssets.length > 0) {
        assetSelect.disabled = false; // Buka status disable dropdown Nama Perangkat
        filteredAssets.forEach(asset => {
            const opt = document.createElement('option');
            opt.value = asset.id;
            opt.textContent = `${asset.name} (${asset.tag})`;
            assetSelect.appendChild(opt);
        });
    } else {
        assetSelect.innerHTML = '<option value="">-- Tidak ada aset cocok --</option>';
        assetSelect.disabled = true;
    }
}

// 4. MILIK INSPECTION: Fungsi menghapus baris isian tertentu pada tabel detail
window.removeRow = function (index) {
    const row = document.getElementById(`row-${index}`);
    if (row) {
        row.remove();
        reIndexRows(); // Susun ulang indeks name array agar POST data DTO ke controller valid
    }
}

// 5. MILIK INSPECTION: Fungsi internal menyusun ulang urutan array properti form C# DTO Binder
function reIndexRows() {
    const rows = document.querySelectorAll('#detailBody tr');
    let i = 0;
    rows.forEach((row) => {
        row.id = `row-${i}`;


        const catSelect = row.querySelector('.cat-sel');
        if (catSelect) catSelect.setAttribute('onchange', `window.onCategoryChange(${i})`);

        const subSelect = row.querySelector('.sub-sel');
        if (subSelect) {
            subSelect.id = `subtype-${i}`;
            subSelect.setAttribute('onchange', `window.onSubtypeChange(${i})`);
        }

        const assetSelect = row.querySelector('.ast-sel');
        if (assetSelect) {
            assetSelect.id = `asset-${i}`;
            assetSelect.name = `Details[${i}].AssetId`;
        }

        const conditionSelect = row.querySelector('.cond-sel');
        if (conditionSelect) conditionSelect.name = `Details[${i}].Condition`;

        const statusSelect = row.querySelector('.stat-sel');
        if (statusSelect) statusSelect.name = `Details[${i}].Status`;

        const notesInput = row.querySelector('.note-inp');
        if (notesInput) notesInput.name = `Details[${i}].Notes`;

        const deleteBtn = row.querySelector('.delete-btn');
        if (deleteBtn) deleteBtn.setAttribute('onclick', `window.removeRow(${i})`);

        i++;
    });
    rowIndex = i;
}
