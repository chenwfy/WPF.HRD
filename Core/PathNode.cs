namespace WPF.HRD.Core
{
    /// <summary>
    /// 路径节点
    /// </summary>
    public struct PathNode
    {
        /// <summary>
        /// 父级布局UINT64数值
        /// </summary>
        public long ParentCode { get; set; }

        /// <summary>
        /// 当前布局UINT64数值
        /// </summary>
        public long CurrentCode { get; set; }

        /// <summary>
        /// 当前布局空白棋子位置
        /// </summary>
        public BlankPosition BlankPosition { get; set; }

        /// <summary>
        /// 当前布局是否为解法最后布局
        /// </summary>
        public bool IsLast { get; set; }
    }
}