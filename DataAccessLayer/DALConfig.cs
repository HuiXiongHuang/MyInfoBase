using System;

using System.Data.SqlClient;

namespace DataAccessLayer
{
    /// <summary>
    /// 
    /// 封装数据存取层的一些配置信息
    /// </summary>
    public class DALConfig
    {
        /// <summary>
        /// 最大sqlce数据库文件大小设置为不超过4G
        /// </summary>
        // private static String connectStringTemplate = "metadata=res://*/MyDBModel.csdl|res://*/MyDBModel.ssdl|res://*/MyDBModel.msl;provider=System.Data.SqlServerCe.4.0;provider connection string=\"data source={0};Max Database Size=4000;\"";
        /// <summary>
        /// sql server数据库连接字符串格式
        /// </summary>
        //  private static String connectStringTemplate= "metadata=res://*/MyDBModel.csdl|res://*/MyDBModel.ssdl|res://*/MyDBModel.msl;provider=System.Data.SqlClient;provider connection string=&quot;data source={0};integrated security=True;MultipleActiveResultSets=True;App=EntityFramework&quot;";

        /// <summary>
        /// sqlite数据库连接字符串格式
        /// </summary>
        private static String connectStringTemplate = "metadata=res://*/MyDBModelOfSqlite.csdl|res://*/MyDBModelOfSqlite.ssdl|res://*/MyDBModelOfSqlite.msl;provider=System.Data.SQLite.EF6;provider connection string=\"data source={0};\"";

        public static String getEFConnectionString(String dbFileName)
        {
            return String.Format(connectStringTemplate, dbFileName);
        }
        public static String getEFConnectionStringSqlce(String dbFileName)
        {
            return String.Format(connectStringTemplate, dbFileName);
        }
        public static string getConStrSql()
        {

            string connectionString = new System.Data.EntityClient.EntityConnectionStringBuilder
            {
                Metadata = "res://*",
                Provider = "System.Data.SqlClient",
                ProviderConnectionString = new System.Data.SqlClient.SqlConnectionStringBuilder
                {
                    InitialCatalog = "MyDB",
                    DataSource = "192.168.43.211",
                    IntegratedSecurity = false,
                    //UserID = getUID(),                 // User ID such as "sa"
                    //Password = getPWD(),               // hide the password
                    UserID = "sa",                 // User ID such as "sa"
                    Password = "999999",               // hide the password
                }.ConnectionString
            }.ConnectionString;

            return connectionString;
        }

        //public static string BuildEntityConnectionStringFromAppSettings(string nameOfConnectionString)
        //{
        //    //var shortConnectionString = GetConnectionStringByName(nameOfConnectionString);

        //    // Specify the provider name, server and database. 
        //    string providerName = "System.Data.SqlClient";

        //    // Initialize the connection string builder for the 
        //    // underlying provider taking the short connection string.
        //    SqlConnectionStringBuilder sqlBuilder =
        //        new SqlConnectionStringBuilder(shortConnectionString);

        //    // Set the properties for the data source.
        //    sqlBuilder.IntegratedSecurity = false;

        //    // Build the SqlConnection connection string. 
        //    string providerString = sqlBuilder.ToString();

        //    // Initialize the EntityConnectionStringBuilder.
        //    EntityConnectionStringBuilder entityBuilder =
        //        new EntityConnectionStringBuilder();

        //    //Set the provider name.
        //    entityBuilder.Provider = providerName;

        //    // Set the provider-specific connection string.
        //    entityBuilder.ProviderConnectionString = providerString;

        //    // Set the Metadata location.
        //    entityBuilder.Metadata = String.Format("res://*/Application.{0}.Data.Model.{0}Model.csdl|res://*/Application.{0}.Data.Model.{0}Model.ssdl|res://*/Application.{0}.Data.Model.{0}Model.msl", nameOfConnectionString);
        //    return entityBuilder.ToString();
        //}
        /// <summary>
        /// 由SQL Server每行限制为8060字节，因此，在此限制Text字段保存的最长字串为3000个字符（之所以是不能更大一些，是因为
        /// Path字段也有长度）
        /// </summary>
        public const int MaxTextFieldSize = 3000;
    }
}
