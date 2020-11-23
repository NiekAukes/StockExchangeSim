using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using StockExchangeSim.Views;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace StockExchangeSim.Views
{
    public sealed partial class CompanyDetail : UserControl
    {
        public CompanyDetail()
        {
            this.InitializeComponent();
            companyList.Items.Clear();


        }


        public void InsertDetails(List<Eco.Company> companies)
        {
            List<Eco.Company> listofCompanies = companies;

            companyList.Items.Clear();

            //insert company names:
            for(int i = 0; i<listofCompanies.Count; i++){
                listofCompanies[i].initName();

                //make listboxitem to shove name into (with force)
                ListBoxItem item = new ListBoxItem();
                item.Name = listofCompanies[i].name;
                companyList.Items.Add(item);
            }

            //load the first company's details in already, before the user has to click anything
            Eco.Company comp1 = listofCompanies[0];
            CompanyName.Text = comp1.name;
            companyValue.Text = "Company value: " + comp1.Value.ToString();
            currStockPrice.Text = "Stock price: " + comp1.stockprice.ToString();

            //shove stock prices into chart: biek baukes moet daarmit mar eem helpen
            //stockPriceChart
        }

        private void companyList_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            //make style of selected listboxitem different
        }

        private void companyList_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            //if pressed and released load new company data into the usercontrol
        }
    }
}
