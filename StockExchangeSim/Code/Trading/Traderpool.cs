using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Eco
{
    public class Traderpool
    {
        public Traderpool()
        {
            this.traders = new List<Trader>();
            this.thread = new Thread(TraderUpdate);
            
        }

        public List<Trader> traders { get; set; }
        Thread thread { get; set; }
        public void StartThread()
        {
            thread.Start();
        }
        public void TraderUpdate()
        {
            while (Master.inst.alive)
            {
                foreach (Trader td in traders)
                {
                    if (Master.inst.active)
                        td.Update();
                    else
                        Thread.Sleep(10);
                }
                
            }
            
        }

    }
}
