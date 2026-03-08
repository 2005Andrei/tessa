using Avalonia.Controls;
using Avalonia.Interactivity;
using tessa.ViewModels;
using System;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using Avalonia.Input;
using Avalonia.Platform.Storage;
using System.Collections.Generic;

namespace tessa.Views;

// it seems that events are hardwired to look for the method in the MainWindow.axaml.cs file
// for commands and stuff, should do that in MainWindowViewModel
public partial class MainWindow : Window {

    public MainWindow() {
        InitializeComponent();
        DataContext = new MainWindowViewModel();
    }



    public async void FileUpload(object? sender, RoutedEventArgs e) {
        Console.WriteLine("btn clicked");
        var storageProvider = TopLevel.GetTopLevel(this)?.StorageProvider;
        if (storageProvider == null) return;

        var files = await storageProvider.OpenFilePickerAsync(new Avalonia.Platform.Storage.FilePickerOpenOptions {
            Title = "Select Images",
            AllowMultiple = true,
            FileTypeFilter = new[] {
                    new FilePickerFileType("Images") {
                        Patterns = new[] {
                            "*.jpg",
                            "*.jpeg",
                            "*.heic",
                            "*.png"
                        }
                    }

                }
        });

        Console.WriteLine($"Files: {files}");
        Console.WriteLine($"Are there any: {files.Count}");

        if (files != null && files.Count > 0) {
            var filePaths = files.Select(f => f.Path.LocalPath);
            foreach (var f in filePaths) {
                Console.WriteLine($"File path: {f}");
            }
            ProcessFiles(filePaths);
        }
    }


    private void DropTarget_Drop(object? sender, DragEventArgs e) {
        if (e.DataTransfer.Contains(DataFormat.File)) {
            var files = e.DataTransfer.TryGetFiles();
            if (files != null) {
                var validExtensions = new[] {
                            ".jpg",
                            ".jpeg",
                            ".heic",
                            ".png"
                };

                var imgPaths = files.Select(f => f.Path.LocalPath)
                    .Where(path => validExtensions.Any(ext =>
                                path.EndsWith(ext, System.StringComparison.OrdinalIgnoreCase)))
                    .ToList();

                ProcessFiles(imgPaths);
            }
        }
    }

    public void ProcessFiles(IEnumerable<string> filePaths) {
        if (filePaths.Any()) {
            if (this.DataContext is MainWindowViewModel viewModel) {
                viewModel.AddFiles(filePaths);
            }
        }
    }


}
