namespace Borg.Framework.Services
{
    internal interface ISlugifierService : IFrameworkService
    {
        string Slugify(string source, int maxlength = 42);
    }
}