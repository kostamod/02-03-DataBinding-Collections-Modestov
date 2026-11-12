using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace InvoiceApp;

public partial class EditPage : Page
{
    private readonly InvoiceStore store;
    private readonly Invoice draft;
    private readonly Invoice? original;
    private bool dateInvalid;

    public EditPage(InvoiceStore store, Invoice draft, Invoice? original)
    {
        InitializeComponent();
        this.store = store;
        this.draft = draft;
        this.original = original;
        DateBox.DateValidationError += (_, _) => dateInvalid = true;
        DateBox.SelectedDateChanged += (_, _) => dateInvalid = false;
        DataContext = draft;
    }

    private void Add_Click(object sender, RoutedEventArgs e)
    {
        if (!ItemsGrid.CommitEdit(DataGridEditingUnit.Row, true)) return;
        var item = new InvoiceItem();
        draft.Items.Add(item);
        ItemsGrid.SelectedItem = item;
        ItemsGrid.ScrollIntoView(item);
    }

    private void Remove_Click(object sender, RoutedEventArgs e)
    {
        if (ItemsGrid.SelectedItem is InvoiceItem item)
        {
            ItemsGrid.CancelEdit(DataGridEditingUnit.Row);
            draft.Items.Remove(item);
        }
    }

    private void Back_Click(object sender, RoutedEventArgs e) => NavigationService.GoBack();

    private void Save_Click(object sender, RoutedEventArgs e)
    {
        if (!ItemsGrid.CommitEdit(DataGridEditingUnit.Cell, true) ||
            !ItemsGrid.CommitEdit(DataGridEditingUnit.Row, true))
        {
            MessageBox.Show("Проверьте цену и количество.");
            return;
        }
        if (string.IsNullOrWhiteSpace(draft.Number) || string.IsNullOrWhiteSpace(draft.Supplier) ||
            DateBox.SelectedDate is null || dateInvalid || draft.Items.Any(item => string.IsNullOrWhiteSpace(item.Name)))
        {
            MessageBox.Show("Укажите номер, поставщика, дату и названия товаров.");
            return;
        }

        var list = store.Invoices.ToList();
        int index = original is null ? -1 : list.IndexOf(original);
        if (index == -1) list.Add(draft);
        else list[index] = draft;

        try
        {
            store.Save(list);
            if (index == -1) store.Invoices.Add(draft);
            else store.Invoices[index] = draft;
            NavigationService.GoBack();
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            MessageBox.Show("Не удалось сохранить накладную.");
        }
    }
}
