using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KTM_MART.Models
{
    public sealed class ArrayComparer : IEqualityComparer<string[]>
    {
        public bool Equals(string[] x, string[] y)
        {
            if (x[0] == y[0])
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public int GetHashCode(string[] obj)
        {
            return obj[0].GetHashCode() + obj[1].GetHashCode();
        }
    }
}
