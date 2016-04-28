using WPF.HRD.Core.Chess;

namespace WPF.HRD.Core
{
    /// <summary>
    /// 设置棋子可移动至的新位置完成后的回调委托
    /// </summary>
    /// <param name="currentChess"></param>
    /// <param name="moveDirection"></param>
    /// <param name="newBlankPosition"></param>
    public delegate void SetNewPositionDelegate(ChessBase currentChess, Direction moveDirection, BlankPosition newBlankPosition);
    
    /// <summary>
    /// 棋局完成后的回调委托
    /// </summary>
    public delegate void LayoutCompleted();
}
