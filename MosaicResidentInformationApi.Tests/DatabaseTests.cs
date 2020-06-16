using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using MosaicResidentInformationApi.V1.Infrastructure;
using NUnit.Framework;

namespace MosaicResidentInformationApi.Tests
{
    [NonParallelizable]
    [TestFixture]
    public class DatabaseTests
    {
        private MosaicContext _context;
        private IDbContextTransaction _transaction;
        private DbContextOptionsBuilder _builder;

        protected MosaicContext MosaicContext => _context;

        [OneTimeSetUp]
        public void RunBeforeAnyTests()
        {
            _builder = new DbContextOptionsBuilder();
            _builder.UseNpgsql(ConnectionString.TestDatabase());
        }

        [SetUp]
        public void SetUp()
        {
            _context = new MosaicContext(_builder.Options);
            _context.Database.EnsureCreated();
            _transaction = MosaicContext.Database.BeginTransaction();
        }

        [TearDown]
        public void TearDown()
        {
            _transaction.Rollback();
            _transaction.Dispose();
        }
    }
}
