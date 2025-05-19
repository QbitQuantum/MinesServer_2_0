using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinesServer.Server
{
    public class ServerTime2
    {
        public void Start()
        {
            Task.Run(() =>
            {
                Stopwatch stopwatch = new Stopwatch();
                double num9 = 16.666666666666668;
                double num10 = 0.0;
                int num11 = 0;
                while (true)
                {
                    double totalMilliseconds = stopwatch.Elapsed.TotalMilliseconds;
                    if (totalMilliseconds + num10 >= num9)
                    {
                        num11++;
                        num10 += totalMilliseconds - num9;
                        stopwatch.Reset();
                        stopwatch.Start();
                        //update
                        double num12 = stopwatch.Elapsed.TotalMilliseconds + num10;
                        if (num12 < num9)
                        {
                            int num13 = (int)(num9 - num12) - 1;
                            if (num13 > 1)
                            {
                                Thread.Sleep(num13 - 1);
                                /* if zero players
                                if (!Netplay.HasClients)
                                {
                                    num10 = 0.0;
                                    Thread.Sleep(10);
                                }*/
                            }
                        }
                    }
                    Thread.Sleep(0);
                }
            });
        }
    }
}
