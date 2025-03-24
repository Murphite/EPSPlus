

using EPSPlus.Domain.Entities;

namespace EPSPlus.Domain.IRepositories;

public interface IEmployerRepository
{
    Task<Employer> RegisterEmployerAsync(Employer employer);
    Task<Employer?> GetEmployerByIdAsync(string employerId);
    Task UpdateEmployerAsync(Employer employer);
}

