namespace EzCMS.Core.Models.Shared
{
    public class PagingModel
    {
        /// <summary>
        /// Index, start from 0
        /// </summary>
        public int PageIndex { get; set; }

        public int NextPage
        {
            get { return PageIndex < TotalPage ? PageIndex + 1 : TotalPage; }
        }

        public int PreviousPage
        {
            get { return PageIndex - 1 >= 0 ? PageIndex - 1 : 0; }
        }

        public int PageSize { get; set; }

        public int TotalPage { get; set; }
    }
}
