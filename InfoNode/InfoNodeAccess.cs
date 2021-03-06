﻿using DataAccessLayer;
using DataAccessLayer.InfoNodeDA;
using InterfaceLibrary;
using Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows.Threading;

namespace InfoNode
{
    public class InfoNodeAccess : IDataAccess
    {
        private InfoNodeRepository repository = null;
        //public InfoNodeAccess()
        //{
        //    repository = new InfoNodeRepository(DALConfig.EFConnectString);
        //}
        private string EFConString = "";

        public InfoNodeAccess(String EFConnectionString)
        {
            repository = new InfoNodeRepository(EFConnectionString);
            EFConString = EFConnectionString;
        }
        /// <summary>
        /// 新建一个InfoNode记录，不包容任何文件
        /// </summary>
        /// <param name="dataInfoObject"></param>
        /// <returns></returns>
        public int Create(IDataInfo dataInfoObject)
        {
            if (dataInfoObject == null || (dataInfoObject as InfoNodeDataInfo) == null)
            {
                return 0;
            }
            InfoNodeDB dbobj = InfoNodeHelper.changeToInfoNodeDB(dataInfoObject as InfoNodeDataInfo);
            int result = repository.AddInfoNodeDB(dbobj);
            //将数据库生成的ID值传回
            dataInfoObject.ID = (int)dbobj.ID;
            return 0;
        }

        public int DeleteDataInfoObjectOfNodeAndItsChildren(string nodePath)
        {
            if (String.IsNullOrEmpty(nodePath))
            {
                return 0;
            }
            return repository.DeleteInfoNodeDBAndItsChildByPath(nodePath);
        }

        public int DeleteDataInfoObject(IDataInfo dataInfoObject)
        {
            if (dataInfoObject == null)
            {
                return 0;
            }
            return repository.DeleteInfoNodeDB(dataInfoObject.Path);
        }

        public int UpdateDataInfoObject(IDataInfo dataInfoObject)
        {
            InfoNodeDataInfo obj = dataInfoObject as InfoNodeDataInfo;
            if (String.IsNullOrEmpty(obj.Text) == false && obj.Text.Length > DALConfig.MaxTextFieldSize)
            {
                obj.Text = obj.Text.Substring(0, DALConfig.MaxTextFieldSize);
            }
            if (dataInfoObject == null || obj == null)
            {
                return 0;
            }
            bool isNew = false;
            InfoNodeDB dbobj = repository.GetInfoNodeDBWithoutFileInfosByPath(obj.Path);
            if (dbobj == null)
            {
                dbobj = new InfoNodeDB();
                isNew = true;

            }
            dbobj.ModifyTime = obj.ModifyTime;
            dbobj.Text = obj.Text;
            dbobj.Path = obj.Path;
            dbobj.Type = obj.IconType;
            dbobj.RTFText = (String.IsNullOrEmpty(obj.RTFText)) ? null : Encoding.UTF8.GetBytes(obj.RTFText);
            if (isNew)
            {
                return repository.AddInfoNodeDB(dbobj);
            }
            else
            {

                return repository.UpdateInfoNodeDB(dbobj);


            }
        }

        public IDataInfo GetDataInfoObjectByPath(string nodePath)
        {
            InfoNodeDB dbobj = repository.GetInfoNodeDBWithoutFileInfosByPath(nodePath);
            ObservableCollection<DBFileInfo> files = repository.GetFileInfosOfInfoNodeDB(nodePath);

            ObservableCollection<DBLabelInfo> labels = repository.GetLabelInfosOfInfoNodeDB(nodePath);

            InfoNodeDataInfo InfoNodeInfo = InfoNodeHelper.changeToInfoNodeDataInfo(dbobj);
            if (InfoNodeInfo != null)
            {
                InfoNodeInfo.AttachFiles = files;
                InfoNodeInfo.AttachLabels = labels;
            }

            return InfoNodeInfo;
        }

