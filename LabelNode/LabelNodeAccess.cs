using DataAccessLayer;
using DataAccessLayer.InfoNodeDA;
using DataAccessLayer.LabelNodeDA;
using InterfaceLibrary;
using Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace LabelNode
{
    public class LabelNodeAccess : IDataAccess
    {
        private LabelNodeRepository repository = null;
        //public InfoNodeAccess()
        //{
        //    repository = new InfoNodeRepository(DALConfig.EFConnectString);
        //}

        public LabelNodeAccess(String EFConnectionString)
        {
            repository = new LabelNodeRepository(EFConnectionString);
        }
        /// <summary>
        /// 新建一个LabelNode记录，不包容任何关联节点
        /// </summary>
        /// <param name="dataInfoObject"></param>
        /// <returns></returns>
        public int Create(IDataInfo dataInfoObject)
        {
            if (dataInfoObject == null || (dataInfoObject as LabelNodeDataInfo) == null)
            {
                return 0;
            }
            LabelNodeDB dbobj = LabelNodeHelper.changeToLabelNodeDB(dataInfoObject as LabelNodeDataInfo);
            int result = repository.AddLabelNodeDB(dbobj);
            //将数据库生成的ID值传回
            dataInfoObject.ID = dbobj.ID;
            return 0;
        }

        public int DeleteDataInfoObjectOfNodeAndItsChildren(string nodePath)
        {
            if (String.IsNullOrEmpty(nodePath))
            {
                return 0;
            }
            return repository.DeleteLabelNodeDBAndItsChildByPath(nodePath);
        }

        public int DeleteDataInfoObject(IDataInfo dataInfoObject)
        {
            if (dataInfoObject == null)
            {
                return 0;
            }
            return repository.DeleteLabelNodeDB(dataInfoObject.Path);
        }

        public int UpdateDataInfoObject(IDataInfo dataInfoObject)
        {
            LabelNodeDataInfo obj = dataInfoObject as LabelNodeDataInfo;
            if (String.IsNullOrEmpty(obj.Text) == false && obj.Text.Length > DALConfig.MaxTextFieldSize)
            {
                obj.Text = obj.Text.Substring(0, DALConfig.MaxTextFieldSize);
            }
            if (dataInfoObject == null || obj == null)
            {
                return 0;
            }
            bool isNew = false;
            LabelNodeDB dbobj = repository.GetLabelNodeDBWithoutInfoNodeDBsByPath(obj.Path);
            if (dbobj == null)
            {
                dbobj = new LabelNodeDB();
                isNew = true;

            }
            dbobj.ModifyTime = obj.ModifyTime;
            dbobj.Label = obj.Text;
            dbobj.Path = obj.Path;
            // dbobj.RTFText = (String.IsNullOrEmpty(obj.RTFText)) ? null : Encoding.UTF8.GetBytes(obj.RTFText);
            if (isNew)
            {
                return repository.AddLabelNodeDB(dbobj);
            }
            else
            {

                return repository.UpdateLabelNodeDB(dbobj);


            }
        }

        public IDataInfo GetDataInfoObjectByPath(string nodePath)
        {
            LabelNodeDB dbobj = repository.GetLabelNodeDBWithoutInfoNodeDBsByPath(nodePath);
            ObservableCollection<DBInfoNodeInfo> infos = repository.GetDBInfoNodeInfosOfLabelNodeDB(nodePath);

        
            LabelNodeDataInfo nodeInfo = LabelNodeHelper.changeToLabelNodeDataInfo(dbobj);
            if (nodeInfo != null)
            {
     
                nodeInfo.AttachInfoNodeInfos = infos;
            }

            return nodeInfo;
        }

        public void UpdateNodePath(string oldPath, string newPath)
        {
            repository.UpdateNodePaths(oldPath, newPath);
        }


        public void DeleteFilesOfInfoNodeDB(String InfoNodeDBPath, List<int> fileIDs)
        {
            repository.DeleteInfoNodesOfLabelNodeDB(InfoNodeDBPath, fileIDs);
        }
        /// <summary>
        /// 按照fileID从数据库中提取文件的二进制数据
        /// </summary>
        /// <param name="fileID"></param>
        /// <returns></returns>
        public byte[] getFileContent(int fileID)
        {
            return repository.getInfoNodeContent(fileID);
        }

        /// <summary>
        /// 添加关联的InfoNode,并传回ID
        /// </summary>
        /// <param name="infoNodeInfo"></param>
        /// <param name="infoNodeContent"></param>
        public void AddInfoNodeAssociation(string nodePath, DBInfoNodeInfo infoNodeInfo, InfoNodeDB infoNode)
        {
            //InfoNodeDB infoNode = DBInfoNodeInfo.toInfoNodeDB(infoNodeInfo, infoNodeContent);
            repository.AddInfoNodeOfLabelNodeDB(nodePath, infoNode);
            //将ID传回
            infoNodeInfo.ID = infoNode.ID;
        }

        public LabelNodeDB GetLabelNodeDBWithoutInfoNodeDBsByPath(String path)
        {
            return repository.GetLabelNodeDBWithoutInfoNodeDBsByPath( path);
        }
    }
}
