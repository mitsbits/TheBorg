namespace Borg.Framework.MVC.BuildingBlocks.Devices
{
    public interface IPageContent
    {
        string Title { get; }
        string Subtitle { get; }
        string[] Body { get; }
    }

    public class PageContent : IPageContent
    {
        public string Title { get; set; }
        public string Subtitle { get; set; }

        public string[] Body { get; set; }
    }

    public interface IPageContentAccessor<out TPage> where TPage : IPageContent
    {
        TPage Page { get; }
    }


}