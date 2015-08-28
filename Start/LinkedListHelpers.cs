using System.Collections.Generic;
using System.Linq;

namespace Start {
	internal static class LinkedListHelpers {
		public static IEnumerable<LinkedListNode<T>> Nodes<T>(this LinkedList<T> lst) {
			for (var cur = lst.First; cur != null; cur = cur.Next) {
				yield return cur;
			}
		}

		public static IEnumerable<LinkedListNode<T>> NextNodes<T>(this LinkedListNode<T> lst) {
			for (var cur = lst; cur != null; cur = cur.Next) {
				yield return cur;
			}
		}

		public static IEnumerable<LinkedListNode<T>> GetRange<T>(this LinkedList<T> lst, LinkedListNode<T> start,
			LinkedListNode<T> end) {
			for (var cur = start; cur != end && cur != null; cur = cur.Next) {
				yield return cur;
			}
			yield return end;
		}

		public static LinkedListNode<T> Skip<T>(this LinkedListNode<T> start, int count) {
			for (int i = 0; i < count; i++) {
				start = start.Next;
			}
			return start;
		} 

		public static void RemoveRange<T>(this LinkedList<T> lst, LinkedListNode<T> start, LinkedListNode<T> end) {
			var range = lst.GetRange(start, end).ToList();
			range.ForEach(lst.Remove);
		}

		public static LinkedListNode<T> AddLastRange<T>(this LinkedList<T> lst, IEnumerable<T> seq) {
			LinkedListNode<T> first = null;
			foreach (var item in seq) {
				var newTail = lst.AddLast(item);
				if (first == null) {
					first = newTail;
				}
			}
			return first;
		}

		public static LinkedListNode<T> AddAfterRange<T>(this LinkedList<T> lst, LinkedListNode<T> after, IEnumerable<T> seq) {
			seq.Aggregate(after, lst.AddAfter);
			return after;
		} 
	}
}