using System;
using GameStore.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Api.Data;

public class GameStoreContext : DbContext
{
    public GameStoreContext(DbContextOptions options) : base(options) {}

    public DbSet<Genre> Genres { get; set; }

    public DbSet<Game> Games { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);


        modelBuilder.Entity<Genre>().HasData(
            new Genre {
                Id = 1,
                Name = "Fighting"
            },
            new Genre {
                Id = 2,
                Name = "Racing"
            },
            new Genre {
                Id = 3,
                Name = "Rocket League"
            }
        );

        modelBuilder.Entity<Game>().HasData(
            new Game {
                Id = 1,
                Name = "Fighter 1",
                GenreId = 1,
                Price = 29,
                ReleaseDate = new DateOnly(2022, 2, 25)
            },
            new Game {
                Id = 2,
                Name = "NFSP 3",
                GenreId = 2,
                Price = 50,
                ReleaseDate = new DateOnly(2003, 3, 19)
            },
            new Game {
                Id = 3,
                Name = "Rocket League",
                GenreId = 3,
                Price = 0,
                ReleaseDate = new DateOnly(2017, 2, 25)
            }
        );
    }
}
