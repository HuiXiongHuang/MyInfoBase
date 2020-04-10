﻿using System;

namespace WPFSuperTreeView
{
    public class NodeMoveEventArgs : EventArgs
    {
        /// <summary>
        /// 被操作的节点
        /// </summary>
        public TreeViewIconsItem Node { get; set; }
        public String PrevPath { get; set; }
        public NodeMoveType MoveType { get; set; }
    }
}
