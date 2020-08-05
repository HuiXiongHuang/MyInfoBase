
using Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Entity;
using System.Linq;
using EntityState = System.Data.Entity.EntityState;

namespace DataAccessLayer.LabelNodeDA
{
    /// <summary>
    /// 完成信息节点的增删改查功能
    /// </summary>
    public class LabelNodeRepository : BaseRepository<MyDBEntitiesSqlite>
    {
        private String EFConnectionString = "";


        //public InfoNodeRepository(MyDBEntities dbContext) :
        //   base(dbContext)
        //{

        //}
        public LabelNodeRepository()
            : base(new MyDBEntitiesSqlite(""))
        {

        }
        public LabelNodeRepository(string efConnectionString)
            : base(new MyDBEntitiesSqlite(efConnectionString))
        {
            EFConnectionString = efConnectionString;
            //_dbContext = new MyDBEntities(EFConnectionString);

        }
 #region 更新数据库
        /// <summary>
        /// 更新节点的路径：查找所有路径以oldPath打头的记录，将其路径替换为以newPath打头
        /// 有效路径前后应该以“/”包围
        /// </summary>
        /// <param name="oldPath"></param>
        /// <param name="newPath"></param>
        public void UpdateNodePaths(String oldPath, String newPath)
        {

            var query = from item in _dbContext.LabelNodeDBs
                        where item.Path.StartsWith(oldPath)
                        select item;
            foreach (var item in query)
            {
                item.Path = item.Path.Replace(oldPath, newPath);
            }

            //_dbContext.SaveChanges();
            using (MyDBEntitiesSqlite context = new MyDBEntitiesSqlite(EFConnectionString))
            {
                _dbContext.SaveChanges();
            }

         }

        /// <summary>
        /// 向数据库中添加一条记录
        /// </summary>
        /// <param name="LabelNodeObj"></param>
        /// <returns></returns>
        public int AddLabelNodeDB(LabelNodeDB LabelNodeObj)
        {
            if (LabelNodeObj == null)
            {
                return 0;
            }
            //_dbContext.LabelNodeDBs.Add(LabelNodeObj);
            //return _dbContext.SaveChanges()
            using (MyDBEntitiesSqlite context = new MyDBEntitiesSqlite(EFConnectionString))
            {
                context.LabelNodeDBs.Add(LabelNodeObj);
                return context.SaveChanges();

            }
        }
        /// <summary>
        /// 更新节点的信息
        /// </summary>
        /// <param name="LabelNodeObj"></param>
        public int UpdateLabelNodeDB(LabelNodeDB LabelNodeObj)
        {
            if (LabelNodeObj == null)
            {
                return 0;
            }

            //LabelNodeDB LabelNodeToModify = _dbContext.LabelNodeDBs.FirstOrDefault(p => p.ID == LabelNodeObj.ID);
            //if (LabelNodeToModify != null)
            //{
            //    LabelNodeToModify.ModifyTime = LabelNodeObj.ModifyTime;
            //    LabelNodeToModify.Label = LabelNodeObj.Label;
            //    return _dbContext.SaveChanges();
            //}
            using (MyDBEntitiesSqlite context = new MyDBEntitiesSqlite(EFConnectionString))
            {
                LabelNodeDB LabelNodeToModify = context.LabelNodeDBs.FirstOrDefault(p => p.ID == LabelNodeObj.ID);
                if (LabelNodeToModify != null)
                {
                    LabelNodeToModify.ModifyTime = LabelNodeObj.ModifyTime;
                  //  LabelNodeToModify.Label = LabelNodeObj.Label;
                    return context.SaveChanges();
                }
            }
                return 0;

        }
        #endregion
        #region "提取信息"
        #region 备用
        /// <summary>
        /// 提取所有的节点（包括其相关联的信息节点）

        /// </summary>
        /// <returns></returns>
        public List<LabelNodeDB> GetAllLabelNodeDBWithItsInfoNode()
        {
            using (MyDBEntitiesSqlite context = new MyDBEntitiesSqlite(EFConnectionString))
            {
                var query = from item in context.LabelNodeDBs.Include("InfoNodeDBs")
                            select item;
                return query.ToList();
            }

        }

        /// <summary>
        /// 提取所有的节点（不包括其相关联的信息节点）
        ///
        /// </summary>
        /// <returns></returns>
        public List<LabelNodeDB> GetAllLabelNodeDBWithoutItsInfoNodeDB()
        {
            using (MyDBEntitiesSqlite context = new MyDBEntitiesSqlite(EFConnectionString))
            {
                var query = from item in context.LabelNodeDBs
                            select item;
                return query.ToList();
            }
        }

