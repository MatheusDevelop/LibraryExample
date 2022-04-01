namespace Library.Domain.Entities
{
    public class BookHold:Entity
    {
        public bool Completed { get; private set; }
        public bool Canceled { get; private set; }
        public bool Active { get; private set; }
        public DateTime? ExpirationDate { get; private set; }
        public Book Book { get; private set; }

        public BookHold(DateTime? expirationDate, Book book)
        {
            ExpirationDate = expirationDate;
            Active = true;
            Book = book;
        }

        public void Cancel() 
        {
            if (Completed)
                throw new InvalidOperationException("This hold cannot be cancel because is already completed");
            Canceled = true;
        }
        public void Desactive()
        {
            Active = false;
        }
        public void Complete() 
        {
            Completed = true;    
        }

    }
}