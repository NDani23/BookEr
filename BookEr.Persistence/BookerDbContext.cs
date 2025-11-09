using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace BookEr.Persistence
{
    public class BookerDbContext : IdentityDbContext<ApplicationUser,IdentityRole<int>, int>
    {
        public DbSet<Book> Books { get; set; } = null!;
        public DbSet<Volume> Volumes { get; set; } = null!;
        public DbSet<Borrow> Borrows { get; set; } = null!;
        public DbSet<ApplicationUser> ApplicationUsers { get; set; } = null!;
        public DbSet<Visitor> Visitors { get; set; } = null!;
        public DbSet<Librarian> Librarians { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<ApplicationUser>(entity => { entity.ToTable("Users"); });
            builder.Entity<Visitor>(entity => { entity.ToTable("Visitors"); });
            builder.Entity<Librarian>(entity => { entity.ToTable("Librarians"); });
            builder.Entity<Librarian>().HasKey(c => new { c.UserId });
            builder.Entity<Visitor>().HasKey(c => new { c.UserId });
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                IConfigurationRoot configuration = new ConfigurationBuilder()
                   .SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("appsettings.json")
                   .Build();
                var connectionString = configuration.GetConnectionString("SqlServerConnection");
                optionsBuilder.UseSqlServer(connectionString);
            }
        }
        public BookerDbContext() {}
        public BookerDbContext(DbContextOptions<BookerDbContext> options)
            : base(options)
        {
        }
    }
}
