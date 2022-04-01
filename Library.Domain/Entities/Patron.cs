using Library.Domain.Enum;

namespace Library.Domain.Entities
{
    public class Patron:Entity
    {
        private List<BookHold> Holds;
        private List<BookCollect> Collection;

        public Patron(EPatronType type)
        {
            Holds = new List<BookHold>();
            Collection = new List<BookCollect>();
            Type = type;
        }

        public EPatronType Type{ get; private set; }
        public IReadOnlyCollection<BookHold> HoldsReadOnly() => Holds.AsReadOnly();

        public IReadOnlyCollection<BookCollect> CollectionReadOnly() => Collection.AsReadOnly();


        public void Hold(Book book) 
        {
            var overdueCheckouts = Collection.Where(e => e.IsOverdue);
            if(overdueCheckouts.Count() >= 2)
                throw new InvalidOperationException("This patron dont have permission to hold because has two overdue checkouts");
            if (Type == EPatronType.Regular && book.IsRestricted)
                throw new InvalidOperationException("This book is restricted to regular patrons");
            var activeHolds = Holds.Where(e => e.Active);
            if (Type == EPatronType.Regular && activeHolds.Count() >= 5)
                throw new InvalidOperationException("This patron already reached max holds (5 holds)");
            DateTime? expirationDate = Type == EPatronType.Regular ? DateTime.Now.AddDays(15): null;
            BookHold newHold = new(expirationDate,book);
            Holds.Add(newHold);
        }
        private BookHold? GetHold(Book book) => Holds.FirstOrDefault(e => e.Book.Id == book.Id);
        public void CancelHold(Book book) 
        {
            BookHold? hold = GetHold(book);
            if (hold is null)
                throw new InvalidOperationException("Is not possible cancel hold because this patron not have hold with this book");
            hold.Cancel();
        }
        public void Checkout(Book book,int expirationDays) 
        {
            BookHold? hold = GetHold(book);
            if (hold is null)
                throw new InvalidOperationException("Is not possible checkout because this patron not have hold with this book");
            var collect = new BookCollect(book,expirationDays);
            Collection.Add(collect);
            hold.Desactive();
            hold.Complete();
        }
        public void ReturnBook(Book book) 
        {
            BookCollect? collect = Collection.FirstOrDefault(e => e.Book.Id == book.Id);
            if (collect is null)
                throw new InvalidOperationException("Not found this book collected by this person");
            collect.Complete();
            collect.Verify();
        }

    }
}