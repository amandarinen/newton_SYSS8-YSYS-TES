using Moq;
using Npgsql;
using System.Data;


namespace ProductManager.Tests;

[TestClass]
public class UnitTestProductRepo
{
    public void CleanUpProductsTable()
    {
        using var connection = new NpgsqlConnection("Host=localhost;Port=5432;Username=postgres;Password=mysecretpassword;Database=postgres");
        connection.Open();
        using var cmd = connection.CreateCommand();
        cmd.CommandText = "DELETE FROM products";
        cmd.ExecuteNonQuery();
        connection.Close();
    }

    [TestInitialize]
    public void Initialize()
    {
        CleanUpProductsTable();
    }

    [TestMethod]
    public void TestGetProductsByCategoryRealDb()
    {
        // Arrange
        var productRepository = new ProductRepository();

        var product = new Product
        {
            Name = "iPhone 17 Pro",
            Category = "Tech",
            Price = "13000"
        };
        var expectedCount = 1;
        productRepository.InsertProduct(product);

        // Act
        var result = productRepository.GetProductsByCategory("Tech");

        // Assert
        Assert.AreEqual(expectedCount, result.Count);
    }

    [TestMethod]
    public void TestGetProductsByCategoryWithMock()
    {
        // Arrange
        var expectedProducts = new List<Product>
        {
            new Product { Id = 1, Name = "iPhone 17 Pro", Category = "Tech", Price = "13000" }
        };
        var mockConnection = new Mock<IDbConnection>();
        var mockCommand = new Mock<IDbCommand>();
        var mockReader = new Mock<IDataReader>();

        var readCallCount = 0;

        mockReader.Setup(r => r.Read()).Returns(() => readCallCount++ == 0);

        //ID
        mockReader.Setup(r => r.GetInt32(0)).Returns(1);
        // Name
        mockReader.Setup(r => r.GetString(1)).Returns("iPhone 17 Pro");
        // Category
        mockReader.Setup(r => r.GetString(2)).Returns("Tech");
        // Price
        mockReader.Setup(r => r.GetString(3)).Returns("13000");
       
        mockCommand.Setup(c => c.ExecuteReader()).Returns(mockReader.Object);
     
        mockConnection.Setup(c => c.CreateCommand()).Returns(mockCommand.Object);

        var productRepository = new ProductRepository(mockConnection.Object);

        // Act
        var result = productRepository.GetProductsByCategory("Tech");

        // Assert
        Assert.AreEqual(expectedProducts.Count, result.Count);
        Assert.AreEqual(expectedProducts[0].Id, result[0].Id);
        Assert.AreEqual(expectedProducts[0].Name, result[0].Name);
        Assert.AreEqual(expectedProducts[0].Category, result[0].Category);
        Assert.AreEqual(expectedProducts[0].Price, result[0].Price);
    }
}