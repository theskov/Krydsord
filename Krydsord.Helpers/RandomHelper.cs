using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Krydsord.Helpers
{
    public class RandomHelper
    {
        private Random random;
        public RandomHelper(int seed)
        {
            random = new Random(seed);
        }
        public int randomInInterval(int max, bool includeMaxInResultRange)
        {
            if (includeMaxInResultRange) return (int)(random.NextDouble() * max + 1); // Hopefully selects random number in [1,max]
            return (int)(random.NextDouble() * max); // Hopefully selects random number in [0,max-1]
        }
    }
}
