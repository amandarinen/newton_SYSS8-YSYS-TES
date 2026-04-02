namespace RickAndMortyApp.Tests;

[TestClass]
[TestCategory("Integration")]
public sealed class RickAndMortyManagerIntegratonTests
{
    [TestMethod]
    [DataRow("Squanchy", 15)]
    public void TestGetCharacterId_ReturnCorrectId(string characterName, int excpectedId)
    {
        //Arrange
        var manager = new RickAndMortyEpisodeService();

        //Act
        var characterId = manager.GetCharacterID(characterName).Result;

        //Assert
        Assert.AreEqual(excpectedId, characterId);
    }

    [TestMethod]
    [DataRow("Squanchy", new string[] { "S01E11", "S02E03", "S02E08" })]
    public void TestEpisodeWhereCharacterIsPresent_ReturnCorrectEpisode(string characterName, string[] expectedEpisodes)
    {
        //Arrange
        var manager = new RickAndMortyEpisodeService();

        //Act
        var episodes = manager.GetEpisodesWhereCharacterIsPresent(characterName).Result;

        //Assert
        CollectionAssert.AreEqual(expectedEpisodes, episodes)
    }
}
