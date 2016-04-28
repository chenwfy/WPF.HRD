using System.Windows.Controls;
using WPF.HRD.Core.Chess;

namespace WPF.HRD.Core
{
    /// <summary>
    /// 面向自动求解步骤显示的布局类
    /// </summary>
    public static class StepShow
    {
        /// <summary>
        /// 棋盘网格尺寸
        /// </summary>
        private const int GridSize = 50;

        /// <summary>
        /// 
        /// </summary>
        static StepShow()
        { 
        }

        /// <summary>
        /// 画布局
        /// </summary>
        /// <param name="layoutCode"></param>
        /// <param name="container"></param>
        public static void Draw(this long layoutCode, Panel container)
        {
            ChessType chessType;
            long tempCode = 0;
            for (int r = 0; r < Common.GridRows; r++)
            {
                for (int c = 0; c < Common.GridColumns; c++)
                {
                    int idx = r * Common.GridColumns + c;
                    tempCode = Common.ChessBit << (idx * 3);
                    chessType = (ChessType)((int)((tempCode & layoutCode) >> (idx * 3)));
                    ChessBase currentChess = chessType.CreateChess();
                    if (chessType != ChessType.Blank)
                    {
                        currentChess.CreateElement(GridSize, 0, (double)c * GridSize, (double)r * GridSize);
                        container.Children.Add(currentChess.Element);
                    }
                }
            }
        }
    }
}