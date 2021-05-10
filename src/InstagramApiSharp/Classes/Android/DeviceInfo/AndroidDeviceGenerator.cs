using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;

namespace InstagramApiSharp.Classes.Android.DeviceInfo
{
    public class AndroidDeviceGenerator
    {
        private static readonly List<string> DevicesNames = new List<string>
        {
            AndroidDevices.XPERIA_Z5,
            AndroidDevices.HONOR_8LITE,
            AndroidDevices.XIAOMI_MI_4W,
            AndroidDevices.XIAOMI_HM_1SW,
            AndroidDevices.HTC_ONE_PLUS
        };

        public static Dictionary<string, AndroidDevice> AndroidAndroidDeviceSets = new Dictionary<string, AndroidDevice>
        {
            {
                "xperia-z5",
                new AndroidDevice
                {
                    AndroidBoardName = "msm8994",
                    AndroidBootloader = "s1",
                    DeviceBrand = "Sony",
                    DeviceModel = "E6653",
                    DeviceModelIdentifier = "E6653",
                    FirmwareBrand = "E6653",
                    HardwareManufacturer = "Sony",
                    HardwareModel = "E6653",
                    DeviceGuid = Guid.NewGuid(),
                    PhoneGuid = Guid.NewGuid(),
                    Resolution = "1440x2560",
                    Dpi = "640dpi"
                }
            },
            {
                AndroidDevices.HONOR_8LITE,
                new AndroidDevice
                {
                    AndroidBoardName = "HONOR",
                    DeviceBrand = "HUAWEI",
                    HardwareManufacturer = "HUAWEI",
                    DeviceModel = "PRA-LA1",
                    DeviceModelIdentifier = "PRA-LA1",
                    FirmwareBrand = "HWPRA-H",
                    HardwareModel = "hi6250",
                    DeviceGuid = Guid.NewGuid(),
                    PhoneGuid = Guid.NewGuid(),
                    Resolution = "1080x1812",
                    Dpi = "480dpi",
                }
            },
            {
                AndroidDevices.XIAOMI_MI_4W,
                new AndroidDevice
                {
                    AndroidBoardName = "MI",
                    DeviceBrand = "Xiaomi",
                    HardwareManufacturer = "Xiaomi",
                    DeviceModel = "MI-4W",
                    DeviceModelIdentifier = "4W",
                    FirmwareBrand = "4W",
                    HardwareModel = "cancro",
                    DeviceGuid = Guid.NewGuid(),
                    PhoneGuid = Guid.NewGuid(),
                    Resolution = "1080x1920",
                    Dpi = "480dpi",
                }
            },
            {
                AndroidDevices.XIAOMI_HM_1SW,
                new AndroidDevice
                {
                    AndroidBoardName = "HM",
                    DeviceBrand = "Xiaomi",
                    HardwareManufacturer = "Xiaomi",
                    DeviceModel = "HM-1SW",
                    DeviceModelIdentifier = "1SW",
                    FirmwareBrand = "1SW",
                    HardwareModel = "armani",
                    DeviceGuid = Guid.NewGuid(),
                    PhoneGuid = Guid.NewGuid(),
                    Resolution = "720x1280",
                    Dpi = "320dpi",
                }
            },
            {
                AndroidDevices.HTC_ONE_PLUS,
                new AndroidDevice
                {
                    AndroidBoardName = "One",
                    DeviceBrand = "Htc",
                    HardwareManufacturer = "Htc",
                    DeviceModel = "One-Plus",
                    DeviceModelIdentifier = "Plus",
                    FirmwareBrand = "Plus",
                    HardwareModel = "A3010",
                    DeviceGuid = Guid.NewGuid(),
                    PhoneGuid = Guid.NewGuid(),
                    Resolution = "1080x1920",
                    Dpi = "380dpi",
                }
            }
        };

        static readonly Random Rnd = new Random();
        private static AndroidDevice LastDevice;
        public static AndroidDevice GetRandomAndroidDevice()
        {
            TryLabel:
            var randomDeviceIndex = Rnd.Next(0, DevicesNames.Count);
            var device = AndroidAndroidDeviceSets.ElementAt(randomDeviceIndex).Value;
            device.PhoneGuid = Guid.NewGuid();
            device.DeviceGuid = Guid.NewGuid();
            device.DeviceId = ApiRequestMessage.GenerateDeviceIdFromGuid(device.DeviceGuid);
            device.PigeonSessionId = Guid.NewGuid();
            device.PushDeviceGuid = Guid.NewGuid();
            device.FamilyDeviceGuid = Guid.NewGuid();
            device.IGBandwidthSpeedKbps = $"{Rnd.Next(1233, 1567).ToString(CultureInfo.InvariantCulture)}.{Rnd.Next(100, 999).ToString(CultureInfo.InvariantCulture)}";
            device.IGBandwidthTotalTimeMS = Rnd.Next(781, 999).ToString(CultureInfo.InvariantCulture);
            device.IGBandwidthTotalBytesB = ((int)((double.Parse(device.IGBandwidthSpeedKbps, CultureInfo.InvariantCulture) * double.Parse(device.IGBandwidthTotalTimeMS, CultureInfo.InvariantCulture)) + Rnd.Next(100, 999))).ToString();

            if (LastDevice != null)
                if (device.DeviceId == LastDevice.DeviceId)
                    goto TryLabel;
            LastDevice = device;
            return device;
        }

        public static AndroidDevice GetByName(string name)
        {
            return AndroidAndroidDeviceSets[name];
        }

        public static AndroidDevice GetById(string deviceId)
        {
            return (from androidAndroidDeviceSet in AndroidAndroidDeviceSets
                    where androidAndroidDeviceSet.Value.DeviceId == deviceId
                    select androidAndroidDeviceSet.Value).FirstOrDefault();
        }
    }
}