using Microsoft.EntityFrameworkCore;
using BPMS_DAL;

namespace BPMS_Tests.Factories
{
    public static class DbContextInMemoryFactory
    {
        public static BpmsDbContext CreateDbContext(string databseName)
        {
            DbContextOptionsBuilder<BpmsDbContext> contextOptionsBuilder = new DbContextOptionsBuilder<BpmsDbContext>();
            contextOptionsBuilder.UseInMemoryDatabase(databseName);
            BpmsDbContext context = new BpmsDbContext(contextOptionsBuilder.Options);
            context.Database.EnsureCreated();

            return context;
        }
    }
}