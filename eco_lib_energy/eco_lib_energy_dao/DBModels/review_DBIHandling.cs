namespace eco_lib_energy_dao.DBModels
{
    interface energy_DBIHandling
    {
        #region ElectricityConsumption
        public DBElectricityConsumption AddDBElectricityConsumption(DBElectricityConsumption dbElectricityConsumption);
        public List<DBElectricityConsumption> AddDBElectricityConsumptions(List<DBElectricityConsumption> dbElectricityConsumptions);
        public IQueryable<DBElectricityConsumption> GetDBElectricityConsumptions(string query = "", bool trackable = false);
        public DBElectricityConsumption UpdateDBElectricityConsumption(DBElectricityConsumption dbElectricityConsumption);
        public List<DBElectricityConsumption> UpdateDBElectricityConsumptions(List<DBElectricityConsumption> dbElectricityConsumptions);

        public DBElectricityConsumption DeleteDBElectricityConsumption(DBElectricityConsumption dbElectricityConsumption);
        public List<DBElectricityConsumption> DeleteDBElectricityConsumptions(List<DBElectricityConsumption> dbElectricityConsumptions);
        #endregion

        #region City
        public DBCity AddDBCity(DBCity DBCity);
        public List<DBCity> AddDBCities(List<DBCity> DBCities);
        public IQueryable<DBCity> GetDBCities(string query = "", bool trackable = false);
        public DBCity UpdateDBCity(DBCity DBCity);
        public List<DBCity> UpdateDBCities(List<DBCity> DBCities);

        public DBCity DeleteDBCity(DBCity DBCity);
        public List<DBCity> DeleteDBCities(List<DBCity> DBCities);
        #endregion

        #region RenewableElectricity
        public DBRenewableElectricity AddDBRenewableElectricity(DBRenewableElectricity dbRenewableElectricity);
        public List<DBRenewableElectricity> AddDBRenewableElectricities(List<DBRenewableElectricity> dbRenewableElectricities);
        public IQueryable<DBRenewableElectricity> GetDBRenewableElectricities(string query = "", bool trackable = false);
        public DBRenewableElectricity UpdateDBRenewableElectricity(DBRenewableElectricity dbRenewableElectricity);
        public List<DBRenewableElectricity> UpdateDBRenewableElectricities(List<DBRenewableElectricity> dbRenewableElectricities);

        public DBRenewableElectricity DeleteDBRenewableElectricity(DBRenewableElectricity dbRenewableElectricity);
        public List<DBRenewableElectricity> DeleteDBRenewableElectricities(List<DBRenewableElectricity> dbRenewableElectricities);
        #endregion

        #region ElectricityProduction
        public DBElectricityProduction AddDBElectricityProduction(DBElectricityProduction dbElectricityProduction);
        public List<DBElectricityProduction> AddDBElectricityProductions(List<DBElectricityProduction> dbElectricityProductions);
        public IQueryable<DBElectricityProduction> GetDBElectricityProductions(string query = "", bool trackable = false);
        public DBElectricityProduction UpdateDBElectricityProduction(DBElectricityProduction dbElectricityProduction);
        public List<DBElectricityProduction> UpdateDBElectricityProductions(List<DBElectricityProduction> dbElectricityProductions);

        public DBElectricityProduction DeleteDBElectricityProduction(DBElectricityProduction dbElectricityProduction);
        public List<DBElectricityProduction> DeleteDBElectricityProductions(List<DBElectricityProduction> dbElectricityProductions);
        #endregion

        #region Fuel
        public DBFuel AddDBFuel(DBFuel DBFuel);
        public List<DBFuel> AddDBFuels(List<DBFuel> DBFuels);
        public IQueryable<DBFuel> GetDBFuels(string query = "", bool trackable = false);
        public DBFuel UpdateDBFuel(DBFuel DBFuel);
        public List<DBFuel> UpdateDBFuels(List<DBFuel> DBFuels);

        public DBFuel DeleteDBFuel(DBFuel DBFuel);
        public List<DBFuel> DeleteDBFuels(List<DBFuel> DBFuels);
        #endregion

        public int CommitToDb();

    }
}
