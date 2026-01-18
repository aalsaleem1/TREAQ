using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Traeq.Data;
using Traeq.Models;
using Traeq.Areas.Pharmacy.ViewModels;

namespace Traeq.Areas.Pharmacy.Services
{
    public interface IMedicineService
    {
        Task<int> GetPharmacyIdByUserId(string userId);
        Task<Medicine> CreateMedicineAsync(CreateMedicineViewModel model, string userId, string imagePath = null);
        Task<List<Medicine>> GetMedicinesByPharmacyAsync(string userId);
        Task<Medicine> GetByIdAsync(int id, string userId);
        Task UpdateAsync(int id, CreateMedicineViewModel model, string userId, string? imagePath);
        Task ToggleActiveAsync(int id, string userId);
        Task SoftDeleteAsync(int id, string userId);
    }

    public class MedicineService : IMedicineService
    {
        private readonly AppDbContext _context;
        private readonly UserManager<User> _userManager;

        public MedicineService(AppDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<int> GetPharmacyIdByUserId(string userId)
        {
            var pharmacyInfo = await _context.PharmacyLegalInfos
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (pharmacyInfo == null)
                throw new Exception("Pharmacy information not found for this user");

            return pharmacyInfo.Id;
        }

        public async Task<Medicine> CreateMedicineAsync(CreateMedicineViewModel model, string userId, string imagePath = null)
        {
            var pharmacyId = await GetPharmacyIdByUserId(userId);

            var medicine = new Medicine
            {
                MedicineName = model.MedicineName,
                ExpiryDate = model.ExpiryDate,
                MedicineDescription = model.MedicineDescription,
                ScientificName = model.ScientificName,
                ImageURL = imagePath,
                Quantity = model.Quantity,
                Price = model.Price,
                Category = model.Category,
                PharmacyLegalInfoId = pharmacyId,
                CreateDate = DateTime.UtcNow,
                EditDate = DateTime.UtcNow
            };

            _context.Medicines.Add(medicine);
            await _context.SaveChangesAsync();

            return medicine;
        }

        public async Task<List<Medicine>> GetMedicinesByPharmacyAsync(string userId)
        {
            var pharmacyId = await GetPharmacyIdByUserId(userId);

            return await _context.Medicines
                .Include(m => m.PharmacyLegalInfo)
                .Where(m => m.PharmacyLegalInfoId == pharmacyId && m.IsDelete==false)
                .OrderByDescending(m => m.CreateDate)
                .ToListAsync();
        }

        public async Task<Medicine> GetByIdAsync(int id, string userId)
        {
            var pharmacyId = await GetPharmacyIdByUserId(userId);

            var medicine = await _context.Medicines
                .FirstOrDefaultAsync(m => m.Id == id &&
                                          m.PharmacyLegalInfoId == pharmacyId &&
                                          !m.IsDelete);

            if (medicine == null)
                throw new Exception("Medicine Not Found");

            return medicine;
        }

        public async Task UpdateAsync(int id, CreateMedicineViewModel model, string userId, string? imagePath)
        {
            var medicine = await GetByIdAsync(id, userId);

            medicine.MedicineName = model.MedicineName;
            medicine.Price = model.Price;
            medicine.Quantity = model.Quantity;
            medicine.Category = model.Category;
            medicine.ScientificName = model.ScientificName;
            medicine.MedicineDescription = model.MedicineDescription;
            medicine.ExpiryDate = model.ExpiryDate;

            if (!string.IsNullOrEmpty(imagePath))
                medicine.ImageURL = imagePath;

            medicine.EditDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }

        public async Task ToggleActiveAsync(int id, string userId)
        {
            var medicine = await GetByIdAsync(id, userId);
            medicine.IsActive = !medicine.IsActive;
            await _context.SaveChangesAsync();
        }

        public async Task SoftDeleteAsync(int id, string userId)
        {
            var medicine = await GetByIdAsync(id, userId);
            medicine.IsDelete = true;
            await _context.SaveChangesAsync();
        }

    }
}