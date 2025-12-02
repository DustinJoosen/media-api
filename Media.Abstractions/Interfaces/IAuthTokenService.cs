using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Media.Abstractions.Interfaces
{
    public interface IAuthTokenService
    {

        Task<Guid> CreateToken();
    }
}
