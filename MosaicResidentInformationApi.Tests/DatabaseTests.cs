using System;
using System.Data.Common;
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
        protected MosaicContext MosaicContext;
        private IDbContextTransaction _transaction;

        [OneTimeSetUp]
        public void RunBeforeAnyTests()
        {
            var builder = new DbContextOptionsBuilder();
            builder.UseNpgsql(ConnectionString.TestDatabase());

            MosaicContext = new MosaicContext(builder.Options);
            MosaicContext.Database.EnsureCreated();
        }

        [SetUp]
        public void SetUp()
        {
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
