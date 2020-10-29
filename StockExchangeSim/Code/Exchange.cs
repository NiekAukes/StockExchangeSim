using Eco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace StockExchangeSim.Code
{
    public struct ExchangeDetails
    {

    }
    public class Exchange
    {
        public List<Company> companies;
        public List<Trader> traders; 
        public Exchange()
        {

        }
        public ExchangeDetails? ExchangeStock(Stock stock, IStockOwner newOwner)
        {
            
            newOwner.AddStock(stock);

            stock.Owner.UpdateHoldings();
            return null;
        }
    }
}
