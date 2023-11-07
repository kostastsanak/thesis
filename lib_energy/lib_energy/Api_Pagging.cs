namespace lib_energy
{
    public class Api_Pagging
    {
        public int CurrentPage { get; set; } = 0;
        public int TotalEntries { get; set; } = 0;
        public int EntriesPerPage { get; set; } = 0;
        public int TotalPages { get; set; } = 0;
        public int EntriesStart { get; set; } = 0;
    }
}
