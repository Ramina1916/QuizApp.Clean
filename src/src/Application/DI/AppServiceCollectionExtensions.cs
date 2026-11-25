using Microsoft.Extensions.DependencyInjection;
using FluentValidation;
using QuizApp.Application.Interfaces;
using QuizApp.Application.Services;
using QuizApp.Application.Validators;

namespace QuizApp.Application.DI
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddValidatorsFromAssemblyContaining<CreateQuestionDtoValidator>();

            services.AddScoped<IQuizService, QuizService>();
            services.AddScoped<IQuizAttemptService, QuizAttemptService>();
            services.AddScoped<IQuizGradingService, QuizGradingService>();

            return services;
        }
    }
}