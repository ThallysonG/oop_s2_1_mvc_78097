using Bogus;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using oop_s2_1_mvc_78097.Models;

namespace oop_s2_1_mvc_78097.Data
{
    public static class DbInitializer
    {
        public static async Task SeedRolesAndAdminAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            const string adminRole = "Admin";
            const string adminEmail = "admin@library.com";
            const string adminPassword = "Admin123!";

            if (!await roleManager.RoleExistsAsync(adminRole))
            {
                await roleManager.CreateAsync(new IdentityRole(adminRole));
            }

            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(adminUser, adminPassword);

                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    throw new Exception($"Failed to create admin user: {errors}");
                }
            }

            if (!await userManager.IsInRoleAsync(adminUser, adminRole))
            {
                await userManager.AddToRoleAsync(adminUser, adminRole);
            }
        }

        public static async Task SeedLibraryDataAsync(ApplicationDbContext context)
        {
            await context.Database.MigrateAsync();

            if (await context.Books.AnyAsync() ||
                await context.Members.AnyAsync() ||
                await context.Loans.AnyAsync())
            {
                return;
            }

            var categories = new[]
            {
                "Fiction",
                "Science",
                "History",
                "Technology",
                "Education"
            };

            var bookFaker = new Faker<Book>()
                .RuleFor(b => b.Title, f => f.Lorem.Sentence(3, 2).Replace(".", ""))
                .RuleFor(b => b.Author, f => f.Name.FullName())
                .RuleFor(b => b.Isbn, f => f.Random.ReplaceNumbers("978-1-####-#####"))
                .RuleFor(b => b.Category, f => f.PickRandom(categories))
                .RuleFor(b => b.IsAvailable, true);

            var memberFaker = new Faker<Member>()
                .RuleFor(m => m.FullName, f => f.Name.FullName())
                .RuleFor(m => m.Email, (f, m) => f.Internet.Email(m.FullName))
                .RuleFor(m => m.Phone, f => f.Phone.PhoneNumber());

            var books = bookFaker.Generate(20);
            var members = memberFaker.Generate(10);

            await context.Books.AddRangeAsync(books);
            await context.Members.AddRangeAsync(members);
            await context.SaveChangesAsync();

            var random = new Random();

            var selectedBooks = books.OrderBy(_ => random.Next()).Take(15).ToList();
            var loans = new List<Loan>();

            for (int i = 0; i < 15; i++)
            {
                var loanDate = DateTime.Today.AddDays(-random.Next(2, 30));
                var dueDate = loanDate.AddDays(7);

                DateTime? returnedDate = null;

                if (i < 5)
                {
                    returnedDate = dueDate.AddDays(-random.Next(1, 4));
                }
                else if (i < 10)
                {
                    returnedDate = null;
                }
                else
                {
                    returnedDate = null;
                    dueDate = DateTime.Today.AddDays(-random.Next(1, 10));
                }

                loans.Add(new Loan
                {
                    BookId = selectedBooks[i].Id,
                    MemberId = members[random.Next(members.Count)].Id,
                    LoanDate = loanDate,
                    DueDate = dueDate,
                    ReturnedDate = returnedDate
                });

                if (returnedDate == null)
                {
                    selectedBooks[i].IsAvailable = false;
                }
            }

            await context.Loans.AddRangeAsync(loans);
            await context.SaveChangesAsync();
        }
    }
}