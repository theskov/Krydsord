using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Krydsord.Helpers
{
    public static class SortedStringListHelper
    {
        public static List<string> StartsWith(this List<string> s, string prefix)
        {
            var first = BinaryFindFirstOrLast(s, prefix, Direction.Down);
            if (first < 0) return new List<string>();
            var last = BinaryFindFirstOrLast(s, prefix, Direction.Up, first);
            
            return s.GetRange(first, last - first);
        }

        public static bool AnyStartsWith(this List<string> s, string prefix)
        {
            var index = BinarySearchAny(s, prefix);
            return index >= 0 && s.ElementAt(index).StartsWith(prefix);
        }

        private static int BinarySearchAny(List<string> list, string prefix)
        {
            int max = list.Count-1;
            int min = 0;
            int length = prefix.Length;
            
            while (max >= min)
            {
                int index = (max + min)/2;
                string currentItem = list.ElementAt(index).Substring(0, length);
                int comparison = String.Compare(prefix, currentItem);
                if (comparison > 0)
                    min = index + 1;
                else if (comparison < 0)
                    max = index - 1;
                else return index;
            }
            return -1;
        }

        private static int BinaryFindFirstOrLast(List<string> list, string prefix, Direction direction, int min = 0)
        {
            int max = list.Count - 1;
            int length = prefix.Length;
            int comparison = -1;

            while (min < max)
            {
                int index = direction == Direction.Down
                                ? (max + min)/2
                                : (max + min + 1)/2;

                string currentItem = list.ElementAt(index).Substring(0, length);
                comparison = String.Compare(prefix, currentItem);
                switch (direction)
                {
                        case Direction.Down:
                            if (comparison > 0)
                                min = index + 1;
                            else
                                max = index;
                        break;
                        case Direction.Up:
                            if (comparison < 0)
                                max = index - 1;
                            else
                                min = index;
                        break;
                }
            }
            if (max == min && String.Compare(prefix, list.ElementAt(min).Substring(0, length)) == 0) return min;
            return -1;
        }

        private enum Direction
        {
            Up, Down
        }
    }
}
