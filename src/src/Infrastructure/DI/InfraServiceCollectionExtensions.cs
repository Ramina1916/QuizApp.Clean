using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using QuizApp.Application.Interfaces;
using QuizApp.Infrastructure.Persistence;
using QuizApp.Infrastructure.Persistence.Repositories;

namespace QuizApp.Infrastructure.DI
{
    public static class InfraServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructureServices (this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(
                options => options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"))); // get the directory from appsettings.json

            // Register IRepository
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IQuizRepository, QuizRepository>();
            services.AddScoped<IQuizAttemptRepository, QuizAttemptRepository>();

            // Register unit of work
            services.AddScoped<IUnitOfWork, UnitOfWork>();
        
            return services;
        }
    }
}
