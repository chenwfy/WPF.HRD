using System.Collections.Generic;
using WPF.HRD.Core.Chess;

namespace WPF.HRD.Core
{
    /// <summary>
    /// 公共辅助类
    /// </summary>
    public static class Common
    {
        /// <summary>
        /// 棋盘网格行数
        /// </summary>
        public const int GridRows = 5;

        /// <summary>
        /// 棋盘网格列数
        /// </summary>
        public const int GridColumns = 4;

        /// <summary>
        /// 棋子类型计算基础常数
        /// </summary>
        public const long ChessBit = 7;

        /// <summary>
        /// 将布局UINT64数值转换为棋子布局集合
        /// </summary>
        /// <param name="layoutCode">布局UINT64数值</param>
        /// <returns>棋子布局集合</returns>
        public static List<ChessBase> GetChessList(this long layoutCode)
        {
            List<ChessBase> chessList = new List<ChessBase>(0);
            ChessType chessType;
            long tempCode = 0;
            for (int r = 0; r < GridRows; r++)
            {
                for (int c = 0; c < GridColumns; c++)
                {
                    int idx = r * GridColumns + c;
                    tempCode = ChessBit << (idx * 3);
                    chessType = (ChessType)((int)((tempCode & layoutCode) >> (idx * 3)));
                    if (chessType != ChessType.Blank)
                    {
                        ChessBase currentChess = chessType.CreateChess();
                        currentChess.Position = idx;
                        currentChess.NextPosition = idx;
                        chessList.Add(currentChess);
                    }
                }
            }
            return chessList;
        }

        /// <summary>
        /// 获取当前布局UINT64数值
        /// </summary>
        /// <param name="chessList">当前棋子布局集合</param>
        /// <returns>当前布局UINT64数值</returns>
        public static long GetLayoutCode(this List<ChessBase> chessList)
        {
            long result = 0;
            foreach (var chess in chessList)
            {
                long idx = chess.Position;
                result += (long)((int)chess.ChessType) << ((int)(3 * idx));
            }
            return result;
        }

        /// <summary>
        /// 本布局是否完成
        /// </summary>
        /// <param name="chessList">当前棋子布局集合</param>
        /// <returns>布局是否完成</returns>
        public static bool LayoutFinished(this List<ChessBase> chessList)
        {
            //return chessList.Single(c => c.ChessType == ChessType.Block).Position == 13;
            foreach (var chess in chessList)
            {
                if (chess.ChessType == ChessType.Block && chess.Position == 13)
                    return true;
            }
            return false;
        }
    }
}