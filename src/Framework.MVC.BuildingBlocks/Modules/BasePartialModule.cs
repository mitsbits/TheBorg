using Borg.Infra.Core.ExtensionMethods;
using Borg.Infra.DTO;
using System.Collections.Generic;

namespace Borg.Framework.MVC.BuildingBlocks.Modules
{
    public abstract class BasePartialModule : IModule<Tidings>
    {
        protected BasePartialModule(string friendlyName)
            : this()
        {
            FriendlyName = friendlyName;
        }

        protected BasePartialModule(string friendlyName, IDictionary<string, string> parameters)
            : this(friendlyName)
        {
            Parameters.AppendAndUpdate(parameters);
        }

        private BasePartialModule()
        {
            Parameters = new Tidings();
        }

        public abstract ModuleType ModuleType { get; }

        public string FriendlyName { get; }

        public Tidings Parameters { get; }

        protected object GetInternalValue(string keyPrefix, string key)
        {
            var dickey = $"{keyPrefix}{key}";
            return Parameters.ContainsKey(dickey) ? Parameters[dickey] : string.Empty;
        }

        protected void SetInternalValue(string keyPrefix, string key, string value)
        {
            var dickey = $"{keyPrefix}{key}";
            if (!Parameters.ContainsKey(dickey))
            {
                Parameters.Add(dickey, value);
            }
            else
            {
                Parameters[dickey] = value;
            }
        }
    }
}

namespace Borg
{
}