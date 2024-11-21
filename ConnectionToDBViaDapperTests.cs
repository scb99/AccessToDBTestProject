using Moq;
using System.Data;

namespace DataAbstractions.Dapper.Tests;

public class ConnectionToDBViaDapperTests
{
    private readonly Mock<IDbConnection> _mockConnection;
    private readonly ConnectionToDBViaDapper _component;

    public ConnectionToDBViaDapperTests()
    {
        _mockConnection = new Mock<IDbConnection>();
        //var sql = "SELECT * FROM TestTable";
        //_mockConnection.Setup(c => c.QueryAsync<object>(sql, null, null, null, null)).ReturnsAsync([]);
        _component = new ConnectionToDBViaDapper(_mockConnection.Object);
    }

    [Fact]
    public void Constructor_NullConnection_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new ConnectionToDBViaDapper(null));
    }

    [Fact]
    public void GetUnderlyingConnection_ReturnsConnection()
    {
        var result = _component.GetUnderlyingConnection();
        Assert.Equal(_mockConnection.Object, result);
    }

    [Fact]
    public void ConnectionString_GetSet_WorksCorrectly()
    {
        _mockConnection.SetupProperty(c => c.ConnectionString, "InitialConnectionString");
        _component.ConnectionString = "NewConnectionString";
        Assert.Equal("NewConnectionString", _component.ConnectionString);
    }

    [Fact]
    public void ConnectionTimeout_ReturnsCorrectValue()
    {
        _mockConnection.Setup(c => c.ConnectionTimeout).Returns(30);
        Assert.Equal(30, _component.ConnectionTimeout);
    }

    [Fact]
    public void Database_ReturnsCorrectValue()
    {
        _mockConnection.Setup(c => c.Database).Returns("TestDatabase");
        Assert.Equal("TestDatabase", _component.Database);
    }

    [Fact]
    public void State_ReturnsCorrectValue()
    {
        _mockConnection.Setup(c => c.State).Returns(ConnectionState.Open);
        Assert.Equal(ConnectionState.Open, _component.State);
    }

    [Fact]
    public void BeginTransaction_NoParams_CallsBeginTransaction()
    {
        _component.BeginTransaction();
        _mockConnection.Verify(c => c.BeginTransaction(), Times.Once);
    }

    [Fact]
    public void BeginTransaction_WithIsolationLevel_CallsBeginTransaction()
    {
        _component.BeginTransaction(IsolationLevel.ReadCommitted);
        _mockConnection.Verify(c => c.BeginTransaction(IsolationLevel.ReadCommitted), Times.Once);
    }

    [Fact]
    public void Close_CallsClose()
    {
        _component.Close();
        _mockConnection.Verify(c => c.Close(), Times.Once);
    }

    [Fact]
    public void ChangeDatabase_CallsChangeDatabase()
    {
        _component.ChangeDatabase("NewDatabase");
        _mockConnection.Verify(c => c.ChangeDatabase("NewDatabase"), Times.Once);
    }

    [Fact]
    public void CreateCommand_CallsCreateCommand()
    {
        _component.CreateCommand();
        _mockConnection.Verify(c => c.CreateCommand(), Times.Once);
    }

    [Fact]
    public void Open_ConnectionClosed_CallsOpen()
    {
        _mockConnection.Setup(c => c.State).Returns(ConnectionState.Closed);
        _component.Open();
        _mockConnection.Verify(c => c.Open(), Times.Once);
    }

    [Fact]
    public void Open_ConnectionOpen_DoesNotCallOpen()
    {
        _mockConnection.Setup(c => c.State).Returns(ConnectionState.Open);
        _component.Open();
        _mockConnection.Verify(c => c.Open(), Times.Never);
    }

    //[Fact]
    //public async Task QueryAsync_CallsQueryAsync()
    //{
    //    var sql = "SELECT * FROM TestTable";
    //    await _component.QueryAsync<object>(sql);
    //    _mockConnection.Verify(c => c.QueryAsync<object>(sql, null, null, null, null), Times.Once);
    //}

    //[Fact]
    //public async Task ExecuteAsync_CallsExecuteAsync()
    //{
    //    var sql = "DELETE FROM TestTable WHERE ID = @ID";
    //    await _component.ExecuteAsync(sql);
    //    _mockConnection.Verify(c => c.ExecuteAsync(sql, null, null, null, null), Times.Once);
    //}

    [Fact]
    public void Dispose_ConnectionOpen_CallsCloseAndDispose()
    {
        _mockConnection.Setup(c => c.State).Returns(ConnectionState.Open);
        _component.Dispose();
        _mockConnection.Verify(c => c.Close(), Times.Once);
        _mockConnection.Verify(c => c.Dispose(), Times.Once);
    }

    [Fact]
    public void Dispose_ConnectionClosed_CallsDisposeOnly()
    {
        _mockConnection.Setup(c => c.State).Returns(ConnectionState.Closed);
        _component.Dispose();
        _mockConnection.Verify(c => c.Close(), Times.Never);
        _mockConnection.Verify(c => c.Dispose(), Times.Once);
    }
}