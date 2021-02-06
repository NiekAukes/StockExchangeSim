using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eco
{
    //bid ask implementatie
    public class Holder
    {
        public event Stocktraded
        public int MaxStockLimit = int.MaxValue;
        public Trader Owner { get; set; }
        public List<Stock> Stocks { get; set; }
        //public SynchronizedCollection<Liquidity> liquidity1m = new SynchronizedCollection<Liquidity>();
        public BidAsk bidask { get; set; }
        public Holder(Company cp, Trader trader)
        {
            Owner = trader;
            bidask = new BidAsk(cp);
            Stocks = new List<Stock>();
        }
        public override string ToString()
        {
            return "owned by: " + Owner.ToString();
        }
    }

    public interface IBidAsk
    {
        Company cp { get; set; }
        float Bid { get; set; }
        float Ask { get; set; }
    }

    public class BidAsk : IBidAsk
    {
        public Company cp { get; set; }
        public float Bid { get; set; }
        public float Ask { get; set; }
        public BidAsk(Company company)
        {
            cp = company;
            Bid = 0;
            Ask = 0;
        }
    }
    public class OLD_BidAsk : BidAsk, IBidAsk
    {
        public List<Stock> Stocks { get; set; }
        public SynchronizedCollection<Liquidity> liquidity1m { get; set; }
        public OLD_BidAsk(Company company) : base(company)
        {
            Stocks = new List<Stock>();
            liquidity1m = new SynchronizedCollection<Liquidity>();
        }
    }

    public class SortedSyncCollection<T>
    {
        public bool Ascending = true;
        public SynchronizedCollection<T> list { get; set; }
        public void AddSortedItem(T item, string PropertyName)
        {
            object property = item.GetType().GetProperty(PropertyName).GetValue(item, null);
            for (int i = 0; i < list.Count; i++)
            {
                object comparison = list[i].GetType().GetProperty(PropertyName).GetValue(list[i], null);
                if (Ascending ? (float)property > (float)comparison : (float)property < (float)comparison)
                {
                    list.Insert(i, item);
                }
                    
            }
        }
        public SortedSyncCollection()
        {
            list = new SynchronizedCollection<T>();
        }
    }

    public class SellOrder
    {
        public bool isPermanent { get; set; }
        public Company cp { get; set; }
        public Stock Stock { get; set; }
        public float LimitPrice { get; set; }
        public SellOrder(Company cp, Stock stock, float limitPrice)
        {
            this.cp = cp ?? throw new ArgumentNullException(nameof(cp));
            Stock = stock ?? throw new ArgumentNullException(nameof(stock));
            LimitPrice = limitPrice;
        }
    }
    public class BuyOrder
    {
        public bool isPermanent { get; set; }
        public Company cp { get; set; }
        public Trader Buyer { get; set; }
        public float LimitPrice { get; set; }
        public BuyOrder(Company cp, Trader buyer, float limitPrice)
        {
            this.cp = cp ?? throw new ArgumentNullException(nameof(cp));
            Buyer = buyer ?? throw new ArgumentNullException(nameof(buyer));
            LimitPrice = limitPrice;
        }
    }

    public interface IExchange
    {
        float money { get; set; }

        bool BuyStock(Company cp, Trader buyer);
        void RegisterCompany(Company cp, int partition);
        void RegisterTrader(Trader t);
        void SellStock(Stock stock);
    }
}
