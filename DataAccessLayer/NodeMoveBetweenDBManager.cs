using InterfaceLibrary;
using Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;

namespace DataAccessLayer
{
    /// <summary>
    /// 此类负责完成在不同数据库中转移记录的功能
    /// </summary>
    public class NodeMoveBetweenDBManager
    {
        ///// <summary>
        ///// 源数据库文件名
        ///// </summary>
        //private String _SourceDBFileWithPath = "";
        ///// <summary>
        ///// 目标数据库文件名
        ///// </summary>
        //private String _TargetDBFileWithPath = "";

        private MyDBEntitiesSqlite SourceDbContext = null;
        private MyDBEntitiesSqlite TargetDbContext = null;
        public NodeMoveBetweenDBManager(String SourceDBFileWithPath, String TargetDBFileWithPath)
        {
            //_SourceDBFileWithPath = SourceDBFileWithPath;
            SourceDbContext = new MyDBEntitiesSqlite(DALConfig.getEFConnectionString(SourceDBFileWithPath));
            //_TargetDBFileWithPath = TargetDBFileWithPath;
            TargetDbContext = new MyDBEntitiesSqlite(DALConfig.getEFConnectionString(TargetDBFileWithPath));
        }
        /// <summary>
        /// 将源数据库中的数据记录移到另一个数据库
        /// </summary>
        /// <param name="SourceRootNodePath"></param>
        /// <param name="RootNodeType"></param>
        /// <param name="TargetRootNodePath"></param>
        public void MoveNodeBetweenDB(String SourceRootNodePath, String TargetRootNodePath)
        {

            //bool IsAddToTargetRoot = false;

            int slashIndex = SourceRootNodePath.LastIndexOf("/", SourceRootNodePath.Length - 2);
            //源节点文本，即在树中显示的文本
            String SourceRootNodeText = SourceRootNodePath.Substring(slashIndex + 1);
           
 

            //处理InfoNode节点，提取其相关联的所有文件
            var sourceInfoNodeNodes = from node in SourceDbContext.InfoNodeDBs.Include("DiskFiles").AsNoTracking()
                                    where node.Path.StartsWith(SourceRootNodePath)
                                    select node;
            foreach (var InfoNodeNode in sourceInfoNodeNodes)
            {
                //if (!IsAddToTargetRoot)
                //{
                //源路径去掉开头的“/”之后，拼接到目标路径之后
                InfoNodeNode.Path = TargetRootNodePath + InfoNodeNode.Path.Replace(SourceRootNodePath, SourceRootNodeText);
                //}

                //else
                //{
                //    InfoNodeNode.Path = InfoNodeNode.Path.Replace(SourceRootNodePath, SourceRootNodeText);
                //}

                TargetDbContext.InfoNodeDBs.Add(InfoNodeNode);
            }


            //提交更改

            TargetDbContext.SaveChanges();

            TargetDbContext.Dispose();

            //在源数据库中删除相关详细信息节点记录
            SourceDbContext.Database.ExecuteSqlCommand("Delete from DetailTextDB where Path like {0}", SourceRootNodePath + "%");
            //在源数据库中删除所有文件夹节点相关的记录，涉及三个表，为简单起见，使用EF完成。
            var query = from InfoNode in SourceDbContext.InfoNodeDBs
                        where InfoNode.Path.StartsWith(SourceRootNodePath)
                        select InfoNode;
            foreach (var InfoNode in query)
            {

                List<DiskFileInfo> files = InfoNode.DiskFileInfoes.ToList();

                foreach (var file in files)
                {
                    SourceDbContext.Entry(file).State =System.Data.Entity.EntityState.Deleted;
                }

                SourceDbContext.InfoNodeDBs.Remove(InfoNode);
            }
            SourceDbContext.SaveChanges();
            SourceDbContext.Dispose();



        }
    }
}