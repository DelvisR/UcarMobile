using Microsoft.EntityFrameworkCore;
using UcarMobileApi.Infrastructure.Data;

namespace UcarMobileApi.Tests.TestHelpers;

public static class TestDbContextFactory
{
    public static AppDbContext CreateInMemoryContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;
        return new AppDbContext(options);
    }
}
