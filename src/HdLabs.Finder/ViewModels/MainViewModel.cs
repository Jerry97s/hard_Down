using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using HdLabs.Common.Mvvm;
using HdLabs.Finder.Models;

namespace HdLabs.Finder.ViewModels;

public sealed class MainViewModel : ViewModelBase
{
    private string _query = "";
    private string _status = "Type to search.";
    private bool _isSearching;
    private FileSearchResult? _selected;
    private CancellationTokenSource? _cts;

    public string Title => "EzLabs Finder";

    public string Query
    {
        get => _query;
        set => SetProperty(ref _query, value);
    }

    public string Status
    {
        get => _status;
        set => SetProperty(ref _status, value);
    }

    public bool IsSearching
    {
        get => _isSearching;
        set => SetProperty(ref _isSearching, value);
    }

    public ObservableCollection<FileSearchResult> Results { get; } = new();

    public FileSearchResult? Selected
    {
        get => _selected;
        set => SetProperty(ref _selected, value);
    }

    public AsyncRelayCommand SearchCommand { get; }
    public RelayCommand CancelCommand { get; }
    public RelayCommand OpenSelectedCommand { get; }
    public RelayCommand OpenSelectedFolderCommand { get; }

    public MainViewModel()
    {
        SearchCommand = new AsyncRelayCommand(SearchAsync, () => !IsSearching);
        CancelCommand = new RelayCommand(Cancel, () => IsSearching);
        OpenSelectedCommand = new RelayCommand(OpenSelected, () => Selected is not null);
        OpenSelectedFolderCommand = new RelayCommand(OpenSelectedFolder, () => Selected is not null);
    }

    private void Cancel()
    {
        _cts?.Cancel();
    }

    private async Task SearchAsync()
    {
        var q = (Query ?? "").Trim();
        if (string.IsNullOrWhiteSpace(q))
        {
            Status = "Enter a query.";
            return;
        }

        Cancel();
        _cts = new CancellationTokenSource();
        var ct = _cts.Token;

        IsSearching = true;
        Status = "Searching...";
        Results.Clear();
        Selected = null;

        try
        {
            // MVP: simple recursive scan under user profile.
            // Next step: add indexing + cached DB for 0.1s search experience.
            var root = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            var max = 200;

            var found = await Task.Run(() => Scan(root, q, max, ct), ct);

            foreach (var r in found)
                Results.Add(r);

            Status = Results.Count == 0 ? "No results." : $"Found {Results.Count} items (max {max}).";
        }
        catch (OperationCanceledException)
        {
            Status = "Canceled.";
        }
        catch (Exception ex)
        {
            Status = $"Error: {ex.Message}";
        }
        finally
        {
            IsSearching = false;
        }
    }

    private static List<FileSearchResult> Scan(string root, string query, int max, CancellationToken ct)
    {
        var results = new List<FileSearchResult>(capacity: Math.Min(max, 64));
        var q = query.ToLowerInvariant();

        var pending = new Stack<string>();
        pending.Push(root);

        while (pending.Count > 0)
        {
            ct.ThrowIfCancellationRequested();
            var dir = pending.Pop();

            IEnumerable<string> subDirs;
            IEnumerable<string> files;

            try
            {
                subDirs = Directory.EnumerateDirectories(dir);
            }
            catch
            {
                continue;
            }

            try
            {
                files = Directory.EnumerateFiles(dir);
            }
            catch
            {
                files = Array.Empty<string>();
            }

            foreach (var f in files)
            {
                ct.ThrowIfCancellationRequested();

                var name = Path.GetFileName(f);
                if (!name.Contains(query, StringComparison.OrdinalIgnoreCase))
                    continue;

                FileInfo? fi = null;
                try { fi = new FileInfo(f); } catch { /* ignore */ }

                results.Add(new FileSearchResult
                {
                    Name = name,
                    FullPath = f,
                    DirectoryPath = Path.GetDirectoryName(f) ?? dir,
                    SizeBytes = fi?.Exists == true ? fi.Length : null,
                    LastWriteTime = fi?.Exists == true ? fi.LastWriteTimeUtc : null
                });

                if (results.Count >= max)
                    return results;
            }

            foreach (var sd in subDirs)
            {
                ct.ThrowIfCancellationRequested();

                // Light pruning: skip some huge/system folders for MVP.
                var folderName = Path.GetFileName(sd);
                if (folderName.Equals("AppData", StringComparison.OrdinalIgnoreCase))
                    continue;

                pending.Push(sd);
            }
        }

        return results;
    }

    private void OpenSelected()
    {
        if (Selected is null) return;
        TryStart(new ProcessStartInfo(Selected.FullPath) { UseShellExecute = true });
    }

    private void OpenSelectedFolder()
    {
        if (Selected is null) return;
        TryStart(new ProcessStartInfo("explorer.exe", $"/select,\"{Selected.FullPath}\"") { UseShellExecute = true });
    }

    private static void TryStart(ProcessStartInfo psi)
    {
        try { Process.Start(psi); }
        catch { /* ignore for MVP */ }
    }
}
