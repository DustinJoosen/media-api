using Media.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Media.Test.Helpers
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

            _context = new MediaDbContext(options);
        }
    }
}
