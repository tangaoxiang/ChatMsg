using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Queue队列泛型类的演示
{
    public partial class Form1 : Form
    {
        //创建一个只能存储int类型的队列
        Queue<int> q = new Queue<int>(500);

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //入队
            for (int i = 0; i < 10; i++)
            {
                q.Enqueue(i);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int count = q.Count;
            string res = "";

            for (int i = 0; i < count; i++)
            {
                res+=q.Dequeue();
            }

            MessageBox.Show(res);
             
        }
    }
}
