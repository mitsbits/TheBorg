using Borg.Infra.DTO;

namespace Borg.Infra.Postal
{
    public class SuccessEmailAccountResponse : EmailAccountResponse
    {
        public SuccessEmailAccountResponse(string id, string description = "") : base(id, true, description)
        {
        }
    }

    public class FailedEmailAccountResponse : EmailAccountResponse
    {
        public FailedEmailAccountResponse(string id, string description = "") : base(id, false, description)
        {
        }
    }

    public class EmailAccountResponse : Tidings
    {
        internal EmailAccountResponse(string id, bool success = true, string description = "")
        {
            MessageId = id;
            Success = success;
            Description = description;
            Type = GetType().FullName;
        }

        public string MessageId
        {
            get { return this[DefinedKeys.Id] ?? string.Empty; }
            private set { this[DefinedKeys.Id] = value; }
        }

        public bool Success
        {
            get { return RootByKey[DefinedKeys.Id].GetValue<bool>(); }
            private set { RootByKey[DefinedKeys.Id].SetValue<bool>(value); }
        }

        public string Description
        {
            get { return this[DefinedKeys.Display] ?? string.Empty; }
            private set { this[DefinedKeys.Display] = value; }
        }

        public string Type
        {
            get { return this[DefinedKeys.Identifier] ?? string.Empty; }
            private set { this[DefinedKeys.Identifier] = value; }
        }
    }
}