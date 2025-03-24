
using EPSPlus.Domain.Entities;

namespace EPSPlus.Domain.IServices;

public interface IJwtService
{
    public string GenerateToken(ApplicationUser user, IList<string> roles);
}
