using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Krydsord.Interfaces;

namespace Krydsord.Helpers
{
    public static class SortedComparableListHelper<T> where T : INthComparable, IStartsWith
    {
        public static List<T> StartsWith(List<T> s, T prefix, int bogstavIndex = 0)
        {
            var first = BinaryFindFirstOrLast(s, prefix, Direction.Down, bogstavIndex);
            if (first < 0) return new List<T>();
            var last = BinaryFindFirstOrLast(s, prefix, Direction.Up, bogstavIndex, first);

            return s.GetRange(first, last - first);
        }

        private static int BinaryFindFirstOrLast(List<T> list, T prefix, Direction direction, int bogstavIndex, int min = 0)
        {
            int max = list.Count - 1;
            int comparison = -1;

            while (min < max)
            {
                int index = direction == Direction.Down
                                ? (max + min) / 2
                                : (max + min + 1) / 2;
                T currentItem = list.ElementAt(index);
                comparison = currentItem.CompareToPrefix(prefix, bogstavIndex);
                switch (direction)
                {
                    case Direction.Up:
                        if (comparison < 0)
                            min = index + 1;
                        else
                            max = index;
                        break;
                    case Direction.Down:
                        if (comparison > 0)
                            max = index - 1;
                        else
                            min = index;
                        break;
                }
            }
            if (max == min && comparison == 0) return min;
            return -1;
        }

        private enum Direction
        {
            Up, Down
        }
    }
}
