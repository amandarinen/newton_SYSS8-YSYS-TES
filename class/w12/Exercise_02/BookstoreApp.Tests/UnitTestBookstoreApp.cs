namespace BookstoreApp.Tests;

[TestClass]
public class UnitTestBookstoreApp
{
    [TestMethod]
    public void TestAddNewBook()
    {
        //Arrange
        var isbn = "1234567890";
        var expectedStock = 10;
        var dotnetBook = new BookstoreApp.Book(isbn, "Test Book", "Test Author", expectedStock);
        var library = new BookstoreApp.BookstoreInventory();

        //Act
        library.AddBook(dotnetBook);

        //Assert
        var stock = library.CheckStock(isbn);
        Assert.AreEqual(expectedStock, stock);
    }
}