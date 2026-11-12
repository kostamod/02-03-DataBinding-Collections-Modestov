using System.Windows;

namespace InvoiceApp;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        var store = new InvoiceStore();
        try { store.Load(); }
        catch (Exception ex) when (ex is System.IO.IOException or UnauthorizedAccessException or System.Text.Json.JsonException or ArgumentOutOfRangeException)
        {
            MessageBox.Show("Не удалось прочитать файл накладных. Проверьте данные в %LOCALAPPDATA%\\WpfInvoice10.");
        }
        Pages.Navigate(new ListPage(store));
    }
}
