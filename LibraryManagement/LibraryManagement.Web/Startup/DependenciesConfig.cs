using LibraryManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Configuration;
using Microsoft.Extensions.DependencyInjection;
using LibraryManagement.Core.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using LibraryManagement.Core.Constants;

namespace LibraryManagement.Web.Startup
{
    public static class DependenciesConfig
    {
        public static void AddDependencies(this WebApplicationBuilder builder)
        {
            builder.Services.AddControllersWithViews();

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                // Password settings
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequiredLength = 8;

                // Lockout settings
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15); // Khóa tài khoản trong 15 phút
                options.Lockout.MaxFailedAccessAttempts = 5; // // Tài khoản bị khóa sau 5 lần đăng nhập thất bại

                // User settings
                options.User.RequireUniqueEmail = true; // Bắt buộc email không được trùng nhau
                options.SignIn.RequireConfirmedEmail = true; // Bắt buộc xác nhận email trước khi đăng nhập

            }).AddEntityFrameworkStores<ApplicationDbContext>()
              .AddDefaultTokenProviders();

            builder.Services.ConfigureApplicationCookie(options => {
                options.LoginPath = "/Account/Login"; // chưa đăng nhập mà vào trang yêu cầu xác thực thì sẽ chuyển hướng đến trang này
                options.AccessDeniedPath = "/Account/AccessDenied"; //người dùng không có quyền truy cập một trang nào đó, họ sẽ bị chuyển hướng đến /Account/AccessDenied thay vì thấy lỗi 403.
                options.SlidingExpiration = true; // thời gian hết hạn của cookie sẽ được gia hạn sau mỗi lần request
                options.ExpireTimeSpan = TimeSpan.FromHours(1); // thời gian hết hạn của cookie là 1 giờ
            });

            builder.Services.AddAuthentication()
            .AddJwtBearer(options => {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["JwtIssuer"],
                    ValidAudience = builder.Configuration["JwtAudience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(builder.Configuration["JwtSecretKey"]!))
                };
            });

            // Cấu hình Authorization Policies
            builder.Services.AddAuthorization(options => {
                options.AddPolicy(Policies.RequireAdmin,
                    policy => policy.RequireRole(Roles.Admin));

                options.AddPolicy(Policies.RequireLibrarian,
                    policy => policy.RequireRole(Roles.Admin, Roles.Librarian));

                options.AddPolicy(Policies.ManageBooks,
                    policy => policy.RequireClaim(Claims.ManageInventory));

                options.AddPolicy(Policies.ManageUsers,
                    policy => policy.RequireRole(Roles.Admin));

                options.AddPolicy(Policies.BorrowBooks,
                    policy => policy.RequireRole(Roles.Member));
            });
        }
 
    }
}
