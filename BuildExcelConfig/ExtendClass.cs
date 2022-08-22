using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildExcelConfig
{
    internal static class ExtendClass
    {
        public static int? ToIntOrNull(this string str)
        {
            try
            {
                int i = int.Parse(str);
                return i;
            }
            catch
            {
                return null;
            }
        }
    }
}
