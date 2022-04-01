using Library.Domain.Entities;
using Library.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Domain.Services
{
    public class PatronServices
    {
        private readonly IPatronRepository repository;
        public PatronServices(IPatronRepository _repository)
        {
            repository = _repository;
        }
        private async Task<Tuple <Book,Patron>> GetEntities(int patronId,int bookId)
        {
            Patron patron = await repository.GetById(patronId);
            Book book = await repository.GetBookById(bookId);

            if (patron is null)
                throw new Exception("Patron not found");
            if (book is null)
                throw new Exception("Book not found");
            return Tuple.Create<Book, Patron>(book, patron);
        }
        public async Task HoldBook(int patronId,int bookId)
        {
            var entites = await GetEntities(patronId, bookId);
            var book = entites.Item1;
            var patron = entites.Item2;
            patron.Hold(book);
            book.HoldBy(patron);
            await repository.Save();
        }
        public async Task CancelHold(int patronId,int bookId)
        {
            var entites = await GetEntities(patronId, bookId);
            var book = entites.Item1;
            var patron = entites.Item2;
            patron.CancelHold(book);
            book.UnholdBy(patron);
            await repository.Save();
        }
        public async Task CheckoutBook(int patronId,int bookId)
        {
            var entites = await GetEntities(patronId, bookId);
            var book = entites.Item1;
            var patron = entites.Item2;
            patron.Checkout(book,60);
            await repository.Save();
        }

        public async Task ReturnBook(int patronId, int bookId)
        {
            var entites = await GetEntities(patronId, bookId);
            var book = entites.Item1;
            var patron = entites.Item2;
            patron.ReturnBook(book);
            book.UnholdBy(patron);
            await repository.Save();
        }

    }
}
