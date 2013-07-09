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
            var first = BinaryFindFirstOrLastFromAny(s, prefix, s.Count / 2, Direction.Down, bogstavIndex);
            if (!s[first].StartsWith(prefix, bogstavIndex)) return new List<T>();
            var last = BinaryFindFirstOrLastFromAny(s, prefix, first, Direction.Up, bogstavIndex);

            return s.GetRange(first, last - first);
        }

        private static int BinaryFindFirstOrLastFromAny(List<T> list, T prefix, int anyMatchingIndex, Direction direction, int bogstavIndex = 0)
        {
            int length = direction == Direction.Down ? anyMatchingIndex : list.Count - anyMatchingIndex; // length from index to end of list in specified direction
            int divisor = 2;
            int index = direction == Direction.Down ? length / divisor : anyMatchingIndex + (length / divisor); // Set index halfway from matching index and end of list in specified direction
            T currentItem = list.ElementAt(index);
            while (true)
            {
                var oldIndex = index;
                var comparison = currentItem.CompareTo(prefix, bogstavIndex);

                // Too far up
                if (!currentItem.StartsWith(prefix) && comparison > 0) direction = Direction.Down;
                // Too far down
                else if (!currentItem.StartsWith(prefix) && comparison < 0) direction = Direction.Up;
                // If none of the above matched, we are within the range of matches, should move avay from anyMatchingIndex
                else direction = index > anyMatchingIndex ? Direction.Down : Direction.Up;
                divisor *= 2;
                if (direction == Direction.Up) index += length / divisor;
                else index -= length / divisor;

                if (oldIndex == index)
                {
                    if (currentItem.StartsWith(prefix, bogstavIndex)) return index;
                    if (direction == Direction.Down) return ++index; // one too far down
                    return --index; // on too high
                }
                currentItem = list.ElementAt(index);
            }
        }

        private enum Direction
        {
            Up, Down
        }
    }
}
