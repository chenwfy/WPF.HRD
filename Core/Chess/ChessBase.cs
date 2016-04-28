using System.Windows.Controls.Primitives;

namespace WPF.HRD.Core.Chess
{
    /// <summary>
    /// 棋子基类
    /// </summary>
    public abstract class ChessBase
    {
        /// <summary>
        /// 棋子位置索引
        /// </summary>
        public int Position { get; set; }

        /// <summary>
        /// 当前棋子下一次的位置索引
        /// </summary>
        public int NextPosition { get; set; }

        /// <summary>
        /// 棋子类型
        /// </summary>
        public ChessType ChessType { get; private set; }

        /// <summary>
        /// 棋子UI控件
        /// </summary>
        public Thumb Element { get; protected set; }

        /// <summary>
        /// 创建棋子基类新实例
        /// </summary>
        public ChessBase()
        {
        }

        /// <summary>
        /// 带棋子类型创建棋子基类新实例
        /// </summary>
        /// <param name="chessType"></param>
        public ChessBase(ChessType chessType)
        {
            this.ChessType = chessType;
        }

        /// <summary>
        /// 创建棋子UI控件
        /// </summary>
        /// <param name="gridSize">棋盘网格尺寸</param>
        /// <param name="idx">同类型棋子出现次数，用于索引棋子UI图片等</param>
        /// <param name="left">距离上级容器左边距</param>
        /// <param name="top">距离上级容器上边距</param>
        public virtual void CreateElement(double gridSize, int idx, double left, double top)
        { 
        }

        /// <summary>
        /// 棋子是否可以移动
        /// </summary>
        /// <param name="blankPosition">空白格子的位置</param>
        /// <param name="gridRows">网格行数</param>
        /// <param name="gridColumns">网格列数</param>
        /// <returns>棋子是否可以移动</returns>
        public virtual bool CanMove(BlankPosition blankPosition, int gridRows, int gridColumns)
        {
            return this.CanMoveUp(blankPosition, gridRows, gridColumns) 
                || this.CanMoveDown(blankPosition, gridRows, gridColumns) 
                || this.CanMoveLeft(blankPosition, gridRows, gridColumns) 
                || this.CanMoveRight(blankPosition, gridRows, gridColumns);
        }

        /// <summary>
        /// 棋子是否可以向上移动一格
        /// </summary>
        /// <param name="blankPosition">空白格子的位置</param>
        /// <param name="gridRows">网格行数</param>
        /// <param name="gridColumns">网格列数</param>
        /// <returns>棋子是否可以向上移动一格</returns>
        public virtual bool CanMoveUp(BlankPosition blankPosition, int gridRows, int gridColumns)
        {
            return false;
        }

        /// <summary>
        /// 棋子是否可以向下移动一格
        /// </summary>
        /// <param name="blankPosition">空白格子的位置</param>
        /// <param name="gridRows">网格行数</param>
        /// <param name="gridColumns">网格列数</param>
        /// <returns>棋子是否可以向下移动一格</returns>
        public virtual bool CanMoveDown(BlankPosition blankPosition, int gridRows, int gridColumns)
        {
            return false;
        }

        /// <summary>
        /// 棋子是否可以向左移动一格
        /// </summary>
        /// <param name="blankPosition">空白格子的位置</param>
        /// <param name="gridRows">网格行数</param>
        /// <param name="gridColumns">网格列数</param>
        /// <returns>棋子是否可以向左移动一格</returns>
        public virtual bool CanMoveLeft(BlankPosition blankPosition, int gridRows, int gridColumns)
        {
            return false;
        }

        /// <summary>
        /// 棋子是否可以向右移动一格
        /// </summary>
        /// <param name="blankPosition">空白格子的位置</param>
        /// <param name="gridRows">网格行数</param>
        /// <param name="gridColumns">网格列数</param>
        /// <returns>棋子是否可以向右移动一格</returns>
        public virtual bool CanMoveRight(BlankPosition blankPosition, int gridRows, int gridColumns)
        {
            return false;
        }

        /// <summary>
        /// 设置棋子可移动至的新位置
        /// </summary>
        /// <param name="moveDirection">移动方向</param>
        /// <param name="gridColumns">网格列数</param>
        /// <param name="blankPosition">空白网格</param>
        /// <param name="callBack">设置完成后的回调</param>
        public virtual void SetNewPosition(Direction moveDirection, int gridColumns, BlankPosition blankPosition, SetNewPositionDelegate callBack = null)
        {
            this.NextPosition = this.Position;

            if (moveDirection == Direction.Up)
                this.NextPosition -= gridColumns;

            if (moveDirection == Direction.Down)
                this.NextPosition += gridColumns;

            if (moveDirection == Direction.Left)
                this.NextPosition -= 1;

            if (moveDirection == Direction.Right)
                this.NextPosition += 1;

            if (callBack != null)
                callBack(this, moveDirection, blankPosition);
        }

        /// <summary>
        /// 移动棋子（该方法用于玩家模式）并返回新的空白棋子位置
        /// </summary>
        /// <param name="blankPosition">当前空白棋子位置</param>
        /// <param name="moveDirection">当前棋子移动方向</param>
        /// <param name="gridColumns">网格列数</param>
        /// <returns>新的空白棋子位置</returns>
        public virtual BlankPosition ChessMove(BlankPosition blankPosition, Direction moveDirection, int gridColumns)
        {
            return new BlankPosition { Position1 = blankPosition.Position1, Position2 = blankPosition.Position2 };
        }

        /// <summary>
        /// 尝试移动棋子（该方法用于自动求解）
        /// </summary>
        /// <param name="blankPosition">当前空白棋子位置</param>
        /// <param name="gridRows">网格行数</param>
        /// <param name="gridColumns">网格列数</param>
        /// <param name="callBack">设置完成后的回调</param>
        public virtual void TryChessMove(BlankPosition blankPosition, int gridRows, int gridColumns, SetNewPositionDelegate callBack)
        {
            if (this.CanMoveDown(blankPosition, gridRows, gridColumns))
                this.SetNewPosition(Direction.Down, gridColumns, blankPosition, callBack);
            
            if (this.CanMoveLeft(blankPosition, gridRows, gridColumns))
                this.SetNewPosition(Direction.Left, gridColumns, blankPosition, callBack);
            
            if (this.CanMoveRight(blankPosition, gridRows, gridColumns))
                this.SetNewPosition(Direction.Right, gridColumns, blankPosition, callBack);

            if (this.CanMoveUp(blankPosition, gridRows, gridColumns))
                this.SetNewPosition(Direction.Up, gridColumns, blankPosition, callBack);
        }
    }
}