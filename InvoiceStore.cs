using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;

namespace InvoiceApp;

public class InvoiceStore
{
    private readonly string folder = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "WpfInvoice10");
    private string FileName => Path.Combine(folder, "invoices.json");
    private static readonly JsonSerializerOptions Options = new() { WriteIndented = true };
    public ObservableCollection<Invoice> Invoices { get; } = new();
    private bool loadFailed = true;

    public void Load()
    {
        if (File.Exists(FileName))
        {
            var list = JsonSerializer.Deserialize<List<Invoice>>(File.ReadAllText(FileName)) ?? throw new JsonException();
            foreach (var invoice in list) Invoices.Add(invoice);
        }
        loadFailed = false;
    }

    public int NewId()
    {
        int id;
        do { id = Random.Shared.Next(1, int.MaxValue); }
        while (Invoices.Any(invoice => invoice.Id == id));
        return id;
    }

    public static Invoice Copy(Invoice invoice)
    {
        return JsonSerializer.Deserialize<Invoice>(JsonSerializer.Serialize(invoice))!;
    }

    public void Save(List<Invoice> list)
    {
        if (loadFailed) throw new IOException("Файл данных не был прочитан.");
        Directory.CreateDirectory(folder);
        string temp = Path.Combine(folder, $"{Guid.NewGuid()}.tmp");
        try
        {
            File.WriteAllText(temp, JsonSerializer.Serialize(list, Options));
            File.Move(temp, FileName, overwrite: true);
        }
        finally { File.Delete(temp); }
    }
}
