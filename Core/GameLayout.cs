using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Animation;
using WPF.HRD.Core.Chess;

namespace WPF.HRD.Core
{
    /// <summary>
    /// 面向游戏玩家的棋盘布局类
    /// </summary>
    public class GameLayout
    {
        /// <summary>
        /// 棋盘网格尺寸
        /// </summary>
        private const int GridSize = 100;

        /// <summary>
        /// 当前布局UINT64数值
        /// </summary>
        private long LayoutCode = 0;

        /// <summary>
        /// 布局棋子实例集合
        /// </summary>
        private List<ChessBase> ChessList = null;

        /// <summary>
        /// 空白棋子位置
        /// </summary>
        public BlankPosition BlankPosition { get; set; }

        /// <summary>
        /// 棋局完成后的回调
        /// </summary>
        public LayoutCompleted LayoutCompleted { get; set; }

        /// <summary>
        /// 创建面向游戏玩家的棋盘布局新实例
        /// </summary>
        /// <param name="layoutCode">布局数值</param>
        /// <param name="blankPosition1">空白棋子1位置</param>
        /// <param name="blankPosition2">空白棋子2位置</param>
        public GameLayout(long layoutCode, int blankPosition1, int blankPosition2)
        {
            this.LayoutCode = layoutCode;
            this.ChessList = new List<ChessBase>(0);
            this.BlankPosition = new BlankPosition { Position1 = blankPosition1, Position2 = blankPosition2 };
        }

        /// <summary>
        /// 初始化布局
        /// </summary>
        /// <param name="container"></param>
        public void Draw(Panel container)
        {
            IDictionary<ChessType, int> chessCountDict = new Dictionary<ChessType, int>(4)
            {
                {ChessType.Square, 0}, {ChessType.HRect, 0}, {ChessType.VRect, 0 }, {ChessType.Block, 0}
            };
            ChessType chessType;
            long tempCode = 0;
            for (int r = 0; r < Common.GridRows; r++)
            {
                for (int c = 0; c < Common.GridColumns; c++)
                {
                    int idx = r * Common.GridColumns + c;
                    tempCode = Common.ChessBit << (idx * 3);
                    chessType = (ChessType)((int)((tempCode & this.LayoutCode) >> (idx * 3)));
                    if (chessType != ChessType.Blank)
                    {
                        ChessBase currentChess = chessType.CreateChess();
                        currentChess.Position = idx;
                        currentChess.NextPosition = idx;
                        currentChess.CreateElement(GridSize, chessCountDict[chessType], (double)(c * GridSize), (double)(r * GridSize));
                        currentChess.Element.DragCompleted += new DragCompletedEventHandler((sender, e) =>
                        {
                            Direction moveDirection = this.GetChessMoveDirection(currentChess, e.HorizontalChange, e.VerticalChange);
                            if (moveDirection != Direction.Hold)
                                currentChess.SetNewPosition(moveDirection, Common.GridColumns, this.BlankPosition, this.MoveChessToNext);
                        });
                        container.Children.Add(currentChess.Element);
                        this.ChessList.Add(currentChess);
                        chessCountDict[chessType]++;
                    }
                }
            }
        }

        /// <summary>
        /// 终止游戏
        /// </summary>
        public void Abort()
        {
            Dispose();
        }

        /// <summary>
        /// 获取棋子可移动方向
        /// </summary>
        /// <param name="chess">棋子对象</param>
        /// <param name="moveX">移动横向距离</param>
        /// <param name="moveY">移动纵向距离</param>
        /// <returns>棋子可移动方向</returns>
        private Direction GetChessMoveDirection(ChessBase chess, double moveX, double moveY)
        {
            double absX = Math.Abs(moveX);
            double absY = Math.Abs(moveY);
            Direction moveDiretion = Direction.Hold;
            if (absX > 0 || absY > 0)
            {
                if (absX >= absY)
                {
                    if (moveX > 0 && chess.CanMoveRight(this.BlankPosition, Common.GridRows, Common.GridColumns))
                        moveDiretion = Direction.Right;

                    if (moveX < 0 && chess.CanMoveLeft(this.BlankPosition, Common.GridRows, Common.GridColumns))
                        moveDiretion = Direction.Left;
                }
                else
                {
                    if (moveY > 0 && chess.CanMoveDown(this.BlankPosition, Common.GridRows, Common.GridColumns))
                        moveDiretion = Direction.Down;

                    if (moveY < 0 && chess.CanMoveUp(this.BlankPosition, Common.GridRows, Common.GridColumns))
                        moveDiretion = Direction.Up;
                }
            }
            return moveDiretion;
        }

        /// <summary>
        /// 移动棋子到新的位置
        /// </summary>
        /// <param name="chess">待移动的棋子</param>
        /// <param name="moveDirection">移动方向</param>
        /// <param name="newBlankPosition">新的空白格子</param>
        private void MoveChessToNext(ChessBase chess, Direction moveDirection, BlankPosition newBlankPosition)
        {
            int oldPosition = chess.Position;
            int newPosition = chess.NextPosition;
            this.BlankPosition = chess.ChessMove(newBlankPosition, moveDirection, Common.GridColumns);
            chess.Position = chess.NextPosition;
            this.MoveAnimation(chess.Element, newPosition, oldPosition, moveDirection);
        }

        /// <summary>
        /// 棋子移动动画
        /// </summary>
        /// <param name="element"></param>
        /// <param name="chessNewPosition"></param>
        /// <param name="chessOldPosition"></param>
        /// <param name="moveDirection"></param>
        private void MoveAnimation(FrameworkElement element, int chessNewPosition, int chessOldPosition, Direction moveDirection)
        {
            double from = 0, to = 0;
            string propertyPathName = string.Empty;
            if (moveDirection == Direction.Up || moveDirection == Direction.Down)
            {
                from = ((int)(chessOldPosition / Common.GridColumns)) * GridSize;
                to = ((int)(chessNewPosition / Common.GridColumns)) * GridSize;
                propertyPathName = "(Canvas.Top)";
            }
            if (moveDirection == Direction.Left || moveDirection == Direction.Right)
            {
                from = ((int)(chessOldPosition % Common.GridColumns)) * GridSize;
                to = ((int)(chessNewPosition % Common.GridColumns)) * GridSize;
                propertyPathName = "(Canvas.Left)";
            }

            Storyboard storyboard = new Storyboard();
            DoubleAnimation da = new DoubleAnimation();
            da.From = from;
            da.To = to;
            da.Duration = new Duration(TimeSpan.FromMilliseconds(200d));
            Storyboard.SetTarget(da, element);
            Storyboard.SetTargetProperty(da, new PropertyPath(propertyPathName));
            storyboard.Children.Add(da);
            storyboard.Completed += new EventHandler((sender, e) =>
            {
                if (this.ChessList.LayoutFinished())
                {
                    this.LayoutCompleted();
                    this.Dispose();
                }
            });
            storyboard.Begin();
        }

        /// <summary>
        /// 释放相关资源
        /// </summary>
        private void Dispose()
        {
            this.ChessList.Clear();
            this.ChessList = null;
            this.BlankPosition = new BlankPosition { Position1 = -1, Position2 = -1 };
            this.LayoutCompleted = null;
            GC.Collect();
        }
    }
}