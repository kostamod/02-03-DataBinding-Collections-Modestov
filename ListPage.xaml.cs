using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace InvoiceApp;

public partial class ListPage : Page
{
    private readonly InvoiceStore store;

    public ListPage(InvoiceStore store)
    {
        InitializeComponent();
        this.store = store;
        DataContext = store;
    }

    private void New_Click(object sender, RoutedEventArgs e)
    {
        NavigationService.Navigate(new EditPage(store, new Invoice { Id = store.NewId() }, null));
    }

    private void Open_Click(object sender, RoutedEventArgs e)
    {
        if (Grid.SelectedItem is Invoice invoice)
            NavigationService.Navigate(new EditPage(store, InvoiceStore.Copy(invoice), invoice));
        else
            MessageBox.Show("Выберите накладную в таблице.");
    }

    private void Delete_Click(object sender, RoutedEventArgs e)
    {
        if (Grid.SelectedItem is not Invoice invoice)
        {
            MessageBox.Show("Выберите накладную в таблице.");
            return;
        }
        if (MessageBox.Show($"Удалить накладную {invoice.Number}?", "Удаление",
            MessageBoxButton.YesNo) != MessageBoxResult.Yes) return;

        var list = store.Invoices.ToList();
        list.Remove(invoice);
        try
        {
            store.Save(list);
            store.Invoices.Remove(invoice);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            MessageBox.Show("Не удалось сохранить список.");
        }
    }
}
