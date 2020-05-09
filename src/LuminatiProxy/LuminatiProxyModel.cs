using ItWasMe.HttpBasics;
using System; 

namespace ItWasMe.LuminatiProxy
{
    public class LuminatiProxyModel : ProxyModel
    {
        private int _expirationCount = 1;
        public string CountryCode { get; set; }

        public DateTime FreezeTime { get; private set; } = DateTime.UtcNow;


        public void Freeze(TimeSpan expirationTime)
        {
            _expirationCount++;
            var total = TimeSpan.FromTicks(expirationTime.Ticks * _expirationCount);
            FreezeTime = DateTime.UtcNow.Add(total);
        }
    }
}
