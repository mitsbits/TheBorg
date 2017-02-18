using Borg.Infra.DTO;

namespace Borg.Framework.MVC.BuildingBlocks.Navigation
{
    public interface INavigationGroup : IWeighted
    {
        string Display { get; }
        string Key { get; }
        string Description { get; }
    }

    public interface INavigationItem : IWeighted
    {
        
    }
}