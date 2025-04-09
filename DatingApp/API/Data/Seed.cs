using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class Seed
{
    public static async Task SeedUsers(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
    {
        if (await userManager.Users.AnyAsync()) return;

        var file = await File.ReadAllTextAsync("Data/UserSeedData.json");
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = false };
        var users = JsonSerializer.Deserialize<List<AppUser>>(file, options);
        if (users == null) return;

        var roles = new List<AppRole>{
            new AppRole{Name = "Member"},
            new AppRole{Name = "Admin"},
            new AppRole{Name = "Moderator"}
        };
        foreach (var role in roles)
        {
            await roleManager.CreateAsync(role);
        }
        foreach (var user in users)
        {
            user.UserName = user.UserName!.ToLower();
            user.Photos[0].IsApproved = true;
            await userManager.CreateAsync(user, "Pa$$w0rd");
            await userManager.AddToRoleAsync(user, "Member");
        }
        var admin = new AppUser
        {
            UserName = "admin",
            KnownAs = "Admin",
            Gender = "",
            City = "",
            Country = ""
        };
        await userManager.CreateAsync(admin, "Pa$$w0rd");
        await userManager.AddToRolesAsync(admin, ["Admin", "Moderator"]);
    }
    public static async Task SeedInterests(DataContext dataContext)
    {
        if (await dataContext.Interests.AnyAsync())
        {
            return;
        }
        string[] stringInterests = ["Music", "Movies", "Reading", "Gym", "Hiking", "Travel", "Yoga", "Painting", "Gaming", "Astrology", "Dancing", "Cooking", "Clubbing"];
        var its = new List<Interest>();
        for (int i = 0; i < stringInterests.Length; i++)
        {
            its.Add(new Interest
            {
                Name = stringInterests[i]
            });
        }
        foreach (var interest in its)
        {
            dataContext.Interests.Add(interest);
        }
        await dataContext.SaveChangesAsync();
    }
    public static async Task AssignInterestsToUsers(DataContext dataContext)
    {
        if (!await dataContext.Interests.AnyAsync() || !await dataContext.Users.AnyAsync())
        {
            Console.WriteLine("Table is empty");
            return;
        }
        if (await dataContext.UserInterests.AnyAsync())
        {
            return;
        }
        var interests = await dataContext.Interests.ToListAsync();
        var users = await dataContext.Users.ToListAsync();
        if (users.Count == 0)
        {
            Console.WriteLine("Could not find the data needed to assign interests to users");
            return;
        }
        var userInterests = new List<AppUserInterest>();
        var random = new Random();
        foreach (var user in users)
        {
            var numberOfInterests = 5;
            var selectedInterests = interests.OrderBy(i => random.Next()).Take(numberOfInterests).ToList();
            foreach (var interest in selectedInterests)
            {
                userInterests.Add(new AppUserInterest
                {
                    UserId = user.Id,
                    InterestId = interest.Id
                });
            }
        }
        dataContext.AddRange(userInterests);
        await dataContext.SaveChangesAsync();
    }
}