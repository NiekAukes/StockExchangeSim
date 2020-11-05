using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;
using StockExchangeSim;
using StockExchangeSim.Views;
using Telerik.UI.Xaml.Controls.Data;
using Windows.Security.Authentication.Web.Provider;
using System.Threading;

namespace Eco
{
    public class Field
    {
        Random rn = null;

        public int id;
        public int companyAmount;
        public int maxCompanyStartAmount = 12;

        public double Innovation = 1.5;
        public double Scandals = 0.2;
        public float ScandalSeverity = 1;

        public double MarketShare = 1.1;
        public double TotalCompetitiveness = 0;
        public double TotalValue = 0;

        int startamount;
        public List<Company> companies = new List<Company>();
        List<Company> startcompanies = null;



        public Field(int Id)
        {
            id = Id;
            rn = new Random(Master.Seed * (Id + 1));
            companyAmount = rn.Next(1, maxCompanyStartAmount);
            for (int i = 0; i < companyAmount; i++)
            {
                Company cp = new Company(this);
                cp.id = i;
                cp.SetValue(rn.NextDouble() * 2000);
                cp.stockprice = cp.Value / 100;

                Master.exchange.RegisterCompany(cp);
                cp.BecomePublic();

                companies.Add(cp);
            }
            startcompanies = new List<Company>(companies);
            startamount = companyAmount;

        }
        int scandaltick = 0;

        public void Update()
        {
            //calculate innovation (FIXED)
            if (rn.NextDouble() < Innovation * MainPage.master.SecondsPerTick / 15 / 1450.461994) // 5617.61515
            {
                if (rn.NextDouble() < Innovation * MainPage.master.SecondsPerTick / 15 / 1450.461994) //5617.61515
                {
                    //an innovation can create a new company
                    if (rn.NextDouble() < 0.4)
                    {
                        Company newcp = new Company(this);
                        newcp.id = companyAmount++;
                        newcp.SetValue(rn.NextDouble() * 600);
                        double highvalue = Math.Pow(rn.NextDouble(), 2);
                        newcp.Competitiveness += highvalue * 200;
                        companies.Add(newcp);
                        startcompanies.Add(newcp);


                    }
                    else
                    {
                        Company cp = companies[rn.Next(companies.Count)];
                        double highvalue = Math.Pow(rn.NextDouble(), 2);
                        cp.Competitiveness += highvalue * 0.2 * cp.Value;

                    }
                }
            }

            //calculate scandals
            if (rn.NextDouble() < Scandals * 9999 * MainPage.master.SecondsPerTick / 15 / 1450.461994)
            {
                if (rn.NextDouble() < Scandals * 9999 * MainPage.master.SecondsPerTick / 15 / 1450.461994)
                {
                    scandaltick++;
                    if (scandaltick > 9999)
                    {
                        //impact is based on company value
                        Company cp = companies[rn.Next(companies.Count)];
                        double highvalue = Math.Pow(rn.NextDouble() * 1, 3);
                        cp.Competitiveness -= highvalue * 0.1 * ScandalSeverity * cp.Value;
                        scandaltick = 0;
                    }
                }
            }
            if (companies.Count < 1 || (companies.Count == 1 && companies[0].Value < 2000 *
                (companies[0].Competitiveness / 100) / Master.Conjucture))
            {
                Company newcp = new Company(this);
                newcp.id = companyAmount++;
                newcp.SetValue(rn.NextDouble() * 500);
                companies.Add(newcp);
                startcompanies.Add(newcp);
            }

            //ordinary things VERVANGEN MET CONCURRENTIEPOSITIE => CHECK
            double value = Math.Pow(rn.NextDouble() - 0.5, 3);
            int select = rn.Next(0, companies.Count);
            companies[select].Competitiveness += value * 0.001 * companies[select].Value;

            TotalCompetitiveness = 0;
            TotalValue = 0;

            double tick = MainPage.master.SecondsPerTick / (24.0 * 60.0 * 60);

            //check for bankrupty
            for (int i = 0; i < companies.Count; i++)
            {
                TotalCompetitiveness += companies[i].Competitiveness;
                TotalValue += companies[i].Value;
            }
            for (int i = 0; i < companies.Count; i++)
            {
                companies[i].age += tick;
                companies[i].Update();
                if (companies[i].Value < -300)
                {
                    //BANKRUPT
                    companies[i].Bankrupt = true;
                    if (MainPage.inst != null)
                    {
                        Company cp = companies[i];
                        var ignore = MainPage.inst.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                        {
                            cp.stockViewModel.prices.Clear();
                        });
                    }
                    companies.RemoveAt(i);

                    
                }
            }
        }
        
        public void print()
        {
            
            Debug.WriteLine("\nfield id: " + id + ", started with " + startamount);
            
            for (int i = 0; i < startcompanies.Count; i++)
            {
                if (startcompanies[i].Bankrupt)
                {
                    Debug.WriteLine("Company " + startcompanies[i].id + ": BANKRUPT, Time alive: " + startcompanies[i].age + " days");
                }
                else
                {
                    Debug.WriteLine("Company " + startcompanies[i].id + ": " + startcompanies[i].Value + ", age: " + startcompanies[i].age + " days");
                }
            }
        }
        public string getInfo()
        {
            string displaay = "";
            displaay += "field id: " + id + ", started with " + startamount;

            for (int i = 0; i < startcompanies.Count; i++)
            {
                if (startcompanies[i].Bankrupt)
                {
                    displaay += "\nCompany " + startcompanies[i].id + ": BANKRUPT,\t Time alive: " + Math.Floor(startcompanies[i].age) + " days";
                }
                else
                {
                    if (MainPage.master.SecondsPerTick >= 15)
                    {
                        //decemille
                        double lastgain = Math.Floor(startcompanies[i].LastDeceMilleGain * 1000 * 1000 * 100) / 1000;
                        string lastgainstr = lastgain < 0.1 ? (lastgain * 1000).ToString() + " µ%" : (lastgain).ToString() + " m%";
                        displaay += "\nCompany " + startcompanies[i].id +
                            ": " + Math.Floor(startcompanies[i].LastDeceMilleValue) +
                            ",\t gain: " + lastgainstr +
                            ",\t age: " + Math.Floor(startcompanies[i].age) + " days";
                    }
                    else if (MainPage.master.SecondsPerTick >= 7.5)
                    {
                        //mille
                        double lastgain = Math.Floor(startcompanies[i].LastMilleGain * 1000 * 1000 * 100) / 1000;
                        string lastgainstr = lastgain < 0.1 ? (lastgain * 1000).ToString() + " µ%" : (lastgain).ToString() + " m%";

                        displaay += "\nCompany " + startcompanies[i].id +
                            ": " + Math.Floor(startcompanies[i].LastMilleValue) +
                            ",\t gain: " + lastgainstr +
                            ",\t age: " + Math.Floor(startcompanies[i].age) + " days";
                    }
                    else
                    {
                        //centum
                        double lastgain = Math.Floor(startcompanies[i].LastCentumGain * 1000 * 1000 * 100) / 1000;
                        string lastgainstr = lastgain < 0.1 ? (lastgain * 1000).ToString() + " µ%" : (lastgain).ToString() + " m%";
                        displaay += "\nCompany " + startcompanies[i].id +
                        ": " + Math.Floor(startcompanies[i].LastCentumValue) +
                        ",\t gain: " + lastgainstr + ",\t price: " + Math.Floor(startcompanies[i].stockprice * 100) / 100 + 
                        ",\t age: " + Math.Floor(startcompanies[i].age) + " days";
                    }
                }
            }
            
            

            return displaay + "\n\n";
        }
    }
}
