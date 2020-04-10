using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataAccessLayer
{
    /// <summary>
    /// 完成在多个表中搜索数据的工作
    /// </summary>
    public class NodeDataSearchRepository
    {
        /// <summary>
        /// 在数据库中查找包容指定字串的记录，返回其路径集合
        /// </summary>
        /// <param name="FindWhat"></param>
        /// <returns></returns>
        public List<String> SearchDataNodeText(String FindWhat, String EFConnectionString)
        {

            using (MyDBEntities context = new MyDBEntities(EFConnectionString))
            {
                List<String> result = new List<string>();
                

                //搜索文件夹节点表
                var query = from node in context.InfoNodeDBs
                        where node.Text.IndexOf(FindWhat) != -1
                        select node.Path;
                result.AddRange(query.ToList());
                return result;
            }
        }
    }
}
