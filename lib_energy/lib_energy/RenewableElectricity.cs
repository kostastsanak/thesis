namespace lib_energy
{
    public class RenewableElectricity
    {
        public class ApiEnergyPagging
        {
            public Api_Pagging Pagging { get; set; }
            public List<RenewableElectricity.Response> RenewableElectricity { get; set; }
        }
        public class Response
        {
            public double Energy_mwh { get; set; }
            public DateTime Date { get; set; }
        }

        public class Search
        {
            public int? Year { get; set; }
            public int? Month { get; set; }
            public int? Day { get; set; }
            public byte? GroupByYear { get; set; }
            public byte? GroupByMonth { get; set; }
            public int? Limit { get; set; }
            public int? Page { get; set; }
            public int? NoPagging { get; set; }
        }

    }


}