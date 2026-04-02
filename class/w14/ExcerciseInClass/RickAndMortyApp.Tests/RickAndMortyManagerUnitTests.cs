using Moq;
using Moq.Protected;
using System.Net;
using System.Text;

namespace RickAndMortyApp.Tests;

[TestClass]
[TestCategory("Unit")]
public sealed class RickAndMortyEpisodeServiceUnitTests
{
    private RickAndMortyEpisodeService _service = null!;
    private Mock<HttpMessageHandler> _mockHandler = null!;
    private HttpClient _httpClient = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockHandler = new Mock<HttpMessageHandler>();
        _httpClient = new HttpClient(_mockHandler.Object);
        _service = new RickAndMortyEpisodeService(_httpClient);
    }

    [TestMethod]
    [DataRow("Squanchy", 331)]
    [DataRow("Rick Sanchez", 1)]
    public async Task GetCharacterID_ReturnsCorrectID_UsingMockedHttpClient(string characterName, int expectedId)
    {
        // Arrange
        SetupMockHandler(CreateCharacterResponse(expectedId, characterName));

        // Act
        var characterId = await _service.GetCharacterID(characterName);

        // Assert
        Assert.AreEqual(expectedId, characterId);
    }

    [TestMethod]
    [DataRow("Squanchy", new string[] { "S01E11", "S02E03", "S02E08" })]
    public async Task GetEpisodesWhereCharacterIsPresent_ReturnsCorrectEpisodes_UsingMockedHttpClient(
        string characterName, string[] expectedEpisodes)
    {
        // Arrange
        SetupMockHandler(
            CreateCharacterResponse(331, characterName), CreateEpisodeResponse()
        );

        // Act
        var episodes = await _service.GetEpisodesWhereCharacterIsPresent(characterName);

        // Assert
        CollectionAssert.AreEqual(expectedEpisodes, episodes);
    }

    private void SetupMockHandler(params HttpResponseMessage[] responses)
    {
        var sequence = _mockHandler
            .Protected()
            .SetupSequence<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            );

        foreach (var response in responses)
        {
            sequence = sequence.ReturnsAsync(response);
        }
    }

    private static HttpResponseMessage CreateCharacterResponse(int id, string name)
    {
        return CreateJsonResponse(
            $$"""
            {
              "info": { "count": 1, "pages": 1, "next": null, "prev": null },
              "results": [
                { "id": {{id}}, "name": "{{name}}" }
              ]
            }
            """
        );
    }

    private static HttpResponseMessage CreateEpisodeResponse()
    {
        return CreateJsonResponse(
            """
            {
              "info": { "count": 3, "pages": 1, "next": null, "prev": null },
              "results": [
                {
                  "id": 11,
                  "name": "Dummy Episode 1",
                  "episode": "S01E11",
                  "characters": ["https://rickandmortyapi.com/api/character/331"],
                  "url": "https://rickandmortyapi.com/api/episode/11",
                  "created": "2017-11-10T12:56:33.798Z"
                },
                {
                  "id": 16,
                  "name": "Dummy Episode 2",
                  "episode": "S02E03",
                  "characters": ["https://rickandmortyapi.com/api/character/331"],
                  "url": "https://rickandmortyapi.com/api/episode/16",
                  "created": "2017-11-10T12:56:33.798Z"
                },
                {
                  "id": 21,
                  "name": "Dummy Episode 3",
                  "episode": "S02E08",
                  "characters": ["https://rickandmortyapi.com/api/character/331"],
                  "url": "https://rickandmortyapi.com/api/episode/21",
                  "created": "2017-11-10T12:56:33.798Z"
                }
              ]
            }
            """
        );
    }

    private static HttpResponseMessage CreateJsonResponse(string json)
    {
        return new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };
    }
}