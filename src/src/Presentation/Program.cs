using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using QuizApp.Application.Interfaces;
using QuizApp.Application.Services;
using QuizApp.Infrastructure.Persistence;
using FluentValidation;
using FluentValidation.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddFluentValidationAutoValidation();

builder.Services.AddDbContext<IApplicationDbContext, ApplicationDbContext>(
    options => options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"))); // get the directory from appsettings.json

// Registe Validators in the Application assembly
builder.Services.AddValidatorsFromAssemblyContaining<QuizApp.Application.Validators.CreateQuestionDtoValidator>();


builder.Services.AddScoped<IQuizService, QuizService>();
builder.Services.AddScoped<IQuizAttemptService, QuizAttemptService>();
builder.Services.AddScoped<IQuizGradingService, QuizGradingService>();

// Add Identity services to the container
builder.Services.AddDefaultIdentity<IdentityUser>(
    options =>
        options.SignIn.RequireConfirmedAccount = false
        )
    .AddEntityFrameworkStores<ApplicationDbContext>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.UseAuthentication();  
app.UseAuthorization();

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();

app.MapStaticAssets();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Quizzes}/{action=Index}/{id?}"
    ).WithStaticAssets();


app.MapRazorPages();

app.Run();
