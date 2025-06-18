using Microsoft.EntityFrameworkCore;
using Mx.Persistence.Model;
using Mx.Persistence.Util;
using Mx.TestInt.Util;

namespace Mx.TestInt;

public class UserTestBase(WebApiTestFixture webApiFixture) : WebApiTestBase(webApiFixture)
{
    protected override async ValueTask ImportSeedDataAsync(DatabaseContext context)
    {
        // Erstelle einige Testbenutzer
        var user1 = new User
        {
            Name = "Max Mustermann",
            Age = 28,
            Weight = 75.5,
            Ssn = "123-45-6789"
        };
        
        var user2 = new User
        {
            Name = "Anna Schmidt",
            Age = 32,
            Weight = 62.3,
            Ssn = "987-65-4321"
        };
        
        var user3 = new User
        {
            Name = "Tom Fischer",
            Age = 45,
            Weight = 88.7,
            Ssn = "456-78-9012"
        };
        
        context.Users.Add(user1);
        context.Users.Add(user2);
        context.Users.Add(user3);
        
        await context.SaveChangesAsync();
    }
    
    protected async Task<User> GetUserByNameAsync(string name)
    {
        User? user = null;
        await ModifyDatabaseContentAsync(async context =>
        {
            user = await context.Users.FirstOrDefaultAsync(u => u.Name == name);
        });
        return user ?? throw new InvalidOperationException($"User with name '{name}' not found");
    }
}
