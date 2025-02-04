using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStore.Dto.Request;

public record SaleRequestDto(int ConcertId, short TicketsQuantity, string Email, string FullName);
