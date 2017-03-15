namespace Borg.Framework.System
{
    public abstract class BorgFeature : IBorgFeature
    {
        protected BorgFeature(string name)
        {
            Name = name;
        }

        public string Name { get; }
        public bool Enabled { get; } = true;
    }
}