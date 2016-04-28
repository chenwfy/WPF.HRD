using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Controls;
using WPF.HRD.Core;

namespace WPF.HRD
{
    /// <summary>
    /// ShowWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ShowWindow : Window
    {
        /// <summary>
        /// 
        /// </summary>
        public ShowWindow()
        {
            InitializeComponent();
            
            long toSolveLayout = App.CurrentLayoutCode;
            int[] blankPositions = App.CurrentBlankPositions;
            this.SolveGameLayout(toSolveLayout, blankPositions);
        }

        /// <summary>
        /// 棋局求解
        /// </summary>
        /// <param name="gameLayout"></param>
        /// <param name="blankPositions"></param>
        private void SolveGameLayout(long gameLayout, int[] blankPositions)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            PathFinder finder = new PathFinder(gameLayout, blankPositions[0], blankPositions[1]);
            IList<long> stepList = finder.FindPath();

            stopwatch.Stop();
            this.Lb_Result.Content = string.Format("本局求解步数 {0}！ 耗时 {1} 毫秒!", stepList.Count, stopwatch.ElapsedMilliseconds);

            if (stepList.Count > 0)
            {
                StackPanel stackPanelVertical = new StackPanel { Width = (double)this.Show_Box.GetValue(FrameworkElement.WidthProperty), Orientation = Orientation.Vertical };
                this.Show_Box.Content = stackPanelVertical;

                stepList = stepList.Reverse().ToList();
                int rowCount = stepList.Count / 2;
                if (stepList.Count % 2 > 0)
                    rowCount++;
                int curRowIndex = 0;

                DispatcherTimer timer = new DispatcherTimer();
                timer.Interval = TimeSpan.FromMilliseconds(100);
                timer.IsEnabled = true;
                timer.Tick += new EventHandler((sender, e) =>
                {
                    if (curRowIndex < rowCount)
                    {
                        StackPanel stackHorizontal = new StackPanel
                        {
                            Width = (double)this.Show_Box.GetValue(FrameworkElement.WidthProperty) - 20,
                            Orientation = Orientation.Horizontal,
                            Margin = new Thickness(10d, 10d, 10d, 10d)
                        };

                        
                        Canvas canvas = new Canvas { Width = 200, Height = 250 };
                        long stepCode = stepList[curRowIndex * 2];
                        stepCode.Draw(canvas);
                        stackHorizontal.Children.Add(canvas);
                        canvas = null;

                        if (stepList.Count > curRowIndex * 2 + 1)
                        {
                            stepCode = stepList[curRowIndex * 2 + 1];
                            canvas = new Canvas { Width = 200, Height = 250, Margin = new Thickness(20d, 0d, 0d, 0d) };
                            stepCode.Draw(canvas);
                            stackHorizontal.Children.Add(canvas);
                            canvas = null;
                        }

                        stackPanelVertical.Children.Add(stackHorizontal);
                    }
                    else
                    {
                        timer.Stop();
                    }
                    curRowIndex++;
                });
                timer.Start();
            }                
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_Return_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
