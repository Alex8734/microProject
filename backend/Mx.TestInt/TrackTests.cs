using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Mx.Persistence.Model;
using Mx.Requests;
using Mx.Responses;
using Mx.TestInt.Util;

namespace Mx.TestInt;

public class TrackTests(WebApiTestFixture webApiFixture) : TrackTestBase(webApiFixture) 
{
    [Fact]
    public async Task GetAllTracks_ShouldReturnAllTracks()
    {
        // Act
        var response = await ApiClient.GetAsync("/api/Track");
        
        // Assert
        response.EnsureSuccessStatusCode();
        var tracks = await response.Content.ReadFromJsonAsync<List<TrackResponse>>(JsonOptions);
        
        Assert.NotNull(tracks);
        Assert.Equal(4, tracks!.Count);
        Assert.Contains(tracks, t => t.Name == "Easy Test Track");
        Assert.Contains(tracks, t => t.Name == "Medium Test Track");
        Assert.Contains(tracks, t => t.Name == "Hard Test Track");
        Assert.Contains(tracks, t => t.Name == "Expert Test Track");
    }
    
    [Fact]
    public async Task GetTrackById_WithExistingId_ShouldReturnTrack()
    {
        // Arrange
        var track = await GetTrackByNameAsync("Medium Test Track");
        
        // Act
        var response = await ApiClient.GetAsync($"/api/Track/{track.Id}");
        
        // Assert
        response.EnsureSuccessStatusCode();
        var returnedTrack = await response.Content.ReadFromJsonAsync<TrackResponse>(JsonOptions);
        
        Assert.NotNull(returnedTrack);
        Assert.Equal(track.Id, returnedTrack!.Id);
        Assert.Equal(track.Name, returnedTrack.Name);
        Assert.Equal(track.LengthInKm, returnedTrack.LengthInKm);
        Assert.Equal(track.Difficulty, returnedTrack.Difficulty);
    }
    
    [Fact]
    public async Task GetTrackById_WithNonExistingId_ShouldReturnNotFound()
    {
        // Act
        var response = await ApiClient.GetAsync("/api/Track/999");
        
        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
    
    [Fact]
    public async Task CreateTrack_WithValidData_ShouldCreateAndReturnTrack()
    {
        // Arrange
        var request = new TrackRequest
        {
            Name = "New Test Track",
            LengthInKm = 6.5,
            Difficulty = TrackDifficulty.Medium
        };
        
        // Act
        var response = await ApiClient.PostAsJsonAsync("/api/Track", request, JsonOptions);
        
        // Assert
        response.EnsureSuccessStatusCode();
        var createdTrack = await response.Content.ReadFromJsonAsync<TrackResponse>(JsonOptions);
        
        Assert.NotNull(createdTrack);
        Assert.Equal(request.Name, createdTrack!.Name);
        Assert.Equal(request.LengthInKm, createdTrack.LengthInKm);
        Assert.Equal(request.Difficulty, createdTrack.Difficulty);
        
        // Verify it was actually saved to the database
        var track = await GetTrackByNameAsync("New Test Track");
        Assert.Equal(createdTrack.Id, track.Id);
    }
    
    [Fact]
    public async Task DeleteTrack_WithExistingId_ShouldRemoveTrack()
    {
        // Arrange
        var request = new TrackRequest
        {
            Name = "Track To Delete",
            LengthInKm = 4.0,
            Difficulty = TrackDifficulty.Easy
        };
        
        var createResponse = await ApiClient.PostAsJsonAsync("/api/Track", request, JsonOptions);
        createResponse.EnsureSuccessStatusCode();
        var createdTrack = await createResponse.Content.ReadFromJsonAsync<TrackResponse>(JsonOptions);
        
        // Act
        var response = await ApiClient.DeleteAsync($"/api/Track/{createdTrack!.Id}");
        
        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        
        // Verify it was actually removed from the database
        var getResponse = await ApiClient.GetAsync($"/api/Track/{createdTrack.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }
    
    [Fact]
    public async Task DeleteTrack_WithNonExistingId_ShouldReturnNotFound()
    {
        // Act
        var response = await ApiClient.DeleteAsync("/api/Track/999");
        
        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
