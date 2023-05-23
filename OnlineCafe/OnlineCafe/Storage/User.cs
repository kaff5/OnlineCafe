using Microsoft.AspNetCore.Identity;

namespace OnlineCafe.Storage
{
    public class User : IdentityUser<Guid>
    {
        public string Name { get; set; }
        public DateTime BirthDate { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public ICollection<UserRole> Roles { get; set; }
    }
}
