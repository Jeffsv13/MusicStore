using MusicStore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStore.Repositories.Abstractions;

public interface ICustomerRepository : IRepositoryBase<Customer>
{
    Task<Customer?> GetByEmailAsync(string email);
}
