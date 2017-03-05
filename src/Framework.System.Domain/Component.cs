using Borg.Infra.CQRS;
using System;


namespace Framework.System.Domain
{
    public abstract class Component : IEntity<string>, IEntity<int>, IActivatable
    {
        protected Component()
        {
            CQRSKey = Guid.NewGuid().ToString();
        }

        protected Component(Guid cqrsKey)
        {
            CQRSKey = cqrsKey.ToString();
        }

        protected Component(int id) : this()
        {
            Id = id;
        }

        public abstract int Id { get; protected set; }

        public string CQRSKey { get; protected set; }
        string IEntity<string>.Id => CQRSKey;
        public bool Activated { get; protected set; }

        public virtual void SwitchOn()
        {
            if (!Activated ) Activated = true;
        }

        public virtual void SwitchOff()
        {
            if (Activated) Activated = false;
        }
    }

    public class Page : Component, IPageContent
    {
        public string Title { get; protected set; }


        public string Body { get; protected set; }


        protected Page() : base()
        {
        }

        public Page(int id) : base(id)
        {
        }

        public Page(int id, string title, string body) : this(id)
        {
            Title = title;
            Body = body;
        }

        public override int Id { get; protected set; }
    }


    public interface IActivatable
    {
        bool Activated { get; }

        void SwitchOn();

        void SwitchOff();
    }

    public interface IPageContent
    {
        string Title { get; }
        string Body { get; }
    }
}