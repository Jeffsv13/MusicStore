using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStore.Dto.Request;

public class ResetPasswordRequestDto
{
    public string Email { get; set; } = default!;
}
