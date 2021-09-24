using MySql.Data.MySqlClient;//调用MySQL动态库

namespace ClassLibrary_FQY
{
    /// <summary>
    /// MySQL操作类
    /// </summary>
    public class mysqlHelper
    {
        /// <summary>
        /// 连接一个已经存在的数据库,并打开
        /// </summary>
        /// <param name="strSQL">数据库信息SQL语句</param>
        public void ConnectMysql(string strConn)
        {
            try
            {
                MySqlConnection mySqlConnection = new MySqlConnection(strConn);
                mySqlConnection.OpenAsync();
            }
            catch
            {


            }

        }

        /// <summary>
        /// 连接一个已经存在的数据库,并关闭
        /// </summary>
        /// <param name="strSQL">数据库信息SQL语句</param>
        public void DisconnectMysql(string strConn)
        {
            try
            {
                MySqlConnection mySqlConnection = new MySqlConnection(strConn);
                mySqlConnection.CloseAsync();
            }
            catch
            {


            }
        }

        /// <summary>
        /// 创建数据库
        /// </summary>
        /// <param name="strSQL">数据库登录信息</param>
        /// <param name="name">数据库名称</param>
        /// <returns>受SQL语句影响的行数</returns>
        public int CreateDB(string strConn, string name)
        {
            //如果不存在，则创建；否则，不执行
            string DBname = string.Format("CREATE DATABASE IF NOT EXISTS {0}", name);
            MySqlConnection conn = new MySqlConnection(strConn);
            MySqlCommand cmd = new MySqlCommand(DBname, conn);
            conn.OpenAsync();
            int result = cmd.ExecuteNonQuery();//对连接对象执行SQL语句，返回受影响的行数
            conn.CloseAsync();
            return result;
        }

        /// <summary>
        /// 删除数据库
        /// </summary>
        /// <param name="strSQL">数据库登录信息</param>
        /// <param name="name">数据库名称</param>
        /// <returns>受SQL语句影响的行数</returns>
        public int DropDB(string strConn, string name)
        {
            //如果存在，则删除；否则，不执行
            string DBname = string.Format("DROP DATABASE IF EXISTS {0}", name);
            MySqlConnection conn = new MySqlConnection(strConn);
            MySqlCommand cmd = new MySqlCommand(DBname, conn);
            conn.OpenAsync();
            int result = cmd.ExecuteNonQuery();//对连接对象执行SQL语句，返回受影响的行数
            conn.CloseAsync();
            return result;
        }
        /// <summary>
        /// 创建数据表
        /// </summary>
        /// <param name="strSQL">数据库登录信息</param>
        /// <param name="name">数据表名称</param>
        /// <returns>受SQL语句影响的行数</returns>
        public int CreateTable(string strConn, string name)
        {
            string strCreateTable = string.Format("CREATE TABLE IF NOT EXISTS {0}", name);//不存在创建，已存在不创建
            MySqlConnection conn = new MySqlConnection(strConn);
            MySqlCommand cmd = new MySqlCommand(strCreateTable, conn);
            conn.OpenAsync();
            int result = cmd.ExecuteNonQuery();//对连接对象执行SQL语句，返回受影响的行数
            conn.CloseAsync();
            return result;
        }

        /// <summary>
        /// 删除数据表
        /// </summary>
        /// <param name="strSQL">数据库登录信息</param>
        /// <param name="name">数据表名称</param>
        /// <returns>受SQL语句影响的行数</returns>
        public int DropTable(string strConn, string name)
        {
            string strDropTable = string.Format("CREATE TABLE IF EXISTS {0}", name);
            MySqlConnection conn = new MySqlConnection(strConn);
            MySqlCommand cmd = new MySqlCommand(strDropTable, conn);
            conn.OpenAsync();
            int result = cmd.ExecuteNonQuery();//对连接对象执行SQL语句，返回受影响的行数
            conn.CloseAsync();
            return result;
        }
        /// <summary>
        /// SQL语句通用指令
        /// </summary>
        /// <param name="strConn">数据库登录信息</param>
        /// <param name="strSQL">SQL语句</param>
        /// <returns>受SQL语句影响的行数</returns>
        public int SQL_GeneralCommand(string strConn, string strSQL)
        {
            MySqlConnection conn = new MySqlConnection(strConn);
            MySqlCommand Cmd = new MySqlCommand(strSQL, conn);
            conn.OpenAsync();
            int result = Cmd.ExecuteNonQuery();
            conn.CloseAsync();
            return result;
        }
        /// <summary>
        /// 读取数据表中的数据
        /// </summary>
        /// <param name="strConn">数据库登录信息</param>
        /// <param name="name">表名</param>
        /// <returns>SQL查询内容</returns>
        public MySqlDataReader ReadData(string strConn, string name)
        {
            string sqlCommand = string.Format("SELECT * FROM {0}", name);
            MySqlConnection conn = new MySqlConnection(strConn);
            MySqlCommand cmd = new MySqlCommand(sqlCommand, conn);
            MySqlDataReader reader = null;//查询结果读取器
            conn.OpenAsync();
            reader = cmd.ExecuteReader();//执行查询，并将结果返回给读取器
            reader.Close();
            conn.CloseAsync();
            return reader;
        }



    }
}
