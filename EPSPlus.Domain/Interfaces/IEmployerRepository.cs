

using EPSPlus.Domain.Entities;

namespace EPSPlus.Domain.Interfaces;

public interface IEmployerRepository
{
    Task<Employer> AddEmployerAsync(Employer employer);
    Task<Employer?> GetEmployerByIdAsync(string employerId);
    Task UpdateEmployerAsync(Employer employer);
    Task<bool> IsEmailUniqueAsync(string email);
    Task<bool> IsPhoneUniqueAsync(string phone);
    Task<bool> IsRegistrationNumberUniqueAsync(string registrationNumber);
    Task<bool> IsFullNameUniqueAsync(string name);
}

