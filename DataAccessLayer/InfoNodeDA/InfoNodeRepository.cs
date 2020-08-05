
using DataAccessLayer.InfoNodeDA;
using Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using EntityState = System.Data.Entity.EntityState;

namespace DataAccessLayer.InfoNodeDA
{
    /// <summary>
    /// 完成信息节点的增删改查功能
    /// </summary>
    public class InfoNodeRepository : BaseRepository<MyDBEntitiesSqlite>
    {
        private String EFConnectionString = "";


        //public InfoNodeRepository(MyDBEntities dbContext) :
        //   base(dbContext)
        //{

        //}
        public InfoNodeRepository()
            : base(new MyDBEntitiesSqlite(""))
        {

        }
        public InfoNodeRepository(string efConnectionString)
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

            var query = from item in _dbContext.InfoNodeDBs
                        where item.Path.StartsWith(oldPath)
                        select item;
            foreach (var item in query)
            {
                item.Path = item.Path.Replace(oldPath, newPath);
            }
             _dbContext.SaveChanges();


        }

        /// <summary>
        /// 向数据库中添加一条记录
        /// </summary>
        /// <param name="InfoNodeObj"></param>
        /// <returns></returns>
        public int AddInfoNodeDB(InfoNodeDB InfoNodeObj)
        {
            if (InfoNodeObj == null)
            {
                return 0;
            }
           
                _dbContext.InfoNodeDBs.Add(InfoNodeObj);
                 return _dbContext.SaveChanges();       
        }
        /// <summary>
        /// 更新节点的信息
        /// </summary>
        /// <param name="InfoNodeObj"></param>
        public int UpdateInfoNodeDB(InfoNodeDB InfoNodeObj)
        {
            if (InfoNodeObj == null)
            {
                return 0;
            }

            InfoNodeDB InfoNodeToModify = _dbContext.InfoNodeDBs.FirstOrDefault(p => p.ID == InfoNodeObj.ID);
            if (InfoNodeToModify != null)
            {
                InfoNodeToModify.ModifyTime = InfoNodeObj.ModifyTime;
                InfoNodeToModify.Text = InfoNodeObj.Text;
                InfoNodeToModify.RTFText = InfoNodeObj.RTFText;
                InfoNodeToModify.Type = InfoNodeObj.Type;
                return _dbContext.SaveChanges();

            }

            return 0;

        }
        public int UpdateFile(DiskFileContent diskFileContent)
        {

            if (diskFileContent == null)
            {
                return 0;
            }
            using (MyDBEntitiesSqlite context = new MyDBEntitiesSqlite(EFConnectionString))
            {
               
                DiskFileContent file = context.DiskFileContents.FirstOrDefault(f => f.ID == diskFileContent.ID);
                if (file != null)
                {
                    file.FileKey = diskFileContent.FileKey;
                    file.SavedFile = diskFileContent.SavedFile;
                    return context.SaveChanges();
                }
            }
            return 0;            
        }
        public int UpdateFileInfo(DiskFileInfo diskFileInfo)
        {

            if (diskFileInfo == null)
            {
                return 0;
            }
            using (MyDBEntitiesSqlite context = new MyDBEntitiesSqlite(EFConnectionString))
            {

                DiskFileInfo fileInfo = context.DiskFileInfoes.FirstOrDefault(f => f.ID == diskFileInfo.ID);
                if (fileInfo != null)
                {
                    fileInfo.IsEncrypted = diskFileInfo.IsEncrypted;
                    return context.SaveChanges();
                }
            }
            return 0;


        }
        #endregion
        #region "提取信息"

        /// <summary>
        /// 提取所有的节点（包括其相关联的文件）
        /// 尽量少用，这将导致占用大量内存
        /// </summary>
        /// <returns></returns>
        public List<InfoNodeDB> GetAllInfoNodeDBWithItsDiskFileInfo()
        {
            using (MyDBEntitiesSqlite context = new MyDBEntitiesSqlite(EFConnectionString))
            {
                var query = from item in context.InfoNodeDBs.Include("DiskFileInfoes")
                            select item;
                return query.ToList();
            }

        }

        /// <summary>
        /// 提取所有的节点（不包括其相关联的文件）
        ///
        /// </summary>
        /// <returns></returns>
        public List<InfoNodeDB> GetAllInfoNodeDBWithoutItsDiskFileInfo()
        {
            using (MyDBEntitiesSqlite context = new MyDBEntitiesSqlite(EFConnectionString))
            {
                var query = from item in context.InfoNodeDBs
                            select item;
                return query.ToList();
            }
        }

