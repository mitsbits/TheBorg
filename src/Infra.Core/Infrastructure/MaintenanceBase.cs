
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Borg.Infra
{
    public class MaintenanceBase : IDisposable
    {
        private ScheduledTimer _maintenanceTimer;
        private readonly ILoggerFactory _loggerFactory;



        public MaintenanceBase(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;

        }

        protected void InitializeMaintenance(TimeSpan? dueTime = null, TimeSpan? intervalTime = null)
        {
            _maintenanceTimer = new ScheduledTimer(DoMaintenanceAsync, _loggerFactory, dueTime, intervalTime);
        }

        protected void ScheduleNextMaintenance(DateTime utcDate)
        {
            _maintenanceTimer.ScheduleNext(utcDate);
        }

        protected virtual Task<DateTime?> DoMaintenanceAsync()
        {
            return Task.FromResult<DateTime?>(DateTime.MaxValue);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _maintenanceTimer?.Dispose();
            }
        }
    }
}