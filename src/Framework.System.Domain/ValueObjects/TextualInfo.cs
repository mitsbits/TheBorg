using Borg.Infra.CQRS;

namespace Framework.System.Domain.ValueObjects
{
    public class TextualInfo : ValueObject<TextualInfo>
    {
        protected TextualInfo()
        {
        }

        protected internal TextualInfo(string title, string body)
        {
            Title = title;
            Body = body;
        }

        public string Title { get; protected set; }
        public string Body { get; protected set; }

        public TextualInfo NewTitle(string title)
        {
            return new TextualInfo(title, Body);
        }

        public TextualInfo NewBody(string body)
        {
            return new TextualInfo(Title, body);
        }

        public static TextualInfo Create(string title, string boby)
        {
            return new TextualInfo(title, boby);
        }
    }
}