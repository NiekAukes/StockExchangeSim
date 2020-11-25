using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eco
{
    class FieldNameInfo
    {
        public string FieldName { get; set; }
        public List<string> CompanyNames { get; set; }
    }
    class TableOfNames
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
                    "Fools Studio"
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

                }
            },
            new FieldNameInfo()
            {
                FieldName = "Construction Sector",
                CompanyNames = new List<string>()
                {
                    "Jelle's Boubedriuw B.V.",
                    "Bob de Douwer GMBH",
                    
                }
            },

        };
    }
}
