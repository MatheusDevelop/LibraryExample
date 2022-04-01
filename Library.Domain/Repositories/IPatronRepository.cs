using Library.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Domain.Repositories
{
    public interface IPatronRepository
    {
        Task<Patron> GetById(int id);
        Task<Book> GetBookById(int id);
        Task Save();

    }
}
