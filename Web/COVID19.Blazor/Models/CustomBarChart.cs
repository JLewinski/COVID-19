using ChartJs.Blazor.ChartJS.BarChart;
using ChartJs.Blazor.ChartJS.BarChart.Axes;
using ChartJs.Blazor.ChartJS.Common.Axes;
using ChartJs.Blazor.ChartJS.Common.Axes.Ticks;
using ChartJs.Blazor.ChartJS.Common.Properties;
using ChartJs.Blazor.ChartJS.Common.Wrappers;
using ChartJs.Blazor.Charts;
using ChartJs.Blazor.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COVID19.Blazor.Models
{
    public interface ICustomBarChart
    {
        BarConfig Config { get; }
        ChartJsBarChart Chart { get; set; }
        string Label { get; set; }
        void Clear();
        Task Update();
    }

    public class CustomBarChart<T> : ICustomBarChart where T : class
    {
        private readonly BarConfig _barConfig;
        private readonly BarDataset<T> _data;

        public BarConfig Config => _barConfig;
        public ChartJsBarChart Chart { get; set; }
        private static readonly Random s_rand = new Random();
        
        public static byte[] RandomColor()
        {
            byte[] rgb = new byte[3];

            lock (s_rand)
            {
                s_rand.NextBytes(rgb);
            }

            return rgb;
        }

        public CustomBarChart()
        {
            var baseColor = RandomColor();

            _barConfig = new BarConfig
            {
                Options = new BarOptions
                {
                    Title = new OptionsTitle
                    {
                        Display = true,
                    },
                    Scales = new BarScales
                    {
                        XAxes = new List<CartesianAxis>
                        {
                            new BarCategoryAxis()
                        },
                        YAxes = new List<CartesianAxis>
                        {
                            new BarLinearCartesianAxis
                            {
                                Ticks = new LinearCartesianTicks
                                {
                                    BeginAtZero = true
                                }
                            }
                        }
                    }
                }
            };
            _data = new BarDataset<T>
            {
                BackgroundColor = ColorUtil.ColorString(baseColor[0], baseColor[1], baseColor[2], 0.8),
                BorderWidth = 0,
                HoverBackgroundColor = ColorUtil.ColorString(baseColor[0], baseColor[1], baseColor[2], 1),
                HoverBorderColor = ColorUtil.ColorString(baseColor[0], baseColor[1], baseColor[2], 1),
                HoverBorderWidth = 1,
                BorderColor = "#ffffff"
            };
            _barConfig.Data.Datasets.Add(_data);
        }

        public bool ShowTitle
        {
            get => _barConfig.Options.Title.Display;
            set => _barConfig.Options.Title.Display = value;
        }

        public string Label
        {
            get => _data.Label;
            set => _data.Label = value;
        }

        public string Title
        {
            get => _barConfig.Options.Title.Text.SingleValue;
            set => _barConfig.Options.Title.Text = value;
        }

        public List<string> Labels
        {
            get => _barConfig.Data.Labels;
            set => _barConfig.Data.Labels = value;
        }

        public IReadOnlyCollection<T> Data
        {
            get => _data.Data;
        }

        public void Add(T obj) => _data.Add(obj);
        public void AddRange(IEnumerable<T> data) => _data.AddRange(data);
        public void Clear() => _data.RemoveAll(x => true);
        public void Remove(T element) => _data.Remove(element);
        public void RemoveAt(int index) => _data.RemoveAt(index);

        public Task Update() => Chart.Update();
    }
}
