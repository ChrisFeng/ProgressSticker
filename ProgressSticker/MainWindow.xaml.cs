using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace ProgressSticker
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
        }
        const int columnPoint = 20;//20個燈號
        const int rowPoint = 25;//25排
        const int lightFreq = 25;//25毫秒

        public Dictionary<int, Dictionary<int, CPoint>> Points { get; set; } = GenerateLight(rowPoint, columnPoint);

        public static Dictionary<int, Dictionary<int, CPoint>> GenerateLight(int RowPoint, int ColumnPoint)
        {
            var LightArray = new Dictionary<int, Dictionary<int, CPoint>>();
            for (int i = 0; i < RowPoint; i++)
            {
                var column = new Dictionary<int, CPoint>();
                for (int j = 0; j < ColumnPoint; j++)
                {
                    if (i % 2 == 0)
                    {
                        column[(i * ColumnPoint) + (j + 1)] = new CPoint();//1~20, start form 1
                    }
                    else
                    {
                        column[(i * ColumnPoint + ColumnPoint) - j] = new CPoint();//40~21, start form 1
                    }
                }
                LightArray[i + 1] = column;//LightArray[1]~LightArray[RowPoint], start form 1
            }
            return LightArray;
        }

        public class CPoint : INotifyPropertyChanged
        {
            private string color = "Red";
            public string Color
            {
                get { return color; }
                set
                {
                    if (value != color)
                    {
                        color = value;
                        NotifyPropertyChanged();
                    }
                }
            }
            public event PropertyChangedEventHandler PropertyChanged;
            private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            LightButton.IsEnabled = false;//懶得寫command...用這代替
            for (int lightIndex = 1; lightIndex <= rowPoint*columnPoint; lightIndex++)
            {
                var changeLightTask = new Task(()=>
                {
                    ChangeLight(lightIndex, columnPoint);
                    Thread.Sleep(lightFreq);
                });
                changeLightTask.Start();
                await changeLightTask;
            }
            LightButton.IsEnabled = true;//懶得寫command...用這代替
        }
        /// <summary>
        /// LightIndex為燈號編號, ColumnPoint代表一排幾個燈號
        /// </summary>
        /// <param name="LightIndex"></param>
        /// <param name="ColumnPoint"></param>
        public void ChangeLight(int LightIndex, int ColumnPoint)
        {
            var rowNum = LightIndex / ColumnPoint;
            if (LightIndex % ColumnPoint == 0)
            {
                Points[rowNum][LightIndex].Color = "Green";
            }
            else
            {
                Points[rowNum + 1][LightIndex].Color = "Green";
            }
        }
    }
}