        /// <summary>
        /// 按照路径提取信息节点对象(仅提取第一个匹配的），找不到，返回null
        /// 获取的信息节点对象包容其所有的关联文件
        /// Note:尽量少用，因此方法会加载所有文件内容，占用大量内存
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public InfoNodeDB GetInfoNodeDBWithFilesByPath(String path)
        {
            if (String.IsNullOrEmpty(path))
            {
                return null;
            }
            using (MyDBEntitiesSqlite context = new MyDBEntitiesSqlite(EFConnectionString))
            {
                return context.InfoNodeDBs.Include("DiskFileInfoes").FirstOrDefault(p => p.Path == path);
            }
        }
        /// <summary>
        /// 依据路径提取信息节点的数据，不包括其包容的文件信息，
        /// 此方法应该与GetFileInfosOfInfoNode()结合起来，向外界返回真正有用的信息
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public InfoNodeDB GetInfoNodeDBWithoutFileInfosByPath(String path)
        {
            if (String.IsNullOrEmpty(path))
            {
                return null;
            }
            using (MyDBEntitiesSqlite context = new MyDBEntitiesSqlite(EFConnectionString))
            {
                return context.InfoNodeDBs.FirstOrDefault(p => p.Path == path);
            }
           // return _dbContext.InfoNodeDBs.FirstOrDefault(p => p.Path == path);
        }
        /// <summary>
        /// 依据路径提取信息节点的数据，不包括其包容的文件信息，
        /// 此方法应该与GetFileInfosOfInfoNode()结合起来，向外界返回真正有用的信息
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public InfoNodeDB GetInfoNodeDBWithoutFileInfosByPathContinuos(String path)
        {
            if (String.IsNullOrEmpty(path))
            {
                return null;
            }

             return _dbContext.InfoNodeDBs.FirstOrDefault(p => p.Path == path);

        }
        /// <summary>
        /// 按照Path提取相应节点所有的文件信息(是FileInfo对象）SqlCeException: 在 WHERE、HAVING、GROUP BY、ON 或 IN 子句中不能使用 ntext 和 image 数据类型，除非将这些数据类型与 LIKE 或 IS NULL 谓词一起使用。

        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public ObservableCollection<DBFileInfo> GetFileInfosOfInfoNodeDB(String path)
        {
            if (String.IsNullOrEmpty(path))
            {
                return null;
            }
            ObservableCollection<DBFileInfo> fileInfos = new ObservableCollection<DBFileInfo>();
            using (MyDBEntitiesSqlite context = new MyDBEntitiesSqlite(EFConnectionString))
            {
                var query = from item in context.InfoNodeDBs
                            where item.Path == path
                            select item;
                InfoNodeDB InfoNodeObj = query.FirstOrDefault();
                if (InfoNodeObj != null)
                {
                    foreach (var file in InfoNodeObj.DiskFileInfoes)
                    {
                        fileInfos.Add(new DBFileInfo()
                        {
                            AddTime = file.AddTime.Value,
                            FilePath = file.FilePath,
                            FileSize = file.FileSize.Value,
                            FileHash=file.FileHash,
                            FileContentID=(long)file.FileContentID,
                            ID = file.ID,
                            IsEncrypted=(bool)file.IsEncrypted
                        });

                    }

                }
                return fileInfos;
            }
        }

        /// <summary>
        /// 按照Path提取相应节点所有的标签信息(是LabelInfo对象）SqlCeException: 在 WHERE、HAVING、GROUP BY、ON 或 IN 子句中不能使用 ntext 和 image 数据类型，除非将这些数据类型与 LIKE 或 IS NULL 谓词一起使用。
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public ObservableCollection<DBLabelInfo> GetLabelInfosOfInfoNodeDB(String path)
        {
            if (String.IsNullOrEmpty(path))
            {
                return null;
            }
            ObservableCollection<DBLabelInfo> labelInfos = new ObservableCollection<DBLabelInfo>();
            using (MyDBEntitiesSqlite context = new MyDBEntitiesSqlite(EFConnectionString))
            {
                var query = from item in context.InfoNodeDBs
                            where item.Path == path
                            select item;
                InfoNodeDB InfoNodeObj = query.FirstOrDefault();
                if (InfoNodeObj != null)
                {
                    foreach (var label in InfoNodeObj.LabelNodeDBs)
                    {
                        labelInfos.Add(new DBLabelInfo()
                        {
                            ModifyTime = label.ModifyTime.Value,
                            Path = label.Path,
                           // Label = label.Label,
                            ID = label.ID
                        });

                    }

                }
                return labelInfos;
            }
        }
        /// <summary>
        /// 按照路径提取信息节点对象(仅提取第一个匹配的），找不到，返回null
        /// 获取的信息节点对象包容其所有的关联标签节点
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public InfoNodeDB GetInfoNodeDBWithLabelNodeDBsByPath(String path)
        {
            if (String.IsNullOrEmpty(path))
            {
                return null;
            }
            using (MyDBEntitiesSqlite context = new MyDBEntitiesSqlite(EFConnectionString))
            {
                return context.InfoNodeDBs.Include("LabelNodeDBs").FirstOrDefault(p => p.Path == path);
            }
        }


