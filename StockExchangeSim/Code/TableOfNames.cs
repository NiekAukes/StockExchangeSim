using System.Collections.Generic;

namespace Eco
{
    public static class Thoughts
    {
        //for normal traders
        public static string buy = "I'm going to buy.";
        public static string sell = "I'm going to sell.";
        public static string idk = "I'm not sure what to do.";
        public static string wait = "I'm going to wait a little.";
        public static string hold = "I think I'm going to hold onto my stocks.";
        public static string hate = "I absolutely despise this company.";

        //for liquidity traders
        public static string loweringPrice = "I'm lowering my price.";
        public static string increasingPrice = "I'm increasing my price.";
        /*public static string loweringBidPrice = "I'm lowering my bid price.";
        public static string increasingBidPrice = "I'm increasing my bid price.";*/

        //for breakout traders

    }
    public class TraderNames
    {
        public List<string> traderNames = new List<string>()
        {
            "Tom de Trader" ,
            "Jan met de Korte Achternaam" ,
            "Bob de Bouwer" ,
            "Henk de Handelaar" ,
            "Pietje Precies" ,
            "Klaas Komma" ,
            "Niek Aukes" ,
            "Kees Postma" ,
            "Max-Friso Schaap" ,
            "Douwe Osinga" ,
            "Jacob de Haas" ,
            "Stefan Spekreijse" ,
            "Franz Ferdinand von Habsburg" ,
            "Louis Dijk" ,
            "Lena van Dijk" ,
            "Marjolein Voetberg" ,
            "Dita Steijn" ,
            "Mitch McConnel",
            "Bram de Broker",
            "Julien Vries",
            "Salty Dennis",
            "Frank Frankington",
            "Ashura Babangida",
            "Uvuvwevwe Onyetenyevwe Ugwembubwem Ossas",
            "Nhlanhla Ganiah Nhlakanipho",
            "Louis Ouioui",
            "Jarno Babatunde",
            "Maarten Leuter Koning",
            "Tjitte Kast",
            "Camille"
        };
        public TraderNames()
        {

        }
}

    public class FieldNameInfo
    {
        public string FieldName { get; set; }
        public List<string> CompanyNames { get; set; }
    }

    public class TableOfNames
    {
        public List<FieldNameInfo> NameInfo = new List<FieldNameInfo>()
        {

            new FieldNameInfo()
            {
                FieldName = "IT Sector",
                CompanyNames = new List<string>()
                {
                    "Mapple Inc.",
                    "MacroHard Corporation",
                    "OSCOM BV",
                    "MVideo Corporation",
                    "Outtel Inc.",
                    "Computer4U",
                    "BitBox Software",
                    "DeltaSquare Inc.",
                    "Volvo Studios",
                    "Elgoog Inc.",
                    "Hipbone Inc.",
                    "Fools Studio",
                }
            },
            new FieldNameInfo()
            {
                FieldName = "Medical Sector",
                CompanyNames = new List<string>()
                {
                    "American Academy of Family Physicians",
                    "Abortion Services company",
                    "Pilzen BioDegradables",
                    "BioEntech",
                    "OldSchoola Medicine",
                    "AZ Voetbalverband",
                    "AAG Labs",
                    "Prik 'n Weg",
                    "SanofiGSK Vaccinations",
                    "CMG Health Corp.",
                    "Cardinal Health Inc.",
                    "Jan & Jan",
                }
            },
            new FieldNameInfo()
            {
                FieldName = "Economic Sector",
                CompanyNames = new List<string>()
                {
                    "ABN Ambro Group",
                    "Deutsche Bank",
                    "Hare Banking",
                    "Rabobank",
                    "DNB ASA",
                    "HSBC Holdings PLC",
                    "Barclays",
                    "JPMorgan Chase Bank",
                    "Berkshire Hathaway",
                    "Alphabet Limited",
                    "Sony Corporation",
                    "Northern Trust Corporation",
                }
            },
            new FieldNameInfo()
            {
                FieldName = "Food Supplement Sector",
                CompanyNames = new List<string>()
                {
                    "Corona Distillery",
                    "Hertog Kees Brewery",
                    "Jupiter Beer",
                    "Big Smack Food Corp.",
                    "Bec Dongalds Inc.",

                    "comp 6",
                    "comp 7",
                    "comp 8",
                    "comp 9",
                    "comp 10",
                    "comp 11",
                    "comp 12"
                }
            },
            new FieldNameInfo()
            {
                FieldName = "Construction Sector",
                CompanyNames = new List<string>()
                {
                    "Jelle's Boubedriuw B.V.",
                    "Bob de Douwer GMBH",
                    "Steentje Los B.V.",
                    "Keibest N.V.",
                    "Steengoed GMBH",
                    "Grondig Gegraven",

                    "comp 7",
                    "comp 8",
                    "comp 9",
                    "comp 10",
                    "comp 11",
                    "comp 12"
                }
            },

        };
    }
}
