namespace API.Helpers
{
    public class PaginationHeader
    {
        public PaginationHeader(int currentPage, int itemPerPage, int totalItem, int totalPage)
        {
            CurrentPage = currentPage;
            ItemsPerPage = itemPerPage;
            TotalItems = totalItem;
            TotalPage = totalPage;
        }

        public int CurrentPage { get; set; }
        public int ItemsPerPage { get; set; }
        public int TotalItems { get; set; }
        public int TotalPage { get; set; }

        
    }
}