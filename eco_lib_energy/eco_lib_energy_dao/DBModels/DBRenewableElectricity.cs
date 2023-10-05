namespace eco_lib_energy_dao.DBModels
{
    public class DBRenewableElectricity
    {
        public long RenewableElectricityID { get; set; }
        public double Energy_mwh { get; set; }
        public DateTime Date { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public int Day { get; set; }
    }
}