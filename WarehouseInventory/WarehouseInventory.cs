using System;
using System.Collections.Generic;
using WarehouseInventory;

namespace WarehouseInventory
{
    // a. Marker Interface
    public interface IInventoryItem
    {
        int Id { get; }
        string Name { get; }
        int Quantity { get; set; }
    }

    // b. ElectronicItem
    public class ElectronicItem : IInventoryItem
    {
        public int Id { get; }
        public string Name { get; }
        public int Quantity { get; set; }
        public string Brand { get; }
        public int WarrantyMonths { get; }

        public ElectronicItem(int id, string name, int quantity, string brand, int warrantyMonths)
        {
            Id = id;
            Name = name;
            Quantity = quantity;
            Brand = brand;
            WarrantyMonths = warrantyMonths;
        }

        public override string ToString()
        {
            return $"[Electronic] ID: {Id}, Name: {Name}, Brand: {Brand}, Warranty: {WarrantyMonths} months, Qty: {Quantity}";
        }
    }

    // c. GroceryItem
    public class GroceryItem : IInventoryItem
    {
        public int Id { get; }
        public string Name { get; }
        public int Quantity { get; set; }
        public DateTime ExpiryDate { get; }

        public GroceryItem(int id, string name, int quantity, DateTime expiryDate)
        {
            Id = id;
            Name = name;
            Quantity = quantity;
            ExpiryDate = expiryDate;
        }

        public override string ToString()
        {
            return $"[Grocery] ID: {Id}, Name: {Name}, Expiry: {ExpiryDate:yyyy-MM-dd}, Qty: {Quantity}";
        }
    }

    // e. Custom Exceptions
    public class DuplicateItemException : Exception
    {
        public DuplicateItemException(string message) : base(message) { }
    }

    public class ItemNotFoundException : Exception
    {
        public ItemNotFoundException(string message) : base(message) { }
    }

    public class InvalidQuantityException : Exception
    {
        public InvalidQuantityException(string message) : base(message) { }
    }

    // d. Generic InventoryRepository
    public class InventoryRepository<T> where T : IInventoryItem
    {
        private Dictionary<int, T> _items = new Dictionary<int, T>();

        public void AddItem(T item)
        {
            if (_items.ContainsKey(item.Id))
            {
                throw new DuplicateItemException($"Item with ID {item.Id} already exists.");
            }
            _items[item.Id] = item;
        }

        public T GetItemById(int id)
        {
            if (!_items.ContainsKey(id))
            {
                throw new ItemNotFoundException($"Item with ID {id} not found.");
            }
            return _items[id];
        }

        public void RemoveItem(int id)
        {
            if (!_items.Remove(id))
            {
                throw new ItemNotFoundException($"Item with ID {id} not found for removal.");
            }
        }

        public List<T> GetAllItems()
        {
            return new List<T>(_items.Values);
        }

        public void UpdateQuantity(int id, int newQuantity)
        {
            if (newQuantity < 0)
            {
                throw new InvalidQuantityException("Quantity cannot be negative.");
            }
            if (!_items.ContainsKey(id))
            {
                throw new ItemNotFoundException($"Item with ID {id} not found for update.");
            }
            _items[id].Quantity = newQuantity;
        }
    }

    // f. WarehouseManager
    public class WarehouseManager
    {
        private InventoryRepository<ElectronicItem> _electronics = new InventoryRepository<ElectronicItem>();
        private InventoryRepository<GroceryItem> _groceries = new InventoryRepository<GroceryItem>();

        public void SeedData()
        {
            _electronics.AddItem(new ElectronicItem(1, "Laptop", 10, "Dell", 24));
            _electronics.AddItem(new ElectronicItem(2, "Smartphone", 15, "Samsung", 12));

            _groceries.AddItem(new GroceryItem(1, "Apples", 50, DateTime.Now.AddDays(10)));
            _groceries.AddItem(new GroceryItem(2, "Milk", 30, DateTime.Now.AddDays(5)));
        }

        public void PrintAllItems<T>(InventoryRepository<T> repo) where T : IInventoryItem
        {
            foreach (var item in repo.GetAllItems())
            {
                Console.WriteLine(item);
            }
        }

        public void IncreaseStock<T>(InventoryRepository<T> repo, int id, int quantity) where T : IInventoryItem
        {
            try
            {
                var item = repo.GetItemById(id);
                repo.UpdateQuantity(id, item.Quantity + quantity);
                Console.WriteLine($"Stock increased for item ID {id}. New Quantity: {item.Quantity}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        public void RemoveItemById<T>(InventoryRepository<T> repo, int id) where T : IInventoryItem
        {
            try
            {
                repo.RemoveItem(id);
                Console.WriteLine($"Item with ID {id} removed successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        public void Run()
        {
            Console.WriteLine("---- Grocery Items ----");
            PrintAllItems(_groceries);

            Console.WriteLine("\n---- Electronic Items ----");
            PrintAllItems(_electronics);

            Console.WriteLine("\n--- Testing Exceptions ---");

            try
            {
                _groceries.AddItem(new GroceryItem(1, "Bananas", 20, DateTime.Now.AddDays(7))); // duplicate
            }
            catch (DuplicateItemException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            RemoveItemById(_electronics, 99); // non-existent

            try
            {
                _groceries.UpdateQuantity(2, -5); // invalid quantity
            }
            catch (InvalidQuantityException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }

    // Main Application
    public class Program
    {
        public static void Main()
        {
            WarehouseManager manager = new WarehouseManager();
            manager.SeedData();
            manager.Run();
        }
    }
}
 