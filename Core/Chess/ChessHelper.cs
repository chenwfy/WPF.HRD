namespace WPF.HRD.Core.Chess
{
    /// <summary>
    /// 棋子实例扩展类
    /// </summary>
    public static class ChessHelper
    {
        /// <summary>
        /// 由棋子类型创建对应棋子实例
        /// </summary>
        /// <param name="chessType">棋子类型</param>
        /// <returns>棋子实例</returns>
        public static ChessBase CreateChess(this ChessType chessType)
        {
            switch (chessType)
            {
                case ChessType.Square:
                    return new ChessSquare();
                case ChessType.HRect:
                    return new ChessHRect();
                case ChessType.VRect:
                    return new ChessVRect();
                case ChessType.Block:
                    return new ChessBlock();
                case ChessType.Blank:
                default:
                    return new ChessBlank();
            }
        }
    }
}