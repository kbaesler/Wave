using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Miner.Interop;

namespace Wave.Samples.AUs
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("==========================================");
            Console.WriteLine("Wave Sample: AU Triggers");
            Console.WriteLine("==========================================");

            using (new AutoUpdaterModeReverter(mmAutoUpdaterMode.mmAUMNoEvents))
            {
                
            }
        }
    }
}
