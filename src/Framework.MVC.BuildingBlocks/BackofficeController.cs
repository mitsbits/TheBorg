using System;
using System.Linq;
using Borg.Framework.MVC.BuildingBlocks.Interactions;
using Borg.Framework.System;
using Borg.Infra.Messaging;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace Borg.Framework.MVC
{
    public abstract class BackofficeController : FrameworkController
    {
        protected BackofficeController(IBackofficeService<BorgSettings> systemService) : base(systemService)
        {
        }

        protected IBackofficeService<BorgSettings> Backoffice => System as IBackofficeService<BorgSettings>;

        #region Pager

        private RequestPager _pager;

        protected RequestPager Pager
        {
            get
            {
                string pageNumerVariableName = System.Settings.Backoffice.Pager.PageVariable;
                string rowCountVariableName = System.Settings.Backoffice.Pager.RowsVariable;

                if (_pager != null) return _pager;

                var p = 1;

                if (!string.IsNullOrWhiteSpace(Request.Query[pageNumerVariableName]))
                    int.TryParse(Request.Query[pageNumerVariableName], out p);

                var r = 10;

                if (!string.IsNullOrWhiteSpace(Request.Query[rowCountVariableName]))
                    int.TryParse(Request.Query[rowCountVariableName], out r);

                _pager = new RequestPager() { Current = p, RowCount = r };
                return _pager;
            }
        }

        protected class RequestPager
        {
            public int Current { get; internal set; }
            public int RowCount { get; internal set; }
        }

        #endregion Pager

        #region Redirect Messages

        protected void AddRedirectMessage(ResponseStatus status, string title, string message = "")
        {
            this.AddRedirectMessages(System, new[] { new ServerResponse(status, title, message) });
        }

        protected void AddRedirectMessage(Exception exception)
        {
            Logger.LogError(null, exception, "exception");
            this.AddRedirectMessages(System, new[] { new ServerResponse(exception) });
        }

        protected void AddRedirectMessage(ModelStateDictionary state)
        {
            foreach (var stm in state.Where(stm => stm.Value.Errors != null && stm.Value.Errors.Count > 0))
            {
                this.AddRedirectMessages(System,
                    stm.Value.Errors.Select(
                        e => new ServerResponse(ResponseStatus.Error, e.ErrorMessage, e.Exception?.Message ?? string.Empty)).ToArray());
            }
        }

        #endregion Redirect Messages
    }
}