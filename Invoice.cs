using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace InvoiceApp;

public class Invoice : INotifyPropertyChanged
{
    public int Id { get; set; }

    private string number = "";
    public string Number
    {
        get => number;
        set
        {
            number = value;
            Changed(nameof(Number));
        }
    }

    private string supplier = "";
    public string Supplier
    {
        get => supplier;
        set
        {
            supplier = value;
            Changed(nameof(Supplier));
        }
    }

    private DateTime date = DateTime.Today;
    public DateTime Date
    {
        get => date;
        set
        {
            date = value;
            Changed(nameof(Date));
        }
    }

    [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
    public ObservableCollection<InvoiceItem> Items { get; } = new();

    [JsonIgnore]
    public decimal InvoiceTotal => Items.Sum(item => item.ItemTotal);

    public event PropertyChangedEventHandler? PropertyChanged;

    public Invoice()
    {
        Items.CollectionChanged += ItemsChanged;
    }

    private void ItemsChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        // После изменения списка следим только за позициями, которые в нём остались.
        foreach (InvoiceItem item in watchedItems)
            item.PropertyChanged -= ItemChanged;

        watchedItems.Clear();
        foreach (InvoiceItem item in Items)
        {
            watchedItems.Add(item);
            item.PropertyChanged += ItemChanged;
        }
        Changed(nameof(InvoiceTotal));
    }

    private readonly List<InvoiceItem> watchedItems = new();

    private void ItemChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(InvoiceItem.ItemTotal))
            Changed(nameof(InvoiceTotal));
    }

    private void Changed(string name) => PropertyChanged?.Invoke(this, new(name));
}
