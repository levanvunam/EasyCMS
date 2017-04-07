namespace EzCMS.Core.Models.RotatingImageGroups.Widgets
{
    public class RotatingImageWidget
    {
        public int Id { get; set; }

        public int Index { get; set; }

        public string Title { get; set; }

        public string ImageUrl { get; set; }

        public string Text { get; set; }

        public string Url { get; set; }

        public string UrlTarget { get; set; }

        public int? RecordOrder { get; set; }
    }
}