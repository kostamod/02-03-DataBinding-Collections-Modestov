using System.ComponentModel;
using System.Text.Json.Serialization;

namespace InvoiceApp;

public class InvoiceItem : INotifyPropertyChanged
{
    private string name = "";
    public string Name
    {
        get => name;
        set
        {
            name = value;
            Changed(nameof(Name));
        }
    }

    private decimal price;
    public decimal Price
    {
        get => price;
        set
        {
            if (value < 0 || value > 1_000_000_000) throw new ArgumentOutOfRangeException();
            price = value;
            Changed(nameof(Price));
            Changed(nameof(ItemTotal));
        }
    }

    private int count = 1;
    public int Count
    {
        get => count;
        set
        {
            if (value < 1 || value > 1_000_000) throw new ArgumentOutOfRangeException();
            count = value;
            Changed(nameof(Count));
            Changed(nameof(ItemTotal));
        }
    }

    [JsonIgnore]
    public decimal ItemTotal => Price * Count;

    public event PropertyChangedEventHandler? PropertyChanged;

    private void Changed(string name) => PropertyChanged?.Invoke(this, new(name));
}
