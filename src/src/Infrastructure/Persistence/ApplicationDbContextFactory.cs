using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuizApp.Infrastructure.Persistence
{
    // concrete implementation of IDesignTimeDbContextFactory<ApplicationDbContext> to create instances of ApplicationDbContext at design time.
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        /*
         Apply the factory pattern to create instances of ApplicationDbContext. 
         Because Normally EF needs the running app to find the connection string, and there's no AppSettings.json file in the Infrastructure project, we need to provide a way to create ApplicationDbContext instances without relying on the running app.
         This factory allows us to create instances of ApplicationDbContext without needing the running app, which is useful for design-time scenarios like migrations and unit tests.
        */
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

            // Create a new instance of ApplicationDbContext with the specified options. The connection string is hardcoded here for design-time purposes, but in a real application, you would typically retrieve it from a configuration file or environment variable.
            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=QuizAppClean;Trusted_Connection=True;TrustServerCertificate=True;");

            return new ApplicationDbContext(optionsBuilder.Options);
        }

    }
}
