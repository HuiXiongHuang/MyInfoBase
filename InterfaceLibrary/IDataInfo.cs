﻿using System;
using System.Windows.Controls;
using System.Windows.Media;
namespace InterfaceLibrary
{
    /// <summary>
    /// 代表每种节点类型必须封装的基本数据信息
    /// </summary>
    public interface IDataInfo
    {
        /// <summary>
        /// 本节点数据信息是否己经从底层存储机构中提取
        /// </summary>
        bool HasBeenLoadFromStorage { get; set; }
        long ID { get; set; }
        String Path { get; set; }
        /// <summary>
        /// 创建或修改的时间
        /// </summary>
        DateTime ModifyTime { get; set; }
        /// <summary>
        /// 节点类型
        /// </summary>
        String NodeType { get; }
        /// 图标类型
        /// </summary>
        String IconType { get; set; }
        /// <summary>
        /// 节点名称
        /// </summary>
        string NodeText { get; set; }
        /// <summary>
        /// 正常情况下的图标
        /// </summary>
        ImageSource NormalIcon { get; }
        /// <summary>
        /// 选中状态下的图标
        /// </summary>
        ImageSource SelectedIcon { get; }
        /// <summary>
        /// 新图标
        /// </summary>
        ImageSource FileIcon { get; }
        /// <summary>
        /// 本节点UI界面的最顶层控件，通常为ContentControl或ItemsControl
        /// </summary>
        Control RootControl { get; }
        /// <summary>
        /// 是否应该将自己的界面嵌入到主程序的UI界面中（有些节点可能采用弹出独立窗口的方式）
        /// </summary>
        bool ShouldEmbedInHostWorkingBench { get; }



        /// <summary>
        /// 刷新显示
        /// </summary>
        void RefreshDisplay();
        /// <summary>
        /// 将数据对象与UI绑定
        /// </summary>
        void BindToRootControl();
        /// <summary>
        /// 强制刷新数据对象（通常是使用用户界面上控件的当前值刷新本对象的字段值）
        /// </summary>
        void RefreshMe();
        /// <summary>
        /// 设置节点所关联的可视化界面的数据存取对象
        /// </summary>
        /// <param name="accessObj"></param>
        void SetRootControlDataAccessObj(IDataAccess accessObj);
        /// <summary>
        /// 提供一个主窗体的引用，节点可以使用它来访问主窗体的特定功能，比如在主窗体的状态栏上显示信息
        /// </summary>
        IMainWindowFunction MainWindow { get; set; }
    }
}
