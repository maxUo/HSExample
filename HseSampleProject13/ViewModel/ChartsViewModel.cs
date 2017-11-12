using System;
using System.Collections.Generic;
using System.Windows.Input;
using Microcharts;
using PropertyChanged;
using Xamarin.Forms;

namespace HseSampleProject13.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class ChartsViewModel
    {
        private static readonly Random rand = new Random();

        public ChartsViewModel()
        {
            GenerationCommand = new Command(Generate);
        }

        private string GetRandomColour()
        {
            return String.Format("#{0:X6}", rand.Next(0x1000000));
        }

        private void Generate()
        {
            List<Microcharts.Entry> entries = new List<Microcharts.Entry>();

            for (int i = 0; i < rand.Next(3, 8); i++)
            {
                var res = rand.Next(10, 401);
                entries.Add(new Microcharts.Entry(res)
                {
                    Label = i.ToString(),
                    ValueLabel = res.ToString(),
                    Color = SkiaSharp.SKColor.Parse(GetRandomColour())
                });
            }
            int choice = rand.Next(5);
            switch (choice)
            {
            case 0:
                ChartView = new PointChart() { Entries = entries };
                break;
            case 1:
                ChartView = new BarChart() { Entries = entries };
                break;
            case 2:
                ChartView = new DonutChart() { Entries = entries };
                break;
            case 3:
                ChartView = new RadialGaugeChart() { Entries = entries };
                break;
            case 4:
                ChartView = new LineChart() { Entries = entries };
                break;
            }
            MessagingCenter.Send(ChartView,"NewChartAvaliable");
        }

        private Chart ChartView { get; set; }
        public ICommand GenerationCommand { get; set; }
    }
}
