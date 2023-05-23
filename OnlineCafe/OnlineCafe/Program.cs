using OnlineCafe.Services;
using OnlineCafe.Services.Configuratons;
using OnlineCafe.Storage;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages().AddRazorRuntimeCompilation();

CleanerConfig cleaner = new CleanerConfig();
builder.Configuration.Bind("ForCleaner", cleaner);


builder.Services.AddScoped<IUsersService, UsersService>();
builder.Services.AddScoped<IMenuService, MenuService>();
builder.Services.AddScoped<IOrdersService, OrdersService>();
builder.Services.AddScoped<IOrdersManagementService, OrdersManagementService>();
builder.Services.AddSingleton<ICartService, CartService>();
builder.Services.AddSingleton(cleaner);
builder.Services.AddHostedService<CartCleaner>();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))); // установление подключения к SQL серверу с использованием строки подключения из конфига




builder.Services.AddIdentity<User, Role>() // Добавление identity к проекту
    .AddEntityFrameworkStores<ApplicationDbContext>() // указание контекста
    .AddSignInManager<SignInManager<User>>() // явное указание того, что менеджер авторизации должен работать с переопределенной моделью пользователя
    .AddUserManager<UserManager<User>>() // аналогично для менеджера юзеров
    .AddRoleManager<RoleManager<Role>>(); // аналогично для менеджера ролей
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(); // добавление cookie аутентификации в проект (стандартная модель аутентификации в MVC проектах с Identity)
var app = builder.Build();
using var serviceScope = app.Services.CreateScope();
var context = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();
// auto migration
//context.Database.Migrate();
await app.ConfigureIdentityAsync();

app.UseAuthentication();
app.UseAuthorization();







if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Menu}/{action=Index}/{id?}");

app.Run();
