namespace Library.Domain.Entities
{
    public class BookCollect : Entity
    {
        public bool Completed { get; private set; }
        public DateTime ExpirationDate { get; private set; }
        public bool IsOverdue { get; private set; }
        public Book Book { get; private set; }
        public BookCollect(Book book,int expirationDays)
        {
            ExpirationDate = DateTime.Now.AddDays(expirationDays);
            Book = book;
            Verify();
        }

        public void Complete()
        {
            Completed = true;
        }
        public void Verify()
        {
            if (Completed)
            {
                IsOverdue = false;
                return;
            }
            IsOverdue = DateTime.Now > ExpirationDate;
        }
    }
}