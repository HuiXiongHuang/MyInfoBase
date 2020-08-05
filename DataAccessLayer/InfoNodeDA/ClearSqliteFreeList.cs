using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;


namespace DataAccessLayer.InfoNodeDA
{
   public  class ClearSqliteFreeList
    {
        private string database_name;
      public  ClearSqliteFreeList(string EFconstring)
        {
            database_name = DALConfig.getEFConnectionString(EFconstring);
        }
        public  void ClearFreeList()
        {
            string str ="sqlite3" +database_name+ "'VACUUM';";

            System.Diagnostics.Process p = new System.Diagnostics.Process();
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.UseShellExecute = false;    //是否使用操作系统shell启动
            p.StartInfo.RedirectStandardInput = true;//接受来自调用程序的输入信息
            p.StartInfo.RedirectStandardOutput = true;//由调用程序获取输出信息
            p.StartInfo.RedirectStandardError = true;//重定向标准错误输出
            p.StartInfo.CreateNoWindow = true;//不显示程序窗口
            p.Start();//启动程序

            //向cmd窗口发送输入信息
            p.StandardInput.WriteLine(str + "&exit");

            p.StandardInput.AutoFlush = true;
            //p.StandardInput.WriteLine("exit");
            //向标准输入写入要执行的命令。这里使用&是批处理命令的符号，表示前面一个命令不管是否执行成功都执行后面(exit)命令，如果不执行exit命令，后面调用ReadToEnd()方法会假死
            //同类的符号还有&&和||前者表示必须前一个命令执行成功才会执行后面的命令，后者表示必须前一个命令执行失败才会执行后面的命令



            //获取cmd窗口的输出信息
            //string output = p.StandardOutput.ReadToEnd();

            //StreamReader reader = p.StandardOutput;
            //string line=reader.ReadLine();
            //while (!reader.EndOfStream)
            //{
            //    str += line + "  ";
            //    line = reader.ReadLine();
            //}

          //  p.WaitForExit();//等待程序执行完退出进程
            p.Close();


            //Console.WriteLine(output);
        }

       //public  void sqlConnection(string username, string password)
       // {
       //     string strSQLconn = ""; strSQLconn += "Server=localhost;"; //Server是服务器地址，如果是本地的数据库可以直接写localhost  
       //     strSQLconn += "initial catalog=HFD;";   //数据库名称        
       //     strSQLconn += "user id=sa;";       //登录数据库的用户名            

       //     strSQLconn += "password=12345;";   //数据库的登录密码，这里是我自己的，你需要输入你自己的用户名和密码      
       //     strSQLconn += "Connect Timeout=5";  // 数据超时时间        
       //     string sqlStr = "SELECT count(1) FROM tb_Users WHERE username='" + username +"' and password='"+password+"' ";     //数据库的查询语句  
       //     SqlConnection conn;    //声明连接      
       //     conn = new SqlConnection(strSQLconn);       
       //     conn.Open();

       //     SqlCommand comm = new SqlCommand(sqlStr, conn);
       //     comm = new SqlCommand("VACUUM", conn);
       // }
       
    }
}
