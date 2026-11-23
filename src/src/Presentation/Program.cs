using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using QuizApp.Application.Interfaces;
using QuizApp.Application.Services;
using QuizApp.Infrastructure.Persistence;
using FluentValidation;
using FluentValidation.AspNetCore;
using QuizApp.Infrastructure.Persistence.Repositories;
using QuizApp.Infrastructure.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddFluentValidationAutoValidation();

builder.Services.AddDbContext<ApplicationDbContext>(
    options => options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"))); // get the directory from appsettings.json

// Register IRepository
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IQuizRepository, QuizRepository>();
builder.Services.AddScoped<IQuizAttemptRepository, QuizAttemptRepository>();

// Register unit of work
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Registe Validators in the Application assembly
builder.Services.AddValidatorsFromAssemblyContaining<QuizApp.Application.Validators.CreateQuestionDtoValidator>();


builder.Services.AddScoped<IQuizService, QuizService>();
builder.Services.AddScoped<IQuizAttemptService, QuizAttemptService>();
builder.Services.AddScoped<IQuizGradingService, QuizGradingService>();

// Add Identity services to the container
builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
    {
        options.Password.RequiredLength = 8;
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = true;

        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.AllowedForNewUsers = true;

        // Enable this after configuring an IEmailSender and confirmation workflow.
        options.SignIn.RequireConfirmedAccount = false;
        options.User.RequireUniqueEmail = true;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddScoped<IUserClaimsPrincipalFactory<ApplicationUser>, ApplicationUserClaimsPrincipalFactory>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.Name = "QuizApp.Auth";
    options.ExpireTimeSpan = TimeSpan.FromHours(8);
    options.SlidingExpiration = true;
    options.LoginPath = "/Identity/Account/Login";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.UseHttpsRedirection();
app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Quizzes}/{action=Index}/{id?}"
).WithStaticAssets();

app.MapRazorPages();

app.Run();