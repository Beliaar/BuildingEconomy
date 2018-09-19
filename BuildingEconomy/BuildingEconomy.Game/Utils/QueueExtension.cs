using System;
using System.Collections.Generic;

namespace BuildingEconomy.Utils
{
    public static class QueueExtension
    {
#if NET461 // Current .NET version that XENKO uses.
        public static bool TryDequeue<T>(this Queue<T> queue, out T result)
        {
            try
            {
                result = queue.Dequeue();
                return true;
            }
            catch (InvalidOperationException)
            {
                result = default;
                return false;
            }
        }
#endif
    }
}
