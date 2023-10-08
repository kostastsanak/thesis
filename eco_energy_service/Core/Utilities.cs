using eco_lib_energy;

namespace eco_energy_services.Core
{
    public static class Utilities
    {
        public static bool isObjPropertySame(System.Type typeSource, System.Type typeTarget)
        {
            var nullableTypeSource = Nullable.GetUnderlyingType(typeSource);
            typeSource = nullableTypeSource ?? typeSource;

            var nullableTypeTarget = Nullable.GetUnderlyingType(typeTarget);
            typeTarget = nullableTypeTarget ?? typeTarget;

            if (typeSource == typeTarget)
            {
                return true;
            }
            return false;
        }

        public static Api_Pagging CalculatePagging(int? currentPage, int totalEntries, int? entriesPerPage, int defaultEntriesPerPage, bool? noPagging = false)
        {
            var pagging = new Api_Pagging();
            try
            {
                entriesPerPage = entriesPerPage > 0 && entriesPerPage < defaultEntriesPerPage ? entriesPerPage : defaultEntriesPerPage;

                pagging.TotalEntries = totalEntries;

                pagging.EntriesPerPage = (int)(noPagging != null && (bool)noPagging ? totalEntries : entriesPerPage);

                pagging.EntriesStart = currentPage > 0 ? (int)(currentPage - 1) * pagging.EntriesPerPage : 0;

                pagging.EntriesStart = pagging.EntriesStart > pagging.TotalEntries ? pagging.TotalEntries - pagging.EntriesPerPage : pagging.EntriesStart;

                pagging.EntriesStart = noPagging != null && (bool)noPagging ? 0 : pagging.EntriesStart;

                pagging.CurrentPage = pagging.EntriesStart < pagging.EntriesPerPage ? 1 : pagging.EntriesStart / pagging.EntriesPerPage + 1;

                pagging.TotalPages = pagging.TotalEntries % pagging.EntriesPerPage != 0 ? pagging.TotalEntries / pagging.EntriesPerPage + 1 : pagging.TotalEntries / pagging.EntriesPerPage;

                pagging.TotalPages = pagging.TotalPages != 0 ? pagging.TotalPages : 1;
            }
            catch (Exception ex)
            {
                throw ex.InnerException ?? ex;
            }
            return pagging;
        }
        
        public static bool IsListSafe<T>(this List<T> list)
        {
            return list != null && list.Any();
        }
    }
}
