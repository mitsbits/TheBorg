namespace Borg.Infra.DTO
{
    public class Tiding : Catalogued, IWeighted
    {


        public Tiding(string key, string value = "") 
        {
            Key = key;
            Value = value;
        }

        public virtual double Weight { get; set; }

        public virtual Tidings Children { get; } = new Tidings();
    }

    public interface IWeighted
    {
        double Weight { get; }
    }
}