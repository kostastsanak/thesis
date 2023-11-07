namespace lib_energy_dao.DBModels
{
    public class DBElectricityConsumption
    {
        public long ElectricityConsumptionID { get; set; }
        public long? CityID { get; set; }
        public double Energy_mwh { get; set; }
        public string Area { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public int Day { get; set; }
        public DateTime Date { get; set; }
    }
}