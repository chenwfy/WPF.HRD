namespace WPF.HRD.Core.Chess
{
    /// <summary>
    /// 空白棋子
    /// </summary>
    public class ChessBlank : ChessBase
    {
        /// <summary>
        /// 创建空白棋子新实例
        /// </summary>
        public ChessBlank()
            : base(ChessType.Blank)
        {        
        }
    }
}