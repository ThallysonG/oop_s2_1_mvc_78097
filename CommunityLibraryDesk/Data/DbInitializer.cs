using Bogus;
using CommunityLibraryDesk.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CommunityLibraryDesk.Data
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            await context.Database.MigrateAsync();

            const string adminRole = "Admin";
            const string adminEmail = "admin@library.local";
            const string adminPassword = "Admin123!";

            if (!await roleManager.RoleExistsAsync(adminRole))
            {
                await roleManager.CreateAsync(new IdentityRole(adminRole));
            }

            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new IdentityUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                var createResult = await userManager.CreateAsync(adminUser, adminPassword);
                if (createResult.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, adminRole);
                }
            }

            if (context.Books.Any() || context.Members.Any() || context.Loans.Any())
                return;

            var categories = new[] { "Fiction", "Science", "History", "Technology", "Kids" };

            var bookFaker = new Faker<Book>()
                .RuleFor(b => b.Title, f => f.Lorem.Sentence(3))
                .RuleFor(b => b.Author, f => f.Name.FullName())
                .RuleFor(b => b.Isbn, f => f.Random.ReplaceNumbers("978##########"))
                .RuleFor(b => b.Category, f => f.PickRandom(categories))
                .RuleFor(b => b.IsAvailable, true);

            var books = bookFaker.Generate(20);

            var memberFaker = new Faker<Member>()
                .RuleFor(m => m.FullName, f => f.Name.FullName())
                .RuleFor(m => m.Email, f => f.Internet.Email())
                .RuleFor(m => m.Phone, f => f.Phone.PhoneNumber());

            var members = memberFaker.Generate(10);

            context.Books.AddRange(books);
            context.Members.AddRange(members);
            await context.SaveChangesAsync();

            var random = new Random();
            var loans = new List<Loan>();

            var selectedBooks = books.Take(15).ToList();

            for (int i = 0; i < 15; i++)
            {
                var loanDate = DateTime.Today.AddDays(-random.Next(1, 20));
                var dueDate = loanDate.AddDays(7);

                DateTime? returnedDate = null;

                if (i < 5)
                {
                    returnedDate = dueDate.AddDays(-1);
                    selectedBooks[i].IsAvailable = true;
                }
                else if (i < 10)
                {
                    returnedDate = null;
                    selectedBooks[i].IsAvailable = false;
                }
                else
                {
                    returnedDate = null;
                    dueDate = DateTime.Today.AddDays(-random.Next(1, 7));
                    selectedBooks[i].IsAvailable = false;
                }

                loans.Add(new Loan
                {
                    BookId = selectedBooks[i].Id,
                    MemberId = members[random.Next(members.Count)].Id,
                    LoanDate = loanDate,
                    DueDate = dueDate,
                    ReturnedDate = returnedDate
                });
            }

            context.Loans.AddRange(loans);
            await context.SaveChangesAsync();
        }
    }
}