        /// <summary>
        /// 按照路径提取信息节点对象(仅提取第一个匹配的），找不到，返回null
        /// 获取的标签节点对象包容其所有的关联信息节点
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public LabelNodeDB GetLabelNodeDBWithInfoNodeDBByPath(String path)
        {
            if (String.IsNullOrEmpty(path))
            {
                return null;
            }
            using (MyDBEntitiesSqlite context = new MyDBEntitiesSqlite(EFConnectionString))
            {
                return context.LabelNodeDBs.Include("InfoNodeDBs").FirstOrDefault(p => p.Path == path);
            }
        }
        #endregion

        /// <summary>
        /// 依据路径提取标签节点的数据，不包括其包容的信息节点，
        /// 此方法应该与GetDBInfoNodeInfosOfLabelNodeDB()结合起来，向外界返回真正有用的信息
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public LabelNodeDB GetLabelNodeDBWithoutInfoNodeDBsByPath(String path)
        {
            if (String.IsNullOrEmpty(path))
            {
                return null;
            }
            using (MyDBEntitiesSqlite context = new MyDBEntitiesSqlite(EFConnectionString))
            {
                return context.LabelNodeDBs.FirstOrDefault(p => p.Path == path);
            }
           // return _dbContext.InfoNodeDBs.FirstOrDefault(p => p.Path == path);
        }
        /// <summary>
        /// 按照Path提取相应节点所有的信息节点信息(是DBInfoNodeInfo对象）SqlCeException: 在 WHERE、HAVING、GROUP BY、ON 或 IN 子句中不能使用 ntext 和 image 数据类型，除非将这些数据类型与 LIKE 或 IS NULL 谓词一起使用。
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public ObservableCollection<DBInfoNodeInfo> GetDBInfoNodeInfosOfLabelNodeDB(String path)
        {
            if (String.IsNullOrEmpty(path))
            {
                return null;
            }
            ObservableCollection<DBInfoNodeInfo> infoNodeInfos = new ObservableCollection<DBInfoNodeInfo>();
            using (MyDBEntitiesSqlite context = new MyDBEntitiesSqlite(EFConnectionString))
            {
                var query = from item in context.LabelNodeDBs
                            where item.Path == path
                            select item;
                LabelNodeDB LabelNodeObj = query.FirstOrDefault();
                if (LabelNodeObj != null)
                {
                   
                        foreach (var info in LabelNodeObj.InfoNodeDBs)
                        {
                            infoNodeInfos.Add(new DBInfoNodeInfo()
                            {
                                ModifyTime = info.ModifyTime.Value,
                                Path = info.Path,
                                InfoNodeHearder = info.Text,
                                ID = info.ID
                            });

                        }                  

                }
                return infoNodeInfos;
            }
        }
      
        #endregion

        #region "添加信息节点关联"

        /// <summary>
        /// 添加标签与信息节点的关联关系。
        /// </summary>
        /// <param name="LabelNodeDBPath"></param>
        /// <param name="infoNode"></param>
        public int AddInfoNodeOfLabelNodeDB(String LabelNodeDBPath, InfoNodeDB infoNode)
        {
            if (string.IsNullOrEmpty(LabelNodeDBPath) || infoNode == null)
            {
                return 0;
            }
            using (MyDBEntitiesSqlite context = new MyDBEntitiesSqlite(EFConnectionString))
            {
                LabelNodeDB LabelNodeObj = context.LabelNodeDBs.FirstOrDefault(p => p.Path == LabelNodeDBPath);
                if (LabelNodeObj == null)
                {
                    return 0;
                }
                //由于infoNode是之前从数据库中提取出来的实体数据，而不是新建的数据，所以，infoNode在EF中已经被标记有上下文，
                //通过引用赋值方式，并不能消除此上下文标记。此时如果需要在当前上下文中使用，必须先将它加入当前上下文中。
                //这种方式添加的关联会复制一份数据，不推荐。
                context.InfoNodeDBs.Add(infoNode);
                //以上。
                LabelNodeObj.InfoNodeDBs.Add(infoNode);
                return context.SaveChanges();
            }

        }
        /// <summary>
        /// 将多个信息节点追加到信息节点中
        /// 如果标签节点找不到，或者关联的信息节点为空集合，什么也不干，返回
        /// </summary>
        /// <param name="LabelNodePath"></param>
        /// <param name="infoNodes"></param>
        public void AddInfoNodesOfLabelNode(String LabelNodePath, List<InfoNodeDB> infoNodes)
        {
            if (string.IsNullOrEmpty(LabelNodePath) || infoNodes == null || infoNodes.Count == 0)
            {
                return;
            }
            using (MyDBEntitiesSqlite context = new MyDBEntitiesSqlite(EFConnectionString))
            {
                LabelNodeDB LabelNodeObj = context.LabelNodeDBs.FirstOrDefault(p => p.Path == LabelNodePath);
                if (LabelNodeObj == null)
                {
                    return;
                }
                foreach (var nodes in infoNodes)
                {
                    LabelNodeObj.InfoNodeDBs.Add(nodes);
                }
                context.SaveChanges();
            }
        }
        #endregion

