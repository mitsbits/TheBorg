﻿using Borg.Infra;
using Microsoft.Extensions.Logging;

namespace Borg.Framework.System
{
    public interface ISystemService<out TSettings> : ILoggerFactory, ISerializer where TSettings : BorgSettings
    {
        TSettings Settings { get; }
    }
}