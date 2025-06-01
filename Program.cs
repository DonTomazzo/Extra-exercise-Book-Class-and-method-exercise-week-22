using System;
using System.Collections.Generic;
using System.Linq; // N\u00f6dv\u00e4ndig f\u00f6r LINQ

namespace ConsoleApplikation
{
    // Generell basklass f\u00f6r objekt med pris, kategori och namn.
    // Detta uppfyller kravet om minst tv\u00e5 klasser tillsammans med Book-klassen.
    public abstract class InventoryItem // Anv\u00e4nder abstract d\u00e5 vi inte direkt skapar InventoryItem-objekt
    {
        public string Category { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; } // decimal \u00e4r b\u00e4ttre f\u00f6r pengar \u00e4n double

        // Konstruktor f\u00f6r InventoryItem
        public InventoryItem(string category, string productName, decimal price)
        {
            Category = category;
            ProductName = productName;
            Price = price;
        }

        // Abstrakt metod som m\u00e5ste implementeras av \u00e4rvande klasser
        public abstract string PrintDetails();
    }

    // Book-klassen \u00e4rver nu fr\u00e5n InventoryItem
    public class Book : InventoryItem
    {
        public string Author { get; set; }
        public int Pages { get; set; }

        // Konstruktor f\u00f6r Book. Anropar basklassens konstruktor med 'base()'
        public Book(string category, string productName, decimal price, string author, int pages)
            : base(category, productName, price)
        {
            Author = author;
            Pages = pages;
        }

        // Implementerar den abstrakta PrintDetails-metoden
        public override string PrintDetails()
        {
            // F\u00f6r att matcha det \u00e4ldre formatet ("J. K. Rowling, Harry Potter and the Sorcerer's Stone, 223 pages")
            // anv\u00e4nder vi bara f\u00f6rfattare, titel och sidor.
            // Om Category och Price ska inkluderas, \u00e4ndra den h\u00e4r raden.
            return $"{Author}, {ProductName}, {Pages} pages, Price: {Price:C2}, Category: {Category}";
        }
    }

    class Program
    {
        static List<InventoryItem> inventoryList = new List<InventoryItem>(); // Global lista f\u00f6r att kunna \u00e5teranv\u00e4ndas

        static void Main(string[] args)
        {
            Console.WriteLine("## Inventory Management Console App");
            Console.WriteLine("V\u00e4lkommen till Inventory Management. Du kan l\u00e4gga till olika typer av objekt.");
            Console.WriteLine("Skriv 'q' n\u00e4r som helst f\u00f6r att avsluta inmatningen.");
            Console.WriteLine();

            bool keepAdding = true;
            while (keepAdding)
            {
                Console.WriteLine("\n--- Meny ---");
                Console.WriteLine("1. L\u00e4gg till en ny bok");
                Console.WriteLine("2. Visa befintlig inventarielista");
                Console.WriteLine("3. S\u00f6k i inventarielistan (Level 4)");
                Console.WriteLine("q. Avsluta programmet");
                Console.Write("V\u00e4lj ett alternativ: ");

                string choice = Console.ReadLine()?.ToLower();

                switch (choice)
                {
                    case "1":
                        AddBook();
                        break;
                    case "2":
                        PresentInventoryList();
                        break;
                    case "3":
                        SearchInventory();
                        break;
                    case "q":
                        keepAdding = false;
                        break;
                    default:
                        Console.WriteLine("Ogiltigt val, f\u00f6rs\u00f6k igen.");
                        break;
                }
            }

            Console.WriteLine("\nProgrammet avslutas. Tack f\u00f6r idag!");
            Console.ReadKey();
        }

        // Metod f\u00f6r att l\u00e4gga till en bok
        static void AddBook()
        {
            string category, productName, author;
            decimal price;
            int pages;
            string input;

            Console.WriteLine("\n--- L\u00e4gg till en ny bok ---");

            // Kategori
            category = GetStringInput("Ange Kategori (t.ex. Fiktion, Fakta): ");

            // Produktnamn (Titel)
            productName = GetStringInput("Ange bokens Titel: ");

            // Pris
            price = GetDecimalInput("Ange Pris: ");

            // F\u00f6rfattare
            author = GetStringInput("Ange f\u00f6rfattare: ");

            // Antal sidor
            pages = GetIntInput("Ange antal sidor: ");

            // Skapa ett nytt Book-objekt och l\u00e4gg till det i listan
            Book newBook = new Book(category, productName, price, author, pages);
            inventoryList.Add(newBook);
            Console.WriteLine("Boken har lagts till i inventariet!");
        }

