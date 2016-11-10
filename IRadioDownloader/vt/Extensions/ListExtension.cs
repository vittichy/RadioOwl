using System;
using System.Collections.Generic;

namespace vt.Extensions
{
    public static class ListExtension
    {
        /// <summary>
        /// return previous item from list
        /// </summary>
        public static T Prev<T>(this List<T> list, T item)
        {
            var index = list.IndexOf(item);
            var prevItem = (index > 0) ? list[index - 1] : default(T);
            return prevItem;
        }


        /// <summary>
        /// return next item from List
        /// </summary>
        public static T Next<T>(this List<T> list, T item)
        {
            var index = list.IndexOf(item);
            var nextItem = (index < (list.Count - 1)) ? list[index + 1] : default(T);
            return nextItem;
        }


        /// <summary>
        /// return Queue of T
        /// </summary>
        public static Queue<T> ToQueue<T>(this List<T> value)
        {
            var queue = new Queue<T>(value);
            return queue;            
        }


        public static string ToCrLfText<T>(this List<T> list)
        {
            var text = string.Join(Environment.NewLine, list);
            return text;
        }

    }
}
