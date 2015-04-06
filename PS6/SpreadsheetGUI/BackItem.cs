using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SS
{
    class BackItem
    {
        public string name;
        public string value;

        public BackItem(string cellName, string oldVal)
        {
            name = cellName;
            value = oldVal;
        }
    }
}
