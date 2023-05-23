using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace OnlineCafe.Storage
{
    public class Role : IdentityRole<Guid>
    {
        public RoleType Type { get; set; }
        public ICollection<UserRole> Users { get; set; }
    }
    public enum RoleType
    {
        [Display(Name = ApplicationRoleNames.Administrator)]
        Administrator,
        [Display(Name = ApplicationRoleNames.User)]
        User
    }


}
