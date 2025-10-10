using System;

List<Book> books = new();

// Allow user to enter a book title and add it to a list
Console.WriteLine("Enter book: ");
while (true)
{
    Console.Write("\nBook title: ");
    var title = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(title))
        break;

    Console.Write("Author: ");
    var author = Console.ReadLine();

    Console.Write("Year published: ");
    if (int.TryParse(Console.ReadLine(), out int year))
    {
        books.Add(new Book(title, author ?? "Unknown", year));
        Console.WriteLine($"Added: {title}");
    }
    else
    {
        Console.WriteLine("Invalid year. Book not added.");
    }
}

// Display all entered books
Console.WriteLine("\n\n");
if (books.Count > 0)
{
    foreach (var book in books)
    {
        Console.WriteLine($"- {book.Title} by {book.Author} ({book.YearPublished})");
    }
}
else
{
    Console.WriteLine("No books added.");
}

var borrower1 = new Borrower("1", "John Doe", new List<Book>());
Console.WriteLine($"First borrower: {borrower1.Name}, Books: {borrower1.BorrowedBooks.Count}");

if (books.Count == 0)
{
    books.Add(new Book("The Great Gatsby", "F. Scott Fitzgerald", 1925));
    books.Add(new Book("1984", "George Orwell", 1949));
    books.Add(new Book("The Hunger Games", "Suzanne Collins", 2008));
    books.Add(new Book("Ready Player One", "Ernest Cline", 2011));
    books.Add(new Book("The Martian", "Andy Weir", 2014));
}

var updatedBorrowedBooks = new List<Book>(borrower1.BorrowedBooks) { books[0] };

// Clone a borrower using 'with' to add a new borrowed book
var borrower2 = borrower1 with { BorrowedBooks = updatedBorrowedBooks };
Console.WriteLine($"Cloned borrower: {borrower2.Name}, Books: {borrower2.BorrowedBooks.Count}");
Console.WriteLine($"Borrowed: {borrower2.BorrowedBooks[0].Title}");

var librarian = new Librarian("John Doe", "john.doe@library.com", "Fiction");
Console.WriteLine($"Librarian: {librarian.Name}, Email: {librarian.Email}, Section: {librarian.LibrarySection}");

DisplayObjectInfo(books[0]);
DisplayObjectInfo(borrower2);
DisplayObjectInfo(librarian);

// Filter books published after 2010 and display them
Func<Book, bool> filterAfter2010 = static book => book.YearPublished > 2010;
var recentBooks = books.Where(filterAfter2010).ToList();

if (recentBooks.Count > 0)
{
    foreach (var book in recentBooks)
    {
        Console.WriteLine($"- {book.Title} by {book.Author} ({book.YearPublished})");
    }
}
else
{
    Console.WriteLine("No books published after 2010.");
}

// Create a method that receives an object and:
static void DisplayObjectInfo(object obj)
{
    switch (obj)
    {
        // If it's a Book, display title and year
        case Book book:
            Console.WriteLine($"Book: '{book.Title}', Year: {book.YearPublished}");
            break;
        // If it's a Borrower, display name and number of borrowed books
        case Borrower borrower:
            Console.WriteLine($"Borrower: {borrower.Name}, Borrowed books: {borrower.BorrowedBooks.Count}");
            break;
        // Else, display 'Unknown type'
        default:
            Console.WriteLine("Unknown type");
            break;
    }
}

// Create a Book record with Title, Author, YearPublished
public record Book(string Title, string Author, int YearPublished);

// Create a Borrower record with Id, Name, BorrowedBooks (list of Book)
public record Borrower(string Id, string Name, List<Book> BorrowedBooks);

// Create a Librarian class with Name, Email, LibrarySection set only at initialization
public class Librarian
{
    public string Name { get; init; }
    public string Email { get; init; }
    public string LibrarySection { get; init; }

    public Librarian(string name, string email, string librarySection)
    {
        Name = name;
        Email = email;
        LibrarySection = librarySection;
    }
}