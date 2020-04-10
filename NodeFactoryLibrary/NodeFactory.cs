using InfoNode;
using InterfaceLibrary;
using LabelNode;
using System;

namespace NodeFactoryLibrary
{
    /// <summary>
    /// 依据节点类型，创建相应的默认对象
    /// </summary>
    public class NodeFactory
    {
        /// <summary>
        /// 用于引用主窗体，节点可以使用此引用调用主窗体所提供的功能（比如显示信息）
        /// </summary>
        public static IMainWindowFunction _mainWindow = null;
        /// <summary>
        /// 依据节点类型创建"空白"的数据信息对象，其HasBeenLoadFromStorage属性为false
        /// （无需数据存储的节点，如OnlyText例此，其HasBeenLoadFromStorage始终为true）
        /// </summary>
        /// <param name="treeNodeType"></param>
        /// <returns></returns>
        public static NodeDataObject CreateDataInfoNode(String treeNodeType, String EFConnectionString)
        {
            NodeDataObject nodeDataObject = new NodeDataObject();

            if (treeNodeType == "InfoNode")
            {
                InfoNodeDataInfo info = new InfoNodeDataInfo() { MainWindow = _mainWindow };
                InfoNodeAccess access = new InfoNodeAccess(EFConnectionString);
                info.SetRootControlDataAccessObj(access);

                nodeDataObject.DataItem = info;
                //设置数据未装入标记
                nodeDataObject.DataItem.HasBeenLoadFromStorage = false;

                nodeDataObject.AccessObject = access;

            }
            if (treeNodeType == "LabelNode")
            {
                LabelNodeDataInfo info = new LabelNodeDataInfo() { MainWindow = _mainWindow };
                LabelNodeAccess accessObj = new LabelNodeAccess(EFConnectionString);
                info.SetRootControlDataAccessObj(accessObj);
                nodeDataObject.DataItem = info;
                //设置数据未装入标记
                nodeDataObject.DataItem.HasBeenLoadFromStorage = false;
                nodeDataObject.AccessObject = accessObj;
            }
            return nodeDataObject;
        }
        /// <summary>
        /// 依据数据库连接字串（EntityFramework格式）创建合适的IDataAcess对象
        /// </summary>
        /// <param name="treeNodeType"></param>
        /// <param name="EFConnectionString"></param>
        /// <returns></returns>
        public static IDataAccess CreateNodeAccessObject(String treeNodeType, String EFConnectionString)
        {
            NodeDataObject nodeDataObject = new NodeDataObject();

            if (treeNodeType == "InfoNode")
            {
                return new InfoNodeAccess(EFConnectionString);
            }
            if (treeNodeType == "LabelNode")
            {
                return new LabelNodeAccess(EFConnectionString);
            }
            return null;
        }
    }
}
