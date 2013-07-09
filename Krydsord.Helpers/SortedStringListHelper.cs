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
            var first = BinaryFindFirstOrLastFromAny(s, prefix, s.Count/2, Direction.Down);
            if (!s[first].StartsWith(prefix)) return new List<string>();
            var last = BinaryFindFirstOrLastFromAny(s, prefix, first, Direction.Up);
            
            return s.GetRange(first, last - first);
        }

        public static bool AnyStartsWith(this List<string> s, string prefix)
        {
            var index = BinarySearchAny(s, prefix);
            return index >= 0 && s.ElementAt(index).StartsWith(prefix);
        }

        // TODO: Ret efter http://en.wikipedia.org/wiki/Binary_search_algorithm - også i sortedcomparable
        private static int BinarySearchAny(List<string> list, string prefix)
        {
            int length = list.Count;
            int divisor = 2;
            int index = length/divisor;
            string currentItem = list.ElementAt(index);
            while (!currentItem.StartsWith(prefix))
            {
                var oldIndex = index;
                if (String.Compare(prefix, currentItem) > 0)
                {
                    divisor *= 2;
                    index += length/divisor;
                }
                else
                {
                    divisor *= 2;
                    index -= length/divisor;
                }
                if (oldIndex == index) return -1;
                currentItem = list.ElementAt(index);
            }
            return index;
        }

        private static int BinaryFindFirstOrLastFromAny(List<string> list, string prefix, int anyMatchingIndex, Direction direction)
        {
            int length = direction == Direction.Down ? anyMatchingIndex : list.Count - anyMatchingIndex; // length from index to end of list in specified direction
            int divisor = 2;
            int index = direction == Direction.Down ? length / divisor : anyMatchingIndex + (length / divisor); // Set index halfway from matching index and end of list in specified direction
            string currentItem = list.ElementAt(index);
            while (true)
            {
                var oldIndex = index;
                var comparison = String.Compare(currentItem, prefix);
                
                // Too far up
                if (!currentItem.StartsWith(prefix) && comparison > 0) direction = Direction.Down;
                // Too far down
                else if (!currentItem.StartsWith(prefix) && comparison < 0) direction = Direction.Up;
                    // If none of the above matched, we are within the range of matches, should move avay from anyMatchingIndex
                else direction = index > anyMatchingIndex ? Direction.Down : Direction.Up;
                divisor *= 2;
                if (direction == Direction.Up) index += length/divisor;
                else index -= length/divisor;

                if (oldIndex == index)
                {
                    if (currentItem.StartsWith(prefix)) return index;
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
