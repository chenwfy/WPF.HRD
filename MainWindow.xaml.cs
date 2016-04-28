using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using WPF.HRD.Core;
using WPF.HRD.Core.Chess;

namespace WPF.HRD
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// 初始布局
        /// </summary>
        private List<ChessBase> InitChessList = new List<ChessBase>(0);

        /// <summary>
        /// 网格方块控件字典
        /// </summary>
        private Dictionary<string, Rectangle> GridRect_Dict = new Dictionary<string, Rectangle>(0);

        /// <summary>
        /// 已经被占用的网格序号集合
        /// </summary>
        private List<int> HasSetRectIdxList = new List<int>(0);

        /// <summary>
        /// 游戏布局
        /// </summary>
        private GameLayout gameLayout = null;

        /// <summary>
        /// 构造函数
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 界面LOAD事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //绘制网格
            DrawGridLine();
        }

        /// <summary>
        /// 绘制网格
        /// </summary>
        private void DrawGridLine()
        {
            Rectangle rect;
            for (int r = 0; r < 5; r++)
            {
                for (int c = 0; c < 4; c++)
                {
                    rect = new Rectangle();
                    rect.Stroke = new SolidColorBrush(Colors.Gray);
                    rect.Width = 100;
                    rect.Height = 100;
                    this.MainCanvas.Children.Add(rect);
                    //X Y
                    double rx = c * 100;
                    double ry = r * 100;

                    // r * 4 + c;
                    rect.Name = "Rect_" + (r * 4 + c);

                    Canvas.SetLeft(rect, rx);
                    Canvas.SetTop(rect, ry);

                    this.GridRect_Dict.Add(rect.Name, rect);
                    this.HasSetRectIdxList.Add(r * 4 + c);
                }
            }
            rect = null;
        }

        /// <summary>
        /// 鼠标左键点击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainMark_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point p = e.GetPosition(this.MainCanvas);
            int px = (int)(p.X / 100);
            int py = (int)(p.Y / 100);
            int idx = py * 4 + px;
            SetRectangleColor(idx);
        }

        /// <summary>
        /// 清空画板按钮点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_Clear_Click(object sender, RoutedEventArgs e)
        {
            this.InitChessList.Clear();
            this.GridRect_Dict.Clear();
            this.HasSetRectIdxList.Clear();
            this.MainCanvas.Children.Clear();
            DrawGridLine();
        }

        /// <summary>
        /// 手动玩游戏解局
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_Start_Click(object sender, RoutedEventArgs e)
        {
            this.button_Start.Visibility = Visibility.Collapsed;
            this.button_Abort.Visibility = Visibility.Visible;

            this.BoardBox.Visibility = Visibility.Visible;
            gameLayout = new GameLayout(App.CurrentLayoutCode, App.CurrentBlankPositions[0], App.CurrentBlankPositions[1]);
            gameLayout.LayoutCompleted = this.GameFinished;
            gameLayout.Draw(this.BoardBox);
        }

        /// <summary>
        /// 终止解局
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_Abort_Click(object sender, RoutedEventArgs e)
        {
            this.button_Start.Visibility = Visibility.Visible;
            this.button_Abort.Visibility = Visibility.Collapsed;

            this.BoardBox.Visibility = Visibility.Collapsed;
            this.BoardBox.Children.Clear();
            if (null != gameLayout) {
                gameLayout.Abort();
                gameLayout = null;
            }
        }

        /// <summary>
        /// 布局绘制完成，验证并求解
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_Verify_Click(object sender, RoutedEventArgs e)
        {
            if (this.HasSetRectIdxList.Count != 2)
            {
                MessageBox.Show("布局必须，且只能有2个空白网格！");
                return;
            }
            if (this.InitChessList.Count(c => c.ChessType == ChessType.Block) > 1)
            {
                MessageBox.Show("布局必须，且只能有1个2*2棋子！");
                return;
            }

            long layoutCode = this.InitChessList.GetLayoutCode();
            int[] blankPositions = this.HasSetRectIdxList.ToArray();

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            PathFinder finder = new PathFinder(layoutCode, blankPositions[0], blankPositions[1]);
            IList<long> stepList = finder.FindPath();

            stopwatch.Stop();
            this.label_VerifyTxt.Content = string.Format("本局求解步数 {0}！ 耗时 {1} 毫秒!", stepList.Count, stopwatch.ElapsedMilliseconds);

            if (stepList.Count > 0)
            {
                App.CurrentLayoutCode = layoutCode;
                App.CurrentBlankPositions = blankPositions;
                ShowWindow showWin = new ShowWindow();
                showWin.Show();

                this.button_Start.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// 设置网格背景颜色并且判断类型
        /// </summary>
        /// <param name="idx"></param>
        private void SetRectangleColor(int idx)
        {
            string rectName = "Rect_" + idx;
            Rectangle tmpRect = this.GridRect_Dict[rectName];
            ChessBase chess = null;
            if (this.radioButton1.IsChecked.Value)
            {
                // 2*2 棋子 (田)
                int p1 = idx, p2 = p1 + 1, p3 = p1 + 4, p4 = p2 + 4;
                if (p1 % 4 > 2 || p1 > 15)
                {
                    MessageBox.Show("该网格不能放置2*2棋子！");
                    return;
                }

                if (this.HasSetRectIdxList.Contains(p1) && this.HasSetRectIdxList.Contains(p2) && this.HasSetRectIdxList.Contains(p3) && this.HasSetRectIdxList.Contains(p4))
                {
                    tmpRect.Stroke = new SolidColorBrush(Colors.Gray);
                    tmpRect.Fill = new SolidColorBrush(Colors.Red);

                    tmpRect = this.GridRect_Dict["Rect_" + p2];
                    tmpRect.Stroke = new SolidColorBrush(Colors.Gray);
                    tmpRect.Fill = new SolidColorBrush(Colors.Red);

                    tmpRect = this.GridRect_Dict["Rect_" + p3];
                    tmpRect.Stroke = new SolidColorBrush(Colors.Gray);
                    tmpRect.Fill = new SolidColorBrush(Colors.Red);

                    tmpRect = this.GridRect_Dict["Rect_" + p4];
                    tmpRect.Stroke = new SolidColorBrush(Colors.Gray);
                    tmpRect.Fill = new SolidColorBrush(Colors.Red);

                    this.HasSetRectIdxList.Remove(idx);
                    this.HasSetRectIdxList.Remove(p2);
                    this.HasSetRectIdxList.Remove(p3);
                    this.HasSetRectIdxList.Remove(p4);

                    chess = ChessType.Block.CreateChess();
                    chess.Position = idx;
                    this.InitChessList.Add(chess);

                    return;
                }
                else
                {
                    MessageBox.Show("该网格不能放置2*2棋子！");
                    return;
                }
            }
            if (this.radioButton2.IsChecked.Value)
            {
                // 2*1 棋子 (日)
                int p1 = idx, p2 = p1 + 4;
                if (p1 > 15)
                {
                    MessageBox.Show("该网格不能放置2*1棋子！");
                    return;
                }

                if (this.HasSetRectIdxList.Contains(p1) && this.HasSetRectIdxList.Contains(p2))
                {
                    tmpRect.Stroke = new SolidColorBrush(Colors.Gray);
                    tmpRect.Fill = new SolidColorBrush(Colors.Blue);

                    tmpRect = this.GridRect_Dict["Rect_" + p2];
                    tmpRect.Stroke = new SolidColorBrush(Colors.Gray);
                    tmpRect.Fill = new SolidColorBrush(Colors.Blue);

                    this.HasSetRectIdxList.Remove(idx);
                    this.HasSetRectIdxList.Remove(p2);

                    chess = ChessType.VRect.CreateChess();
                    chess.Position = idx;
                    this.InitChessList.Add(chess);

                    return;
                }
                else
                {
                    MessageBox.Show("该网格不能放置2*1棋子！");
                    return;
                }
            }
            if (this.radioButton3.IsChecked.Value)
            {
                // 1*2 棋子 (口口)
                int p1 = idx, p2 = p1 + 1;
                if (p1 % 4 > 2)
                {
                    MessageBox.Show("该网格不能放置1*2棋子！");
                    return;
                }

                if (this.HasSetRectIdxList.Contains(p1) && this.HasSetRectIdxList.Contains(p2))
                {
                    tmpRect.Stroke = new SolidColorBrush(Colors.Gray);
                    tmpRect.Fill = new SolidColorBrush(Colors.Orange);

                    tmpRect = this.GridRect_Dict["Rect_" + p2];
                    tmpRect.Stroke = new SolidColorBrush(Colors.Gray);
                    tmpRect.Fill = new SolidColorBrush(Colors.Orange);

                    this.HasSetRectIdxList.Remove(idx);
                    this.HasSetRectIdxList.Remove(p2);

                    chess = ChessType.HRect.CreateChess();
                    chess.Position = idx;
                    this.InitChessList.Add(chess);

                    return;
                }
                else
                {
                    MessageBox.Show("该网格不能放置1*2棋子！");
                    return;
                }
            }
            if (this.radioButton4.IsChecked.Value)
            {
                // 1*1 棋子 (口)
                if (this.HasSetRectIdxList.Contains(idx))
                {
                    tmpRect.Stroke = new SolidColorBrush(Colors.Gray);
                    tmpRect.Fill = new SolidColorBrush(Colors.Yellow);
                    this.HasSetRectIdxList.Remove(idx);

                    chess = ChessType.Square.CreateChess();
                    chess.Position = idx;
                    this.InitChessList.Add(chess);

                    return;
                }
                else
                {
                    MessageBox.Show("该网格已放置其他棋子！");
                    return;
                }
            }
        }

        /// <summary>
        /// 棋局结束
        /// </summary>
        private void GameFinished()
        {
            MessageBox.Show("恭喜！本局完成！");
        }
    }
}