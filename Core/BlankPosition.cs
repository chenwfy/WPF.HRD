namespace WPF.HRD.Core
{
    /// <summary>
    /// 空白棋子所在位置（每个布局有且仅有2个空白网格）
    /// </summary>
    public struct BlankPosition
    {
        /// <summary>
        /// 第一个空白网格位置
        /// </summary>
        public int Position1 { get; set; }

        /// <summary>
        /// 第二个空白网格位置
        /// </summary>
        public int Position2 { get; set; }
    }
}