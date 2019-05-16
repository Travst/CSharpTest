using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;

namespace BirthdayMes
{
    class BirthdayMes
    {
        //private string connStr = "Server=localhost;Database=TestDB;Trusted_Connection=SSPI";
        //private string connStr = "Data Source=LAPTOP-1GGVO8JF;Initial Catalog=TestDB;Integrated Security=True";

        static public DataTable ExcuteQuery(String connstr, string sql)
        {
            DataTable dt = new DataTable();
            try
            {
                using(SqlConnection conn = new SqlConnection(connstr))
                {
                    conn.Open();
                    using(SqlDataAdapter msda = new SqlDataAdapter(sql, conn))
                    {
                        msda.Fill(dt);
                    }
                    conn.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            //没有填充数据返回null
            if (dt.Rows.Count == 0)
                dt = null;
            return dt;
        }
    }

    public class BirthdayMessage
    {
        private static bool MessageDB(string connstr, DataTable dt)
        {
            //初始化待写入table数据
            string message = "【东莞农行】亲爱的X，今天是您的生日。东莞农行的发展，离不开您的努力和付出。"
                +"在这个特殊的日子里，真诚祝愿您：生日快乐，生活美满，工作顺利！\n"
                +"--中国农业银行东莞分行";
            DataTable table = new DataTable();
            table.Columns.Add("Phone", typeof(string));
            table.Columns.Add("Message", typeof(string));
            foreach(DataRow dr in dt.Rows)
            {
                DataRow drow = table.NewRow();
                drow["Phone"] = dr["Telephone"];
                drow["Message"] = message.Replace("X", dr["UserName"].ToString());
                table.Rows.Add(drow);
            }

            SqlTransaction tran = null;//声明一个事务对象
            try
            {
                using(SqlConnection conn = new SqlConnection(connstr))
                {
                    conn.Open();
                    using(tran = conn.BeginTransaction())
                    {
                        using(SqlBulkCopy scopy = new SqlBulkCopy(conn, SqlBulkCopyOptions.Default, tran))
                        {
                            scopy.DestinationTableName = "TestDB.dbo.[UserMessage]";//指定服务器上的目标表
                            scopy.WriteToServer(table);//table数据写入数据库
                            tran.Commit();//提交事务
                            return true;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                if (tran != null)
                    tran.Rollback();
                return false;
            }
        }
        //调用接口
        //参数为待使用数据库连接字符串，例如"Server=localhost;Database=TestDB;Trusted_Connection=SSPI"
        public static bool insertMessage(string connectstring)
        {
            bool result = false;
            string connstr = connectstring;
            //取出当前系统时间
            string[] date = DateTime.Now.ToString("MM-dd").Split('-');
            string monthday = date[0] + date[1];
            string sql = "select UserName,Telephone from UserInfo where substring(PersonID,11,4) ="+monthday;
            //执行查询
            DataTable table = BirthdayMes.ExcuteQuery(connstr, sql);
            if (table == null)
                result = false;
            else if (MessageDB(connstr, table) == true)
                result = true;
            return result;
        }
    }
}
