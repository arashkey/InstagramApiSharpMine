/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharp
 * Nuget package: https://www.nuget.org/packages/InstagramApiSharp
 * 
 * IRANIAN DEVELOPERS
 */

namespace InstagramApiSharp.Enums
{
    public enum InstaApiVersionType
    {
        /// <summary>
        ///     v44.0.0.9.93 => No more consent required error.
        /// </summary>
        Version44 = 0,
        /// <summary>
        ///     Api version => No more consent required error.
        /// </summary>
        Version35 = 1,
        /// <summary>
        ///     Api version 61.0.0.19.86 => All data like signature key, version code and ... is for v44 except instagram version
        /// </summary>
        Version61 = 2,
        /// <summary>
        ///     Api version 64.0.0.14.96
        /// </summary>
        Version64 = 3,
        /// <summary>
        ///     Api version 74.0.0.21.99 => All data like signature key, version code and ... is for v64 except instagram version
        /// </summary>
        Version74 = 4,
        /// <summary>
        ///     Api version 76.0.0.15.395
        /// </summary>
        Version76 = 5,
        /// <summary>
        ///     Api version 86.0.0.24.87
        /// </summary>
        Version86 = 6,
        /// <summary>
        ///     Api version 87.0.0.18.99
        /// </summary>
        Version87 = 7,
        /// <summary>
        ///     Api version 88.0.0.14.99
        /// </summary>
        Version88 = 8,
        /// <summary>
        ///     Api version 89.0.0.21.101
        /// </summary>
        Version89 = 9,
        /// <summary>
        ///     Api version 90.0.0.18.110
        /// </summary>
        Version90 = 10,
        /// <summary>
        ///     Api version 91.0.0.18.118
        /// </summary>
        Version91 = 11,
        /// <summary>
        ///     Api version 94.0.0.22.116
        /// </summary>
        Version94 = 12,

    }
}
