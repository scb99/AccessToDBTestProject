using DataAbstractions.Dapper;
using Moq;
using System.Data;

namespace DataLibrary.Tests;

public class DataReadAndSaveCommandsTests
{
    private readonly Mock<IDataAccessor> _mockDataAccessor;
    private readonly DataReadAndSaveCommands _component;

    public DataReadAndSaveCommandsTests()
    {
        _mockDataAccessor = new Mock<IDataAccessor>();
        _component = new DataReadAndSaveCommands(_mockDataAccessor.Object);
    }

    [Fact]
    public async Task ReadDataFromDBAsync_Should_Return_Expected_Result()
    {
        // Arrange
        var sql = "SELECT * FROM Table";
        var parameters = new { };
        var expected = new List<object> { new() };
        _mockDataAccessor.Setup(db => db.QueryAsync<object>(sql, parameters, null, null, CommandType.Text))
                         .ReturnsAsync(expected);

        // Act
        var result = await _component.ReadDataFromDBAsync<object, object>(sql, parameters);

        // Assert
        Assert.Equal(expected, result);
        _mockDataAccessor.Verify(db => db.Open(), Times.Once);
        _mockDataAccessor.Verify(db => db.Close(), Times.Once);
    }

    [Fact]
    public async Task ReadDataFromDBAsync_Should_Handle_Exceptions()
    {
        // Arrange
        var sql = "SELECT * FROM Table";
        var parameters = new { };
        _mockDataAccessor.Setup(db => db.QueryAsync<object>(sql, parameters, null, null, CommandType.Text))
                         .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _component.ReadDataFromDBAsync<object, object>(sql, parameters));
        _mockDataAccessor.Verify(db => db.Open(), Times.Once);
        _mockDataAccessor.Verify(db => db.Close(), Times.Once);
    }

    [Fact]
    public async Task SaveDataToDBAsync_Should_Return_Expected_Result()
    {
        // Arrange
        var sql = "INSERT INTO Table VALUES (@Value)";
        var parameters = new { Value = "Test" };
        var expected = 1;
        _mockDataAccessor.Setup(db => db.ExecuteAsync(sql, parameters, null, null, CommandType.Text))
                         .ReturnsAsync(expected);

        // Act
        var result = await _component.SaveDataToDBAsync(sql, parameters);

        // Assert
        Assert.Equal(expected, result);
        _mockDataAccessor.Verify(db => db.Open(), Times.Once);
        _mockDataAccessor.Verify(db => db.Close(), Times.Once);
    }

    [Fact]
    public async Task SaveDataToDBAsync_Should_Handle_Exceptions()
    {
        // Arrange
        var sql = "INSERT INTO Table VALUES (@Value)";
        var parameters = new { Value = "Test" };
        _mockDataAccessor.Setup(db => db.ExecuteAsync(sql, parameters, null, null, CommandType.Text))
                         .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _component.SaveDataToDBAsync(sql, parameters));
        _mockDataAccessor.Verify(db => db.Open(), Times.Once);
        _mockDataAccessor.Verify(db => db.Close(), Times.Once);
    }
}