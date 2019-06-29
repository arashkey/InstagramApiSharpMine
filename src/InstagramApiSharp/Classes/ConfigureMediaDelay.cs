/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharp
 * Nuget package: https://www.nuget.org/packages/InstagramApiSharp
 * 
 * IRANIAN DEVELOPERS
 */
using System;

namespace InstagramApiSharp.Classes
{
    public class ConfigureMediaDelay : IConfigureMediaDelay
    {
        private readonly Random _random;
        private readonly int _minSeconds;
        private readonly int _maxSeconds;
        private ConfigureMediaDelay(int minSeconds = 25, int maxSeconds = 35)
        {
            _minSeconds = minSeconds;
            _maxSeconds = maxSeconds;
            _random = new Random(DateTime.Now.Millisecond);
        }
        public static IConfigureMediaDelay FromSeconds(int min = 25, int max = 35)
        {
            if (min > max)
                throw new ArgumentException("Value max should be bigger that value min");

            if (max < 0)
                throw new ArgumentException("Both min and max values should be bigger than 0");

            return new ConfigureMediaDelay(min, max);
        }
        public static IConfigureMediaDelay PreferredDelay() => new ConfigureMediaDelay(25, 35);
        public static IConfigureMediaDelay Empty() => new ConfigureMediaDelay(0, 0);
        public TimeSpan Value => TimeSpan.FromSeconds(_random.Next(_minSeconds, _maxSeconds));
    }
}
