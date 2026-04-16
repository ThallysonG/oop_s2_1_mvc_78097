using Microsoft.EntityFrameworkCore;
using oop_s2_1_mvc_78097.Data;

namespace oop_s2_1_mvc_78097.Tests
{
    public static class TestDbContextFactory
    {
        public static ApplicationDbContext Create()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new ApplicationDbContext(options);
        }
    }
}