        #region "删除操作"

        /// <summary>
        /// 按照路径删除标签节点及其关联关系，但不删除关联对象。哪怕设置了级联删除，将标签在数据库中删除，也仅会自动删除其关联关系，而不删除关联对象
        /// </summary>
        /// <param name="path"></param>
        public int DeleteLabelNodeDBAndItsChildByPath(String path)
        {
            using (MyDBEntitiesSqlite context = new MyDBEntitiesSqlite(EFConnectionString))
            {
                var query = from item in context.LabelNodeDBs
                            where item.Path.StartsWith(path)
                            select item;
                foreach (var LabelNodeObj in query)
                {

                    context.LabelNodeDBs.Remove(LabelNodeObj);
                }
                return context.SaveChanges();
            }

        }
            /// <summary>
            /// 接照路径删除指定的节点记录，其子节点数据不动（因为信息节点不能通过标签节点删除）
            /// </summary>
            /// <param name="path"></param>
            /// <returns></returns>
            public int DeleteLabelNodeDB(String path)
        {
            using (MyDBEntitiesSqlite context = new MyDBEntitiesSqlite(EFConnectionString))
            {
                var query = from item in context.LabelNodeDBs
                            where item.Path == path
                            select item;
                foreach (var LabelNodeObj in query)
                {
                    List<InfoNodeDB> infoNodes = LabelNodeObj.InfoNodeDBs.ToList();

                    foreach (var infoNode in infoNodes)
                    {
                        context.Entry(infoNode).State = EntityState.Deleted;
                    }

                    context.LabelNodeDBs.Remove(LabelNodeObj);
                }
                return context.SaveChanges();
            }

        }

        /// <summary>
        /// 删除所有节点记录
        /// </summary>
        public void DeleteAllLabelNodeDBRecords()
        {
            using (MyDBEntitiesSqlite context = new MyDBEntitiesSqlite(EFConnectionString))
            {
                List<LabelNodeDB> LabelNodeObjs = context.LabelNodeDBs.ToList();
                foreach (var LabelNodeObj in LabelNodeObjs)
                {
                    context.LabelNodeDBs.Remove(LabelNodeObj);
                }
                context.SaveChanges();
            }

        }
        /// <summary>
        /// 删除某个节点中的指定信息节点
        /// </summary>
        /// <param name="LabelNodePath"></param>
        /// <param name="infoNodeID"></param>
        public void DeleteInfoNodeOfLabelNodeDB(String LabelNodePath, long infoNodeID)
        {
            List<long> infoNodeIDs = new List<long>();
            infoNodeIDs.Add(infoNodeID);
            DeleteInfoNodesOfLabelNodeDB(LabelNodePath, infoNodeIDs);

        }

