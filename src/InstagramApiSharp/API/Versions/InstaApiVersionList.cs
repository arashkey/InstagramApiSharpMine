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
                },
                {
                    InstaApiVersionType.Version105,
                    new InstaApiVersion
                    {
                         AppApiVersionCode = "166149717",
                         AppVersion = "105.0.0.18.119",
                         Capabilities = "3brTvwE=",
                         SignatureKey = "d9af03055a2774b684c870c47b05abb1ac96f590820e370356d402e68bd9411f"
                    }
                },
                {
                    InstaApiVersionType.Version113,
                    new InstaApiVersion
                    {
                         AppApiVersionCode = "175574628",
                         AppVersion = "113.0.0.39.122",
                         Capabilities = "3brTvwE=",
                         SignatureKey = "8013dff8e97c16461a8881071210e8271c8fb0cf604d3c7d8f798df621435a18",
                         BloksVersionId = "15f3ececf8692b1eddb3a12f1ffb072cc9e9109aac995e30f1f500b68d9917eb"
                    }
                },
                {
                    InstaApiVersionType.Version117,
                    new InstaApiVersion
                    {
                         AppApiVersionCode = "180322757",
                         AppVersion = "117.0.0.28.123",
                         Capabilities = "3brTvwE=",
                         SignatureKey = "a86109795736d73c9a94172cd9b736917d7d94ca61c9101164894b3f0d43bef4",
                         BloksVersionId = "0a3ae4c88248863609c67e278f34af44673cff300bc76add965a9fb036bd3ca3"
                    }
                },
                {
                    InstaApiVersionType.Version121,
                    new InstaApiVersion
                    {
                         AppApiVersionCode = "185203705",
                         AppVersion = "121.0.0.29.119",
                         Capabilities = "3brTvwE=",
                         SignatureKey = "9193488027538fd3450b83b7d05286d4ca9599a0f7eeed90d8c85925698a05dc",
                         BloksVersionId = "1b030ce63a06c25f3e4de6aaaf6802fe1e76401bc5ab6e5fb85ed6c2d333e0c7"
                    }
                },
                {
                    InstaApiVersionType.Version123,
                    new InstaApiVersion
                    {
                         AppApiVersionCode = "188791674",
                         AppVersion = "123.0.0.21.114",
                         Capabilities = "3brTvwM=",
                         SignatureKey = "8fea883900208c803efa5daebe163d3d75979be19e288368a3e34a465c0f975e",
                         BloksVersionId = "7ab39aa203b17c94cc6787d6cd9052d221683361875eee1e1bfe30b8e9debd74"
                    }
                },
                {
                    InstaApiVersionType.Version124,
                    new InstaApiVersion
                    {
                         AppApiVersionCode = "192992577",
                         AppVersion = "124.0.0.17.473",
                         Capabilities = "3brTvwM=",
                         SignatureKey = "fdec5d441c8c18708cecfbfe184ad1c08a6c99849bcd986061b7909306873e0e",
                         BloksVersionId = "d43b6b50d0644a8d433f1914592fdff0b1a60932deead2b1090e9d7723dc75db"
                    }
                },
                {
                    InstaApiVersionType.Version126,
                    new InstaApiVersion
                    {
                         AppApiVersionCode = "195435560",
                         AppVersion = "126.0.0.25.121",
                         Capabilities = "3brTvwM=",
                         SignatureKey = "8e496c87a09d5e922f6e33df3f399ce298ddbd6f7d6d038417047cc6474a3542",
                         BloksVersionId = "e538d4591f238824118bfcb9528c8d005f2ea3becd947a3973c030ac971bb88e"
                    }
                },
                {
                    InstaApiVersionType.Version129,
                    new InstaApiVersion
                    {
                         AppApiVersionCode = "199325909",
                         AppVersion = "129.0.0.29.119",
                         Capabilities = "3brTvwM=",
                         SignatureKey = "d824458730434607defeba6566f92fc4bb4f34001c06c4d221e761a174e7b194",
                         BloksVersionId = "0e5dc0b9970ffc452602afb7e530944daa9a16c71932e1ba8d322bf84d48bb82"
                    }
                },
                {
                    InstaApiVersionType.Version130,
                    new InstaApiVersion
                    {
                         AppApiVersionCode = "200396014",
                         AppVersion = "130.0.0.31.121",
                         Capabilities = "3brTvwM=",
                         SignatureKey = "f0bdfd5332d66a64d5e04965e6a7ade67c4e2cfc57ea38f0083c0400640a5e20",
                         BloksVersionId = "0e9b6d9c0fb2a2df4862cd7f46e3f719c55e9f90c20db0e5d95791b66f43b367"
                    }
                },
                {
                    InstaApiVersionType.Version136,
                    new InstaApiVersion
                    {
                         AppApiVersionCode = "208061688",
                         AppVersion = "136.0.0.34.124",
                         Capabilities = "3brTvwM=",
                         SignatureKey = "46024e8f31e295869a0e861eaed42cb1dd8454b55232d85f6c6764365079374b",
                         BloksVersionId = "edc619c2e837cfe4107a7229f75ee68836af821b16b1b6ec9637914ba0b973fe"
                    }
                },
                {
                    InstaApiVersionType.Version146,
                    new InstaApiVersion
                    {
                         AppApiVersionCode = "221134032",
                         AppVersion = "146.0.0.27.125",
                         Capabilities = "3brTvw8=",
                         SignatureKey = "SIGNATURE",
                         BloksVersionId = "457c39d23261600a30d5f10fb9ee409065bf9bf8ee5480183a575fabb692a6f7"
                    }
                },
                {
                    InstaApiVersionType.Version157,
                    new InstaApiVersion
                    {
                         AppApiVersionCode = "242168941",
                         AppVersion = "157.0.0.37.120",
                         Capabilities = "3brTvw8=",
                         SignatureKey = "SIGNATURE",
                         BloksVersionId = "3fb505389eb683d8514c257597479c819304491615c89803d7cc0071987eeafe"
                    }
                },
                {
                    InstaApiVersionType.Version164,
                    new InstaApiVersion
                    {
                         AppApiVersionCode = "252055945",
                         AppVersion = "164.0.0.46.123",
                         Capabilities = "3brTvx8=",
                         SignatureKey = "SIGNATURE",
                         BloksVersionId = "d94aad116996ca7cca05e4bce137add77f307b94b8120e154177ed79934a960f"
                    }
                },
                {
                    InstaApiVersionType.Version169,
                    new InstaApiVersion
                    {
                         AppApiVersionCode = "264009049",
                         AppVersion = "169.3.0.30.135",
                         Capabilities = "3brTvx8=",
                         SignatureKey = "SIGNATURE",
                         BloksVersionId = "fe808146fcbce04d3a692219680092ef89873fda1e6ef41c09a5b6a9852bed94"
                    }
                },
                {
                    InstaApiVersionType.Version180,
                    new InstaApiVersion
                    {
                         AppApiVersionCode = "279996068",
                         AppVersion = "180.0.0.31.119",
                         Capabilities = "3brTvx0=",
                         SignatureKey = "SIGNATURE",
                         BloksVersionId = "15a1b1509e8dbb5fa808d33b1f023dbc98e6b10c6036f1bd444a5cab55c46974"
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
