using System;
using System.IO;
using System.Text.Json;
using System.Threading;
using PokerLogViewer.Models;
using PokerLogViewer.Services;
using Xunit;

namespace PokerLogViewer.Tests.Services
{
    public class LogScannerServiceTests
    {
        [Fact]
        public void ParseJsonFile_ValidJson_ReturnsHands()
        {
            // Arrange
            var tempFile = Path.GetTempFileName();
            var json = @"[
                {
                    ""HandID"": 123456,
                    ""TableName"": ""Test Table"",
                    ""Players"": [""Player1"", ""Player2""],
                    ""Winners"": [""Player1""],
                    ""WinAmount"": ""100,00$""
                }
            ]";

            File.WriteAllText(tempFile, json);
            var service = new LogScannerService();
            var handFound = false;
            service.HandFound += (hand) => handFound = true;

            try
            {
                // Act
                service.StartScan(Path.GetDirectoryName(tempFile));
                Thread.Sleep(1000); // Даем время на обработку

                // Assert
                Assert.True(handFound);
            }
            finally
            {
                File.Delete(tempFile);
            }
        }

        [Fact]
        public void ScanDirectory_InvalidJson_GracefulDegradation()
        {
            // Arrange
            var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDir);

            var invalidFile = Path.Combine(tempDir, "invalid.json");
            File.WriteAllText(invalidFile, "{ invalid json }");

            var service = new LogScannerService();
            var completed = false;
            var success = false;

            service.ScanCompleted += (s, msg) =>
            {
                completed = true;
                success = s;
            };

            try
            {
                // Act
                service.StartScan(tempDir);
                Thread.Sleep(1000);

                // Assert - сервис не должен упасть
                Assert.True(completed);
                Assert.True(success); // Graceful degradation - считаем успехом
            }
            finally
            {
                Directory.Delete(tempDir, true);
            }
        }
    }
}