        #endregion

        #region "添加文件"
    
        /// <summary>
        /// 判断所添加的文件是否在数据库中已经存在,存在则返回FileContentID
        /// </summary>
        /// <param name="dBFileInfo"></param>
        /// <returns></returns>
        public bool IsFileExsit(DBFileInfo dBFileInfo)
        {
            List<DiskFileInfo> fileinfolist = new List<DiskFileInfo>();
            //using (MyDBEntitiesSqlite context = new MyDBEntitiesSqlite(EFConnectionString))
            //{
                var query = from item in _dbContext.DiskFileInfoes
                            where item.FileHash == dBFileInfo.FileHash
                            select item;
                 fileinfolist = query.ToList();
            //}
            if (fileinfolist.Count > 0)
            {
                dBFileInfo.FileContentID = (long)fileinfolist[0].FileContentID;

                return true;
            }
            return false;
        }

        public void AddFileToDB(String InfoNodePath, DiskFileContent diskFile)
        {
            if (string.IsNullOrEmpty(InfoNodePath) || diskFile == null)
            {
                return ;
            }
           // InfoNodeDB infoNode = _dbContext.InfoNodeDBs.FirstOrDefault(p => p.Path == InfoNodePath);
            //long infonodeID = infoNode.ID;
            using (MyDBEntitiesSqlite context = new MyDBEntitiesSqlite(EFConnectionString))
            {
                //DiskFileContent diskFileContent=new DiskFileContent();
                InfoNodeDB InfoNodeObj = context.InfoNodeDBs.FirstOrDefault(p => p.Path == InfoNodePath);

                diskFile.InfoNodeDB = InfoNodeObj;
                 diskFile.InfoNodeID = InfoNodeObj.ID;

                //context.DiskFileContents.Add(diskFile);
                InfoNodeObj.DiskFileContents.Add(diskFile);
                context.SaveChanges();


            }
         //   infoNode.DiskFileContents.Add(diskFile);
        }
        /// <summary>
        /// 将一个文件信息加入到节点的文件集合中，不包括文件内容
        /// </summary>
        /// <param name="InfoNodePath"></param>
        /// <param name="fileInfo"></param>
        public void AddFileInfoOfInfoNodeDB(String InfoNodePath, DiskFileInfo fileInfo)
        {
            if (string.IsNullOrEmpty(InfoNodePath) || fileInfo == null)
            {
                return ;
            }
            //using (MyDBEntitiesSqlite context = new MyDBEntitiesSqlite(EFConnectionString))
            //{
                InfoNodeDB InfoNodeObj = _dbContext.InfoNodeDBs.FirstOrDefault(p => p.Path == InfoNodePath);
                if (InfoNodeObj == null)
                {
                    return ;
                }
                InfoNodeObj.DiskFileInfoes.Add(fileInfo);
              //  return _dbContext.SaveChanges();
              
            //}

        }
        /// <summary>
        /// 将多个文件追加到信息节点中
        /// 如果文件夹节点找不到，或者files为空集合，什么也不干，返回
        /// </summary>
        /// <param name="InfoNodeDBPath"></param>
        /// <param name="fileInfos"></param>
        public void AddFileInfosOfInfoNode(String InfoNodePath, List<DiskFileInfo> fileInfos)
        {
            if (string.IsNullOrEmpty(InfoNodePath) || fileInfos == null || fileInfos.Count == 0)
            {
                return;
            }
            using (MyDBEntitiesSqlite context = new MyDBEntitiesSqlite(EFConnectionString))
            {
                InfoNodeDB InfoNodeObj = context.InfoNodeDBs.FirstOrDefault(p => p.Path == InfoNodePath);
                if (InfoNodeObj == null)
                {
                    return;
                }
                foreach (var file in fileInfos)
                {
                    InfoNodeObj.DiskFileInfoes.Add(file);
                }
                context.SaveChanges();
            }
        }
        #endregion
        #region "添加标签关联"

