using Dapper;
using Moq;
using System.Data;

namespace DataAbstractions.Dapper.Tests
{
    public class ConnectionToDBViaDapperTests
    {
        private readonly Mock<IDbConnection> _mockConnection;
        private readonly ConnectionToDBViaDapper _dbAccessor;

        public ConnectionToDBViaDapperTests()
        {
            _mockConnection = new Mock<IDbConnection>();
            //var sql = "SELECT * FROM TestTable";
            //_mockConnection.Setup(c => c.QueryAsync<object>(sql, null, null, null, null)).ReturnsAsync([]);
            _dbAccessor = new ConnectionToDBViaDapper(_mockConnection.Object);
        }

        [Fact]
        public void Constructor_NullConnection_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new ConnectionToDBViaDapper(null));
        }

        [Fact]
        public void GetUnderlyingConnection_ReturnsConnection()
        {
            var result = _dbAccessor.GetUnderlyingConnection();
            Assert.Equal(_mockConnection.Object, result);
        }

        [Fact]
        public void ConnectionString_GetSet_WorksCorrectly()
        {
            _mockConnection.SetupProperty(c => c.ConnectionString, "InitialConnectionString");
            _dbAccessor.ConnectionString = "NewConnectionString";
            Assert.Equal("NewConnectionString", _dbAccessor.ConnectionString);
        }

        [Fact]
        public void ConnectionTimeout_ReturnsCorrectValue()
        {
            _mockConnection.Setup(c => c.ConnectionTimeout).Returns(30);
            Assert.Equal(30, _dbAccessor.ConnectionTimeout);
        }

        [Fact]
        public void Database_ReturnsCorrectValue()
        {
            _mockConnection.Setup(c => c.Database).Returns("TestDatabase");
            Assert.Equal("TestDatabase", _dbAccessor.Database);
        }

        [Fact]
        public void State_ReturnsCorrectValue()
        {
            _mockConnection.Setup(c => c.State).Returns(ConnectionState.Open);
            Assert.Equal(ConnectionState.Open, _dbAccessor.State);
        }

        [Fact]
        public void BeginTransaction_NoParams_CallsBeginTransaction()
        {
            _dbAccessor.BeginTransaction();
            _mockConnection.Verify(c => c.BeginTransaction(), Times.Once);
        }

        [Fact]
        public void BeginTransaction_WithIsolationLevel_CallsBeginTransaction()
        {
            _dbAccessor.BeginTransaction(IsolationLevel.ReadCommitted);
            _mockConnection.Verify(c => c.BeginTransaction(IsolationLevel.ReadCommitted), Times.Once);
        }

        [Fact]
        public void Close_CallsClose()
        {
            _dbAccessor.Close();
            _mockConnection.Verify(c => c.Close(), Times.Once);
        }

        [Fact]
        public void ChangeDatabase_CallsChangeDatabase()
        {
            _dbAccessor.ChangeDatabase("NewDatabase");
            _mockConnection.Verify(c => c.ChangeDatabase("NewDatabase"), Times.Once);
        }

        [Fact]
        public void CreateCommand_CallsCreateCommand()
        {
            _dbAccessor.CreateCommand();
            _mockConnection.Verify(c => c.CreateCommand(), Times.Once);
        }

        [Fact]
        public void Open_ConnectionClosed_CallsOpen()
        {
            _mockConnection.Setup(c => c.State).Returns(ConnectionState.Closed);
            _dbAccessor.Open();
            _mockConnection.Verify(c => c.Open(), Times.Once);
        }

        [Fact]
        public void Open_ConnectionOpen_DoesNotCallOpen()
        {
            _mockConnection.Setup(c => c.State).Returns(ConnectionState.Open);
            _dbAccessor.Open();
            _mockConnection.Verify(c => c.Open(), Times.Never);
        }

        //[Fact]
        //public async Task QueryAsync_CallsQueryAsync()
        //{
        //    var sql = "SELECT * FROM TestTable";
        //    await _dbAccessor.QueryAsync<object>(sql);
        //    _mockConnection.Verify(c => c.QueryAsync<object>(sql, null, null, null, null), Times.Once);
        //}

        //[Fact]
        //public async Task ExecuteAsync_CallsExecuteAsync()
        //{
        //    var sql = "DELETE FROM TestTable WHERE ID = @ID";
        //    await _dbAccessor.ExecuteAsync(sql);
        //    _mockConnection.Verify(c => c.ExecuteAsync(sql, null, null, null, null), Times.Once);
        //}

        [Fact]
        public void Dispose_ConnectionOpen_CallsCloseAndDispose()
        {
            _mockConnection.Setup(c => c.State).Returns(ConnectionState.Open);
            _dbAccessor.Dispose();
            _mockConnection.Verify(c => c.Close(), Times.Once);
            _mockConnection.Verify(c => c.Dispose(), Times.Once);
        }

        [Fact]
        public void Dispose_ConnectionClosed_CallsDisposeOnly()
        {
            _mockConnection.Setup(c => c.State).Returns(ConnectionState.Closed);
            _dbAccessor.Dispose();
            _mockConnection.Verify(c => c.Close(), Times.Never);
            _mockConnection.Verify(c => c.Dispose(), Times.Once);
        }
    }
}