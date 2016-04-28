using System;
using System.Linq;
using System.Collections.Generic;
using WPF.HRD.Core.Chess;

namespace WPF.HRD.Core
{
    /// <summary>
    /// 自动求解类
    /// </summary>
    public class PathFinder
    {
        /// <summary>
        /// 寻路步骤最大递归次数，如果超过该次数仍未得解，则认为该局无解
        /// </summary>
        private const int StepMax = 60000;
        
        /// <summary>
        /// 存放自动求解布局数值的字典，键为当前布局数值，值为衍生出当前布局的上一次布局数值
        /// </summary>
        private IDictionary<long, long> StepCodeDict = null;
        
        /// <summary>
        /// 本局是否已经得解
        /// </summary>
        private bool IsGetResult = false;

        /// <summary>
        /// 初始布局数值
        /// </summary>
        private long InitLayoutCode = 0;

        /// <summary>
        /// 初始布局空白棋子位置
        /// </summary>
        private BlankPosition InitBlankPosition = new BlankPosition { Position1 = -1, Position2 = -1 };

        /// <summary>
        /// 下一步可能产生的解局走法节点集合
        /// </summary>
        private List<PathNode> NextNodeList = null;

        /// <summary>
        /// 当前正在进行尝试的棋子索引
        /// </summary>
        private int ChessIndex = 0;

        /// <summary>
        /// 当前正在尝试求解下一步的解法节点
        /// </summary>
        private PathNode CurrentNode;

        /// <summary>
        /// 构造初始布局
        /// </summary>
        /// <param name="layoutCode">布局数值</param>
        /// <param name="blankPosition1">空白棋子1位置</param>
        /// <param name="blankPosition2">空白棋子2位置</param>
        public PathFinder(long layoutCode, int blankPosition1, int blankPosition2)
        {
            this.StepCodeDict = new Dictionary<long, long>(0);
            this.InitLayoutCode = layoutCode;
            this.InitBlankPosition = new BlankPosition { Position1 = blankPosition1, Position2 = blankPosition2 };
        }

        /// <summary>
        /// 获取解局步骤列表
        /// </summary>
        /// <returns></returns>
        public IList<long> FindPath()
        {
            this.StepCodeDict.Add(this.InitLayoutCode, 0);
            PathNode rootNode = new PathNode { ParentCode = 0, CurrentCode = this.InitLayoutCode, BlankPosition = this.InitBlankPosition };
            this.NextStep(new List<PathNode> { rootNode });

            List<long> stepList = new List<long>(0);
            if (this.IsGetResult && this.StepCodeDict.Count > 0)
            {
                long lastCode = this.StepCodeDict.Last().Key;
                stepList.Add(lastCode);
                long prevCode = this.StepCodeDict[lastCode];
                while (prevCode > 0)
                {
                    stepList.Add(prevCode);
                    prevCode = this.StepCodeDict[prevCode];
                }
            }
            this.Dispose();
            return stepList;
        }

        /// <summary>
        /// 自动求解寻路
        /// </summary>
        /// <param name="nodeList"></param>
        private void NextStep(List<PathNode> nodeList)
        {
            if (this.StepCodeDict.Count >= StepMax)
                return;
            if (this.IsGetResult)
                return;

            List<PathNode> nextNodes = new List<PathNode>(0);
            foreach (var node in nodeList)
            {
                NextNode(node);
                if (this.NextNodeList.Count > 0)
                {
                    foreach (var item in this.NextNodeList)
                    {
                        if (!this.StepCodeDict.Keys.Contains(item.CurrentCode))
                        {
                            this.StepCodeDict.Add(item.CurrentCode, item.ParentCode);
                            nextNodes.Add(item);
                            if (item.IsLast)
                            {
                                this.IsGetResult = true;
                                break;
                            }
                        }
                    }
                }
                if (this.IsGetResult)
                    break;
            }
            if (nextNodes.Count == 0)
                return;
            NextStep(nextNodes);
        }

        /// <summary>
        /// 获取所有可能产生的下一步步骤集合
        /// </summary>
        /// <param name="currentNode"></param>
        /// <returns></returns>
        private void NextNode(PathNode currentNode)
        {
            this.CurrentNode = currentNode;
            this.NextNodeList = new List<PathNode>(0);
            List<ChessBase> chessList = currentNode.CurrentCode.GetChessList();
            for (int idx = 0; idx < chessList.Count; idx++)
            {
                if (chessList[idx].CanMove(currentNode.BlankPosition, Common.GridRows, Common.GridColumns))
                {
                    this.ChessIndex = idx;
                    chessList[idx].TryChessMove(currentNode.BlankPosition, Common.GridRows, Common.GridColumns, this.TryMoveToNext);
                }
            }
        }

        /// <summary>
        /// 尝试将棋子移动到下一个指定的位置
        /// </summary>
        /// <param name="chess">待移动的棋子</param>
        /// <param name="moveDirection">移动方向</param>
        /// <param name="newBlankPosition">新的空白格子</param>
        private void TryMoveToNext(ChessBase chess, Direction moveDirection, BlankPosition newBlankPosition)
        {
            List<ChessBase> chessList = this.CurrentNode.CurrentCode.GetChessList();
            chessList[this.ChessIndex].Position = chess.NextPosition;
            long nodeCode = chessList.GetLayoutCode();
            if (nodeCode != this.CurrentNode.ParentCode && !this.StepCodeDict.Keys.Contains(nodeCode))
            {
                PathNode node = new PathNode
                {
                    CurrentCode = nodeCode,
                    ParentCode = this.CurrentNode.CurrentCode,
                    //BlankPosition = chess.ChessMove(this.CurrentNode.BlankPosition, moveDirection, Common.GridColumns),
                    BlankPosition = chess.ChessMove(newBlankPosition, moveDirection, Common.GridColumns),
                    IsLast = chessList.LayoutFinished()
                };
                this.NextNodeList.Add(node);
            }
        }

        /// <summary>
        /// 释放相关资源
        /// </summary>
        private void Dispose()
        {
            this.StepCodeDict.Clear();
            this.StepCodeDict = null;
            this.InitLayoutCode = 0;
            this.InitBlankPosition = new BlankPosition { Position1 = -1, Position2 = -1 };
            this.NextNodeList.Clear();
            this.NextNodeList = null;
            GC.Collect();
        }
    }
}