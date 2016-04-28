namespace WPF.HRD.Core
{
    /// <summary>
    /// 棋子移动方向枚举
    /// </summary>
    public enum Direction
    {
        /// <summary>
        /// 保持原位不动
        /// </summary>
        Hold = 0,

        /// <summary>
        /// 向上移动一格
        /// </summary>
        Up = 1,

        /// <summary>
        /// 向下移动一格
        /// </summary>
        Down = 2,

        /// <summary>
        /// 向左移动一格
        /// </summary>
        Left = 3,

        /// <summary>
        /// 向右移动一格
        /// </summary>
        Right = 4
    }
}