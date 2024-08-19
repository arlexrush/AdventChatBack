using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventChatApplication.Models
{
    public class Match
    {
        public required string Id { get; set; }
        public float Score { get; set; }
        public float[]? Values { get; set; }
        public Dictionary<string, object>? Metadata { get; set; }
    }
}
