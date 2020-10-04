﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;
using StockExchangeSim;
using StockExchangeSim.Views;
using Telerik.UI.Xaml.Controls.Data;

namespace Eco
{
    public class Company
    {
        Random rn = new Random(Master.Seed);
        Stock CompanyStock = null;

        //setup values
        public int id;
        public Company()
        {
            CompanyStock = CreateStock(100);

            //calculate cumulative demand (cumulatieve vraag)
            //so you can calculate
        }
        //variable values
        #region variableValues
        public void SetValue(double val)
        {
            value = val;
            LastDecemValue = val;
            LastCentumValue = val;
            LastMilleValue = val;
            LastDeceMilleValue = val;
            LastCentuMilleValue = val;
        }
        public double age = 0;
        public bool Bankrupt = false;

        public double value = 50; 
        public double LastDecemValue;
        public double LastCentumValue;
        public double LastMilleValue;
        public double LastDeceMilleValue;
        public double LastCentuMilleValue;

        //Gain Calculation
        public double LastTickGain = 0.0; //value Gained per tick
        public double LastTickSlope = 0.0; // gain Gained per tick
        public double LastDecemGain;
        public double LastCentumGain;
        public double LastMilleGain;
        public double LastDeceMilleGain;
        public double LastCentuMilleGain;
        #endregion

        UInt64 CurrentTick = 0;
        public void Update()
        {
            CurrentTick++;
            //value += Math.Pow((rn.NextDouble() - 0.5) * 5, 3); 
            value += LastTickGain * MainPage.master.SecondsPerTick;
            LastTickGain += -LastTickGain * 0.00001 * MainPage.master.SecondsPerTick;


            if (CurrentTick % 10 == 0)
            {
                LastDecemGain = -(LastDecemValue - value) / LastDecemValue;
                LastDecemValue = value;
            }
            if (CurrentTick % 100 == 0)
            {
                LastCentumGain = -(LastCentumValue - value) / LastCentumValue;
                LastCentumValue = value;
            }
            if (CurrentTick % 1000 == 0)
            {
                LastMilleGain = -(LastMilleValue - value) / LastMilleValue;
                LastMilleValue = value;
            }
            if (CurrentTick % 10000 == 0)
            {
                LastDeceMilleGain = -(LastDeceMilleValue - value) / LastDeceMilleValue;
                LastDeceMilleValue = value;
            }
            if (CurrentTick % 100000 == 0)
            {
                LastCentuMilleGain = -(LastCentuMilleValue - value) / LastCentuMilleValue;
                LastCentuMilleValue = value;
            }
        }

        public class Stock
        {
            public Company company { get; private set; }
            public double value { get { return company.value * Percentage / 100; }}
            public double Percentage { get; private set; }
            public Stock(Company cp, double percentage)
            {
                //create new stock from company
                company = cp;
                Percentage = percentage;
            }

            public Stock(Stock s, double percentage)
            {
                //create new stock from other stock
            }
            public Stock SplitStock(double percentage)
            {
                Stock ret = new Stock(this, percentage);
                return ret;
            }
            public void CombineStock(Stock stock)
            {
                if (company != stock.company)
                {
                #if DEBUG
                    throw new Exception("Stock didn't have same company");
                #endif
                    return;
                }
                Percentage += stock.Percentage;
            }
        }

        private Stock CreateStock(double percentage)
        {
            Stock ret = new Stock(this, percentage);
            return ret;
        }     
    }
    public class Field
    {
        Random rn = new Random(Master.Seed);

        public int id;
        public int companyAmount;

        public double Innovation = 1.0;
        public double Scandals = 0.2;
        public float ScandalSeverity = 1;

        int startamount;
        List<Company> companies = new List<Company>();
        List<Company> startcompanies = null;

