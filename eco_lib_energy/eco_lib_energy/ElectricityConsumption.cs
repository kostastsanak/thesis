namespace eco_lib_energy
{
    public class ElectricityConsumption
    {
        public class ApiEnergyPagging
        {
            public eco_lib_core.Api_Pagging Pagging { get; set; }
            public List<ElectricityConsumption.Response> Consumptions { get; set; }
        }
        public class Response
        {
            public double Energy_mwh { get; set; }
            public string Area { get; set; }
            public DateTime Date { get; set; }
        }

        public class Search
        {
            public long? CityID { get; set; }
            public string? City { get; set; }
            public int? Year { get; set; }
            public int? Month { get; set; }
            public int? Day { get; set; }
            public int? GroupByYear { get; set; }
            public int? GroupByMonth { get; set; }
            public int? GroupByCity { get; set; }
            public int? Limit { get; set; }
            public int? Page { get; set; }
            public int? NoPagging { get; set; }
        }

    }


}