        public void UpdateNodePath(string oldPath, string newPath)
        {
            repository.UpdateNodePaths(oldPath, newPath);
        }
        /// <summary>
        /// 添加文件
        /// </summary>
        /// <param name="dbfileInfo"></param>
        /// <param name="fileContent"></param>
        public string AddFile(string nodePath, DBFileInfo dbfileInfo, byte[] fileContent)
        {
            InfoNodeRepository tempRepository = new InfoNodeRepository(EFConString);
            bool isFileExsit = tempRepository.IsFileExsit(dbfileInfo);
            if (!isFileExsit)
            {
                //添加文件
                DiskFileContent diskFile = new DiskFileContent();
                diskFile.SavedFile = fileContent;
               
                 tempRepository.AddFileToDB(nodePath,diskFile);
                dbfileInfo.FileContentID = diskFile.ID;

            }

            //添加文件信息
            DiskFileInfo fileinfo = DBFileInfo.toDiskFileInfo(dbfileInfo);
            tempRepository.AddFileInfoOfInfoNodeDB(nodePath, fileinfo);
            tempRepository.SaveChanges();
            //将ID传回
            dbfileInfo.ID = fileinfo.ID;


            return "1";

        }
        /// <summary>
        /// 添加标签关联关系
        /// </summary>
        /// <param name="labelInfo"></param>
        /// <param name="labeldb"></param>
        public void AddLabelAssociation(string nodePath, DBLabelInfo labelInfo, LabelNodeDB labeldb)
        {

            repository.AddLabelOfInfoNodeDB(nodePath, labeldb);
            //将ID传回,注意此ID与原labeldb是不一样的，所以添加关联实际会在数据库中新增一个内容一样但ID不一样的对象。
            labelInfo.ID = labeldb.ID;
        }
        public List<long> GetDBFileToBeRemove(List<DBFileInfo> dBFileInfos)
        {
            return repository.GetDBFileToBeRemove(dBFileInfos);
        }
        public void DeleteFilesOfInfoNodeDB(String InfoNodeDBPath, List<long> fileIDs)
        {
            repository.DeleteFileInfosOfInfoNodeDB(InfoNodeDBPath, fileIDs);
        }
        public void DeleteFiles(List<long> fileIDs)
        {
            repository.DeleteFiles(fileIDs);
        }
        public void DeleteLabelOfInfoNodeDB(String InfoNodeDBPath, int labelID)
        {
            repository.DeleteLabelOfInfoNodeDB(InfoNodeDBPath, labelID);
        }
        public void DeleteLabelsOfInfoNodeDB(String InfoNodeDBPath, List<long> labelIDs)
        {
            repository.DeleteLabelsOfInfoNodeDB(InfoNodeDBPath, labelIDs);
        }
        /// <summary>
        /// 按照fileID从数据库中提取文件及其密钥的二进制数据
        /// </summary>
        /// <param name="fileID"></param>
        /// <returns></returns>
        public List<byte[]> getFileContentAndItsKey(long fileID)
        {
            return repository.getFileContentAndItsKey(fileID);
        }
        /// <summary>
        /// 按照fileID从数据库中提取文件的二进制数据
        /// </summary>
        /// <param name="fileID"></param>
        /// <returns></returns>
        public byte[] getFileContent(long fileID)
        {
            return repository.getFileContent(fileID);
        }
        public int UpdateFile(DiskFileContent diskFileContent)
        {
                return repository.UpdateFile(diskFileContent);

        }
        public int UpdateFileInfo(DiskFileInfo diskFileInfo)
        {
            return repository.UpdateFileInfo(diskFileInfo);

        }
        /// <summary>
        /// 按ID获取数据库中的标签
        /// </summary>
        /// <param name="labelID"></param>
        /// <returns></returns>
        public List<LabelNodeDB> getLabelNodeDBs(List<long> labelID)
        {
            List<LabelNodeDB> labelNodes = new List<LabelNodeDB>();
            foreach (var id in labelID)
            {
                labelNodes.Add(repository.getLabelNodeDB(id));
            }
            return labelNodes;
        }
    }
}
