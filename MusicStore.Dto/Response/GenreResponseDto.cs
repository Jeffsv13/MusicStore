using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStore.Dto.Response;

public class GenreResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public bool Status { get; set; } = true;
}
