namespace Borg.Infra.DTO
{
    public class Tiding : Catalogued, IWeighted
    {
        private readonly Tidings _children;

        public Tiding()
        {
            _children = new Tidings();
        }

        public Tiding(string key, string value) : this()
        {
            Key = key;
            Value = value;
        }

        public virtual double Weight { get; set; }

        public virtual Tidings Children
        {
            get { return _children; }
        }
    }

    public interface IWeighted
    {
        double Weight { get; }
    }
}