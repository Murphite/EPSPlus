using EPSPlus.Domain.Entities;

namespace EPSPlus.Domain.Interfaces;

public interface IJwtService
{
    public string GenerateToken(ApplicationUser user, IList<string> roles);
}
