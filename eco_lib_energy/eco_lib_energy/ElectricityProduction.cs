namespace eco_lib_energy
{
    public class ElectricityProduction
    {
        public class ApiEnergyPagging
        {
            public eco_lib_core.Api_Pagging Pagging { get; set; }
            public List<ElectricityProduction.Response> Productions { get; set; }
        }
        public class Response
        {
            public double Energy_mwh { get; set; }
            //public double Percentage { get; set; }
            public DateTime Date { get; set; }
            public string? Fuel { get; set; }
        }

        public class Search
        {
            public List<long>? FuelIDs { get; set; }
            public string? Fuel { get; set; }
            public int? Year { get; set; }
            public int? Month { get; set; }
            public int? Day { get; set; }
            public byte? GroupByFuel { get; set; }
            public byte? GroupByYear { get; set; }
            public byte? GroupByMonth { get; set; }
            public int? Limit { get; set; }
            public int? Page { get; set; }
            public int? NoPagging { get; set; }
        }

    }


}