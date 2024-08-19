using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventChatApplication.Models
{
    public class Vector
    {
        public required string Id { get; set; }
        public Dictionary<string, object>? Metadata { get; set; }
    }
}
