using Mde.Project.WebApi.Entities;

namespace Mde.Project.WebApi.Data
{
    public static class DbInitializer
    {
        public static void Seed(ApplicationDbContext db)
        {
            // Voeg testdata toe als er nog geen events zijn
            if (!db.Events.Any())
            {
                db.Events.AddRange(new[]
                {
                new Event { Title = "Grand Slam Tokyo", Location = "Tokyo, Japan", Date = DateTime.Today.AddDays(10) },
                new Event { Title = "World Masters", Location = "Budapest, Hungary", Date = DateTime.Today.AddDays(20) },
                new Event { Title = "Paris Grand Slam", Location = "Paris, France", Date = DateTime.Today.AddDays(30) }
            });

                db.Judokas.AddRange(new[]
   {
        // -60
        new Judoka { FullName = "Naohisa Takato", Country = "Japan", Category = "-60" },
        new Judoka { FullName = "Yeldos Smetov", Country = "Kazakhstan", Category = "-60" },
        new Judoka { FullName = "Francisco Garrigos", Country = "Spanje", Category = "-60" },
        new Judoka { FullName = "Yang Yung Wei", Country = "Taiwan", Category = "-60" },
        new Judoka { FullName = "Luka Mkheidze", Country = "Frankrijk", Category = "-60" },

        // -66
        new Judoka { FullName = "Hifumi Abe", Country = "Japan", Category = "-66" },
        new Judoka { FullName = "Manuel Lombardo", Country = "Italië", Category = "-66" },
        new Judoka { FullName = "Vazha Margvelashvili", Country = "Georgië", Category = "-66" },
        new Judoka { FullName = "Baruch Shmailov", Country = "Israël", Category = "-66" },
        new Judoka { FullName = "Yondonperenlei Baskhuu", Country = "Mongolië", Category = "-66" },

        // -73
        new Judoka { FullName = "Shohei Ono", Country = "Japan", Category = "-73" },
        new Judoka { FullName = "Hashimoto Soichi", Country = "Japan", Category = "-73" },
        new Judoka { FullName = "An Changrim", Country = "Zuid-Korea", Category = "-73" },
        new Judoka { FullName = "Rustam Orujov", Country = "Azerbeidzjan", Category = "-73" },
        new Judoka { FullName = "Tsend-Ochir Tsogtbaatar", Country = "Mongolië", Category = "-73" },

        // -81
        new Judoka { FullName = "Matthias Casse", Country = "België", Category = "-81" },
        new Judoka { FullName = "Saeid Mollaei", Country = "Mongolië", Category = "-81" },
        new Judoka { FullName = "Dominic Ressel", Country = "Duitsland", Category = "-81" },
        new Judoka { FullName = "Frank De Wit", Country = "Nederland", Category = "-81" },
        new Judoka { FullName = "Vedat Albayrak", Country = "Turkije", Category = "-81" },

        // -90
        new Judoka { FullName = "Mikhail Igolnikov", Country = "Rusland", Category = "-90" },
        new Judoka { FullName = "Nemanja Majdov", Country = "Servië", Category = "-90" },
        new Judoka { FullName = "Marcus Nyman", Country = "Zweden", Category = "-90" },
        new Judoka { FullName = "Noel Van 't End", Country = "Nederland", Category = "-90" },
        new Judoka { FullName = "Eduard Trippel", Country = "Duitsland", Category = "-90" },

        // -100
        new Judoka { FullName = "Varlam Liparteliani", Country = "Georgië", Category = "-100" },
        new Judoka { FullName = "Peter Paltchik", Country = "Israël", Category = "-100" },
        new Judoka { FullName = "Jorge Fonseca", Country = "Portugal", Category = "-100" },
        new Judoka { FullName = "Niiaz Bilalov", Country = "Rusland", Category = "-100" },
        new Judoka { FullName = "Michael Korrel", Country = "Nederland", Category = "-100" },

        // +100
        new Judoka { FullName = "Teddy Riner", Country = "Frankrijk", Category = "+100" },
        new Judoka { FullName = "Guram Tushishvili", Country = "Georgië", Category = "+100" },
        new Judoka { FullName = "Tamerlan Bashaev", Country = "Rusland", Category = "+100" },
        new Judoka { FullName = "Roy Meyer", Country = "Nederland", Category = "+100" },
        new Judoka { FullName = "Rafael Silva", Country = "Brazilië", Category = "+100" }
    });

                db.SaveChanges();
            }
        }
    }
}