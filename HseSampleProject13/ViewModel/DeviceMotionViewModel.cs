using System;
using System.Windows.Input;
using DeviceMotion.Plugin;
using DeviceMotion.Plugin.Abstractions;
using PropertyChanged;
using Xamarin.Forms;

namespace HseSampleProject13.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class DeviceMotionViewModel
    {
        public DeviceMotionViewModel()
        {
            StartCommand = new Command(() =>
            {
                CrossDeviceMotion.Current.Start(
                    MotionSensorType.Accelerometer);
            });

            EndCommand = new Command(() =>
            {
                CrossDeviceMotion.Current.Stop(
                    MotionSensorType.Accelerometer);
            });

            CrossDeviceMotion.Current.SensorValueChanged += (s, a) =>
            {
                //Есть разные сенсоры, аля Компас, Гироскоп или Магнитометр
                //В зависимости от того, что вам нужно, вызывайте
                //Разные MotionSensorType.Some

                if (a.SensorType == MotionSensorType.Accelerometer)
                {
                    XField = FAbs(((MotionVector) a.Value).X) / 36;
                    YField = FAbs(((MotionVector) a.Value).Y) / 36;
                    ZField = FAbs(((MotionVector) a.Value).Z) / 36;
                }
            };
        }
        private double esp = 1E-18;
        private double FAbs(double x)
        {
            if (x - esp < 0)
            {
                x = -x;
            }
            return x;
        }
        public ICommand StartCommand { get; set; }

        public ICommand EndCommand { get; set; }

        public double XField { get; set; }
        public double YField { get; set; }
        public double ZField { get; set; }
    }
}
