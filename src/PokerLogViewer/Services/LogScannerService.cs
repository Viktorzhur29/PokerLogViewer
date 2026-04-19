using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Threading;
using PokerLogViewer.Models;

namespace PokerLogViewer.Services
{
    public class LogScannerService : IFileScanner
    {
        private Thread? _scanThread;
        private volatile bool _isScanning;
        private readonly object _lockObject = new();
        private string _currentDirectory = string.Empty;

        public event Action<string>? StatusChanged;
        public event Action<int>? ProgressChanged;
        public event Action<bool, string>? ScanCompleted;
        public event Action<HandData>? HandFound;

        public void StartScan(string directoryPath)
        {
            lock (_lockObject)
            {
                if (_isScanning)
                    return;

                _currentDirectory = directoryPath;
                _isScanning = true;

                _scanThread = new Thread(ScanDirectory)
                {
                    IsBackground = true,
                    Name = "LogScannerThread"
                };
                
                _scanThread.Start();
            }
        }

        public void StopScan()
        {
            lock (_lockObject)
            {
                if (!_isScanning || _scanThread == null)
                    return;

                _isScanning = false;
                
                try
                {
                    if (_scanThread.IsAlive)
                    {
                        _scanThread.Interrupt();
                        if (!_scanThread.Join(TimeSpan.FromSeconds(5)))
                        {
                            Debug.WriteLine("Thread did not stop gracefully");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error stopping thread: {ex.Message}");
                }
                finally
                {
                    _scanThread = null;
                }
            }
        }

        private void ScanDirectory()
        {
            try
            {
                StatusChanged?.Invoke("Сканирование...");
                
                if (!Directory.Exists(_currentDirectory))
                {
                    throw new DirectoryNotFoundException($"Директория не найдена: {_currentDirectory}");
                }

                var jsonFiles = Directory.GetFiles(_currentDirectory, "*.json", SearchOption.AllDirectories);
                var processedFiles = 0;
                var validHands = 0;

                foreach (var filePath in jsonFiles)
                {
                    if (!_isScanning)
                        break;

                    try
                    {
                        var hands = ParseJsonFile(filePath);
                        
                        foreach (var hand in hands)
                        {
                            if (!_isScanning)
                                break;
                                
                            HandFound?.Invoke(hand);
                            validHands++;
                        }
                        
                        processedFiles++;
                        ProgressChanged?.Invoke((processedFiles * 100) / jsonFiles.Length);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Error processing file {filePath}: {ex.Message}");
                    }
                }

                if (_isScanning)
                {
                    StatusChanged?.Invoke($"✅ Готово ({validHands} раздач обработано)");
                    ScanCompleted?.Invoke(true, $"Успешно обработано {processedFiles} файлов");
                }
            }
            catch (ThreadInterruptedException)
            {
                StatusChanged?.Invoke("⏹️ Сканирование остановлено");
                ScanCompleted?.Invoke(false, "Сканирование прервано пользователем");
                Debug.WriteLine("Scan interrupted");
            }
            catch (Exception ex)
            {
                StatusChanged?.Invoke($"❌ Ошибка: {ex.Message}");
                ScanCompleted?.Invoke(false, ex.Message);
                Debug.WriteLine($"Scan error: {ex}");
            }
            finally
            {
                _isScanning = false;
            }
        }

        private List<HandData> ParseJsonFile(string filePath)
        {
            var hands = new List<HandData>();
            
            var jsonContent = File.ReadAllText(filePath);
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                AllowTrailingCommas = true,
                ReadCommentHandling = JsonCommentHandling.Skip
            };

            try
            {
                var parsedHands = JsonSerializer.Deserialize<List<HandData>>(jsonContent, options);
                if (parsedHands != null)
                {
                    hands.AddRange(parsedHands);
                }
            }
            catch (JsonException)
            {
                try
                {
                    var singleHand = JsonSerializer.Deserialize<HandData>(jsonContent, options);
                    if (singleHand != null)
                    {
                        hands.Add(singleHand);
                    }
                }
                catch (JsonException ex)
                {
                    Debug.WriteLine($"JSON parse error in {filePath}: {ex.Message}");
                    throw;
                }
            }

            return hands;
        }
    }
}
