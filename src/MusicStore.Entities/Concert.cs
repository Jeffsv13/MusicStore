using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStore.Entities;

public class Concert : EntityBase
{
    public string Title { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string ExtendedDescription { get; set; } = default!;
    public string Place { get; set; } = default!;
    public double Price { get; set; }
    public double UnitPrice { get; set; }
    public int GenreId { get; set; }
    public DateTime DateEvent { get; set; }
    public string? ImageUrl { get; set; }
    public int TicketsQuantity { get; set; }
    public bool Finalized { get; set; }
    // Navigation property
    public virtual Genre Genre { get; set; } = default!;
}
