using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using HdLabs.Memo.Models;

namespace HdLabs.Memo.Services;

public sealed class MemoDataService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    };

    public string DataFilePath { get; }

    public MemoDataService()
    {
        var root = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "HdLabs",
            "Memo");
        Directory.CreateDirectory(root);
        DataFilePath = Path.Combine(root, "memos.json");
    }

    public MemoDataRoot Load()
    {
        if (!File.Exists(DataFilePath))
            return new MemoDataRoot();

        try
        {
            var json = File.ReadAllText(DataFilePath);
            var data = JsonSerializer.Deserialize<MemoDataRoot>(json, JsonOptions);
            return data ?? new MemoDataRoot();
        }
        catch
        {
            return new MemoDataRoot();
        }
    }

    public void Save(MemoDataRoot data)
    {
        var json = JsonSerializer.Serialize(data, JsonOptions);
        File.WriteAllText(DataFilePath, json);
    }
}
