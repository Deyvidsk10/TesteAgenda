using Agenda.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Agenda.Infrastructure.Factories
{
    public class AgendaDbContextFactory : IDesignTimeDbContextFactory<AgendaDbContext>
    {
        public AgendaDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AgendaDbContext>();

            var connectionString = "Server=(localdb)\\MSSQLLocalDB;Database=AgendaDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True";

            optionsBuilder.UseSqlServer(connectionString);

            return new AgendaDbContext(optionsBuilder.Options);
        }
    }
}
