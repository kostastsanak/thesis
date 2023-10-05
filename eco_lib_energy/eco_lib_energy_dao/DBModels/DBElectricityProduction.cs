namespace eco_lib_energy_dao.DBModels
{
    public class DBElectricityProduction
    {
        public long ElectricityProductionID { get; set; }
        public long? FuelID { get; set; }
        public double Energy_mwh { get; set; }
        public double Percentage { get; set; }
        public string Fuel { get; set; }
        public DateTime Date { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public int Day { get; set; }
    }
}