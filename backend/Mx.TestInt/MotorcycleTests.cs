using System.Net;
using System.Net.Http.Json;
using Mx.Requests;
using Mx.Responses;
using Mx.TestInt.Util;

namespace Mx.TestInt;

public class MotorcycleTests(WebApiTestFixture webApiFixture) : MotorcycleTestBase(webApiFixture)
{
    [Fact]
    public async Task GetAllMotorcycles_ShouldReturnAllMotorcycles()
    {
        // Act
        var response = await ApiClient.GetAsync("/api/Motorcycle");
        
        // Assert
        response.EnsureSuccessStatusCode();
        var motorcycles = await response.Content.ReadFromJsonAsync<List<MotorcycleResponse>>(JsonOptions);
        
        Assert.NotNull(motorcycles);
        Assert.Equal(3, motorcycles!.Count);
        Assert.Contains(motorcycles, m => m.Model == "Honda CBR");
        Assert.Contains(motorcycles, m => m.Model == "Kawasaki Ninja");
        Assert.Contains(motorcycles, m => m.Model == "Ducati Monster");
    }
    
    [Fact]
    public async Task GetMotorcycleById_WithExistingId_ShouldReturnMotorcycle()
    {
        // Arrange
        var motorcycle = await GetMotorcycleByModelAsync("Honda CBR");
        
        // Act
        var response = await ApiClient.GetAsync($"/api/Motorcycle/{motorcycle.Id}");
        
        // Assert
        response.EnsureSuccessStatusCode();
        var returnedMotorcycle = await response.Content.ReadFromJsonAsync<MotorcycleResponse>(JsonOptions);
        
        Assert.NotNull(returnedMotorcycle);
        Assert.Equal(motorcycle.Id, returnedMotorcycle!.Id);
        Assert.Equal(motorcycle.Model, returnedMotorcycle.Model);
        Assert.Equal(motorcycle.Number, returnedMotorcycle.Number);
        Assert.Equal(motorcycle.Horsepower, returnedMotorcycle.Horsepower);
        Assert.Equal(motorcycle.Track?.Id, returnedMotorcycle.TrackId);
    }
    
