using System;
using System.Collections.Generic;
using System.Diagnostics;
using MicroLibrary;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Reflection.Metadata.Ecma335;

namespace Eco
{
    public class Time
    {
        UInt64 seconds = 0;
        public Time(UInt64 Seconds)
        {
            seconds = Seconds;
        }
        public UInt64 GetSeconds()
        {
            return seconds;
        }
        public float GetMinutes()
        {
            return seconds / 60.0f;
        }
        public float GetHours()
        {
            return seconds / 60.0f / 60;

        }
        public float GetDays()
        {

            return seconds / 60.0f / 60/ 24;
        }
        public float GetMonths()
        {
            return seconds / 60.0f / 60 / 365.25f * 12;

        }
        public float GetYears()
        {
            return seconds / 60.0f / 60 / 365.25f;

        }

        public string ParseTime()
        {
            //seconds
            if (seconds > 60)
            {
                //minutes
                if (seconds > 60*60)
                {
                    //hours
                    if (seconds > 60 * 60 * 24)
                    {
                        //days
                        if (seconds > 60 * 60 * 24 * 365.25)
                        {
                            //years
                        }
                    }
                    else
                    {
                        double hours = Math.Floor(seconds / 60.0 / 60.0);
                        return hours.ToString() + " : " + (((seconds / 60 / 60) - hours) * 60).ToString();
                    }
                }
                else
                {
                    double mins = Math.Floor(seconds / 60.0);
                    return mins.ToString() + " : " + (((seconds / 60) - mins) * 60).ToString();
                }
            }
            else
            {
                return seconds.ToString() + " seconds";
            }
            return "";
        }
        
    }
    public class Master
    {
        public static Int32 Seed = (new Random()).Next();
        System.Threading.Thread thread;
        public void SetTickEveryX(double YearPerSec) //2103840 = 1 year per second
        {
            TPS = 2103840 * YearPerSec;
        }
        public Time time;
        public int FieldAmount;
        public int TraderAmount;
        public int HFTAmount;
        public double TPS = 2103840; //ticks per second. 1577880 = 20s per year
        public double Year;
        //list for fields and traders
        public List<Field> Fields = new List<Field>();
        public List<Trader> Traders = new List<Trader>();

        public Master(int fields, int traders, int hftraders)
        {
            //set vars
            FieldAmount = fields;
            TraderAmount = traders;
            HFTAmount = hftraders;

            //construction
            for (int i = 0; i < fields; i++)
            {
                Fields.Add(new Field());
                Fields[0].id = i;
            }
            for (int i = 0; i < traders; i++)
            {
                Traders.Add(new Trader());
            }

            thread = new System.Threading.Thread(Update);
            thread.Start();
        }

        public void Update()
        {

            MicroStopwatch sw = new MicroStopwatch();
            sw.Start();
            for (int i = 0; i < 100000000000; i++)
            {   
                
                

                for (int j = 0; j < FieldAmount; j++)
                {
                    Fields[j].Update();
                }
                Year = i / (365.25 * 24 * 60 * 4);
                if (i % 100000 == 0)
                {
                    for (int j = 0; j < FieldAmount; j++)
                    {
                        System.Diagnostics.Debug.WriteLine(i / (365.25 * 24 * 60 * 4) + " years past");
                        Fields[j].print();
                    }
                }
                int tickpass = 1000;
                if (i % tickpass == 0 && i != 0)
                {
                    sw.Stop();
                    if ((sw.ElapsedMicroseconds * 0.001) < (tickpass * 1000.0 / TPS))
                    {
                        Thread.Sleep((int)( (tickpass * 1000.0 / TPS) - sw.ElapsedMicroseconds / 1000));
                    }
                    sw.Reset();
                    sw.Start();
                }
            }

            for (int j = 0; j < FieldAmount; j++)
            {
                System.Diagnostics.Debug.WriteLine(100000000000 / (365.25 * 24 * 60 * 60) + " years past");
                Fields[j].print();
            }
        }
    }
}
