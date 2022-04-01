using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Domain.Entities
{
    public class Book:Entity
    {
        public Book(bool isRestricted, bool isAvailable, string title, string author)
        {
            IsRestricted = isRestricted;
            IsAvailable = isAvailable;
            Title = title;
            Author = author;
        }

        public bool IsRestricted { get; private set; }
        public bool IsAvailable { get; private set; }
        public string Title { get; private set; }
        public string Author { get; private set; }
        public Patron? HoldByPatron { get; private set; }

        public void HoldBy(Patron patron)
        {
            if (HoldByPatron != null) 
                throw new InvalidOperationException("Is not possible hold this book because this is already in use");
            HoldByPatron = patron;
        }

        public void UnholdBy(Patron patron)
        {
            if ((HoldByPatron != null) && HoldByPatron.Id != patron.Id)
                throw new InvalidOperationException("Is not possible unhold this book because this patron is not holding this book");
            HoldByPatron = null;
        }

    }
}
