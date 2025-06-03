using System;
using System.Collections.Generic;
using System.Linq; 

namespace ConsoleApplikation
{
   
    // Detta uppfyller kravet om minst två klasser tillsammans med Book-klassen.
    public abstract class InventoryItem 
    {
        public string Category { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; } 

        // Konstruktor f\u00f6r InventoryItem
        public InventoryItem(string category, string productName, decimal price)
        {
            Category = category;
            ProductName = productName;
            Price = price;
        }

       
        public abstract string PrintDetails();
    }

 
    public class Book : InventoryItem
    {
        public string Author { get; set; }
        public int Pages { get; set; }

 
        public Book(string category, string productName, decimal price, string author, int pages)
            : base(category, productName, price)
        {
            Author = author;
            Pages = pages;
        }

      
        public override string PrintDetails()
        {
           
            return $"{Author}, {ProductName}, {Pages} pages, Price: {Price:C2}, Category: {Category}";
        }
    }

    class Program
    {
        static List<InventoryItem> inventoryList = new List<InventoryItem>(); 

        static void Main(string[] args)
        {
            Console.WriteLine("## Inventory Management Console App");
            Console.WriteLine("Välkommen till Inventory Management. Du kan lägga till olika typer av objekt.");
            Console.WriteLine("Skriv 'q' när som helst för att avsluta inmatningen.");
            Console.WriteLine();

            bool keepAdding = true;
            while (keepAdding)
            {
                Console.WriteLine("\n--- Meny ---");
                Console.WriteLine("1. Lägg till en ny bok");
                Console.WriteLine("2. Visa befintlig inventarielista");
                Console.WriteLine("3. Sök i inventarielistan (Level 4)");
                Console.WriteLine("q. Avsluta programmet");
                Console.Write("Välj ett alternativ: ");

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
                        Console.WriteLine("Ogiltigt val, försök igen.");
                        break;
                }
            }

            Console.WriteLine("\nProgrammet avslutas. Tack för idag!");
            Console.ReadKey();
        }

        // Metod för att lägga till en bok
        static void AddBook()
        {
            string category, productName, author;
            decimal price;
            int pages;
            string input;

            Console.WriteLine("\n--- Lägg till en ny bok ---");

            // Kategori
            category = GetStringInput("Ange Kategori (t.ex. Fiktion, Fakta): ");

            // Produktnamn (Titel)
            productName = GetStringInput("Ange bokens Titel: ");

            // Pris
            price = GetDecimalInput("Ange Pris: ");

            // F\u00f6rfattare
            author = GetStringInput("Ange författare: ");

            // Antal sidor
            pages = GetIntInput("Ange antal sidor: ");

            // Skapa ett nytt Book-objekt och lägg till det i listan
            Book newBook = new Book(category, productName, price, author, pages);
            inventoryList.Add(newBook);
            Console.WriteLine("Boken har lagts till i inventariet!");
        }

        // Metod för att presentera inventarielistan
        static void PresentInventoryList(string searchTerm = null)
        {
            Console.WriteLine("\n--- Sammanfattning av inventariet ---");

            if (inventoryList.Count == 0)
            {
                Console.WriteLine("Inventarielistan är tom.");
                return;
            }

            // Sortera listan efter pris från lågt till högt med LINQ
            var sortedList = inventoryList.OrderBy(item => item.Price).ToList(); // LINQ används här

            decimal totalSum = 0;

            foreach (var item in sortedList)
            {
                string details = item.PrintDetails();
                // Level 4: Markera sökt objekt om sökterm finns
                if (!string.IsNullOrEmpty(searchTerm) && details.ToLower().Contains(searchTerm.ToLower()))
                {
                    Console.ForegroundColor = ConsoleColor.Yellow; // Sätt textfärg till gul
                    Console.WriteLine($"-> {details} <-"); // Markera
                    Console.ResetColor(); 
                }
                else
                {
                    Console.WriteLine(details);
                }
                totalSum += item.Price;
            }

            // Summera pris vid presentation av listan
            Console.WriteLine($"\nTotalt pris för alla objekt: {totalSum:C2}");
        }

        // Metod för sökfunktion (Level 4)
        static void SearchInventory()
        {
            Console.WriteLine("\n--- Sök i inventarielistan ---");
            string searchTerm = GetStringInput("Ange sökterm: ");

            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                Console.WriteLine("Söktermen får inte vara tom.");
                return;
            }

            // Sök efter objekt som matchar söktermen i deras detaljer
            // Använder LINQ för sökning
            var searchResults = inventoryList
                                .Where(item => item.PrintDetails().ToLower().Contains(searchTerm.ToLower()))
                                .ToList();

            if (searchResults.Any()) // LINQ: Any() för att kolla om det finns några resultat
            {
                Console.WriteLine($"\n--- Sökresultat för '{searchTerm}' ---");
                // Presentera de s\u00f6kta objekten och markera dem
                PresentInventoryList(searchTerm); // \u00c5teranv\u00e4nder PresentInventoryList med s\u00f6kterm
            }
            else
            {
                Console.WriteLine($"Inga objekt matchade söktermen '{searchTerm}'.");
            }
        }

        // Hjälpmetod för att f\u00e5 str\u00e4nginmatning med felhantering och 'q' check
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
                Console.WriteLine("Inmatningen får inte vara tom. Försök igen.");
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
