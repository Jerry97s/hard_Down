using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using HdLabs.Memo.ViewModels;

namespace HdLabs.Memo;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        var vm = new MainViewModel();
        DataContext = vm;
        vm.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName is nameof(MainViewModel.WindowTopmost))
                Topmost = vm.WindowTopmost;
        };
        Topmost = vm.WindowTopmost;
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        if (DataContext is MainViewModel vm)
            ApplyCardBackground(vm.CardTintHex);

        MemoBody.Focus();
        MemoBody.CaretIndex = MemoBody.Text?.Length ?? 0;
    }

    private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
        if (DataContext is MainViewModel vm)
            vm.OnWindowClosing();
    }

    private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key != Key.Delete)
            return;
        if (Keyboard.FocusedElement is not ListView && Keyboard.FocusedElement is not ListViewItem)
            return;
        if (DataContext is not MainViewModel vm)
            return;
        if (vm.IsListView && vm.DeleteSelectedCommand.CanExecute(null))
        {
            vm.DeleteSelectedCommand.Execute(null);
            e.Handled = true;
        }
    }

    private void MemoList_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (DataContext is MainViewModel vm)
            vm.OpenSelected();
    }

    private void MemoList_OnKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key != Key.Delete)
            return;
        if (DataContext is not MainViewModel vm)
            return;
        if (vm.DeleteSelectedCommand.CanExecute(null))
        {
            vm.DeleteSelectedCommand.Execute(null);
            e.Handled = true;
        }
    }

    private void Chrome_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed)
            DragMove();
    }

    private void Close_Click(object sender, RoutedEventArgs e) => Close();

    private void MoreOptions_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button btn || btn.ContextMenu is null)
            return;

        btn.ContextMenu.PlacementTarget = btn;
        btn.ContextMenu.Placement = PlacementMode.Bottom;
        btn.ContextMenu.DataContext = DataContext;
        btn.ContextMenu.IsOpen = true;
        e.Handled = true;
    }

    private void PlaceholderCalendar_Click(object sender, RoutedEventArgs e) =>
        MessageBox.Show("캘린더/일정 앱과 연동하는 기능은 추후 릴리스에 포함될 수 있습니다.", "일정", MessageBoxButton.OK, MessageBoxImage.Information);

    private void PlaceholderAlarm_Click(object sender, RoutedEventArgs e) =>
        MessageBox.Show("알림(미리 알림)은 추후 릴리스에 포함될 수 있습니다.", "알림", MessageBoxButton.OK, MessageBoxImage.Information);

    private void ColorPickerButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button b || b.ContextMenu is null)
            return;
        b.ContextMenu.PlacementTarget = b;
        b.ContextMenu.Placement = PlacementMode.Bottom;
        b.ContextMenu.DataContext = DataContext;
        b.ContextMenu.IsOpen = true;
    }

    private void ColorTintClick(object sender, RoutedEventArgs e)
    {
        if (sender is not MenuItem { Tag: string hex } || DataContext is not MainViewModel vm)
            return;
        vm.SetCardTintFromUi(hex);
        ApplyCardBackground(hex);
    }

    private void ApplyCardBackground(string? hex8)
    {
        if (string.IsNullOrWhiteSpace(hex8))
        {
            RootMemoCard.Background = (Brush)FindResource("MemoBodyBrush");
            return;
        }

        try
        {
            var c = (Color)ColorConverter.ConvertFromString(hex8);
            var top = Lighter(c, 0.12);
            var bottom = Darker(c, 0.11);
            var brush = new LinearGradientBrush
            {
                StartPoint = new Point(0, 0),
                EndPoint = new Point(0, 1),
            };
            brush.GradientStops.Add(new GradientStop(top, 0));
            brush.GradientStops.Add(new GradientStop(Color.FromArgb(c.A, c.R, c.G, c.B), 0.55));
            brush.GradientStops.Add(new GradientStop(bottom, 1));
            RootMemoCard.Background = brush;
        }
        catch
        {
            RootMemoCard.Background = (Brush)FindResource("MemoBodyBrush");
        }
    }

    private static Color Lighter(Color c, double amount) =>
        Color.FromArgb(c.A,
            (byte)Math.Min(255, (int)(c.R + 255 * amount)),
            (byte)Math.Min(255, (int)(c.G + 255 * amount)),
            (byte)Math.Min(255, (int)(c.B + 255 * amount)));

    private static Color Darker(Color c, double amount) =>
        Color.FromArgb(c.A,
            (byte)Math.Max(0, (int)(c.R * (1 - amount))),
            (byte)Math.Max(0, (int)(c.G * (1 - amount))),
            (byte)Math.Max(0, (int)(c.B * (1 - amount))));

    #region Text helpers (plain TextBox, markdown-style markers)

    private void SyncBodyToViewModel()
    {
        if (DataContext is MainViewModel vm)
            vm.NewBody = MemoBody.Text;
    }

    private void FormatBold_Click(object sender, RoutedEventArgs e)
    {
        InsertAroundSelection(MemoBody, "**", "**");
        SyncBodyToViewModel();
    }

    private void FormatItalic_Click(object sender, RoutedEventArgs e)
    {
        InsertAroundSelection(MemoBody, "*", "*");
        SyncBodyToViewModel();
    }

    private void FormatUnderline_Click(object sender, RoutedEventArgs e)
    {
        InsertAroundSelection(MemoBody, "__", "__");
        SyncBodyToViewModel();
    }

    private void FormatStrike_Click(object sender, RoutedEventArgs e)
    {
        InsertAroundSelection(MemoBody, "~~", "~~");
        SyncBodyToViewModel();
    }

    private void InsertChecklistLine_Click(object sender, RoutedEventArgs e)
    {
        InsertLinePrefix(MemoBody, "☐ ");
        SyncBodyToViewModel();
    }

    private void InsertBulletLine_Click(object sender, RoutedEventArgs e)
    {
        InsertLinePrefix(MemoBody, "· ");
        SyncBodyToViewModel();
    }

    private void InsertImagePlaceholder_Click(object sender, RoutedEventArgs e)
    {
        InsertAtCaret(MemoBody, "\r\n[이미지]\r\n");
        SyncBodyToViewModel();
    }

    private static void InsertAroundSelection(TextBox tb, string left, string right)
    {
        if (string.IsNullOrEmpty(tb.Text))
        {
            tb.Text = left + right;
            tb.CaretIndex = left.Length;
            return;
        }

        var start = tb.SelectionStart;
        var len = tb.SelectionLength;
        var selected = len > 0 ? tb.Text.Substring(start, len) : "";
        var insert = left + (len > 0 ? selected : "") + right;
        if (len > 0)
            tb.Text = tb.Text.Remove(start, len).Insert(start, insert);
        else
            tb.Text = tb.Text.Insert(start, insert);

        tb.CaretIndex = len > 0
            ? start + left.Length + selected.Length + right.Length
            : start + left.Length;
        tb.Select(tb.CaretIndex, 0);
        tb.Focus();
    }

    private static void InsertAtCaret(TextBox tb, string text)
    {
        var i = tb.CaretIndex;
        tb.Text = tb.Text.Insert(i, text);
        tb.CaretIndex = i + text.Length;
        tb.Focus();
    }

    private static void InsertLinePrefix(TextBox tb, string prefix)
    {
        var text = tb.Text ?? "";
        var i = tb.CaretIndex;
        var insert = (i == 0 || text[i - 1] is '\n' or '\r') ? prefix : "\r\n" + prefix;
        tb.Text = text.Insert(i, insert);
        tb.CaretIndex = i + insert.Length;
        tb.Focus();
    }

    #endregion
}
