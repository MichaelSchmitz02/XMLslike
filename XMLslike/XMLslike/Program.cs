using System;
using System.Linq;
using System.Xml.Linq;

class Program
{
    static void Main()
    {
        
        XDocument prviXml = XDocument.Load("prvi.xml");
        XDocument drugiXml = XDocument.Load("drugi.xml");

        
        var prviBooks = prviXml.Descendants("book")
            .Select(book => new
            {
                Id = (string)book.Attribute("id"),
                Image = (string)book.Attribute("image"),
                Name = (string)book.Attribute("name")
            }).ToList();

        var drugiBooks = drugiXml.Descendants("book")
            .Select(book => new
            {
                Id = (string)book.Attribute("id"),
                Image = (string)book.Attribute("image"),
                Name = (string)book.Attribute("name")
            }).ToList();

        
        var differences = prviBooks.Join(drugiBooks,
            prvi => prvi.Id,
            drugi => drugi.Id,
            (prvi, drugi) => new
            {
                Id = prvi.Id,
                ImageDifference = prvi.Image != drugi.Image ? ($"{prvi.Image} vs {drugi.Image}") : null,
                NameDifference = prvi.Name != drugi.Name ? ($"{prvi.Name} vs {drugi.Name}") : null
            })
            .Where(diff => diff.ImageDifference != null || diff.NameDifference != null)
            .ToList();

        
        var missingInSecond = prviBooks.Where(prvi => !drugiBooks.Any(drugi => drugi.Id == prvi.Id))
            .Select(prvi => new { Id = prvi.Id, Issue = "Missing in second document" });

        var missingInFirst = drugiBooks.Where(drugi => !prviBooks.Any(prvi => prvi.Id == drugi.Id))
            .Select(drugi => new { Id = drugi.Id, Issue = "Missing in first document" });

        
        Console.WriteLine("Issued\tIssue Type\t\tIssueInFirst\tIssueInSecond");

        int issueNumber = 1;
        foreach (var diff in differences)
        {
            if (diff.ImageDifference != null)
            {
                Console.WriteLine($"{issueNumber++}\timage is different\t{diff.Id}\t{diff.ImageDifference}");
            }
            if (diff.NameDifference != null)
            {
                Console.WriteLine($"{issueNumber++}\tname is different\t{diff.Id}\t{diff.NameDifference}");
            }
        }

        foreach (var missing in missingInSecond)
        {
            Console.WriteLine($"{issueNumber++}\t{missing.Issue}\t{missing.Id}\t-");
        }

        foreach (var missing in missingInFirst)
        {
            Console.WriteLine($"{issueNumber++}\t{missing.Issue}\t-\t{missing.Id}");
        }
        Console.ReadKey();
    }
}
