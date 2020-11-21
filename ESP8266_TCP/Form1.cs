using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ESP8266_TCP
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            button2.Enabled = false;

            LogNet = new HslCommunication.LogNet.LogNetSingle(Application.StartupPath + @"\log.txt");
        }

        private HslCommunication.ModBus.ModBusTcpClient modBusTcpClient;
        private HslCommunication.LogNet.ILogNet LogNet;

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                modBusTcpClient = new HslCommunication.ModBus.ModBusTcpClient(textBox1.Text, int.Parse(textBox2.Text));
                modBusTcpClient.LogNet = LogNet;

                textBox1.Enabled = false;
                textBox2.Enabled = false;
                button1.Enabled = false;
                button2.Enabled = true;

                //groupBox2.Enabled = true;
                // groupBox3.Enabled = true;

                if (!modBusTcpClient.ConnectServer().IsSuccess)
                {
                    MessageBox.Show("連接ModBus TCP 失敗");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("失敗：" + ex.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //    groupBox2.Enabled = false;
            //   groupBox3.Enabled = false;

            button2.Enabled = false;
            button1.Enabled = true;

            textBox1.Enabled = true;
            textBox2.Enabled = true;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (!ushort.TryParse(textBox8.Text, out ushort address))
            {
                MessageBox.Show("IP位址格式錯誤或超出範圍");
                textBox8.Focus();
                return;
            }

            if (!short.TryParse(textBox7.Text, out short value))
            {
                MessageBox.Show("寫入格式錯誤或超出範圍");
                textBox7.Focus();
                return;
            }

            for (int i = 0; i < 1; i++)
            {
                HslCommunication.OperateResult write = modBusTcpClient.WriteOneRegister(address, value);
                if (write.IsSuccess)
                {
                    MessageInfoShow("寫入位址" + address + "完成");
                }
                else
                {
                    MessageBox.Show(write.ToMessageShowString());
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 20; i++)
            {
                HslCommunication.OperateResult<ushort, ushort> result = GetAddressAndLength();
                if (result.IsSuccess)
                {
                    MessageResultShow(modBusTcpClient.ReadRegister(result.Content1, result.Content2));

                }
                else
                {
                    MessageBox.Show(result.ToMessageShowString());
                }
            }
        }

        private HslCommunication.OperateResult<ushort, ushort> GetAddressAndLength()
        {
            HslCommunication.OperateResult<ushort, ushort> result = new HslCommunication.OperateResult<ushort, ushort>();
            try
            {
                result.Content1 = ushort.Parse(textBox5.Text);
                result.Content2 = ushort.Parse(textBox6.Text);
                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }

            return result;
        }

        private void MessageInfoShow(string message)
        {
            if (textBox3.IsHandleCreated && textBox3.InvokeRequired)
            {
                textBox3.Invoke(new Action<string>(MessageInfoShow), message);
                return;
            }
            // textBox3.AppendText(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + message + Environment.NewLine);
            textBox3.AppendText(message + Environment.NewLine);
        }

        private void MessageResultShow(HslCommunication.OperateResult<byte[]> result)
        {
            if (result.IsSuccess)
            {
                MessageInfoShow(HslCommunication.BasicFramework.SoftBasic.ByteToHexString(result.Content, ' '));
            }
            else
            {
                MessageBox.Show(result.ToMessageShowString());
            }
        }
        private void MessageResultShowBool(HslCommunication.OperateResult<byte[]> result, int length)
        {
            if (result.IsSuccess)
            {
                MessageInfoShow(GetStringFromBoolArray(HslCommunication.BasicFramework.SoftBasic.ByteToBoolArray(result.Content, length)));
            }
            else
            {
                MessageBox.Show(result.ToMessageShowString());
            }
        }

        private void MessageResultShow(HslCommunication.OperateResult result)
        {
            if (result.IsSuccess)
            {
                MessageInfoShow("寫入成功");
            }
            else
            {
                MessageBox.Show(result.ToMessageShowString());
            }
        }

        private string GetStringFromBoolArray(bool[] array)
        {
            StringBuilder sb = new StringBuilder("[");
            if (array != null)
            {
                foreach (var m in array)
                {
                    sb.Append(m);
                    sb.Append(",");
                }
            }

            if (sb.Length > 1)
            {
                sb.Remove(sb.Length - 1, 1);
            }
            sb.Append("]");

            return sb.ToString();
        }
    }
}
