using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace MockingPearls._08_Databases;

public class MyDbContext : DbContext
{
    public MyDbContext(DbContextOptions<MyDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<User> Users { get; set; }
}

public class User
{
    [Key]
    public int Id { get; set; }

    public string Username { get; set; }
}

public class UserRepository
{
    private readonly MyDbContext _context;

    public UserRepository(MyDbContext context)
    {
        _context = context;
    }

    public void CreateUser(string username)
    {
        var entity = new User
        {
            Username = username
        };
        _context.Users.Add(entity);
        _context.SaveChanges();
    }
}

public class DatabaseTest
{
    private static DbContextOptions<MyDbContext> PrepareInMemoryDbContext()
    {
        return new DbContextOptionsBuilder<MyDbContext>()
            .UseInMemoryDatabase("TEST_DB")
            .Options;
    }

    private static DbContextOptions<MyDbContext> PrepareSqlServerDbContext(string connectionString)
    {
        return new DbContextOptionsBuilder<MyDbContext>()
            .UseSqlServer(connectionString)
            .Options;
    }

    [Fact]
    public void ThenInMemory_ShouldWorkSuccessfully()
    {
        // Arrange
        var options = PrepareInMemoryDbContext();
        using var context = new MyDbContext(options);
        var repo = new UserRepository(context);

        // Act 
        repo.CreateUser("Yoda");

        // Assert
        Assert.Equal(1, context.Users.Count());
        Assert.Equal("Yoda", context.Users.First().Username);
    }

}

/*

    Real DB
        Ständig sich selbst und Anderen in die Quere kommen

    InMemory
        SQLite kann ich Relationen
        SQL Server kann keine Relations

    DB File
        Vordefinieren von komplexen Zuständen / großen Daten
        in SQLite möglich, in SQL Server nicht

    Mocking von DbContext und DbSet
        Struggle

  
 */
