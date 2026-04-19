using System;

namespace PokerLogViewer.Services
{
    public interface IFileScanner
    {
        event Action<string>? StatusChanged;
        event Action<int>? ProgressChanged;
        event Action<bool, string>? ScanCompleted;
        event Action<Models.HandData>? HandFound;
        
        void StartScan(string directoryPath);
        void StopScan();
    }
}
