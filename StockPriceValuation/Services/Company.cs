using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace StockPriceValuation.Services
{
    public class Company
    {
        public string Name { get; set; }
        public Industry Industry { get; set; }
        public Stock Stock { get; set; }

        public static Industry GetIndustry(string gicsIndustryGroup)
        {
            var industry = Industry.None;

            switch (gicsIndustryGroup)
            {
                case "Automobiles & Components":
                    industry = Industry.AutomobilesAndComponents;
                    break;
                case "Banks":
                    industry = Industry.Banks;
                    break;
                case "Capital Goods":
                    industry = Industry.CapitalGoods;
                    break;
                case "Class Pend":
                    industry = Industry.ClassPend;
                    break;
                case "Commercial & Professional Services":
                    industry = Industry.CommercialAndProfessionalServices;
                    break;
                case "Consumer Durables & Apparel":
                    industry = Industry.ConsumerDurablesAndApparel;
                    break;
                case "Consumer Services":
                    industry = Industry.ConsumerServices;
                    break;
                case "Diversified Financials":
                    industry = Industry.DiversifiedFinancials;
                    break;
                case "Energy":
                    industry = Industry.Energy;
                    break;
                case "Food & Staples Retailing":
                    industry = Industry.FoodAndStaplesRetailing;
                    break;
                case "Food, Beverage & Tobacco":
                    industry = Industry.Food_BeverageAndTobacco;
                    break;
                case "GICS industry group":
                    industry = Industry.GICSindustrygroup;
                    break;
                case "Health Care Equipment & Services":
                    industry = Industry.HealthCareEquipmentAndServices;
                    break;
                case "Household & Personal Products":
                    industry = Industry.HouseholdAndPersonalProducts;
                    break;
                case "Insurance":
                    industry = Industry.Insurance;
                    break;
                case "Materials":
                    industry = Industry.Materials;
                    break;
                case "Media":
                    industry = Industry.Media;
                    break;
                case "Not Applic":
                    industry = Industry.NotApplic;
                    break;
                case "Pharmaceuticals, Biotechnology & Life Sciences":
                    industry = Industry.Pharmaceuticals_BiotechnologyAndLifeSciences;
                    break;
                case "Real Estate":
                    industry = Industry.RealEstate;
                    break;
                case "Retailing":
                    industry = Industry.Retailing;
                    break;
                case "Semiconductors & Semiconductor Equipment":
                    industry = Industry.SemiconductorsAndSemiconductorEquipment;
                    break;
                case "Software & Services":
                    industry = Industry.SoftwareAndServices;
                    break;
                case "Technology Hardware & Equipment":
                    industry = Industry.TechnologyHardwareAndEquipment;
                    break;
                case "Telecommunication Services":
                    industry = Industry.TelecommunicationServices;
                    break;
                case "Transportation":
                    industry = Industry.Transportation;
                    break;
                case "Utilities":
                    industry = Industry.Utilities;
                    break;
                default:
                    industry = Industry.None;
                    break;
            }

            return industry;
        }
    }

    public enum Industry
    {
        None,
        AutomobilesAndComponents,
        Banks,
        CapitalGoods,
        ClassPend,
        CommercialAndProfessionalServices,
        ConsumerDurablesAndApparel,
        ConsumerServices,
        DiversifiedFinancials,
        Energy,
        FoodAndStaplesRetailing,
        Food_BeverageAndTobacco,
        GICSindustrygroup,
        HealthCareEquipmentAndServices,
        HouseholdAndPersonalProducts,
        Insurance,
        Materials,
        Media,
        NotApplic,
        Pharmaceuticals_BiotechnologyAndLifeSciences,
        RealEstate,
        Retailing,
        SemiconductorsAndSemiconductorEquipment,
        SoftwareAndServices,
        TechnologyHardwareAndEquipment,
        TelecommunicationServices,
        Transportation,
        Utilities
    }
}