    [Fact]
    public async Task GetMotorcycleById_WithNonExistingId_ShouldReturnNotFound()
    {
        // Act
        var response = await ApiClient.GetAsync("/api/Motorcycle/999");
        
        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
    
    [Fact]
    public async Task GetAvailableMotorcycles_ShouldReturnOnlyAvailableMotorcycles()
    {
        // Act
        var response = await ApiClient.GetAsync("/api/Motorcycle/available");
        
        // Assert
        response.EnsureSuccessStatusCode();
        var motorcycles = await response.Content.ReadFromJsonAsync<List<MotorcycleResponse>>(JsonOptions);
        
        Assert.NotNull(motorcycles);
        Assert.Equal(2, motorcycles!.Count);
        Assert.Contains(motorcycles, m => m.Model == "Honda CBR");
        Assert.Contains(motorcycles, m => m.Model == "Kawasaki Ninja");
        Assert.DoesNotContain(motorcycles, m => m.Model == "Ducati Monster");  // Bereits ausgeliehen
    }
    
    [Fact]
    public async Task CreateMotorcycle_WithValidData_ShouldCreateAndReturnMotorcycle()
    {
        // Arrange
        var track = await GetTrackByNameAsync("Motorcycle Easy Track");
        var request = new MotorcycleRequest
        {
            Model = "New Test Motorcycle",
            Number = "TEST-001",
            Horsepower = 120,
            TrackId = track.Id
        };
        
        // Act
        var response = await ApiClient.PostAsJsonAsync("/api/Motorcycle", request, JsonOptions);
        
        // Assert
        response.EnsureSuccessStatusCode();
        var createdMotorcycle = await response.Content.ReadFromJsonAsync<MotorcycleResponse>(JsonOptions);
        
        Assert.NotNull(createdMotorcycle);
        Assert.Equal(request.Model, createdMotorcycle!.Model);
        Assert.Equal(request.Number, createdMotorcycle.Number);
        Assert.Equal(request.Horsepower, createdMotorcycle.Horsepower);
        Assert.Equal(request.TrackId, createdMotorcycle.TrackId);
        
        // Verify it was actually saved to the database
        var motorcycle = await GetMotorcycleByModelAsync("New Test Motorcycle");
        Assert.Equal(createdMotorcycle.Id, motorcycle.Id);
    }
    
    [Fact]
    public async Task CreateMotorcycle_WithInvalidTrackId_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new MotorcycleRequest
        {
            Model = "Invalid Track Motorcycle",
            Number = "INV-001",
            Horsepower = 120,
            TrackId = 999  // Nicht existierende Track-ID
        };
        
        // Act
        var response = await ApiClient.PostAsJsonAsync("/api/Motorcycle", request, JsonOptions);
        
        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
    
    [Fact]
    public async Task DeleteMotorcycle_WithExistingId_ShouldRemoveMotorcycle()
    {
        // Arrange
        var motorcycle = await GetMotorcycleByModelAsync("Honda CBR");
        
        // Act
        var response = await ApiClient.DeleteAsync($"/api/Motorcycle/{motorcycle.Id}");
        
        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        
        // Verify it was actually removed from the database
        var getResponse = await ApiClient.GetAsync($"/api/Motorcycle/{motorcycle.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }
    
    [Fact]
    public async Task DeleteMotorcycle_WithNonExistingId_ShouldReturnNotFound()
    {
        // Act
        var response = await ApiClient.DeleteAsync("/api/Motorcycle/999");
        
        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
    
    [Fact]
    public async Task RentMotorcycle_WithValidData_ShouldRentMotorcycleToUser()
    {
        // Arrange
        var motorcycle = await GetMotorcycleByModelAsync("Kawasaki Ninja");
        var user = await GetUserByNameAsync("Motorcycle Test User");
        
        // Act
        var response = await ApiClient.PostAsync($"/api/Motorcycle/{motorcycle.Id}/rent?userId={user.Ssn}", null);
        
        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        
        // Verify the motorcycle is now rented
        var updatedMotorcycle = await GetMotorcycleByModelAsync("Kawasaki Ninja");
        Assert.NotNull(updatedMotorcycle.RentedBySsn);
        Assert.Equal(user.Ssn, updatedMotorcycle.RentedBySsn);
        
        // Check that it's no longer in the available list
        var availableResponse = await ApiClient.GetAsync("/api/Motorcycle/available");
        availableResponse.EnsureSuccessStatusCode();
        var availableMotorcycles = await availableResponse.Content.ReadFromJsonAsync<List<MotorcycleResponse>>(JsonOptions);
        
        Assert.DoesNotContain(availableMotorcycles!, m => m.Id == motorcycle.Id);
    }
    
    [Fact]
    public async Task RentMotorcycle_AlreadyRented_ShouldReturnBadRequest()
    {
        // Arrange
        var motorcycle = await GetMotorcycleByModelAsync("Ducati Monster");  // Bereits ausgeliehen
        var user = await GetUserByNameAsync("Motorcycle Test User");
        
        // Act
        var response = await ApiClient.PostAsync($"/api/Motorcycle/{motorcycle.Id}/rent?userId={user.Ssn}", null);
        
        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
    
    [Fact]
    public async Task ReturnMotorcycle_WithRentedMotorcycle_ShouldMakeMotorcycleAvailable()
    {
        // Arrange
        var motorcycle = await GetMotorcycleByModelAsync("Ducati Monster");  // Bereits ausgeliehen
        
        // Act
        var response = await ApiClient.PostAsync($"/api/Motorcycle/{motorcycle.Id}/return", null);
        
        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        
        // Verify the motorcycle is now available
        var updatedMotorcycle = await GetMotorcycleByModelAsync("Ducati Monster");
        Assert.Null(updatedMotorcycle.RentedBySsn);
        
        // Check that it's in the available list
        var availableResponse = await ApiClient.GetAsync("/api/Motorcycle/available");
        availableResponse.EnsureSuccessStatusCode();
        var availableMotorcycles = await availableResponse.Content.ReadFromJsonAsync<List<MotorcycleResponse>>(JsonOptions);
        
        Assert.Contains(availableMotorcycles!, m => m.Id == motorcycle.Id);
    }
    
    [Fact]
    public async Task ReturnMotorcycle_NotRented_ShouldReturnBadRequest()
    {
        // Arrange
        var motorcycle = await GetMotorcycleByModelAsync("Honda CBR");  // Nicht ausgeliehen
        
        // Act
        var response = await ApiClient.PostAsync($"/api/Motorcycle/{motorcycle.Id}/return", null);
        
        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
