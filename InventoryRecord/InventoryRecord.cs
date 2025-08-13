using System.Text.Json;

// ---------------------
// Marker Interface
// ---------------------
public interface IInventoryEntity
{
    int Id { get; }
}

// ---------------------
// Immutable Record
// ---------------------
public record InventoryItem(int Id, string Name, int Quantity, DateTime DateAdded) : IInventoryEntity;

// ---------------------
// Generic Inventory Logger
// ---------------------
public class InventoryLogger<T> where T : IInventoryEntity
{
    private List<T> _log = new List<T>();
    private readonly string _filePath;

    public InventoryLogger(string filePath)
    {
        _filePath = filePath;
    }

    public void Add(T item)
    {
        _log.Add(item);
    }

    public List<T> GetAll()
    {
        return new List<T>(_log); // return a copy
    }

    public void SaveToFile()
    {
        try
        {
            using (StreamWriter writer = new StreamWriter(_filePath))
            {
                string json = JsonSerializer.Serialize(_log);
                writer.Write(json);
            }
            Console.WriteLine("Data saved successfully!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving data: {ex.Message}");
        }
    }

    public void LoadFromFile()
    {
        try
        {
            if (!File.Exists(_filePath))
            {
                Console.WriteLine("No file found. Starting with empty log.");
                return;
            }

            using (StreamReader reader = new StreamReader(_filePath))
            {
                string json = reader.ReadToEnd();
                var items = JsonSerializer.Deserialize<List<T>>(json);
                if (items != null)
                    _log = items;
            }
            Console.WriteLine("Data loaded successfully!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading data: {ex.Message}");
        }
    }
}

// ---------------------
// Integration Layer
// ---------------------
public class InventoryApp
{
    private InventoryLogger<InventoryItem> _logger;

    public InventoryApp(string filePath)
    {
        _logger = new InventoryLogger<InventoryItem>(filePath);
    }

    public void SeedSampleData()
    {
        _logger.Add(new InventoryItem(1, "Laptop", 10, DateTime.Now));
        _logger.Add(new InventoryItem(2, "Keyboard", 25, DateTime.Now));
        _logger.Add(new InventoryItem(3, "Mouse", 50, DateTime.Now));
        _logger.Add(new InventoryItem(4, "Monitor", 15, DateTime.Now));
        _logger.Add(new InventoryItem(5, "Headset", 30, DateTime.Now));
    }

    public void SaveData()
    {
        _logger.SaveToFile();
    }

    public void LoadData()
    {
        _logger.LoadFromFile();
    }

    public void PrintAllItems()
    {
        List<InventoryItem> items = _logger.GetAll();
        foreach (var item in items)
        {
            Console.WriteLine($"ID: {item.Id}, Name: {item.Name}, Qty: {item.Quantity}, Added: {item.DateAdded}");
        }
    }
}

// ---------------------
// Main Application Flow
// ---------------------
class InventoryRecord
{
    static void Main(string[] args)
    {
        string filePath = "inventory.json";

        // First session: Add & save data
        InventoryApp app = new InventoryApp(filePath);
        app.SeedSampleData();
        app.SaveData();

        // Simulate new session
        Console.WriteLine("\n--- New Session ---\n");
        app = new InventoryApp(filePath);
        app.LoadData();
        app.PrintAllItems();
    }
}
