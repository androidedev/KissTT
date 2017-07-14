using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ktt3.Util
{
    public static class DTrace
    {
        public static void Trace(string msg)
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine(">>>>>> " + msg);
#endif
        }


        public static void Trace(Exception exception)
        {
#if DEBUG
            int cnt = 0;
            Exception inner = exception.InnerException;
            while (inner != null)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("({0}>{1}) >> {2}", cnt, DateTime.Now.ToLongTimeString(), inner.Message));
                inner = inner.InnerException;
                cnt++;
            }
#endif
        }

    }
}
