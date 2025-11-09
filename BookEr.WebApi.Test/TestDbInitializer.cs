using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookEr.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BookEr.WebApi.Test
{
    public static class TestDbInitializer
    {
        private static BookerDbContext _context = null!;
        //private static UserManager<ApplicationUser> _userManager = null!;
        //private static RoleManager<IdentityRole<int>> _roleManager = null!;

        public static void Initialize(BookerDbContext context, string imageDirectory)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));


            var roles = new[] { "Visitor", "Librarian" };

            /*foreach (var role in roles)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    await _roleManager.CreateAsync(new IdentityRole<int>(role));
                }
            }*/

            SeedBooks(imageDirectory);
            SeedVisitorsAndBorrows(imageDirectory);
            SeedLibrarians();

            _context.SaveChanges();
        }

        private static void SeedBooks(String imgDir)
        {
            IList<Book> defaultBooks = new List<Book>
            {
                new Book()
                {
                    Title = "The Lord of the Rings: The Fellowship of the Ring",
                    Author = "J. R. R. Tolkien",
                    ISBN = "9780618260515",
                    Year = 1954,
                    Image = File.Exists(Path.Combine(imgDir, "Lotr1.png")) ? File.ReadAllBytes(Path.Combine(imgDir, "Lotr1.png")) : null,
                    Volumes = new List<Volume>
                    {
                        new Volume(),
                    }
                },
                new Book()
                {
                    Title = "The Lord of the Rings: The Two Towers",
                    Author = "J. R. R. Tolkien",
                    ISBN = "9780261103948",
                    Year = 1954,
                    Image = File.Exists(Path.Combine(imgDir, "Lotr2.png")) ? File.ReadAllBytes(Path.Combine(imgDir, "Lotr2.png")) : null,
                    Volumes = new List<Volume>
                    {
                        new Volume(),
                        new Volume(),
                        new Volume(),
                        new Volume(),
                    }
                },
                new Book()
                {
                    Title = "The Lord of the Rings: The Return of the King",
                    Author = "J. R. R. Tolkien",
                    ISBN = "9780007136575",
                    Year = 1955,
                    Image = File.Exists(Path.Combine(imgDir, "Lotr3.png")) ? File.ReadAllBytes(Path.Combine(imgDir, "Lotr3.png")) : null,
                    Volumes = new List<Volume>
                    {
                        new Volume(),
                        new Volume(),
                        new Volume(),
                        new Volume(),
                    }
                },
                new Book()
                {
                    Title = "Harry Potter and the Philosopher's Stone",
                    Author = "J. K. Rowling",
                    ISBN = "9780747532699",
                    Year = 1997,
                    Image = File.Exists(Path.Combine(imgDir, "HP1.png")) ? File.ReadAllBytes(Path.Combine(imgDir, "HP1.png")) : null,
                    Volumes = new List<Volume>
                    {
                        new Volume(),
                        new Volume(),
                    }
                },
                new Book()
                {
                    Title = "Harry Potter and the Chamber of Secrets",
                    Author = "J. K. Rowling",
                    ISBN = "9780439064866",
                    Year = 1998,
                    Image = File.Exists(Path.Combine(imgDir, "HP2.png")) ? File.ReadAllBytes(Path.Combine(imgDir, "HP2.png")) : null,
                    Volumes = new List<Volume>
                    {
                        new Volume(),
                        new Volume(),
                    }
                },
                new Book()
                {
                    Title = "Harry Potter and the Prisoner of Azkaban",
                    Author = "J. K. Rowling",
                    ISBN = "9780439136358",
                    Year = 1999,
                    Image = File.Exists(Path.Combine(imgDir, "HP3.png")) ? File.ReadAllBytes(Path.Combine(imgDir, "HP3.png")) : null,
                    Volumes = new List<Volume>
                    {
                        new Volume(),
                        new Volume(),
                    }
                },
                new Book()
                {
                    Title = "harry potter and the goblet of fire",
                    Author = "J. K. Rowling",
                    ISBN = "9780439139595",
                    Year = 2000,
                    Image = File.Exists(Path.Combine(imgDir, "HP4.png")) ? File.ReadAllBytes(Path.Combine(imgDir, "HP4.png")) : null,
                    Volumes = new List<Volume>
                    {
                        new Volume(),
                        new Volume(),
                        new Volume(),
                    }
                },
                new Book()
                {
                    Title = "Harry Potter and the Order of the Phoenix",
                    Author = "J. K. Rowling",
                    ISBN = "9780439358064",
                    Year = 2003,
                    Image = File.Exists(Path.Combine(imgDir, "HP5.png")) ? File.ReadAllBytes(Path.Combine(imgDir, "HP5.png")) : null,
                    Volumes = new List<Volume>
                    {
                        new Volume(),
                        new Volume(),
                        new Volume(),
                    }
                },
                new Book()
                {
                    Title = "Harry Potter and the Half-Blood Prince",
                    Author = "J. K. Rowling",
                    ISBN = "9780439791328",
                    Year = 2005,
                    Image = File.Exists(Path.Combine(imgDir, "HP6.png")) ? File.ReadAllBytes(Path.Combine(imgDir, "HP6.png")) : null,
                    Volumes = new List<Volume>
                    {
                        new Volume(),
                        new Volume(),
                        new Volume(),
                    }
                },
                new Book()
                {
                    Title = "Harry Potter and the Deathly Hallows",
                    Author = "J. K. Rowling",
                    ISBN = "9780545029360",
                    Year = 2007,
                    Image = File.Exists(Path.Combine(imgDir, "HP7.png")) ? File.ReadAllBytes(Path.Combine(imgDir, "HP7.png")) : null,
                    Volumes = new List<Volume>
                    {
                        new Volume(),
                        new Volume(),
                    }
                },
                new Book()
                {
                    Title = "The Hobbit",
                    Author = "J. R. R. Tolkien",
                    ISBN = "9780044403371",
                    Year = 1937,
                    Image = File.Exists(Path.Combine(imgDir, "Hobbit.png")) ? File.ReadAllBytes(Path.Combine(imgDir, "Hobbit.png")) : null,
                    Volumes = new List<Volume>
                    {
                        new Volume(),
                        new Volume(),
                    }
                },
                new Book()
                {
                    Title = "Fire & Blood",
                    Author = "George R. R. Martin",
                    ISBN = "9781524796303",
                    Year = 2018,
                    Image = File.Exists(Path.Combine(imgDir, "GoT1.png")) ? File.ReadAllBytes(Path.Combine(imgDir, "GoT1.png")) : null,
                    Volumes = new List<Volume>
                    {
                        new Volume(),
                        new Volume(),
                    }
                },
                new Book()
                {
                    Title = "A Game of Thrones",
                    Author = "George R. R. Martin",
                    ISBN = "9780007428540",
                    Year = 1996,
                    Image = File.Exists(Path.Combine(imgDir, "GoT2.png")) ? File.ReadAllBytes(Path.Combine(imgDir, "GoT2.png")) : null,
                    Volumes = new List<Volume>
                    {
                        new Volume(),
                        new Volume(),
                    }
                },
                new Book()
                {
                    Title = "A Clash of Kings",
                    Author = "George R. R. Martin",
                    ISBN = "9780006479895",
                    Year = 1998,
                    Image = File.Exists(Path.Combine(imgDir, "GoT3.png")) ? File.ReadAllBytes(Path.Combine(imgDir, "GoT3.png")) : null,
                    Volumes = new List<Volume>
                    {
                        new Volume(),
                    }
                },
                new Book()
                {
                    Title = "A Storm of Swords",
                    Author = "George R. R. Martin",
                    ISBN = "9780008115425",
                    Year = 2000,
                    Image = File.Exists(Path.Combine(imgDir, "GoT4.png")) ? File.ReadAllBytes(Path.Combine(imgDir, "GoT4.png")) : null,
                    Volumes = new List<Volume>
                    {
                        new Volume(),
                    }
                },
                new Book()
                {
                    Title = "A Feast for Crows",
                    Author = "George R. R. Martin",
                    ISBN = "9780553801507",
                    Year = 2011,
                    Image = File.Exists(Path.Combine(imgDir, "GoT5.png")) ? File.ReadAllBytes(Path.Combine(imgDir, "GoT5.png")) : null,
                    Volumes = new List<Volume>
                    {
                        new Volume(),
                        new Volume(),
                        new Volume(),
                    }
                },
                new Book()
                {
                    Title = "A Dance with Dragons",
                    Author = "George R. R. Martin",
                    ISBN = "9780002247399",
                    Year = 2011,
                    Image = File.Exists(Path.Combine(imgDir, "GoT6.png")) ? File.ReadAllBytes(Path.Combine(imgDir, "GoT6.png")) : null,
                    Volumes = new List<Volume>
                    {
                        new Volume(),
                        new Volume(),
                        new Volume(),
                    }
                },
                new Book()
                {
                    Title = "The Great Gatsby",
                    Author = "F. Scott Fitzgerald",
                    ISBN = "9780199536405",
                    Year = 1925,
                    Image = File.Exists(Path.Combine(imgDir, "Gatsby.png")) ? File.ReadAllBytes(Path.Combine(imgDir, "Gatsby.png")) : null,
                    Volumes = new List<Volume>
                    {
                        new Volume(),
                        new Volume(),
                        new Volume(),
                    }
                },
                new Book()
                {
                    Title = "War and Peace",
                    Author = "Leo Tolstoy",
                    ISBN = "9780393042375",
                    Year = 1867,
                    Image = File.Exists(Path.Combine(imgDir, "War.png")) ? File.ReadAllBytes(Path.Combine(imgDir, "War.png")) : null,
                    Volumes = new List<Volume>
                    {
                        new Volume(),
                        new Volume(),
                        new Volume(),
                    }
                },
                new Book()
                {
                    Title = "The Witcher: The Last Wish",
                    Author = "Andrzej Sapkowski",
                    ISBN = "9780316029186",
                    Year = 1993,
                    Image = File.Exists(Path.Combine(imgDir, "Witcher1.png")) ? File.ReadAllBytes(Path.Combine(imgDir, "Witcher1.png")) : null,
                    Volumes = new List<Volume>
                    {
                        new Volume(),
                        new Volume(),
                        new Volume(),
                    }
                },
                new Book()
                {
                    Title = "The Witcher: Sword of Destiny",
                    Author = "Andrzej Sapkowski",
                    ISBN = "9780316389709",
                    Year = 1992,
                    Image = File.Exists(Path.Combine(imgDir, "Witcher2.png")) ? File.ReadAllBytes(Path.Combine(imgDir, "Witcher2.png")) : null,
                    Volumes = new List<Volume>
                    {
                        new Volume(),
                        new Volume(),
                        new Volume(),
                    }
                },

            };

            _context.Books.AddRange(defaultBooks);
            _context.SaveChanges();
        }

        private static void SeedVisitorsAndBorrows(String imgDir)
        {
            Book InitialForTeszt = new Book()
            {
                Title = "Dune",
                Author = "Frank Herbert",
                ISBN = "9780575068568",
                Year = 1965,
                Image = File.Exists(Path.Combine(imgDir, "Dune.png")) ? File.ReadAllBytes(Path.Combine(imgDir, "Dune.png")) : null,
                Volumes = new List<Volume>
                    {
                        new Volume
                        {
                            IsAvailable = false,
                        },
                        new Volume()
                    }
            };
            _context.Books.Add(InitialForTeszt);

            var AppUser = new ApplicationUser();
            AppUser.Name = "Teszt Elek";
            AppUser.PhoneNumber = "06301221431";
            AppUser.Email = "Teszt@elek.com";
            AppUser.UserName = "TesztElek";

            Visitor Initial = new Visitor
            {
                ApplicationUser = AppUser,
                Address = "1234 Tesztfalva Teszt utca 54",
                Borrows = new List<Borrow>
                {
                    new Borrow
                    {
                        Volume = InitialForTeszt.Volumes.First(),
                        StartDay = DateTime.Today - TimeSpan.FromDays(1),
                        EndDay = DateTime.Today + TimeSpan.FromDays(2),
                        IsActive = true,

                    },
                    new Borrow
                    {
                        Volume = InitialForTeszt.Volumes.First(),
                        StartDay = DateTime.Today + TimeSpan.FromDays(3),
                        EndDay = DateTime.Today + TimeSpan.FromDays(5),
                        IsActive = false,

                    },
                }

            };

            //string password = "Almaalma1";

            //await _userManager.CreateAsync(AppUser, password);
            //await _userManager.AddToRoleAsync(AppUser, "Visitor");
            _context.Visitors.Add(Initial);
            _context.SaveChanges();
        }

        private static void SeedLibrarians()
        {
            var AppUser = new ApplicationUser();
            AppUser.Name = "Könyves Kálmán";
            AppUser.PhoneNumber = "06301221431";
            AppUser.Email = "Könyv@es.com";
            AppUser.UserName = "Karcsi";

            Librarian Initial = new Librarian()
            {
                ApplicationUser = AppUser,
            };

            //string password = "Almaalma2";

            //await _userManager.CreateAsync(AppUser, password);
            //await _userManager.AddToRoleAsync(AppUser, "Librarian");
            _context.Librarians.Add(Initial);
            _context.SaveChanges();
        }
    }

}
