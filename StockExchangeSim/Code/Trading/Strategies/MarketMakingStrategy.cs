﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eco
{
    class MarketMakingStrategy : Trader.Strategy
    {
        public override void Observe()
        {
            throw new NotImplementedException();
        }

        public override Trader.MarketResults StrategyOutcome(Trader trader, ExchangeBrokerMM exchange)
        {
            throw new NotImplementedException();
        }
    }
}