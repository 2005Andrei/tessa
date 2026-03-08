using System.Collections.Generic;
using System.Collections.ObjectModel;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Linq;
using System;


namespace tessa.ViewModels;

public partial class LangItem : ObservableObject {
    [ObservableProperty]
    public string _languageName;

    [ObservableProperty]
    public bool _isDocument;


    [ObservableProperty]
    public bool _isTranslate;

    public string Code { get; set; }

    public LangItem(string languageName, string code, bool isDocument = false, bool isTranslate = false) {
        LanguageName = languageName;
        Code = code;
        IsDocument = isDocument;
        IsTranslate = isTranslate;
    }
}

public class ImagePreview : ObservableObject {
    public string FilePath { get; set; }
    public Bitmap Preview { get; set; }

    public ImagePreview(string filePath) {
        FilePath = filePath;
        Preview = new Bitmap(filePath);
    }
}


public partial class MainWindowViewModel : ViewModelBase {

    public ObservableCollection<ImagePreview> UploadedFiles { get; } = new ObservableCollection<ImagePreview>();
    public bool HasFiles => UploadedFiles.Count > 0;
    public bool Processing = false;

    public ObservableCollection<LangItem> Languages { get; } = new ObservableCollection<LangItem>() {
        new LangItem("German", "deu"),
        new LangItem("German (Fraktur Script)", "deu_frak"),
        new LangItem("German (Latin Fraktur Variant)", "deu_latf"),
        new LangItem("English", "eng", true, true),
        new LangItem("French", "fra"),
        new LangItem("French (Old French / Medieval)", "frm"),
        new LangItem("West Frisian", "fry"),
        new LangItem("Italian", "ita"),
        new LangItem("Italian (Historical)", "ita_old"),
        new LangItem("Japanese", "jpn"),
        new LangItem("Japanese (Vertical Text)", "jpn_vert"),
        new LangItem("Polish", "pol"),
        new LangItem("Portuguese", "por"),
        new LangItem("Romanian", "ron"),
        new LangItem("Russian", "rus"),
    };

    public void AddFiles(IEnumerable<string> newFiles) {
        bool filesAdded = false;


        foreach (var file in newFiles) {
            if (!UploadedFiles.Any(item => item.FilePath == file)) {
                UploadedFiles.Add(new ImagePreview(file));
                Console.WriteLine($"In MainWindowViewModel: {file}");
                filesAdded = true;
            }
        }

        if (filesAdded) {
            OnPropertyChanged(nameof(HasFiles));
        }
    }
}
