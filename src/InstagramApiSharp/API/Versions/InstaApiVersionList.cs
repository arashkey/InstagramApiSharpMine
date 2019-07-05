/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharp
 * Nuget package: https://www.nuget.org/packages/InstagramApiSharp
 * 
 * IRANIAN DEVELOPERS
 */

using InstagramApiSharp.Enums;
using System.Collections.Generic;
using System.Linq;

namespace InstagramApiSharp.API.Versions
{
    internal class InstaApiVersionList
    {
        public static InstaApiVersionList GetApiVersionList() => new InstaApiVersionList();

        public Dictionary<InstaApiVersionType, InstaApiVersion> ApiVersions()
        {
            return new Dictionary<InstaApiVersionType, InstaApiVersion>
            {
                {
                    InstaApiVersionType.Version35,
                    new InstaApiVersion
                    {
                         AppApiVersionCode = "95414346",
                         AppVersion = "35.0.0.20.96",
                         Capabilities = "3brTBw==",
                         SignatureKey = "be01114435207c0a0b11a5cf68faeb82ec4eee37c52e8429af5fff6b54b80b28"
                    }
                },
                {
                    InstaApiVersionType.Version44,
                    new InstaApiVersion
                    {
                         AppApiVersionCode = "107092322",
                         AppVersion = "44.0.0.9.93",
                         Capabilities = "3brTPw==",
                         SignatureKey = "25f955cc0c8f080a0592aa1fd2572d60afacd5f3c03090cf47ca409068b0d2e1"
                    }
                },
                {
                    InstaApiVersionType.Version61,
                    new InstaApiVersion
                    {
                         AppApiVersionCode = "107092322",
                         AppVersion = "61.0.0.19.86",
                         Capabilities = "3brTPw==",
                         SignatureKey = "25f955cc0c8f080a0592aa1fd2572d60afacd5f3c03090cf47ca409068b0d2e1"
                    }
                },
                {
                    InstaApiVersionType.Version64,
                    new InstaApiVersion
                    {
                         AppApiVersionCode = "125398467",
                         AppVersion = "64.0.0.14.96",
                         Capabilities = "3brTvw==",
                         SignatureKey = "ac5f26ee05af3e40a81b94b78d762dc8287bcdd8254fe86d0971b2aded8884a4"
                    }
                },
                {
                    InstaApiVersionType.Version74,
                    new InstaApiVersion
                    {
                         AppApiVersionCode = "125398467",
                         AppVersion = "74.0.0.21.99",
                         Capabilities = "3brTvw==",
                         SignatureKey = "ac5f26ee05af3e40a81b94b78d762dc8287bcdd8254fe86d0971b2aded8884a4"
                    }
                },
                {
                    InstaApiVersionType.Version76,
                    new InstaApiVersion
                    {
                         AppApiVersionCode = "138226743",
                         AppVersion = "76.0.0.15.395",
                         Capabilities = "3brTvw==",
                         SignatureKey = "19ce5f445dbfd9d29c59dc2a78c616a7fc090a8e018b9267bc4240a30244c53b"
                    }
                },
                {
                    InstaApiVersionType.Version86,
                    new InstaApiVersion
                    {
                         AppApiVersionCode = "147375143",
                         AppVersion = "86.0.0.24.87",
                         Capabilities = "3brTvw==",
                         SignatureKey = "19ce5f445dbfd9d29c59dc2a78c616a7fc090a8e018b9267bc4240a30244c53b"
                    }
                },
                {
                    InstaApiVersionType.Version87,
                    new InstaApiVersion
                    {
                         AppApiVersionCode = "148324051",
                         AppVersion = "87.0.0.18.99",
                         Capabilities = "3brTvw==",
                         SignatureKey = "19ce5f445dbfd9d29c59dc2a78c616a7fc090a8e018b9267bc4240a30244c53b"
                    }
                },
                {
                    InstaApiVersionType.Version88,
                    new InstaApiVersion
                    {
                         AppApiVersionCode = "149350061",
                         AppVersion = "88.0.0.14.99",
                         Capabilities = "3brTvw==",
                         SignatureKey = "19ce5f445dbfd9d29c59dc2a78c616a7fc090a8e018b9267bc4240a30244c53b"
                    }
                },
                {
                    InstaApiVersionType.Version89,
                    new InstaApiVersion
                    {
                         AppApiVersionCode = "150338067",
                         AppVersion = "89.0.0.21.101",
                         Capabilities = "3brTvw==",
                         SignatureKey = "937463b5272b5d60e9d20f0f8d7d192193dd95095a3ad43725d494300a5ea5fc"
                    }
                },
                {
                    InstaApiVersionType.Version90,
                    new InstaApiVersion
                    {
                         AppApiVersionCode = "151414277",
                         AppVersion = "90.0.0.18.110",
                         Capabilities = "3brTvw==",
                         SignatureKey = "937463b5272b5d60e9d20f0f8d7d192193dd95095a3ad43725d494300a5ea5fc"
                    }
                },
                {
                    InstaApiVersionType.Version91,
                    new InstaApiVersion
                    {
                         AppApiVersionCode = "152367502",
                         AppVersion = "91.0.0.18.118",
                         Capabilities = "3brTvw==",
                         SignatureKey = "7f2efba692e18dd385499a6727ad440a959d575568d5547594cc54c3ff5bbe35"
                    }
                },
                {
                    InstaApiVersionType.Version94,
                    new InstaApiVersion
                    {
                         AppApiVersionCode = "161478672",
                         AppVersion = "94.0.0.22.116",
                         Capabilities = "3brTvw==",
                         SignatureKey = "446f6292f1da63db9d8d3a9f5af793625173f79bb61de1ddd5cf10ef933a7af7"
                    }
                },
                {
                    InstaApiVersionType.Version100,
                    new InstaApiVersion
                    {
                         AppApiVersionCode = "155374104",
                         AppVersion = "100.0.0.17.129",
                         Capabilities = "3brTvw==",
                         SignatureKey = "e0767f8a7ae9f6c1f9d3674be35d96117f0589960bf3dbd2921f020b33ca4b9f"
                    }
                }
            };
        }

        public InstaApiVersion GetApiVersion(InstaApiVersionType versionType)
        {
            return (from apiVer in ApiVersions()
                    where apiVer.Key == versionType
                    select apiVer.Value).FirstOrDefault();
        }
    }
}
