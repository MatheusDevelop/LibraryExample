using Library.Domain.Entities;
using Library.Domain.Enum;
using System;
using System.Linq;
using Xunit;

namespace Library.Tests.Domain.Entities
{
    public class PatronTests
    {
        [Fact]
        public void ShouldReturnExceptionWhenPatronIsRegularAndBookIsRestricted()
        {
            //Arrange
            var patron = new Patron(EPatronType.Regular);
            var book = new Book(true, true, "Androides sonham com ovelhas elétricas?", "Philip K. Dick");
            //Act 
            var exception = Assert.Throws<InvalidOperationException>(() => patron.Hold(book));
            //Assert
            Assert.Equal(exception.Message, "This book is restricted to regular patrons");
        }
        [Fact]
        public void ShouldReturnExceptionWhenPatronIsRegularAndYourHoldsIsReachedMaxFiveBooks()
        {
            //Arrange
            var patron = new Patron(EPatronType.Regular);
            var book = new Book(false, true, "Androides sonham com ovelhas elétricas?", "Philip K. Dick");
            patron.Hold(book);
            patron.Hold(book);
            patron.Hold(book);
            patron.Hold(book);
            patron.Hold(book);

            //Act
            var exception = Assert.Throws<InvalidOperationException>(() => patron.Hold(book));
            //Assert
            Assert.Equal(exception.Message, "This patron already reached max holds (5 holds)");
        }
        [Fact]
        public void ShouldHaveExpirationDateInHoldBookWhenPatronIsRegular()
        {
            //Arrange
            var patron = new Patron(EPatronType.Regular);
            var book = new Book(false, true, "Androides sonham com ovelhas elétricas?", "Philip K. Dick");
            patron.Hold(book);
            //Act
            var holds = patron.HoldsReadOnly();
            var element = holds.ElementAt(0);
            //Assert
            Assert.NotNull(element.ExpirationDate);
        }

        [Fact]
        public void ShouldHaveTenHoldsWhenPatronIsResearcher()
        {
            //Arrange
            var patron = new Patron(EPatronType.Researcher);
            var book = new Book(false, true, "Androides sonham com ovelhas elétricas?", "Philip K. Dick");

            var numberOfHolds = 10;
            for (int i = 0; i < numberOfHolds; i++)
            {
                patron.Hold(book);
            }

            var holds = patron.HoldsReadOnly();
            //Act
            Assert.Equal(holds.Count, numberOfHolds);
        }
        [Fact]
        public void ShouldHoldRestrictedBookWhenPatronIsResearcher()
        {
            //Arrange
            var patron = new Patron(EPatronType.Researcher);
            var book = new Book(true, true, "Androides sonham com ovelhas elétricas?", "Philip K. Dick");
            //Act
            patron.Hold(book);
            var holds = patron.HoldsReadOnly();
            var element = holds.ElementAt(0);
            //Assert
            Assert.True(element.Book.IsRestricted);
        }

        [Fact]
        public void ShouldHaveExpirationDateNullInHoldBookWhenPatronIsResearcher()
        {
            //Arrange
            var patron = new Patron(EPatronType.Researcher);
            var book = new Book(true, true, "Androides sonham com ovelhas elétricas?", "Philip K. Dick");
            //Act
            patron.Hold(book);
            var holds = patron.HoldsReadOnly();
            var element = holds.ElementAt(0);
            //Assert
            Assert.Null(element.ExpirationDate);
        }

        [Fact]
        public void ShouldReturnExceptionWhenResearcherAreTwoOrMoreOverduesInBookCollections()
        {
            //Arrange
            var patron = new Patron(EPatronType.Researcher);
            var book = new Book(true, true, "Androides sonham com ovelhas elétricas?", "Philip K. Dick");
            //Act
            patron.Hold(book);
            patron.Hold(book);

            patron.Checkout(book, -2);
            patron.Checkout(book, -2);

            //Assert
            var exception = Assert.Throws<InvalidOperationException>(() => patron.Hold(book));
            Assert.Equal(exception.Message, "This patron dont have permission to hold because has two overdue checkouts");
        }

        [Fact]
        public void ShouldReturnExceptionWhenTriesToCancelHoldBookButDontHaveHoldThisBook()
        {
            //Arrange
            var patron = new Patron(EPatronType.Researcher);
            var book = new Book(true, true, "Androides sonham com ovelhas elétricas?", "Philip K. Dick");
            //Act
            var exception = Assert.Throws<InvalidOperationException>(() => patron.CancelHold(book));
            //Assert
            Assert.Equal(exception.Message, "Is not possible cancel hold because this patron not have hold with this book");
        }

        [Fact]
        public void ShouldHaveCancelTagFalseOnBookHoldWhenCancelHoldIsSucessfullyExecuted()
        {
            //Arrange
            var patron = new Patron(EPatronType.Researcher);
            var book = new Book(true, true, "Androides sonham com ovelhas elétricas?", "Philip K. Dick");
            //Act
            patron.Hold(book);
            patron.CancelHold(book);
            var holds = patron.HoldsReadOnly();
            var element = holds.ElementAt(0);

            //Assert
            Assert.True(element.Canceled);
        }

        [Fact]
        public void ShouldReturnExceptionWhenTriesToCheckoutBookButDontHaveHoldThisBook()
        {
            //Arrange
            var patron = new Patron(EPatronType.Researcher);
            var book = new Book(true, true, "Androides sonham com ovelhas elétricas?", "Philip K. Dick");
            //Act
            var exception = Assert.Throws<InvalidOperationException>(() => patron.Checkout(book, 60));
            //Assert
            Assert.Equal(exception.Message, "Is not possible checkout because this patron not have hold with this book");

        }
        [Fact]
        public void ShouldHaveHoldCompletedAndInactiveWhenCheckoutOfHoldIsSuccessFullyExecuted()
        {
            //Arrange
            var patron = new Patron(EPatronType.Researcher);
            var book = new Book(true, true, "Androides sonham com ovelhas elétricas?", "Philip K. Dick");
            //Act
            patron.Hold(book);
            patron.Checkout(book, 60);
            var holds = patron.HoldsReadOnly();
            var element = holds.ElementAt(0);

            //Assert
            Assert.True(element.Completed);
            Assert.False(element.Active);

        }

        [Fact]
        public void ShouldReturnExceptionWhenTriesToReturnBookButDontHaveCollectThisBook()
        {
            var patron = new Patron(EPatronType.Researcher);
            var book = new Book(true, true, "Androides sonham com ovelhas elétricas?", "Philip K. Dick");
            //Act
            var exception = Assert.Throws<InvalidOperationException>(() => patron.ReturnBook(book));
            //Assert
            Assert.Equal(exception.Message, "Not found this book collected by this person");
        }

        [Fact]
        public void ShouldHaveCollectBookWithTagsCompletedAndNotOverdue()
        {
            //Arrange
            var patron = new Patron(EPatronType.Researcher);
            var book = new Book(true, true, "Androides sonham com ovelhas elétricas?", "Philip K. Dick");
            //Act
            patron.Hold(book);
            patron.Checkout(book, -2);
            patron.ReturnBook(book);
            var collects = patron.CollectionReadOnly();
            var element = collects.ElementAt(0);

            //Assert
            Assert.True(element.Completed);
            Assert.False(element.IsOverdue);

        }

    }
}
