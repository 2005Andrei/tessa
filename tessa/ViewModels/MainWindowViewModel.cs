using System.Collections.Generic;
using System.Collections.ObjectModel;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Linq;
using System;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;
using Tesseract;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Diagnostics;


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
    private static HttpClient client = new HttpClient() {
        BaseAddress = new Uri("http://localhost:8000/")
    };

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

    [RelayCommand]
    public async Task ApiCall(string doc_type) {
        Collection<string?>? texts = await GetTextFromImgs();
        var payload = new {
            Message = "Hello",
            Texts = texts
        };


        // string jsonPayload = JsonSerializer.Serialize(payload);
        // var httpContent = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

        string saveDir = AppDomain.CurrentDomain.BaseDirectory;
        string filePath = Path.Combine(saveDir, "tessa (1).docx");

        try {
            using HttpResponseMessage response = await client.PostAsJsonAsync("/test", payload);

            if (response.IsSuccessStatusCode) {
                using (Stream stream = await response.Content.ReadAsStreamAsync()) {
                    using (Stream outStream = File.Open(filePath, FileMode.Create)) {
                        await stream.CopyToAsync(outStream);
                    }
                }
            }

            Console.WriteLine("great success");
            OpenWordDocument(filePath);

        }
        catch (Exception e) {
            Console.WriteLine($"At the ApiCall: {e.Message}");
        }
    }

    static void OpenWordDocument(string filePaht) {
        try {
            ProcessStartInfo process = new ProcessStartInfo {
                FileName = filePaht,
                UseShellExecute = true
            };
            Process.Start(process);
        }
        catch (Exception e) {
            Console.WriteLine($"Err: {e.Message}");
        }
    }

    public async Task<Collection<string>>? GetTextFromImgs() {
        string langCode = "";

        foreach (var item in Languages) {
            if (item.IsDocument) {
                langCode += String.IsNullOrEmpty(langCode) ? $"{item.Code}" : $"+{item.Code}";
            }
        }

        Console.WriteLine($"Code: {langCode}");

        if (!this.UploadedFiles.Any()) {
            Console.WriteLine("I'll send a notification to the user");
            return null;
        }

        Collection<string> Texts = new Collection<string>();

        await Task.Run(() => {
            try {
                this.Processing = true;
                TesseractEngine engine = new TesseractEngine(Path.Combine(AppContext.BaseDirectory, "Assets", "tessdata"), langCode, EngineMode.Default);


                foreach (var file in UploadedFiles) {
                    using var img = Pix.LoadFromFile(file.FilePath);
                    using var page = engine.Process(img);



                    Texts.Add(page.GetText());
                    Console.WriteLine($"Page text: {page.GetText()}");
                    // Console.WriteLine(page.GetHOCRText(1, true));
                }

            }
            catch (Exception e) {
                Console.WriteLine($"task err: {e.InnerException}");
            }
        });

        return Texts;
    }

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