        /// <summary>
        /// 添加信息节点与标签的关联关系。
        /// </summary>
        /// <param name="InfoNodePath"></param>
        /// <param name="labelNode"></param>
        public int AddLabelOfInfoNodeDB(String InfoNodePath, LabelNodeDB labelNode)
        {
            if (string.IsNullOrEmpty(InfoNodePath) || labelNode == null)
            {
                return 0;
            }
            using (MyDBEntitiesSqlite context = new MyDBEntitiesSqlite(EFConnectionString))
            {
                InfoNodeDB InfoNodeObj = context.InfoNodeDBs.FirstOrDefault(p => p.Path == InfoNodePath);
                if (InfoNodeObj == null)
                {
                    return 0;
                }
                context.LabelNodeDBs.Add(labelNode);//添加到同一上下文关系
                InfoNodeObj.LabelNodeDBs.Add(labelNode);
                return context.SaveChanges();
                
            }

        }

        #endregion
        #region "删除操作"
        /// <summary>
        /// 当待删除的数据库中文件对应的文件信息仅剩一个时，需删除此文件，此时返回一个true
        /// </summary>
        /// <param name="dBFileInfo"></param>
        /// <returns></returns>
        public List<long> GetDBFileToBeRemove(List<DBFileInfo> dBFileInfos)
        {
            List<long> fileInfoIDs = new List<long>();
            foreach (var item in dBFileInfos )
            {
                fileInfoIDs.Add(item.ID);
            }
            List<long> fileIds = new List<long>();
            using (MyDBEntitiesSqlite context = new MyDBEntitiesSqlite(EFConnectionString))
            {
                foreach (var info in dBFileInfos)
                {
                    if (info.FileHash != null)
                    {
                    
                    var query = from item in context.DiskFileInfoes
                                where item.FileHash == info.FileHash
                                select item.ID;
                 var list= query.ToList();
                        bool isSubset = list.All(t => fileInfoIDs.Any(b => b == t));
                    if (list.Count == 1|| isSubset)
                    {
                        fileIds.Add(info.FileContentID);
                    }

                    }
                }
              
              
            }

            return fileIds;
        }

        /// <summary>
        /// 按照路径删除InfoNode记录，如果它还有子记录，一并删除,同时删除关联的文件，但不删除关联的标签（至于与标签的关联，因为一端已经消失，关联线自然无意义了）
        /// </summary>
        /// <param name="path"></param>
        public int DeleteInfoNodeDBAndItsChildByPath(String path)
        {
            using (MyDBEntitiesSqlite context = new MyDBEntitiesSqlite(EFConnectionString))
            {
             
                var query = from item in context.InfoNodeDBs
                            where item.Path.StartsWith(path)
                            select item;
                foreach (var InfoNodeObj in query)
                {

                    List<DiskFileInfo> fileinfos = InfoNodeObj.DiskFileInfoes.ToList();

                    foreach (var fileinfo in fileinfos)
                    {
                        context.Entry(fileinfo).State = EntityState.Deleted;

                        var q = from i in context.DiskFileInfoes where i.FileHash == fileinfo.FileHash select i;
                        var list = q.ToList();
                       if(list.Count()==1|| list.All(t => fileinfos.Any(b => b == t)))
                        {
                           var file= context.DiskFileContents.FirstOrDefault(i=>i.ID==fileinfo.FileContentID);
                            context.DiskFileContents.Remove(file); 
                        }
                    }


                    context.InfoNodeDBs.Remove(InfoNodeObj);
                }
                return context.SaveChanges();
            }

        }
        /// <summary>
        /// 接照路径删除指定的节点记录，其子节点数据不动（因为在更换节点
        /// 类型时，需要删除自己，但子节点是不能删除的）
        /// 设计考虑无需更换节点所以无需使用本函数
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public int DeleteInfoNodeDB(String path)
        {
            using (MyDBEntitiesSqlite context = new MyDBEntitiesSqlite(EFConnectionString))
            {
                var query = from item in context.InfoNodeDBs
                            where item.Path == path
                            select item;
                foreach (var InfoNodeObj in query)
                {
                    List<DiskFileInfo> files = InfoNodeObj.DiskFileInfoes.ToList();

                    foreach (var file in files)
                    {
                        context.Entry(file).State = EntityState.Deleted;
                    }
                    List<LabelNodeDB> labelNodes = InfoNodeObj.LabelNodeDBs.ToList();

                    foreach (var labelNode in labelNodes)
                    {
                        context.Entry(labelNode).State = EntityState.Deleted;
                    }

                    context.InfoNodeDBs.Remove(InfoNodeObj);
                }
                return context.SaveChanges();
            }

        }

