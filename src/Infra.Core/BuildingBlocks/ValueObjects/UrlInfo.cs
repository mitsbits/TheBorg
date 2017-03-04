using Borg.Infra.CQRS;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Borg.Infra.BuildingBlocks
{
    public class UrlInfo : ValueObject<UrlInfo>
    {
        private Uri _uri;
        private readonly string _domain;
        private string _source;

        protected UrlInfo()
        {
        }

        protected UrlInfo(Uri uri, string domain = "")
        {
            if (uri.HostNameType != UriHostNameType.Dns) throw new ArgumentOutOfRangeException(nameof(uri));
            _uri = uri;
            _source = _uri.ToString().ToLower();
            if (!string.IsNullOrWhiteSpace(domain))
            {
                domain = domain.TrimStart('.', '/', '\\').TrimEnd('.', '/', '\\').ToLower();
                if (!domain.Contains('.')) throw new ArgumentOutOfRangeException(nameof(domain));
                if (!_uri.Authority.EndsWith(domain, StringComparison.OrdinalIgnoreCase)) throw new ArgumentOutOfRangeException(nameof(domain));
                _domain = domain;
            }
        }

        public UrlInfo(string url, string domain = "") : this(new Uri(url), domain)
        {
        }

        public string Source
        {
            get
            {
                return _uri.ToString().ToLower();
            }
            protected set
            {
                _uri = new Uri(value);
                _source = _uri.ToString().ToLower();
            }
        }

        public string Domain
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(_domain)) return _domain.ToLower();
                var authority = _uri.Authority.ToLower();
                var split = authority.Split('.');
                if (split.Length > 2)
                    return split[split.Length - 2] + "." + split[split.Length - 1];
                else
                    return authority;
            }
        }

        public string SubDomain => _uri.HostNameType != UriHostNameType.Dns ? string.Empty : _uri.DnsSafeHost.GetSubdomain(Domain).ToLower();

        public string[] Segments
        {
            get
            {
                return
                    _uri.Segments.Select(x => x.TrimStart('/').TrimEnd('/'))
                        .Where(x => !string.IsNullOrWhiteSpace(x))
                        .ToArray();
            }
        }

        public IReadOnlyDictionary<string, string[]> Queries
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_uri.Query)) return new Dictionary<string, string[]>();
                var q = _uri.Query.TrimStart('?').Split('&');
                return q.Select(x => Tuple.Create(x.Split('=')[0], (x.Split('=').Length > 1) ? x.Split('=')[1] : string.Empty))
                      .GroupBy(t => t.Item1).Where(g => !string.IsNullOrWhiteSpace(g.Key))
                      .ToDictionary(x => x.Key, x => x.Where(v => !string.IsNullOrWhiteSpace(v.Item2)).Select(v => v.Item2).ToArray());
            }
        }

        public string Fragment => _uri.Fragment;

        public override string ToString()
        {
            return _uri.ToString();
        }
    }

    internal static class UrlInfoExtensions
    {
        public static string GetSubdomain(this string url, string domain = "")
        {
            var subdomain = url;
            if (subdomain != null)
            {
                if (string.IsNullOrWhiteSpace(domain))
                {
                    // Since we were not provided with a known domain, assume that second-to-last period divides the subdomain from the domain.
                    var nodes = url.Split('.');
                    var lastNodeIndex = nodes.Length - 1;
                    if (lastNodeIndex > 0)
                        domain = nodes[lastNodeIndex - 1] + "." + nodes[lastNodeIndex];
                }

                // Verify that what we think is the domain is truly the ending of the hostname... otherwise we're hooped.
                if (!subdomain.EndsWith(domain))
                    throw new ArgumentException("Site was not loaded from the expected domain");

                // Quash the domain portion, which should leave us with the subdomain and a trailing dot IF there is a subdomain.
                subdomain = subdomain.Replace(domain, "");
                // Check if we have anything left.  If we don't, there was no subdomain, the request was directly to the root domain:
                if (string.IsNullOrWhiteSpace(subdomain))
                    return string.Empty;

                // Quash any trailing periods
                subdomain = subdomain.TrimEnd('.');
            }

            return subdomain;
        }
    }
}