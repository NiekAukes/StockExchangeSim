using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App1;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace Eco
{
    public abstract class MarketWatcher<S>
    {
        public Company cp;
        public S Strategy;
        public abstract float UpdateInsights();
        public abstract void RedoInsights();

    }
    
    
}
