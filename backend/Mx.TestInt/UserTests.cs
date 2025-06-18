using System.Net;
using System.Net.Http.Json;
using Mx.Requests;
using Mx.Responses;
using Mx.TestInt.Util;

namespace Mx.TestInt;

public class UserTests(WebApiTestFixture webApiFixture) : UserTestBase(webApiFixture)
{
    [Fact]
    public async Task GetAllUsers_ShouldReturnAllUsers()
    {
        // Act
        var response = await ApiClient.GetAsync("/api/User");
        
        // Assert
        response.EnsureSuccessStatusCode();
        var users = await response.Content.ReadFromJsonAsync<List<UserResponse>>(JsonOptions);
        
        Assert.NotNull(users);
        Assert.Equal(3, users!.Count);
        Assert.Contains(users, u => u.Name == "Max Mustermann");
        Assert.Contains(users, u => u.Name == "Anna Schmidt");
        Assert.Contains(users, u => u.Name == "Tom Fischer");
    }
    
    [Fact]
    public async Task GetUserById_WithExistingId_ShouldReturnUser()
    {
        // Arrange
        var user = await GetUserByNameAsync("Anna Schmidt");
        
        // Act
        var response = await ApiClient.GetAsync($"/api/User/{user.Ssn}");
        
        // Assert
        response.EnsureSuccessStatusCode();
        var returnedUser = await response.Content.ReadFromJsonAsync<UserResponse>(JsonOptions);
        
        Assert.NotNull(returnedUser);
        Assert.Equal(user.Ssn, returnedUser!.Ssn);
        Assert.Equal(user.Name, returnedUser.Name);
        Assert.Equal(user.Age, returnedUser.Age);
        Assert.Equal(user.Weight, returnedUser.Weight);
    }
    
    [Fact]
    public async Task GetUserById_WithNonExistingId_ShouldReturnNotFound()
    {
        // Act
        var response = await ApiClient.GetAsync("/api/User/999");
        
        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
    
    [Fact]
    public async Task CreateUser_WithValidData_ShouldCreateAndReturnUser()
    {
        // Arrange
        var request = new UserRequest
        {
            Name = "New Test User",
            Age = 25,
            Weight = 70.5,
            Ssn = "111-22-3333"
        };
        
        // Act
        var response = await ApiClient.PostAsJsonAsync("/api/User", request, JsonOptions);
        
        // Assert
        response.EnsureSuccessStatusCode();
        var createdUser = await response.Content.ReadFromJsonAsync<UserResponse>(JsonOptions);
        
        Assert.NotNull(createdUser);
        Assert.Equal(request.Name, createdUser!.Name);
        Assert.Equal(request.Age, createdUser.Age);
        Assert.Equal(request.Weight, createdUser.Weight);
        
        // Verify it was actually saved to the database
        var user = await GetUserByNameAsync("New Test User");
        Assert.Equal(createdUser.Ssn, user.Ssn);
    }
    
    [Fact]
    public async Task CreateUser_WithInvalidData_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new UserRequest
        {
            Name = "", // Empty name should fail validation
            Age = 25,
            Weight = 70.5,
            Ssn = "111-22-3333"
        };
        
        // Act
        var response = await ApiClient.PostAsJsonAsync("/api/User", request, JsonOptions);
        
        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
    
    [Fact]
    public async Task DeleteUser_WithExistingId_ShouldRemoveUser()
    {
        // Arrange
        var request = new UserRequest
        {
            Name = "User To Delete",
            Age = 40,
            Weight = 85.0,
            Ssn = "444-55-6666"
        };
        
        var createResponse = await ApiClient.PostAsJsonAsync("/api/User", request, JsonOptions);
        createResponse.EnsureSuccessStatusCode();
        var createdUser = await createResponse.Content.ReadFromJsonAsync<UserResponse>(JsonOptions);
        
        // Act
        var response = await ApiClient.DeleteAsync($"/api/User/{createdUser!.Ssn}");
        
        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        
        // Verify it was actually removed from the database
        var getResponse = await ApiClient.GetAsync($"/api/User/{createdUser.Ssn}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }
    
    [Fact]
    public async Task DeleteUser_WithNonExistingId_ShouldReturnNotFound()
    {
        // Act
        var response = await ApiClient.DeleteAsync("/api/User/999");
        
        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