        // Metod f\u00f6r att presentera inventarielistan
        static void PresentInventoryList(string searchTerm = null)
        {
            Console.WriteLine("\n--- Sammanfattning av inventariet ---");

            if (inventoryList.Count == 0)
            {
                Console.WriteLine("Inventarielistan \u00e4r tom.");
                return;
            }

            // Sortera listan efter pris fr\u00e5n l\u00e5gt till h\u00f6gt med LINQ
            var sortedList = inventoryList.OrderBy(item => item.Price).ToList(); // LINQ anv\u00e4nds h\u00e4r

            decimal totalSum = 0;

            foreach (var item in sortedList)
            {
                string details = item.PrintDetails();
                // Level 4: Markera s\u00f6kt objekt om s\u00f6kterm finns
                if (!string.IsNullOrEmpty(searchTerm) && details.ToLower().Contains(searchTerm.ToLower()))
                {
                    Console.ForegroundColor = ConsoleColor.Yellow; // S\u00e4tt textf\u00e4rg till gul
                    Console.WriteLine($"-> {details} <-"); // Markera
                    Console.ResetColor(); // \u00c5terst\u00e4ll f\u00e4rgen
                }
                else
                {
                    Console.WriteLine(details);
                }
                totalSum += item.Price;
            }

            // Summera pris vid presentation av listan
            Console.WriteLine($"\nTotalt pris f\u00f6r alla objekt: {totalSum:C2}");
        }

        // Metod f\u00f6r s\u00f6kfunktion (Level 4)
        static void SearchInventory()
        {
            Console.WriteLine("\n--- S\u00f6k i inventarielistan ---");
            string searchTerm = GetStringInput("Ange s\u00f6kterm: ");

            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                Console.WriteLine("S\u00f6ktermen f\u00e5r inte vara tom.");
                return;
            }

            // S\u00f6k efter objekt som matchar s\u00f6ktermen i deras detaljer
            // Anv\u00e4nder LINQ f\u00f6r s\u00f6kning
            var searchResults = inventoryList
                                .Where(item => item.PrintDetails().ToLower().Contains(searchTerm.ToLower()))
                                .ToList();

            if (searchResults.Any()) // LINQ: Any() f\u00f6r att kolla om det finns n\u00e5gra resultat
            {
                Console.WriteLine($"\n--- S\u00f6kresultat f\u00f6r '{searchTerm}' ---");
                // Presentera de s\u00f6kta objekten och markera dem
                PresentInventoryList(searchTerm); // \u00c5teranv\u00e4nder PresentInventoryList med s\u00f6kterm
            }
            else
            {
                Console.WriteLine($"Inga objekt matchade s\u00f6ktermen '{searchTerm}'.");
            }
        }

        // Hj\u00e4lpmetod f\u00f6r att f\u00e5 str\u00e4nginmatning med felhantering och 'q' check
        static string GetStringInput(string prompt)
        {
            string input;
            while (true)
            {
                Console.Write(prompt);
                input = Console.ReadLine();

                if (input?.ToLower() == "q")
                {
                    Environment.Exit(0); // Avslutar programmet direkt
                }
                if (!string.IsNullOrWhiteSpace(input))
                {
                    return input.Trim(); // Returnera trimmad str\u00e4ng
                }
                Console.WriteLine("Inmatningen f\u00e5r inte vara tom. F\u00f6rs\u00f6k igen.");
            }
        }

        // Hj\u00e4lpmetod f\u00f6r att f\u00e5 decimalinmatning med felhantering och 'q' check
        static decimal GetDecimalInput(string prompt)
        {
            string input;
            decimal value;
            while (true)
            {
                Console.Write(prompt);
                input = Console.ReadLine();

                if (input?.ToLower() == "q")
                {
                    Environment.Exit(0); // Avslutar programmet direkt
                }
                // Felhantering
                if (decimal.TryParse(input, out value) && value >= 0) // Pris kan inte vara negativt
                {
                    return value;
                }
                Console.WriteLine("Ogiltigt pris. Ange ett numeriskt v\u00e4rde (t.ex. 123.45) eller 'q' f\u00f6r att avsluta.");
            }
        }

        // Hj\u00e4lpmetod f\u00f6r att f\u00e5 int-inmatning med felhantering och 'q' check
        static int GetIntInput(string prompt)
        {
            string input;
            int value;
            while (true)
            {
                Console.Write(prompt);
                input = Console.ReadLine();

                if (input?.ToLower() == "q")
                {
                    Environment.Exit(0); // Avslutar programmet direkt
                }
                // Felhantering
                if (int.TryParse(input, out value) && value > 0) // Antal sidor m\u00e5ste vara positivt
                {
                    return value;
                }
                Console.WriteLine("Ogiltigt antal. Ange ett positivt heltal eller 'q' f\u00f6r att avsluta.");
            }
        }
    }
}
