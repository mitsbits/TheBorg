using System;
using Borg.Framework.GateKeeping.Models;
using Borg.Infra;
using Borg.Infra.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Borg.Framework.System.Backoffice.UserSession
{
    public class UserSession : Tidings, IUserSession
    {
        private const string _cookieName = "Borg.UserSession";
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly HttpContext _context;
        private readonly UserManager<BorgUser> _userManager;
        private readonly ISerializer _serializer;

        public UserSession(IHttpContextAccessor httpContextAccessor, UserManager<BorgUser> userManager, ISerializer serializer)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = httpContextAccessor.HttpContext;
            _userManager = userManager;
            _serializer = serializer;
            ReadState();
            SaveState();
        }

        public DateTimeOffset SessionStart
        {
            get
            {
                if (!ContainsKey("SessionStartKey"))
                {
                    var tiding = new Tiding("SessionStartKey");
                    tiding.SetValue(DateTimeOffset.UtcNow, _serializer);
                    Add(tiding);
                    SaveState();
                }
                return RootByKey["SessionStartKey"].GetValue<DateTimeOffset>(_serializer);
            }
        }

        public string UserIdentifier => _context.User == null ? string.Empty : _userManager.GetUserId(_context.User);

        private void SaveState()
        {
            string data = AsyncHelpers.RunSync(() => _serializer.SerializeToStringAsync(this as Tidings));
            CookieOptions options = new CookieOptions { HttpOnly = true };
            _context.Response.Cookies.Append(_cookieName, data, options);
        }

        private void ReadState()
        {
            if (_context.Request.Cookies.ContainsKey(_cookieName))
            {
                var jsonData = _context.Request.Cookies[_cookieName];
                Tidings data = AsyncHelpers.RunSync(() => _serializer.DeserializeAsync<Tidings>(jsonData));
                Clear();
                foreach (var tiding in data)
                {
                    Add(tiding);
                }
            }
        }

        public T Setting<T>(string key, T value)
        {
            T setting = default(T);
            if (value != null /*&& !value.Equals(default(T))*/)
            {
                setting = value;
                if (ContainsKey(key))
                {
                    RootByKey[key].SetValue(setting, _serializer);
                }
                else
                {
                    var tiding = new Tiding(key);
                    tiding.SetValue(setting, _serializer);
                    Add(tiding);
                }
                SaveState();
            }
            else
            {
                if (ContainsKey(key))
                {
                    setting = RootByKey[key].GetValue<T>(_serializer);
                }
            }
            return setting;
        }

        public T Setting<T>(string key)
        {
            T setting = default(T);
            if (ContainsKey(key))
            {
                setting = RootByKey[key].GetValue<T>(_serializer);
            }
            return setting;
        }
    }
}