using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Entities;

namespace WebApplication1.DAL
{
    public class LibraryEntityDbContext:IdentityDbContext<IdentityUser>
    {
        public LibraryEntityDbContext(DbContextOptions<LibraryEntityDbContext> options) :base(options)
        {}

        public DbSet<Book> Books { get; set; }
        public DbSet<MyBookList> MyBookLists { get; set; }
    }
}
