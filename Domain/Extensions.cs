using System;
using System.Collections.Generic;
using System.Linq;

namespace Domain
{
    public static class Extensions
    {
        public static IEnumerable<IEnumerable<T>> Chunk<T>(this IEnumerable<T> list, int chunkSize)
        {
            if (chunkSize <= 0)
            {
                throw new ArgumentException("chunkSize must be greater than 0.");
            }

            var array = list as T[] ?? list.ToArray();
            while (array.Any())
            {
                yield return array.Take(chunkSize);
                list = array.Skip(chunkSize);
            }
        }
    }
}