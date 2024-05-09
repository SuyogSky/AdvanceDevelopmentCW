using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Bislerium
{
    public class MobileTokens
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Token { get; set; }
        public string UserId { get; set; }
    }

}