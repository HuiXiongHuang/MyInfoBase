using DataAccessLayer;
using Model;
using System;
using System.Text;

namespace InfoNode
{
    /// <summary>
    /// 封装一些辅助功能
    /// </summary>
    public class InfoNodeHelper
    {
        /// <summary>
        /// 转换为InfoNode对象，不理会附属文件集合
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static InfoNodeDB changeToInfoNodeDB(InfoNodeDataInfo obj)
        {
            if (obj == null)
            {
                return null;
            }
            //注意数据库中nvarchar最大允许4000个字符
            if (obj.Text != null && obj.Text.Length > DALConfig.MaxTextFieldSize)
            {
                obj.Text = obj.Text.Substring(0, DALConfig.MaxTextFieldSize);
            }

            InfoNodeDB dbObj = new InfoNodeDB()
            {
                ModifyTime = obj.ModifyTime,
                Path = obj.Path,
                RTFText = String.IsNullOrEmpty(obj.RTFText) ? null : Encoding.UTF8.GetBytes(obj.RTFText),// RTFText是内容
                Text = String.IsNullOrEmpty(obj.Text) ? "" : obj.Text,//Text该属性仅用于在数据库中查找文本
                Type=obj.IconType,
                ID = obj.ID
            };

            return dbObj;
        }
        /// <summary>
        /// 转换为InfoNodeInfo，不理会附属文件集合
        /// </summary>
        /// <param name="dbobj"></param>
        /// <returns></returns>
        public static InfoNodeDataInfo changeToInfoNodeDataInfo(InfoNodeDB dbobj)
        {
            if (dbobj == null)
            {
                return null;
            }
            InfoNodeDataInfo obj = new InfoNodeDataInfo()
            {
                  Text = String.IsNullOrEmpty(dbobj.Text) ? "" : dbobj.Text,
                RTFText = dbobj.RTFText == null ? "" : Encoding.UTF8.GetString(dbobj.RTFText),
                Path = dbobj.Path,
                ModifyTime = dbobj.ModifyTime.Value,
                ID = dbobj.ID

            };
            return obj;
        }

    }
}
