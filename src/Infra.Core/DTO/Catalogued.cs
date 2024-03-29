﻿namespace Borg.Infra.DTO
{
    public abstract class Catalogued : ICatalogued
    {
        public virtual string Key { get; set; }
        public virtual string HumanKey { get; set; }
        public virtual string Value { get; set; }
        public virtual string Hint { get; set; }
        public virtual string Flag { get; set; }

        public override string ToString()
        {
            return $"{Key}|{Value}" + (string.IsNullOrWhiteSpace(Hint) ? string.Empty : $"|{Hint}");
        }
    }
}