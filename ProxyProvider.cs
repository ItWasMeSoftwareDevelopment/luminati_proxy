using System;
using System.Collections.Generic;
using System.Linq; 
using NLog;

namespace ItWasMe.LuminatiProxy
{
    public interface IProxyProvider
    {
        LuminatiProxyModel CurrentProxy { get; }
        LuminatiProxyModel GetProxy();
        List<LuminatiProxyModel> GetAvailableProxyList();
        void SetProxyUnavailable(LuminatiProxyModel proxy, string reason);
    }

    public class ProxyProvider : IProxyProvider
    {
        private readonly IProxyConfig _proxyConfig;
        private readonly List<LuminatiProxyModel> _proxyList;
        private readonly ILogger _logger;
        private const int AverageDuration = 50;
        private LuminatiProxyModel _currentProxy;

        public ProxyProvider(IProxyConfig proxyConfig, ILoggingContext loggingContext)
        {
            _proxyConfig = proxyConfig;
            _proxyList = GenerateProxyList();
            _logger = loggingContext.CreateLogger<ProxyProvider>();
        }

        LuminatiProxyModel IProxyProvider.CurrentProxy => _currentProxy;

        LuminatiProxyModel IProxyProvider.GetProxy()
        {
            return GetProxy();
        }

        List<LuminatiProxyModel> IProxyProvider.GetAvailableProxyList()
        {
            return GetAvailableProxyList();
        }

        void IProxyProvider.SetProxyUnavailable(LuminatiProxyModel proxy, string reason)
        {
            var proxiesCount = _proxyList.Count;
            var seconds = proxiesCount * AverageDuration;
            proxy.Freeze(TimeSpan.FromSeconds(seconds));
            _logger.Info($"LuminatiProxyModel {proxy.CountryCode} will blocked till {proxy.FreezeTime}. Count of blocked:{GetUnAvailableProxyList().Count}({_proxyList.Count}). Reason: {reason}");
        }

        private List<LuminatiProxyModel> GetAvailableProxyList()
        {
            return _proxyList.Where(a => a.FreezeTime <= DateTime.UtcNow).ToList();
        }

        private List<LuminatiProxyModel> GetUnAvailableProxyList()
        {
            return _proxyList.Where(a => a.FreezeTime > DateTime.UtcNow).ToList();
        }

        private LuminatiProxyModel GetProxy()
        {
            var proxyList = GetAvailableProxyList();

            if (GetAvailableProxyList().Any())
            {
                var rand = new Random();
                _currentProxy = proxyList[rand.Next(proxyList.Count)];
                return _currentProxy;
            }

            return null;
        }

        private List<LuminatiProxyModel> GenerateProxyList()
        {
            var list = new List<LuminatiProxyModel>();
            foreach (var country in _proxyConfig.CountryCodes)
            {
                list.Add(new LuminatiProxyModel
                {
                    ProxyIp = _proxyConfig.Proxy.ProxyIp,
                    ProxyPort = _proxyConfig.Proxy.ProxyPort,
                    UserName = $"{_proxyConfig.Proxy.UserName}-country-{country}",
                    Password = _proxyConfig.Proxy.Password,
                    CountryCode = country
                });
            }

            return list;
        }
    }
}
