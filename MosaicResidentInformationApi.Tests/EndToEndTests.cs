using System.Net.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using MosaicResidentInformationApi.V1.Infrastructure;
using Npgsql;
using NUnit.Framework;

namespace MosaicResidentInformationApi.Tests
{
    [NonParallelizable]
    [TestFixture]
    public class EndToEndTests<TStartup> where TStartup : class
    {
        private HttpClient _client;
        private MosaicContext _mosaicContext;

        private MockWebApplicationFactory<TStartup> _factory;
        private NpgsqlConnection _connection;
        private IDbContextTransaction _transaction;
        private DbContextOptionsBuilder _builder;

        protected HttpClient Client => _client;
        protected MosaicContext MosaicContext => _mosaicContext;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _connection = new NpgsqlConnection(ConnectionString.TestDatabase());
            _connection.Open();
            var npgsqlCommand = _connection.CreateCommand();
            npgsqlCommand.CommandText = "SET deadlock_timeout TO 30";
            npgsqlCommand.ExecuteNonQuery();

            _builder = new DbContextOptionsBuilder();
            _builder.UseNpgsql(_connection);
        }

        [SetUp]
        public void BaseSetup()
        {
            _factory = new MockWebApplicationFactory<TStartup>(_connection);
            _client = _factory.CreateClient();
            _mosaicContext = new MosaicContext(_builder.Options);
            _mosaicContext.Database.EnsureCreated();
            _transaction = MosaicContext.Database.BeginTransaction();
        }

        [TearDown]
        public void BaseTearDown()
        {
            _client.Dispose();
            _factory.Dispose();
            _transaction.Rollback();
            _transaction.Dispose();
        }
    }
}