        public Field()
        {
            
            companyAmount = rn.Next(1, 5);
            for (int i = 0; i < companyAmount; i++)
            {
                Company cp = new Company();
                cp.id = i;
                cp.SetValue(rn.NextDouble() * 2000);
                companies.Add(cp);
            }
            startcompanies = new List<Company>(companies);
            startamount = companyAmount;
        }
        public void Update()
        {
            //calculate innovation (FIXED)
            if (rn.NextDouble() < Innovation * MainPage.master.SecondsPerTick/ 15 / 1450.461994) // 5617.61515
            {
                if (rn.NextDouble() < Innovation * MainPage.master.SecondsPerTick / 15 / 1450.461994) //5617.61515
                {
                    //an innovation can create a new company
                    if (rn.NextDouble() < 0.2)
                    {
                        Company newcp = new Company();
                        newcp.id = companyAmount++;
                        newcp.SetValue(rn.NextDouble() * 300);
                        double highvalue = Math.Pow(rn.NextDouble(), 2);
                        newcp.LastTickGain += highvalue * 0.2;
                        companies.Add(newcp);
                        startcompanies.Add(newcp);


                    }
                    else
                    {
                        double highvalue = Math.Pow(rn.NextDouble(), 2);
                        companies[rn.Next(companies.Count)].LastTickGain += highvalue * 0.2;

                        for (int i = 0; i < companies.Count; i++)
                        {
                            companies[i].LastTickGain -= highvalue * 0.1 * 1/companies.Count;
                        }
                    }
                }
            }

            //calculate scandals
            if (rn.NextDouble() < Scandals * MainPage.master.SecondsPerTick / 15 / 1450.461994)
            {
                if (rn.NextDouble() < Scandals * MainPage.master.SecondsPerTick / 15 / 1450.461994)
                {
                    //impact is based on company value
                    Company cp = companies[rn.Next(companies.Count)];
                    double highvalue = Math.Pow(rn.NextDouble() * 1, 3);
                    cp.LastTickGain -= highvalue * cp.value * 0.0002 * ScandalSeverity;
                }
            }
            if (companies.Count < 1 || (companies.Count == 1 && companies[0].value < 500))
            {
                Company newcp = new Company();
                newcp.id = companyAmount++;
                newcp.SetValue(rn.NextDouble() * 300);
                companies.Add(newcp);
                startcompanies.Add(newcp);
            }

            //ordinary things
            double value = Math.Pow(rn.NextDouble() - 0.5, 3);
            int select = rn.Next(0, companies.Count);
            companies[select].LastTickGain += value * 0.0001;

            //check for bankrupty
            for (int i = 0; i < companies.Count; i++)
            {
                double tick = MainPage.master.SecondsPerTick / (24.0 * 60.0 * 60);
                companies[i].age += tick;
                companies[i].Update();
                if (companies[i].value < -300)
                {
                    //BANKRUPT
                    companies[i].Bankrupt = true;
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
                    Debug.WriteLine("Company " + startcompanies[i].id + ": " + startcompanies[i].value + ", age: " + startcompanies[i].age + " days");
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
                    displaay += "\nCompany " + startcompanies[i].id + ": BANKRUPT,\t Time alive: " + startcompanies[i].age + " days";
                }
                else
                {
                    if (MainPage.master.SecondsPerTick >= 15)
                    {
                        //decemille
                        double lastgain = Math.Floor(startcompanies[i].LastDeceMilleGain * 1000 * 1000) / 1000;
                        string lastgainstr = lastgain < 0.1 ? (lastgain * 1000).ToString() + " µ%" : (lastgain).ToString() + " m%";
                        displaay += "\nCompany " + startcompanies[i].id +
                            ": " + Math.Floor(startcompanies[i].LastDeceMilleValue) +
                            ",\t gain: " + lastgainstr +
                            ",\t age: " + Math.Floor(startcompanies[i].age) + " days";
                    }
                    else if (MainPage.master.SecondsPerTick >= 7.5)
                    {
                        //mille
                        double lastgain = Math.Floor(startcompanies[i].LastMilleGain * 1000 * 1000) / 1000;
                        string lastgainstr = lastgain < 0.1 ? (lastgain * 1000).ToString() + " µ%" : (lastgain).ToString() + " m%";

                        displaay += "\nCompany " + startcompanies[i].id +
                            ": " + Math.Floor(startcompanies[i].LastMilleValue) +
                            ",\t gain: " + lastgainstr +
                            ",\t age: " + Math.Floor(startcompanies[i].age) + " days";
                    }
                    else
                    {
                        //centum
                        double lastgain = Math.Floor(startcompanies[i].LastCentumGain * 1000 * 1000) / 1000;
                        string lastgainstr = lastgain < 0.1 ? (lastgain * 1000).ToString() + " µ%" : (lastgain).ToString() + " m%";
                        displaay += "\nCompany " + startcompanies[i].id +
                        ": " + Math.Floor(startcompanies[i].LastCentumValue) +
                        ",\t gain: " + lastgainstr +
                        ",\t age: " + Math.Floor(startcompanies[i].age) + " days";
                    }
                }
            }
            
            

            return displaay;
        }
    }
}
