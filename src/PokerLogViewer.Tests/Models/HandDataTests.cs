using System.Text.Json;
using PokerLogViewer.Models;
using Xunit;

namespace PokerLogViewer.Tests.Models
{
    public class HandDataTests
    {
        [Fact]
        public void Deserialize_ValidJson_CreatesObject()
        {
            // Arrange
            var json = @"{
                ""HandID"": 999888777,
                ""TableName"": ""Monte Carlo #5"",
                ""Players"": [""Alice"", ""Bob"", ""Charlie""],
                ""Winners"": [""Alice""],
                ""WinAmount"": ""2 500,50$""
            }";

            // Act
            var hand = JsonSerializer.Deserialize<HandData>(json);

            // Assert
            Assert.NotNull(hand);
            Assert.Equal(999888777, hand.HandID);
            Assert.Equal("Monte Carlo #5", hand.TableName);
            Assert.Equal(3, hand.Players.Count);
            Assert.Single(hand.Winners);
            Assert.Equal("2 500,50$", hand.WinAmount);
        }

        [Fact]
        public void Deserialize_ArrayJson_ReturnsList()
        {
            // Arrange
            var json = @"[
                { ""HandID"": 1, ""TableName"": ""T1"", ""Players"": [""P1""], ""Winners"": [""P1""], ""WinAmount"": ""10$"" },
                { ""HandID"": 2, ""TableName"": ""T2"", ""Players"": [""P2""], ""Winners"": [""P2""], ""WinAmount"": ""20$"" }
            ]";

            // Act
            var hands = JsonSerializer.Deserialize<List<HandData>>(json);

            // Assert
            Assert.NotNull(hands);
            Assert.Equal(2, hands.Count);
        }
    }
}