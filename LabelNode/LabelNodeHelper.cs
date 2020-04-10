using DataAccessLayer;
using Model;
using System;
using System.Text;

namespace LabelNode
{
    /// <summary>
    /// 封装一些辅助功能
    /// </summary>
    public class LabelNodeHelper
    {
        /// <summary>
        /// 转换为LabelNode对象，不理会附属文件集合
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static LabelNodeDB changeToLabelNodeDB(LabelNodeDataInfo obj)
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

            LabelNodeDB dbObj = new LabelNodeDB()
            {
                ModifyTime = obj.ModifyTime,
                Path = obj.Path,
              //RTFText = String.IsNullOrEmpty(obj.RTFText) ? null : Encoding.UTF8.GetBytes(obj.RTFText),
                Label = String.IsNullOrEmpty(obj.Text) ? "" : obj.Text,
                ID = obj.ID
            };

            return dbObj;
        }
        /// <summary>
        /// 转换为LabelNodeDataInfo，不理会附属文件集合
        /// </summary>
        /// <param name="dbobj"></param>
        /// <returns></returns>
        public static LabelNodeDataInfo changeToLabelNodeDataInfo(LabelNodeDB dbobj)
        {
            if (dbobj == null)
            {
                return null;
            }
            LabelNodeDataInfo obj = new LabelNodeDataInfo()
            {
                 Text = String.IsNullOrEmpty(dbobj.Label) ? "" : dbobj.Label,
              //  RTFText = dbobj.RTFText == null ? "" : Encoding.UTF8.GetString(dbobj.RTFText),
                Path = dbobj.Path,
                ModifyTime = dbobj.ModifyTime.Value,
                ID = dbobj.ID

            };
            return obj;
        }

    }
}
