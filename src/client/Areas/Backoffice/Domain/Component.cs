using Borg.Infra.CQRS;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Framework.System.Domain
{

    public abstract  class Component : IEntity<string>, IActivatable
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

        public int Id { get; protected set; }

        public string CQRSKey { get; protected set; }
        [NotMapped]
        string IEntity<string>.Id => CQRSKey;
        public ActivationStatus ActivationStatus { get; protected set; }

        public virtual void SwitchOn()
        {
            if (ActivationStatus == ActivationStatus.Off) ActivationStatus = ActivationStatus.On;
        }

        public virtual void SwitchOff()
        {
            if (ActivationStatus == ActivationStatus.On) ActivationStatus = ActivationStatus.Off;
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


    }
    public enum ActivationStatus
    {
        Off = -1,
        On = 1
    }

    public interface IActivatable
    {
        ActivationStatus ActivationStatus { get; }

        void SwitchOn();

        void SwitchOff();
    }

    public interface IPageContent
    {
        string Title { get; }
        string Body { get; }
    }
}