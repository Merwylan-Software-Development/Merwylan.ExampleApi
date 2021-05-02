using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Merwylan.ExampleApi.Persistence
{
    public class DesignTimeContextFactory : IDesignTimeDbContextFactory<ExampleContext>
    {
        public ExampleContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ExampleContext>();
            optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDb;Database=Datebase;Trusted_Connection=True;");

            return new ExampleContext(optionsBuilder.Options);
        }
    }
}
