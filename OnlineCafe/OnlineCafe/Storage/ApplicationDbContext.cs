using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace OnlineCafe.Storage
{
	public class ApplicationDbContext
		: IdentityDbContext<User, Role, Guid, IdentityUserClaim<Guid>, UserRole, IdentityUserLogin<Guid>,
			IdentityRoleClaim<Guid>, IdentityUserToken<Guid>>
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
		{
			Database.EnsureCreated();
		}

		public override DbSet<User> Users { get; set; }
		public override DbSet<Role> Roles { get; set; }
		public override DbSet<UserRole> UserRoles { get; set; }
		public DbSet<Address> UserAddress { get; set; }
		public DbSet<Meal> Meals { get; set; }
		public DbSet<Order> Orders { get; set; }
		public DbSet<OrderMeal> OrderMeal { get; set; }

		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);
			builder.Entity<User>(o => { o.ToTable("Users"); });
			builder.Entity<Role>(o => { o.ToTable("Roles"); });
			builder.Entity<UserRole>(o =>
			{
				o.ToTable("UserRoles");
				o.HasOne(x => x.Role)
					.WithMany(x => x.Users)
					.HasForeignKey(x => x.RoleId)
					.OnDelete(DeleteBehavior.Cascade);
				o.HasOne(x => x.User)
					.WithMany(x => x.Roles)
					.HasForeignKey(x => x.UserId)
					.OnDelete(DeleteBehavior.Cascade);
			});

			builder.Entity<Order>(o => { o.HasOne(x => x.User); });

			builder.Entity<OrderMeal>(o =>
			{
				o.ToTable("OrderMeal");
				o.HasOne(x => x.Order).WithMany(x => x.Ordermeals);
				o.HasOne(x => x.Meal).WithMany(x => x.Ordermeals);
			});
		}
	}
}