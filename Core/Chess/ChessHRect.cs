using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace WPF.HRD.Core.Chess
{
    /// <summary>
    /// 1*2（横向，1行2列）棋子
    /// </summary>
    public class ChessHRect : ChessBase
    {
        /// <summary>
        /// 棋子可用资源数组
        /// </summary>
        private static readonly Color[] ChessResources = new Color[5] 
        { 
            Color.FromRgb(7, 180, 240), 
            Color.FromRgb(6, 140, 200), 
            Color.FromRgb(5, 100, 160), 
            Color.FromRgb(4, 60, 120), 
            Color.FromRgb(3, 20, 80) 
        };

        /// <summary>
        /// 创建1*2（横向，1行2列）棋子新实例
        /// </summary>
        public ChessHRect()
            : base(ChessType.HRect)
        { 
        }

        /// <summary>
        /// 创建棋子UI控件
        /// </summary>
        /// <param name="gridSize">棋盘网格尺寸</param>
        /// <param name="idx">同类型棋子出现次数，用于索引棋子UI图片等</param>
        /// <param name="left">距离上级容器左边距</param>
        /// <param name="top">距离上级容器上边距</param>
        public override void CreateElement(double gridSize, int idx, double left, double top)
        {
            Thumb element = new Thumb();
            element.Name = string.Format("Chess_HRect_{0}", idx);
            element.Width = gridSize * 2;
            element.Height = gridSize;
            element.Background = new SolidColorBrush(ChessResources[idx % ChessResources.Length]);
            element.BorderBrush = new SolidColorBrush(ChessResources[idx % ChessResources.Length]);
            element.SetValue(Canvas.LeftProperty, left);
            element.SetValue(Canvas.TopProperty, top);
            this.Element = element;
        }

        /// <summary>
        /// 棋子是否可以向上移动一格
        /// </summary>
        /// <param name="blankPosition">空白格子的位置</param>
        /// <param name="gridRows">网格行数</param>
        /// <param name="gridColumns">网格列数</param>
        /// <returns>棋子是否可以向上移动一格</returns>
        public override bool CanMoveUp(BlankPosition blankPosition, int gridRows, int gridColumns)
        {
            if (this.Position < gridColumns)
                return false;
            int temp = this.Position - gridColumns;
            int temp2 = temp + 1;
            return (temp == blankPosition.Position1 && temp2 == blankPosition.Position2) || (temp == blankPosition.Position2 && temp2 == blankPosition.Position1);
        }

        /// <summary>
        /// 棋子是否可以向下移动一格
        /// </summary>
        /// <param name="blankPosition">空白格子的位置</param>
        /// <param name="gridRows">网格行数</param>
        /// <param name="gridColumns">网格列数</param>
        /// <returns>棋子是否可以向下移动一格</returns>
        public override bool CanMoveDown(BlankPosition blankPosition, int gridRows, int gridColumns)
        {
            if (this.Position >= (gridRows - 1) * gridColumns)
                return false;
            int temp = this.Position + gridColumns;
            int temp2 = temp + 1;
            return (temp == blankPosition.Position1 && temp2 == blankPosition.Position2) || (temp == blankPosition.Position2 && temp2 == blankPosition.Position1);
        }

        /// <summary>
        /// 棋子是否可以向左移动一格
        /// </summary>
        /// <param name="blankPosition">空白格子的位置</param>
        /// <param name="gridRows">网格行数</param>
        /// <param name="gridColumns">网格列数</param>
        /// <returns>棋子是否可以向左移动一格</returns>
        public override bool CanMoveLeft(BlankPosition blankPosition, int gridRows, int gridColumns)
        {
            if (this.Position % gridColumns == 0)
                return false;
            int temp = this.Position - 1;
            return temp == blankPosition.Position1 || temp == blankPosition.Position2;
        }

        /// <summary>
        /// 棋子是否可以向右移动一格
        /// </summary>
        /// <param name="blankPosition">空白格子的位置</param>
        /// <param name="gridRows">网格行数</param>
        /// <param name="gridColumns">网格列数</param>
        /// <returns>棋子是否可以向右移动一格</returns>
        public override bool CanMoveRight(BlankPosition blankPosition, int gridRows, int gridColumns)
        {
            if (this.Position % gridColumns >= 2)
                return false;
            int temp = this.Position + 2;
            return temp == blankPosition.Position1 || temp == blankPosition.Position2;
        }

        /// <summary>
        /// 移动棋子（该方法用于玩家模式）并返回新的空白棋子位置
        /// </summary>
        /// <param name="blankPosition">当前空白棋子位置</param>
        /// <param name="moveDirection">当前棋子移动方向</param>
        /// <param name="gridColumns">网格列数</param>
        /// <returns>新的空白棋子位置</returns>
        public override BlankPosition ChessMove(BlankPosition blankPosition, Direction moveDirection, int gridColumns)
        {
            if (moveDirection == Direction.Up || moveDirection == Direction.Down)
            {
                if (blankPosition.Position1 == this.NextPosition)
                {
                    blankPosition.Position1 = this.Position;
                    blankPosition.Position2 = this.Position + 1;
                }
                else
                {
                    blankPosition.Position1 = this.Position + 1;
                    blankPosition.Position2 = this.Position;
                }
            }

            if (moveDirection == Direction.Left)
            {
                if (blankPosition.Position1 == this.NextPosition)
                    blankPosition.Position1 = this.Position + 1;
                else
                    blankPosition.Position2 = this.Position + 1;
            }

            if (moveDirection == Direction.Right)
            {
                if (blankPosition.Position1 == this.NextPosition + 1)
                    blankPosition.Position1 = this.Position;
                else
                    blankPosition.Position2 = this.Position;
            }

            return new BlankPosition { Position1 = blankPosition.Position1, Position2 = blankPosition.Position2 };
        }

        /// <summary>
        /// 尝试移动棋子（该方法用于自动求解）
        /// </summary>
        /// <param name="blankPosition">当前空白棋子位置</param>
        /// <param name="gridRows">网格行数</param>
        /// <param name="gridColumns">网格列数</param>
        /// <param name="callBack">设置完成后的回调</param>
        public override void TryChessMove(BlankPosition blankPosition, int gridRows, int gridColumns, SetNewPositionDelegate callBack)
        {
            BlankPosition tmpBlankPosition;

            //下
            if (this.CanMoveDown(blankPosition, gridRows, gridColumns))
            {
                base.SetNewPosition(Direction.Down, gridColumns, blankPosition, callBack);
            }

            //左
            if (this.CanMoveLeft(blankPosition, gridRows, gridColumns))
            {
                base.SetNewPosition(Direction.Left, gridColumns, blankPosition, callBack);
                tmpBlankPosition = ChessMove(blankPosition, Direction.Left, gridColumns);
                this.Position = this.NextPosition;
                
                //连续左2
                if (this.CanMoveLeft(tmpBlankPosition, gridRows, gridColumns))
                {
                    base.SetNewPosition(Direction.Left, gridColumns, tmpBlankPosition, callBack);
                    return;
                }              
            }

            //右
            if (this.CanMoveRight(blankPosition, gridRows, gridColumns))
            {
                base.SetNewPosition(Direction.Right, gridColumns, blankPosition, callBack);
                tmpBlankPosition = ChessMove(blankPosition, Direction.Right, gridColumns);
                this.Position = this.NextPosition;
                
                //连续右2
                if (this.CanMoveRight(tmpBlankPosition, gridRows, gridColumns))
                {
                    base.SetNewPosition(Direction.Right, gridColumns, tmpBlankPosition, callBack);
                    return;
                }
            }

            //上
            if (this.CanMoveUp(blankPosition, gridRows, gridColumns))
            {
                base.SetNewPosition(Direction.Up, gridColumns, blankPosition, callBack);
            }
        }
    }
}