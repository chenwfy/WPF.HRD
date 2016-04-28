namespace WPF.HRD.Core.Chess
{
    /// <summary>
    /// 棋子类型枚举
    /// </summary>
    public enum ChessType
    {
        /// <summary>
        /// 空白方块棋子
        /// </summary>
        Blank = 0,

        /// <summary>
        /// 1*1方块棋子
        /// </summary>
        Square = 1,

        /// <summary>
        /// 1*2方块棋子
        /// </summary>
        VRect = 3,

        /// <summary>
        /// 2*1方块棋子
        /// </summary>
        HRect = 6,

        /// <summary>
        /// 2*2方块棋子
        /// </summary>
        Block = 7
    }
}