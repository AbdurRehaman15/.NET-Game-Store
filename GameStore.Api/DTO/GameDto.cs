using System;

namespace GameStore.Api.DTO;

public class GameDto
{

    public required string Name { get; set; }

    public int GenreId { get; set; }
    
    public decimal Price { get; set; }

    public DateOnly ReleaseDate { get; set; }
}