        /// <summary>
        /// 删除所有节点记录
        /// </summary>
        public void DeleteAllInfoNodeRecords()
        {
            using (MyDBEntitiesSqlite context = new MyDBEntitiesSqlite(EFConnectionString))
            {
                List<InfoNodeDB> InfoNodeObjs = context.InfoNodeDBs.ToList();
                foreach (var InfoNodeObj in InfoNodeObjs)
                {
                    context.InfoNodeDBs.Remove(InfoNodeObj);
                }
                context.SaveChanges();
            }

        }
  
        /// <summary>
        /// 删除指定的文件
        /// </summary>
        /// <param name="fileIDs"></param>
        public void DeleteFiles( List<long> fileIDs)
        {
            if ( fileIDs == null || fileIDs.Count == 0)
            {
                return;
            }
            using (MyDBEntitiesSqlite context = new MyDBEntitiesSqlite(EFConnectionString))
            {
                foreach (var item in fileIDs)
                {
                    DiskFileContent file = context.DiskFileContents.FirstOrDefault(i => i.ID == item);
                    if(file!=null)
                    context.DiskFileContents.Remove(file);
                    //确保删除
                    // context.Entry(file).State = EntityState.Deleted;

                   
                }
                context.SaveChanges();
            
               
            }
        

        }
        /// <summary>
        /// 删除某个节点中的指定文件信息
        /// </summary>
        /// <param name="InfoNodePath"></param>
        /// <param name="fileinfoID"></param>
        public void DeleteFileInfoOfInfoNodedDB(String InfoNodePath,long fileinfoID)
        {
            List<long> fileinfoIds = new List<long>();
            fileinfoIds.Add(fileinfoID);
            DeleteFileInfosOfInfoNodeDB(InfoNodePath, fileinfoIds);

        }