        /// <summary>
        /// 从节点所关联的信息节点集合中删除指定的信息节点
        /// </summary>
        /// <param name="LabelNodePath"></param>
        /// <param name="infoNodeIDs"></param>
        public void DeleteInfoNodesOfLabelNodeDB(String LabelNodePath, List<long> infoNodeIDs)
        {
            if (string.IsNullOrEmpty(LabelNodePath) || infoNodeIDs == null || infoNodeIDs.Count == 0)
            {
                return;
            }
            using (MyDBEntitiesSqlite context = new MyDBEntitiesSqlite(EFConnectionString))
            {
                LabelNodeDB LabelNodeObj = context.LabelNodeDBs.FirstOrDefault(p => p.Path == LabelNodePath);
                if (LabelNodeObj == null)
                {
                    return;
                }

                foreach (var infoNode in LabelNodeObj.InfoNodeDBs.ToList())
                {
                    if (infoNodeIDs.IndexOf(infoNode.ID) != -1)
                    {
                        //保证删除InfoNodeAndLabelNode记录
                        LabelNodeObj.InfoNodeDBs.Remove(infoNode);
                        //确保InfoNodeDB删除
                        context.Entry(infoNode).State = EntityState.Deleted;
                    }

                }
                context.SaveChanges();
            }

        }
        /// <summary>
        /// 删除某个节点中的指定信息节点
        /// </summary>
        /// <param name="LabelNodePath"></param>
        /// <param name="infoNodeID"></param>
        public void DeleteInfoNodeOfLabelNodeDBContinuousExecution(String LabelNodePath, long infoNodeID)
        {
            List<long> infoNodeIDs = new List<long>();
            infoNodeIDs.Add(infoNodeID);
            DeleteInfoNodesOfLabelNodeDBContinuousExecution(LabelNodePath, infoNodeIDs);

        }

        /// <summary>
        /// 从节点所关联的信息节点集合中删除指定的信息节点
        /// </summary>
        /// <param name="LabelNodePath"></param>
        /// <param name="infoNodeIDs"></param>
        public void DeleteInfoNodesOfLabelNodeDBContinuousExecution(String LabelNodePath, List<long> infoNodeIDs)
        {
            if (string.IsNullOrEmpty(LabelNodePath) || infoNodeIDs == null || infoNodeIDs.Count == 0)
            {
                return;
            }
           
                LabelNodeDB LabelNodeObj = _dbContext.LabelNodeDBs.FirstOrDefault(p => p.Path == LabelNodePath);
                if (LabelNodeObj == null)
                {
                    return;
                }

                foreach (var infoNode in LabelNodeObj.InfoNodeDBs.ToList())
                {
                    if (infoNodeIDs.IndexOf(infoNode.ID) != -1)
                    {
                        //保证删除InfoNodeAndLabelNode记录
                        LabelNodeObj.InfoNodeDBs.Remove(infoNode);
                    //确保InfoNodeDB删除
                    _dbContext.Entry(infoNode).State = EntityState.Deleted;
                    }

                }
            //注意外部调用放记得多次使用后需调用：SaveChanges();


        }

        /// <summary>
        /// 按照Path提取相应节点所有的信息节点信息(是DBInfoNodeInfo对象）SqlCeException: 在 WHERE、HAVING、GROUP BY、ON 或 IN 子句中不能使用 ntext 和 image 数据类型，除非将这些数据类型与 LIKE 或 IS NULL 谓词一起使用。
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public ObservableCollection<DBInfoNodeInfo> ContinuousQueriesDBInfoNodeInfosOfLabelNodeDB(String path)
        {
            if (String.IsNullOrEmpty(path))
            {
                return null;
            }

            ObservableCollection<DBInfoNodeInfo> infoNodeInfos = new ObservableCollection<DBInfoNodeInfo>();
            //using (MyDBEntities context = new MyDBEntities(EFConnectionString))
            //{
            var query = from item in _dbContext.LabelNodeDBs
                        where item.Path == path
                        select item;
            LabelNodeDB LabelNodeObj = query.FirstOrDefault();
            if (LabelNodeObj != null)
            {
                foreach (var info in LabelNodeObj.InfoNodeDBs)
                {
                    infoNodeInfos.Add(new DBInfoNodeInfo()
                    {
                        ModifyTime = info.ModifyTime.Value,
                        Path = info.Path,
                        InfoNodeHearder = info.Text,
                        ID = info.ID
                    });
                }
            }
            return infoNodeInfos;
            //}
        }
        #endregion
        /// <summary>
        /// 按照信息节点ID从数据库中提取信息节点内容
        /// 找不到返回null
        /// </summary>
        /// <param name="infoNodeID"></param>
        /// <returns></returns>
        public byte[] getInfoNodeContent(int infoNodeID)
        {
            using (MyDBEntitiesSqlite context = new MyDBEntitiesSqlite(EFConnectionString))
            {
                InfoNodeDB infoNode = context.InfoNodeDBs.FirstOrDefault(f => f.ID == infoNodeID);
                if (infoNode != null)
                {
                    return infoNode.RTFText;
                }
            }
            return null;
        }


    }
}
