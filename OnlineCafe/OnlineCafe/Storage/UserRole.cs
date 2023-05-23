using Microsoft.AspNetCore.Identity;

namespace OnlineCafe.Storage
{
    public class UserRole: IdentityUserRole<Guid>
    {
        public virtual User User { get; set; }
        public virtual Role Role { get; set; }

    }
}
