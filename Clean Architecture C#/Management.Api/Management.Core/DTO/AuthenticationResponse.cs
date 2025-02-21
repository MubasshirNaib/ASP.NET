using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Management.Core.DTO
{
    public class AuthenticationResponse
    {
        public string JwToken { get; set; }
        public string Role { get; set; }
    }
}
