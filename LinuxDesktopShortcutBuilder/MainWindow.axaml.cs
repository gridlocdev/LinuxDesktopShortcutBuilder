using System;
using System.IO;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Platform.Storage;

namespace LinuxDesktopShortcutBuilder;

public partial class MainWindow : Window
{
    public class ShortcutProperties
    {
        public string Version { get; set; } = "1.0";
        public string Type { get; set; } = "Application";
        public bool OpensTerminal { get; set; }
        public string? ExecutableFilePath { get; set; }
        public string? Name { get; set; }
        public string? IconFilePath { get; set; }
    }

    private ShortcutProperties Properties { get; set; } = new();

    private string ExportFolder { get; set; } =
        Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".local/share/applications");

    public MainWindow()
    {
        InitializeComponent();
        InitializeFields();
        RenderFileOutputTextBlock();
    }

    private void InitializeFields()
    {
        NameTextBox.Text = Properties.Name;
        ExecutableFilePathTextBox.Text = Properties.ExecutableFilePath;
        IconFilePathTextBox.Text = Properties.IconFilePath;
        VersionTextBox.Text = Properties.Version;
        TypeTextBox.Text = Properties.Type;
        OpensTerminalCheckBox.IsChecked = Properties.OpensTerminal;
        SelectExportFolderTextBox.Text = ExportFolder;
    }

    private void RenderFileOutputTextBlock()
        => FileOutputTextBlock.Text =
            $"""
             [Desktop Entry]
             Version={Properties.Version}
             Type={Properties.Type}
             Terminal={Properties.OpensTerminal}
             Name={Properties.Name}
             Exec={Properties.ExecutableFilePath}
             {(string.IsNullOrEmpty(Properties.IconFilePath) ? "" : $"Icon={Properties.IconFilePath}")}

             """;

    private async void SelectIconButton_OnClick(object? sender, RoutedEventArgs e)
    {
        // Get top level from the current control. Alternatively, you can use Window reference instead.
        var topLevel = TopLevel.GetTopLevel(this);

        if (topLevel == null) return;

        // Start async operation to open the dialog.
        var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Select Icon File",
            AllowMultiple = false
        });

        if (files.Count < 1) return;

        Properties.IconFilePath = files[0].Path.ToString();
        IconFilePathTextBox.Text = files[0].Path.AbsolutePath;
        RenderFileOutputTextBlock();
    }

    private async void SelectExecutableFilePathButton_OnClick(object? sender, RoutedEventArgs e)
    {
        // Get top level from the current control. Alternatively, you can use Window reference instead.
        var topLevel = TopLevel.GetTopLevel(this);

        if (topLevel == null) return;

        // Start async operation to open the dialog.
        var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Select Executable File",
            AllowMultiple = false
        });

        if (files.Count < 1) return;

        Properties.ExecutableFilePath = files[0].Path.ToString();
        ExecutableFilePathTextBox.Text = files[0].Path.AbsolutePath;
        RenderFileOutputTextBlock();
    }

    private void OpensTerminalCheckBox_OnIsCheckedChanged(object? sender, RoutedEventArgs e)
    {
        if (OpensTerminalCheckBox.IsChecked != null)
            Properties.OpensTerminal = OpensTerminalCheckBox.IsChecked.Value;

        RenderFileOutputTextBlock();
    }

    private async void SelectExportFolderButton_OnClick(object? sender, RoutedEventArgs e)
    {
        // Get top level from the current control. Alternatively, you can use Window reference instead.
        var topLevel = TopLevel.GetTopLevel(this);

        if (topLevel == null) return;

        // Start async operation to open the dialog.
        var folders = await topLevel.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
        {
            Title = "Select Shortcut Output Path",
            AllowMultiple = false
        });

        if (folders.Count < 1) return;

        ExportFolder = folders[0].Path.ToString();
    }

    private void NameTextBox_OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (sender is TextBox textBox)
            Properties.Name = textBox.Text;
        RenderFileOutputTextBlock();
    }

    private void ExecutableFilePathTextBox_OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (sender is TextBox textBox)
            Properties.ExecutableFilePath = textBox.Text;
        RenderFileOutputTextBlock();
    }

    private void IconFilePathTextBox_OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (sender is TextBox textBox)
            Properties.IconFilePath = textBox.Text;
        RenderFileOutputTextBlock();
    }

    private void VersionTextBox_OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (sender is TextBox textBox)
            Properties.Version = textBox.Text ?? "";
        RenderFileOutputTextBlock();
    }

    private void TypeTextBox_OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (sender is TextBox textBox)
            Properties.Type = textBox.Text ?? "";
        RenderFileOutputTextBlock();
    }

    private bool ValidateInputFields()
    {
        ErrorListStackPanel.Children.Clear();

        // validate "Name" property has value
        if (string.IsNullOrEmpty(Properties.Name))
            AddErrorToList($"Name cannot be empty");

        // validate "Executable" property has value
        if (string.IsNullOrEmpty(Properties.ExecutableFilePath))
            AddErrorToList($"Executable file path cannot be empty");

        // validate executable path is valid
        if (!File.Exists(Properties.ExecutableFilePath) && !string.IsNullOrEmpty(Properties.ExecutableFilePath))
            AddErrorToList($"Executable at selected path does not exist");

        // validate "IconFilePath" property has value
        if (string.IsNullOrEmpty(Properties.IconFilePath))
            AddErrorToList($"Icon path cannot be empty");

        // validate icon path is valid
        if (!File.Exists(Properties.IconFilePath) && !string.IsNullOrEmpty(Properties.IconFilePath))
            AddErrorToList($"Icon at selected path does not exist");

        // validate export path is valid
        if (!Directory.Exists(ExportFolder))
            AddErrorToList($"Directory {ExportFolder} does not exist");

        return ErrorListStackPanel.Children.Count == 0;
    }

    private void SaveButton_OnClick(object? sender, RoutedEventArgs e)
    {
        var validationPassed = ValidateInputFields();

        ErrorListStackPanel.IsVisible = !validationPassed;

        if (!validationPassed)
            return;

        try
        {
            File.WriteAllText(Path.Join(ExportFolder, $"{Properties.Name}.desktop"), FileOutputTextBlock.Text);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            SaveSuccessTextBlock.IsVisible = false;
            return;
        }

        SaveSuccessTextBlock.IsVisible = true;
    }

    private void AddErrorToList(string errorText)
    {
        ErrorListStackPanel.Children.Add(new TextBlock { Text = $"- {errorText}", Foreground = Brushes.Red });
    }

    // If the app window is displayed in portrait mode, display the output underneath the form inputs
    private void WindowBase_OnResized(object? sender, WindowResizedEventArgs e)
    {
        // Get top level from the current control. Alternatively, you can use Window reference instead.
        var topLevel = TopLevel.GetTopLevel(this);

        if (topLevel == null) return;

        var isOrientationLandscape = topLevel.ClientSize.Width - (topLevel.ClientSize.Width * .25) > ClientSize.Height;

        RootStackPanel.Orientation = isOrientationLandscape
            ? Orientation.Horizontal
            : Orientation.Vertical;
    }
}