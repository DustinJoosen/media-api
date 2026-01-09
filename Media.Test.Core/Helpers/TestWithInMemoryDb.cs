using Media.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Media.Test.Core.Helpers
{
    [TestClass]
    public class TestWithInMemoryDb
    {
        protected MediaDbContext _context;

        [TestInitialize]
        protected virtual void BaseSetup()
        {
            var options = new DbContextOptionsBuilder<MediaDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;

            this._context = new MediaDbContext(options);
        }
    }
}
