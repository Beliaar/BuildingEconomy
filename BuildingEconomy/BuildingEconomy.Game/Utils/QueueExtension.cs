using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildingEconomy.Utils
{
    public static class QueueExtension
    {
#if NET461
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
