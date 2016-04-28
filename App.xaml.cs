using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

namespace WPF.HRD
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// 当前布局UINT64数值
        /// </summary>
        public static long CurrentLayoutCode { get; set; }
        /// <summary>
        /// 当前布局的空白棋子所在位置数组
        /// </summary>
        public static int[] CurrentBlankPositions { get; set; }
    }
}