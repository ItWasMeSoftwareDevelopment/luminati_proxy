﻿using System.Collections.Generic; 

namespace DominatorHouse.LuminatiProxy
{

    public interface IProxyConfig
    {
        ProxyModel Proxy { get; }
        List<string> CountryCodes { get; }
    }

    public class ProxyConfig : IProxyConfig
    {
        public ProxyModel Proxy { get; set;  }
        public List<string> CountryCodes { get; set; }
    }
}
