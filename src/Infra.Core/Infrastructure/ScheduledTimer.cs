﻿using Nito.AsyncEx;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Borg.Infra
{
    public class ScheduledTimer : IDisposable
    {
        private DateTime _next = DateTime.MaxValue;
        private DateTime _last = DateTime.MinValue;
        private readonly Timer _timer;
        private readonly Func<Task<DateTime?>> _timerCallback;
        private readonly TimeSpan _minimumInterval;
        private readonly AsyncLock _lock = new AsyncLock();
        private bool _isRunning = false;
        private bool _shouldRunAgainImmediately = false;


        private readonly ILogger _logger;

        public ScheduledTimer(Func<Task<DateTime?>> timerCallback, ILoggerFactory loggerFactory, TimeSpan? dueTime = null, TimeSpan? minimumIntervalTime = null)
        {
            if (timerCallback == null)
                throw new ArgumentNullException(nameof(timerCallback));

            _logger = loggerFactory.CreateLogger(GetType());

            _timerCallback = timerCallback;

            _minimumInterval = minimumIntervalTime ?? TimeSpan.Zero;

            int dueTimeMs = dueTime.HasValue ? (int)dueTime.Value.TotalMilliseconds : Timeout.Infinite;
            _timer = new Timer(s => RunCallbackAsync().GetAwaiter().GetResult(), null, dueTimeMs, Timeout.Infinite);
        }

        public void ScheduleNext(DateTime? utcDate = null)
        {
            var utcNow = DateTime.UtcNow;
            if (!utcDate.HasValue || utcDate.Value < utcNow)
                utcDate = utcNow;

            _logger.LogDebug($"ScheduleNext called: value={utcDate.Value:O}");

            if (utcDate == DateTime.MaxValue)
            {
                  _logger.LogDebug("Ignoring MaxValue");
                return;
            }

            using (_lock.Lock())
            {
                // already have an earlier scheduled time
                if (_next > utcNow && utcDate > _next)
                {
                       _logger.LogDebug($"Ignoring because already scheduled for earlier time {utcDate.Value.Ticks} {_next.Ticks}");
                    return;
                }

                // ignore duplicate times
                if (_next == utcDate)
                {
                      _logger.LogDebug("Ignoring because already scheduled for same time");
                    return;
                }

                int delay = Math.Max((int)Math.Ceiling(utcDate.Value.Subtract(utcNow).TotalMilliseconds), 0);
                _next = utcDate.Value;
                if (_last == DateTime.MinValue)
                    _last = _next;

                _logger.LogDebug($"Scheduling next: delay={delay}");

                _timer.Change(delay, Timeout.Infinite);
            }
        }

        private async Task RunCallbackAsync()
        {
            if (_isRunning)
            {
                _logger.LogDebug("Exiting run callback because its already running, will run again immediately.");
                _shouldRunAgainImmediately = true;
                return;
            }

            _logger.LogDebug("RunCallbackAsync");

            using (await _lock.LockAsync())
            {
                if (_isRunning)
                {
                          _logger.LogDebug("Exiting run callback because its already running, will run again immediately.");
                    _shouldRunAgainImmediately = true;
                    return;
                }

                _last = DateTime.UtcNow;
            }

            try
            {
                _isRunning = true;
                DateTime? next = null;

                try
                {
                    next = await _timerCallback().AnyContext();
                }
                catch (Exception ex)
                {
                     _logger.LogError("Error running scheduled timer callback: {Message} \n{@Exception}", ex.Message, ex);
                    _shouldRunAgainImmediately = true;
                }

                if (_minimumInterval > TimeSpan.Zero)
                {
                    _logger.LogDebug("Sleeping for minimum interval: {interval}", _minimumInterval);
                    await Task.Delay(_minimumInterval).AnyContext();//DateTime.SleepAsync(_minimumInterval).AnyContext();
                    _logger.LogDebug("Finished sleeping");
                }

                var nextRun = DateTime.UtcNow.AddMilliseconds(10);
                if (_shouldRunAgainImmediately || next.HasValue && next.Value <= nextRun)
                    ScheduleNext(nextRun);
                else if (next.HasValue)
                    ScheduleNext(next.Value);
            }
            catch (Exception ex)
            {
               _logger.LogError( "Error running schedule next callback: {Message} \n {@Exception}", ex.Message, ex);
            }
            finally
            {
                _isRunning = false;
                _shouldRunAgainImmediately = false;
            }
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}