        /// <summary>
        /// 从节点所关联的文件集合中删除指定的文件信息
        /// </summary>
        /// <param name="InfoNodePath"></param>
        /// <param name="fileIDs"></param>
        public void DeleteFileInfosOfInfoNodeDB(String InfoNodePath, List<long> fileIDs)
        {
            if (string.IsNullOrEmpty(InfoNodePath) || fileIDs == null || fileIDs.Count == 0)
            {
                return;
            }
            using (MyDBEntitiesSqlite context = new MyDBEntitiesSqlite(EFConnectionString))
            {
                InfoNodeDB InfoNodeObj = context.InfoNodeDBs.FirstOrDefault(p => p.Path == InfoNodePath);
                if (InfoNodeObj == null)
                {
                    return;
                }

                foreach (var file in InfoNodeObj.DiskFileInfoes.ToList())
                {
                    if (fileIDs.IndexOf(file.ID) != -1)
                    {
                        //保证删除FileAndInfoNode记录
                        InfoNodeObj.DiskFileInfoes.Remove(file);
                        //确保DiskFileInfo删除
                        context.Entry(file).State = EntityState.Deleted;
                    }

                }
                context.SaveChanges();
            }

        }
        /// <summary>
        /// 删除某个节点中的指定标签
        /// </summary>
        /// <param name="InfoNodePath"></param>
        /// <param name="labelID"></param>
        public void DeleteLabelOfInfoNodeDB(String InfoNodePath, long labelID)
        {
            List<long> labelIds = new List<long>();
            labelIds.Add(labelID);
            DeleteLabelsOfInfoNodeDB(InfoNodePath, labelIds);

        }
        /// <summary>
        /// 从节点所关联的标签集合中删除指定的标签
        /// </summary>
        /// <param name="InfoNodePath"></param>
        /// <param name="labelIDs"></param>
        public void DeleteLabelsOfInfoNodeDB(String InfoNodePath, List<long> labelIDs)
        {
            if (string.IsNullOrEmpty(InfoNodePath) || labelIDs == null || labelIDs.Count == 0)
            {
                return;
            }
            using (MyDBEntitiesSqlite context = new MyDBEntitiesSqlite(EFConnectionString))
            {
                InfoNodeDB InfoNodeObj = context.InfoNodeDBs.FirstOrDefault(p => p.Path == InfoNodePath);
                if (InfoNodeObj == null)
                {
                    return;
                }

                foreach (var label in InfoNodeObj.LabelNodeDBs.ToList())
                {
                    if (labelIDs.IndexOf(label.ID) != -1)
                    {
                        //仅删除LabelNodeAndInfoNode记录
                        InfoNodeObj.LabelNodeDBs.Remove(label);
                        //删除数据库中的label，而不仅指记录
                        //context.Entry(label).State = EntityState.Deleted;
                    }

                }
                context.SaveChanges();
            }

        }
        /// <summary>
        /// 连续删除某个节点中的指定标签
        /// </summary>
        /// <param name="InfoNodePath"></param>
        /// <param name="labelID"></param>
        public void DeleteLabelOfInfoNodeDBContinuousExecution(String InfoNodePath, long labelID)
        {
            List<long> labelIds = new List<long>();
            labelIds.Add(labelID);
            DeleteLabelsOfInfoNodeDBContinuousExecution(InfoNodePath, labelIds);

        }
        /// <summary>
        /// 连续从节点所关联的标签集合中删除指定的标签
        /// </summary>
        /// <param name="InfoNodePath"></param>
        /// <param name="labelIDs"></param>
        public void DeleteLabelsOfInfoNodeDBContinuousExecution(String InfoNodePath, List<long> labelIDs)
        {
            if (string.IsNullOrEmpty(InfoNodePath) || labelIDs == null || labelIDs.Count == 0)
            {
                return;
            }
         
                InfoNodeDB InfoNodeObj = _dbContext.InfoNodeDBs.FirstOrDefault(p => p.Path == InfoNodePath);
                if (InfoNodeObj == null)
                {
                    return;
                }

                foreach (var label in InfoNodeObj.LabelNodeDBs.ToList())
                {
                    if (labelIDs.IndexOf(label.ID) != -1)
                    {
                        //保证删除LabelNodeAndInfoNode记录
                        InfoNodeObj.LabelNodeDBs.Remove(label);
                        //确保InfoNodeDB删除
                        _dbContext.Entry(label).State = EntityState.Deleted;
                    }

                }
               // context.SaveChanges();
            

        }

        #endregion
        /// <summary>
        /// 按照文件ID从数据库中提取文件的密钥（索引为0）和内容(索引为1)
        /// 找不到返回null
        /// </summary>
        /// <param name="fileID"></param>
        /// <returns></returns>
        public List<byte[]> getFileContentAndItsKey(long fileID)
        {
            List<byte[]> fileAndKey = new List<byte[]>();
            using (MyDBEntitiesSqlite context = new MyDBEntitiesSqlite(EFConnectionString))
            {
                DiskFileInfo fileinfo = context.DiskFileInfoes.FirstOrDefault(f => f.ID == fileID);
                DiskFileContent fileContent = context.DiskFileContents.FirstOrDefault(f => f.ID == fileinfo.FileContentID);
                if (fileinfo != null)
                {
                    fileAndKey.Add(fileContent.FileKey);
                    fileAndKey.Add(fileContent.SavedFile);
                    return fileAndKey;            
                }
            }
            return null;
        }
        public byte[]getFileContent(long fileID)
        {
          
            using (MyDBEntitiesSqlite context = new MyDBEntitiesSqlite(EFConnectionString))
            {
                DiskFileInfo fileinfo = context.DiskFileInfoes.FirstOrDefault(f => f.ID == fileID);
                DiskFileContent fileContent = context.DiskFileContents.FirstOrDefault(f => f.ID == fileinfo.FileContentID);
                if (fileinfo != null)
                {
                    
                    return fileContent.SavedFile;
                }
            }
            return null;
        }
        /// <summary>
        /// 按照标签ID从数据库中提取标签节点
        /// 找不到返回null
        /// </summary>
        /// <param name="labelID"></param>
        /// <returns></returns>
        public LabelNodeDB getLabelNodeDB(long labelID)
        {
            using (MyDBEntitiesSqlite context = new MyDBEntitiesSqlite(EFConnectionString))
            {
                LabelNodeDB label = context.LabelNodeDBs.FirstOrDefault(f => f.ID == labelID);
                if (label != null)
                {
                    return label;
                }
            }
            return null;
        }


    }
}
