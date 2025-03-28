

using EPSPlus.Domain.Entities;

namespace EPSPlus.Domain.Interfaces;

public interface IAdminRepository
{
    Task<Admin> AddAdminAsync(Admin admin);
    Task<bool> IsEmailUniqueAsync(string email);
}
