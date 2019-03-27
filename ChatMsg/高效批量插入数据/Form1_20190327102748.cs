using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 高效批量插入数据
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //1.0 
            string str = "server=.;database=test;uid=sa;pwd=master;";

            //2.0 构造一个内存表
            DataTable dt = new DataTable();
            dt.Columns.Add("name1", typeof(string));

            //2.0.1 给内存表增加1w条数据
            for (int i = 0; i < 10000; i++)
            {
                DataRow dr = dt.NewRow();
                dr["name1"] = "test" + i;
                dt.Rows.Add(dr);
            }

            System.Diagnostics.Stopwatch st = new System.Diagnostics.Stopwatch();
            st.Start();
            //3.0 实例化SqlBulkCopy对象实例
            using (System.Data.SqlClient.SqlBulkCopy copy = new System.Data.SqlClient.SqlBulkCopy(str))
            {
                //将内存表中的列名称与数据表的列名称做一一映射
                copy.ColumnMappings.Add("name1", "CName");

                //指定要存入的数据表名称
                copy.DestinationTableName = "tb1";
                //调用WriteToServer 方法将数据批量的插入到指定的表中
                copy.WriteToServer(dt);
            }
            st.Stop();
            MessageBox.Show("批量新增1W条数据耗时=" + st.ElapsedMilliseconds + "毫秒");
        }
    }
}
