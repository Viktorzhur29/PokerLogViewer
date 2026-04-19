using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using PokerLogViewer.Models;
using PokerLogViewer.Services;

namespace PokerLogViewer.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly IFileScanner _fileScanner;
        private readonly Dispatcher _dispatcher;
        private HandCollection? _handCollection;

        private string _selectedDirectory = string.Empty;
        private string _statusMessage = "Готов к работе";
        private bool _isScanning;
        private string? _selectedTable;
        private long? _selectedHandId;
        private HandData? _selectedHand;

        public MainViewModel()
        {
            _fileScanner = new LogScannerService();
            _dispatcher = Dispatcher.CurrentDispatcher;

            _fileScanner.StatusChanged += OnStatusChanged;
            _fileScanner.ProgressChanged += OnProgressChanged;
            _fileScanner.ScanCompleted += OnScanCompleted;
            _fileScanner.HandFound += OnHandFound;

            BrowseDirectoryCommand = new RelayCommand(_ => BrowseDirectory());
            StartScanCommand = new RelayCommand(_ => StartScan(), _ => CanStartScan());
            StopScanCommand = new RelayCommand(_ => StopScan(), _ => CanStopScan());

            Tables = new ObservableCollection<string>();
            HandIds = new ObservableCollection<long>();
        }

        public ObservableCollection<string> Tables { get; }
        public ObservableCollection<long> HandIds { get; }

        public string SelectedDirectory
        {
            get => _selectedDirectory;
            set => SetField(ref _selectedDirectory, value);
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set => SetField(ref _statusMessage, value);
        }

        public bool IsScanning
        {
            get => _isScanning;
            set
            {
                if (SetField(ref _isScanning, value))
                {
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public string? SelectedTable
        {
            get => _selectedTable;
            set
            {
                if (SetField(ref _selectedTable, value))
                {
                    UpdateHandIds();
                }
            }
        }

        public long? SelectedHandId
        {
            get => _selectedHandId;
            set
            {
                if (SetField(ref _selectedHandId, value))
                {
                    UpdateSelectedHand();
                }
            }
        }

        public HandData? SelectedHand
        {
            get => _selectedHand;
            set => SetField(ref _selectedHand, value);
        }

        public ICommand BrowseDirectoryCommand { get; }
        public ICommand StartScanCommand { get; }
        public ICommand StopScanCommand { get; }

        private void BrowseDirectory()
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog
            {
                Description = "Выберите папку с JSON логами",
                UseDescriptionForTitle = true,
                ShowNewFolderButton = false
            };

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                SelectedDirectory = dialog.SelectedPath;
            }
        }

        private bool CanStartScan()
        {
            return !IsScanning && !string.IsNullOrWhiteSpace(SelectedDirectory);
        }

        private void StartScan()
        {
            ClearData();
            IsScanning = true;
            _fileScanner.StartScan(SelectedDirectory);
        }

        private bool CanStopScan()
        {
            return IsScanning;
        }

        private void StopScan()
        {
            _fileScanner.StopScan();
        }

        private void ClearData()
        {
            _dispatcher.Invoke(() =>
            {
                _handCollection = new HandCollection(_dispatcher);
                Tables.Clear();
                HandIds.Clear();
                SelectedHand = null;
            });
        }

        private void OnStatusChanged(string message)
        {
            _dispatcher.Invoke(() => StatusMessage = message);
        }

        private void OnProgressChanged(int progress)
        {
            _dispatcher.Invoke(() => StatusMessage = $"Сканирование... {progress}%");
        }

        private void OnScanCompleted(bool success, string message)
        {
            _dispatcher.Invoke(() =>
            {
                IsScanning = false;
                if (!success)
                {
                    StatusMessage = $"❌ Ошибка: {message}";
                }
            });
        }

        private void OnHandFound(HandData hand)
        {
            _dispatcher.Invoke(() =>
            {
                if (_handCollection == null)
                {
                    _handCollection = new HandCollection(_dispatcher);
                }
                
                _handCollection.Hands.Add(hand);
                
                if (!Tables.Contains(hand.TableName))
                {
                    var sortedTables = _handCollection.Hands
                        .Select(h => h.TableName)
                        .Distinct()
                        .OrderBy(t => t)
                        .ToList();

                    Tables.Clear();
                    foreach (var table in sortedTables)
                    {
                        Tables.Add(table);
                    }
                }
            });
        }

        private void UpdateHandIds()
        {
            HandIds.Clear();
            SelectedHandId = null;

            if (string.IsNullOrEmpty(SelectedTable) || _handCollection == null)
                return;

            var handIds = _handCollection.Hands
                .Where(h => h.TableName == SelectedTable)
                .Select(h => h.HandID)
                .OrderBy(id => id)
                .ToList();

            foreach (var id in handIds)
            {
                HandIds.Add(id);
            }
        }

        private void UpdateSelectedHand()
        {
            if (SelectedHandId.HasValue && _handCollection != null)
            {
                SelectedHand = _handCollection.Hands
                    .FirstOrDefault(h => h.HandID == SelectedHandId.Value);
            }
            else
            {
                SelectedHand = null;
            }
        }
    }
}
