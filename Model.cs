using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

public class BloggingContext : DbContext
{
    public DbSet<Post> Posts { get; set; }
    public DbSet<Person> People { get; set; }

    public string DbPath { get; }

    public BloggingContext()
    {
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        DbPath = Path.Join(path, "blogging.db");
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options) {
        var conn = "CONNSTRING";
        options.UseSqlServer(conn);
        // options.UseSqlite();
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Post>()
            .OwnsOne(p => p.Address);
    }

}

public class Post
{
    public int PostId { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }

    public Address Address { get; set; }
}

public class Person
{
    public int PersonId { get; set; }
    public IEnumerable<PersonAddress> Addresses { get; set; } = new List<PersonAddress>();
}

public class Address
{
    [Key]
    public int AddressId { get; set; }
    
    public string City { get; set; }
}

public class PersonAddress : Address
{
    public string Street { get; set; }
}