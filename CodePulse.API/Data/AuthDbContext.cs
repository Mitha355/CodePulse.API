using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CodePulse.API.Data
{
    public class AuthDbContext:IdentityDbContext
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> options):base(options)
        {            
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            //Create Reader & Writer Roles
            var readerRoleId = "36cbe211-3594-496b-83e6-ccfd65ae233c";
            var writerRoleId = "b4117269-5e80-415d-aa1e-569d28515549";

            var roles = new List<IdentityRole> {
                new IdentityRole()
                {
                    Id= readerRoleId,
                    Name="Reader",
                    NormalizedName="Reader".ToUpper(),
                    ConcurrencyStamp=readerRoleId
                },
                new IdentityRole()
                {
                    Id= writerRoleId,
                    Name="Writer",
                    NormalizedName="Writer".ToUpper(),
                    ConcurrencyStamp=writerRoleId
                }
            };
            //Seed the roles
            builder.Entity<IdentityRole>().HasData(roles);

            //Create an admin user
            var adminUserId = "cffcfb3d-cb6a-4f6c-aa7a-6c290d1cc3ff";
            var admin = new IdentityUser()
            {
                Id=adminUserId,
                UserName="admin@codepulse.com",
                Email="admin@codepulse.com",
                NormalizedEmail= "admin@codepulse.com".ToUpper(),
                NormalizedUserName= "admin@codepulse.com".ToUpper()
            };

            admin.PasswordHash = new PasswordHasher<IdentityUser>().HashPassword(admin, "Admin@123");
            builder.Entity<IdentityRole>().HasData(admin);

            //Give roles to admin user

            var adminRoles = new List<IdentityUserRole<string>>
            {
                new()
                {
                    UserId=adminUserId,
                    RoleId=readerRoleId
                },
                new()
                {
                    UserId=adminUserId,
                    RoleId=writerRoleId
                }
            };
            builder.Entity<IdentityUserRole<string>>().HasData(adminRoles);
        }
        
    }
}
