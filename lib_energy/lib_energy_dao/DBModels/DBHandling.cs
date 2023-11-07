using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace lib_energy_dao.DBModels
{
    public class energy_DBHandling : energy_DBIHandling
    {
        private readonly Energy_DBContext _context;

        public energy_DBHandling(IConfiguration configuration)
        {
            var optionsBuilder = new DbContextOptionsBuilder<Energy_DBContext>();
            optionsBuilder.UseMySQL(configuration.GetConnectionString("DBContext"));
            _context = new Energy_DBContext(optionsBuilder.Options);
        }

        #region ElectricityConsumption
        public DBElectricityConsumption AddDBElectricityConsumption(DBElectricityConsumption dbElectricityConsumption)
        {
            try
            {
                _context.DBElectricityConsumption.Add(dbElectricityConsumption);
            }
            catch (Exception ex)
            {
                throw ex.InnerException ?? ex;
            }

            return dbElectricityConsumption;
        }

        public List<DBElectricityConsumption> AddDBElectricityConsumptions(List<DBElectricityConsumption> dbElectricityConsumptions)
        {
            try
            {
                _context.DBElectricityConsumption.AddRange(dbElectricityConsumptions);
            }
            catch (Exception ex)
            {
                throw ex.InnerException ?? ex;
            }

            return dbElectricityConsumptions;
        }

        public IQueryable<DBElectricityConsumption> GetDBElectricityConsumptions(string query = "", bool trackable = false)
        {
            IQueryable<DBElectricityConsumption> result;

            try
            {
                query = string.IsNullOrEmpty(query) ? "1=1" : query;
                result = trackable ? _context.DBElectricityConsumption.Where(query) : _context.DBElectricityConsumption.Where(query).AsNoTracking();
            }
            catch (Exception ex)
            {
                throw ex.InnerException ?? ex;
            }

            return result;
        }

        public DBElectricityConsumption UpdateDBElectricityConsumption(DBElectricityConsumption dbElectricityConsumption)
        {
            try
            {
                _context.DBElectricityConsumption.Update(dbElectricityConsumption);
            }
            catch (Exception ex)
            {
                throw ex.InnerException ?? ex;
            }
            return dbElectricityConsumption;
        }

        public List<DBElectricityConsumption> UpdateDBElectricityConsumptions(List<DBElectricityConsumption> dbElectricityConsumptions)
        {
            try
            {
                _context.DBElectricityConsumption.UpdateRange(dbElectricityConsumptions);
            }
            catch (Exception ex)
            {
                throw ex.InnerException ?? ex;
            }
            return dbElectricityConsumptions;
        }

        public DBElectricityConsumption DeleteDBElectricityConsumption(DBElectricityConsumption dbElectricityConsumption)
        {
            try
            {
                _context.DBElectricityConsumption.Remove(dbElectricityConsumption);
            }
            catch (Exception ex)
            {
                throw ex.InnerException ?? ex;
            }
            return dbElectricityConsumption;
        }

        public List<DBElectricityConsumption> DeleteDBElectricityConsumptions(List<DBElectricityConsumption> dbElectricityConsumptions)
        {
            try
            {
                _context.DBElectricityConsumption.RemoveRange(dbElectricityConsumptions);
            }
            catch (Exception ex)
            {
                throw ex.InnerException ?? ex;
            }
            return dbElectricityConsumptions;
        }

        
        #endregion

        #region City
        public DBCity AddDBCity(DBCity DBCity)
        {
            try
            {
                _context.DBCity.Add(DBCity);
            }
            catch (Exception ex)
            {
                throw ex.InnerException ?? ex;
            }

            return DBCity;
        }

        public List<DBCity> AddDBCities(List<DBCity> DBCities)
        {
            try
            {
                _context.DBCity.AddRange(DBCities);
            }
            catch (Exception ex)
            {
                throw ex.InnerException ?? ex;
            }

            return DBCities;
        }

        public IQueryable<DBCity> GetDBCities(string query = "", bool trackable = false)
        {
            IQueryable<DBCity> result;

            try
            {
                query = string.IsNullOrEmpty(query) ? "1=1" : query;
                result = trackable ? _context.DBCity.Where(query) : _context.DBCity.Where(query).AsNoTracking();
            }
            catch (Exception ex)
            {
                throw ex.InnerException ?? ex;
            }

            return result;
        }

        public DBCity UpdateDBCity(DBCity DBCity)
        {
            try
            {
                _context.DBCity.Update(DBCity);
            }
            catch (Exception ex)
            {
                throw ex.InnerException ?? ex;
            }
            return DBCity;
        }

        public List<DBCity> UpdateDBCities(List<DBCity> DBCities)
        {
            try
            {
                _context.DBCity.UpdateRange(DBCities);
            }
            catch (Exception ex)
            {
                throw ex.InnerException ?? ex;
            }
            return DBCities;
        }

        public DBCity DeleteDBCity(DBCity DBCity)
        {
            try
            {
                _context.DBCity.Remove(DBCity);
            }
            catch (Exception ex)
            {
                throw ex.InnerException ?? ex;
            }
            return DBCity;
        }

        public List<DBCity> DeleteDBCities(List<DBCity> DBCities)
        {
            try
            {
                _context.DBCity.RemoveRange(DBCities);
            }
            catch (Exception ex)
            {
                throw ex.InnerException ?? ex;
            }
            return DBCities;
        }

        #endregion

        #region RenewableElectricity
        public DBRenewableElectricity AddDBRenewableElectricity(DBRenewableElectricity dbRenewableElectricity)
        {
            try
            {
                _context.DBRenewableElectricity.Add(dbRenewableElectricity);
            }
            catch (Exception ex)
            {
                throw ex.InnerException ?? ex;
            }

            return dbRenewableElectricity;
        }

        public List<DBRenewableElectricity> AddDBRenewableElectricities(List<DBRenewableElectricity> dbRenewableElectricities)
        {
            try
            {
                _context.DBRenewableElectricity.AddRange(dbRenewableElectricities);
            }
            catch (Exception ex)
            {
                throw ex.InnerException ?? ex;
            }

            return dbRenewableElectricities;
        }

        public IQueryable<DBRenewableElectricity> GetDBRenewableElectricities(string query = "", bool trackable = false)
        {
            IQueryable<DBRenewableElectricity> result;

            try
            {
                query = string.IsNullOrEmpty(query) ? "1=1" : query;
                result = trackable ? _context.DBRenewableElectricity.Where(query) : _context.DBRenewableElectricity.Where(query).AsNoTracking();
            }
            catch (Exception ex)
            {
                throw ex.InnerException ?? ex;
            }

            return result;
        }

        public DBRenewableElectricity UpdateDBRenewableElectricity(DBRenewableElectricity dbRenewableElectricity)
        {
            try
            {
                _context.DBRenewableElectricity.Update(dbRenewableElectricity);
            }
            catch (Exception ex)
            {
                throw ex.InnerException ?? ex;
            }
            return dbRenewableElectricity;
        }

        public List<DBRenewableElectricity> UpdateDBRenewableElectricities(List<DBRenewableElectricity> dbRenewableElectricities)
        {
            try
            {
                _context.DBRenewableElectricity.UpdateRange(dbRenewableElectricities);
            }
            catch (Exception ex)
            {
                throw ex.InnerException ?? ex;
            }
            return dbRenewableElectricities;
        }

        public DBRenewableElectricity DeleteDBRenewableElectricity(DBRenewableElectricity dbRenewableElectricity)
        {
            try
            {
                _context.DBRenewableElectricity.Remove(dbRenewableElectricity);
            }
            catch (Exception ex)
            {
                throw ex.InnerException ?? ex;
            }
            return dbRenewableElectricity;
        }

        public List<DBRenewableElectricity> DeleteDBRenewableElectricities(List<DBRenewableElectricity> dbRenewableElectricities)
        {
            try
            {
                _context.DBRenewableElectricity.RemoveRange(dbRenewableElectricities);
            }
            catch (Exception ex)
            {
                throw ex.InnerException ?? ex;
            }
            return dbRenewableElectricities;
        }


        #endregion


        #region ElectricityProduction
        public DBElectricityProduction AddDBElectricityProduction(DBElectricityProduction dbElectricityProduction)
        {
            try
            {
                _context.DBElectricityProduction.Add(dbElectricityProduction);
            }
            catch (Exception ex)
            {
                throw ex.InnerException ?? ex;
            }

            return dbElectricityProduction;
        }

        public List<DBElectricityProduction> AddDBElectricityProductions(List<DBElectricityProduction> dbElectricityProductions)
        {
            try
            {
                _context.DBElectricityProduction.AddRange(dbElectricityProductions);
            }
            catch (Exception ex)
            {
                throw ex.InnerException ?? ex;
            }

            return dbElectricityProductions;
        }

        public IQueryable<DBElectricityProduction> GetDBElectricityProductions(string query = "", bool trackable = false)
        {
            IQueryable<DBElectricityProduction> result;

            try
            {
                query = string.IsNullOrEmpty(query) ? "1=1" : query;
                result = trackable ? _context.DBElectricityProduction.Where(query) : _context.DBElectricityProduction.Where(query).AsNoTracking();
            }
            catch (Exception ex)
            {
                throw ex.InnerException ?? ex;
            }

            return result;
        }

        public DBElectricityProduction UpdateDBElectricityProduction(DBElectricityProduction dbElectricityProduction)
        {
            try
            {
                _context.DBElectricityProduction.Update(dbElectricityProduction);
            }
            catch (Exception ex)
            {
                throw ex.InnerException ?? ex;
            }
            return dbElectricityProduction;
        }

        public List<DBElectricityProduction> UpdateDBElectricityProductions(List<DBElectricityProduction> dbElectricityProductions)
        {
            try
            {
                _context.DBElectricityProduction.UpdateRange(dbElectricityProductions);
            }
            catch (Exception ex)
            {
                throw ex.InnerException ?? ex;
            }
            return dbElectricityProductions;
        }

        public DBElectricityProduction DeleteDBElectricityProduction(DBElectricityProduction dbElectricityProduction)
        {
            try
            {
                _context.DBElectricityProduction.Remove(dbElectricityProduction);
            }
            catch (Exception ex)
            {
                throw ex.InnerException ?? ex;
            }
            return dbElectricityProduction;
        }

        public List<DBElectricityProduction> DeleteDBElectricityProductions(List<DBElectricityProduction> dbElectricityProductions)
        {
            try
            {
                _context.DBElectricityProduction.RemoveRange(dbElectricityProductions);
            }
            catch (Exception ex)
            {
                throw ex.InnerException ?? ex;
            }
            return dbElectricityProductions;
        }


        #endregion

        #region Fuel
        public DBFuel AddDBFuel(DBFuel DBFuel)
        {
            try
            {
                _context.DBFuel.Add(DBFuel);
            }
            catch (Exception ex)
            {
                throw ex.InnerException ?? ex;
            }

            return DBFuel;
        }

        public List<DBFuel> AddDBFuels(List<DBFuel> DBFuels)
        {
            try
            {
                _context.DBFuel.AddRange(DBFuels);
            }
            catch (Exception ex)
            {
                throw ex.InnerException ?? ex;
            }

            return DBFuels;
        }

        public IQueryable<DBFuel> GetDBFuels(string query = "", bool trackable = false)
        {
            IQueryable<DBFuel> result;

            try
            {
                query = string.IsNullOrEmpty(query) ? "1=1" : query;
                result = trackable ? _context.DBFuel.Where(query) : _context.DBFuel.Where(query).AsNoTracking();
            }
            catch (Exception ex)
            {
                throw ex.InnerException ?? ex;
            }

            return result;
        }

        public DBFuel UpdateDBFuel(DBFuel DBFuel)
        {
            try
            {
                _context.DBFuel.Update(DBFuel);
            }
            catch (Exception ex)
            {
                throw ex.InnerException ?? ex;
            }
            return DBFuel;
        }

        public List<DBFuel> UpdateDBFuels(List<DBFuel> DBFuels)
        {
            try
            {
                _context.DBFuel.UpdateRange(DBFuels);
            }
            catch (Exception ex)
            {
                throw ex.InnerException ?? ex;
            }
            return DBFuels;
        }

        public DBFuel DeleteDBFuel(DBFuel DBFuel)
        {
            try
            {
                _context.DBFuel.Remove(DBFuel);
            }
            catch (Exception ex)
            {
                throw ex.InnerException ?? ex;
            }
            return DBFuel;
        }

        public List<DBFuel> DeleteDBFuels(List<DBFuel> DBFuels)
        {
            try
            {
                _context.DBFuel.RemoveRange(DBFuels);
            }
            catch (Exception ex)
            {
                throw ex.InnerException ?? ex;
            }
            return DBFuels;
        }

        #endregion

        public int CommitToDb()
        {
            int addedRows;
            try
            {
                addedRows = _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw (ex ?? ex.InnerException)!;
            }
            return addedRows;
        }


    }
}
