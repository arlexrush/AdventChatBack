using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Application.Exceptions
{
    public class NoFoundException:ApplicationException
    {

        public NoFoundException(string name, object key):base($"Entity \"{name}\" ({key}) No founded") { }
    }
}
