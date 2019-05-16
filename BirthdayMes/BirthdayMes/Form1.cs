using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BirthdayMes
{
    //用于测试,显示数据
    public partial class Form1 : Form
    {
        private string connstr;
        public Form1()
        {
            InitializeComponent();
            //连接我的本地数据库
            connstr = "Server=localhost;Database=TestDB;Trusted_Connection=SSPI";
        }

        private void addbutton_Click(object sender, EventArgs e)
        {
            string[] date = DateTime.Now.ToString("MM-dd").Split('-');
            string monthday = date[0] + date[1];
            string sql = "select UserName,Telephone from UserInfo where substring(PersonID,11,4) =" + monthday;
            dataGridView1.DataSource = BirthdayMes.ExcuteQuery(connstr, sql);
            BirthdayMessage.insertMessage(connstr);
        }

        private void showbutton_Click(object sender, EventArgs e)
        {
            string sql = "select * from UserMessage";
            dataGridView1.DataSource = BirthdayMes.ExcuteQuery(connstr, sql);
        }
    }
}
