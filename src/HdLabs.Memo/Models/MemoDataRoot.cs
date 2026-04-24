namespace HdLabs.Memo.Models;

public sealed class MemoDataRoot
{
    public List<MemoItem> Items { get; set; } = new();

    public MemoUserSettings Settings { get; set; } = new();
}

public sealed class MemoUserSettings
{
    public bool WindowTopmost { get; set; }

    /// <summary>카드 본문 기본 틴트 (ARGB #RRGGBB).</summary>
    public string? CardTintHex { get; set; }
}
