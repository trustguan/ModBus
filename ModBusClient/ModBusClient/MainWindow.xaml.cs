using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace _ModBusClient
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private ModBusClient _modBusClient;
        public MainWindow()
        {
            InitializeComponent();
            string[]portNames=ModBusClient.GetPortNames(); 
            foreach (string portName in portNames)
            {
                PortNames.Items.Add(portName);
            }
            
        }

        private void ConectBtn(object sender, RoutedEventArgs e)
        {
            if ((bool)UseRTUCheckBox.IsChecked)
            {
                _modBusClient = new ModBusClient(PortNames.SelectedItem.ToString(), 9600, 8, System.IO.Ports.Parity.None, System.IO.Ports.StopBits.One);
            }
            else
            {
                _modBusClient = new ModBusClient(IPEdit.Text, Int32.Parse(PortEdit.Text));

            }

            if (_modBusClient.Connect())
            {
                MessageBox.Show("连接成功！");
            }
            else
            {
                MessageBox.Show("连接失败！");
            }
        }

        private void CloseBtn(object sender, RoutedEventArgs e)
        {
            _modBusClient.Disconnect();
        }

        private void ReadUintBtn(object sender, RoutedEventArgs e)
        {
            List<ushort> val;
           bool ret=_modBusClient.ReadUintValues(Int32.Parse(RegisterAddrEdit.Text),1,out val);
            if (!ret)
            {
                MessageBox.Show("读取失败！");
            }
            else 
            {
               // float f = GetFloat(val[0], val[1],true,false);
                List<string> results=new List<string>();
                foreach (ushort v in val) {
                    results.Add($"{v}");
                }
                RegisterValEdit.Text = string.Join(",",results);
            }
        }

        private void WriteUintBtn(object sender, RoutedEventArgs e)
        {
            List<ushort> val=new List<ushort>();
            val.Add((ushort)Int32.Parse(RegisterValEdit.Text));
            bool ret = _modBusClient.WriteUintValues(0, val);
            if (!ret)
            {
                MessageBox.Show("写入失败！");
            }
            else {
                MessageBox.Show("写入成功！");
            }
        }

        private void ReadFloatBtn(object sender, RoutedEventArgs e) 
        {
            bool isBig=EndianComboBox.SelectedIndex == 0?true:false;
            bool isSwap=SwapComboBox.SelectedIndex == 0?false:true;
            List<float> val;
            bool ret = _modBusClient.ReadFloatValues(Int32.Parse(RegisterAddrEdit.Text), 1, out val, 1, isBig, isSwap);
            if (!ret)
            {
                MessageBox.Show("读取失败！");
            }
            else
            {
                List<string> results = new List<string>();
                foreach (float v in val)
                {
                    results.Add($"{v}");
                }
                RegisterValEdit.Text = string.Join(",", results);
            }
        }

        private void WriteFloatBtn(object sender, RoutedEventArgs e)
        {
            bool isBig = EndianComboBox.SelectedIndex == 0 ? true : false;
            bool isSwap = SwapComboBox.SelectedIndex == 0 ? false : true;
            List<float> val = new List<float>();
            val.Add(float.Parse(RegisterValEdit.Text));
            bool ret = _modBusClient.WriteFloatValues(0, val,1,isBig,isSwap);
            if (!ret)
            {
                MessageBox.Show("写入失败！");
            }
            else
            {
                MessageBox.Show("写入成功！");
            }
        }

        private void ReadDoubleBtn(object sender, RoutedEventArgs e)
        {
            bool isBig = EndianComboBox.SelectedIndex == 0 ? true : false;
            bool isSwap = SwapComboBox.SelectedIndex == 0 ? false : true;
            List<double> val;
            bool ret = _modBusClient.ReadDoubleValues(Int32.Parse(RegisterAddrEdit.Text), 1, out val, 1, isBig, isSwap);
            if (!ret)
            {
                MessageBox.Show("读取失败！");
            }
            else
            {
                List<string> results = new List<string>();
                foreach (double v in val)
                {
                    results.Add($"{v}");
                }
                RegisterValEdit.Text = string.Join(",", results);
            }
        }

        private void WriteDoubleBtn(object sender, RoutedEventArgs e)
        {
            bool isBig = EndianComboBox.SelectedIndex == 0 ? true : false;
            bool isSwap = SwapComboBox.SelectedIndex == 0 ? false : true;
            List<double> val = new List<double>();
            val.Add(float.Parse(RegisterValEdit.Text));
            bool ret = _modBusClient.WriteDoubleValues(0, val, 1, isBig, isSwap);
            if (!ret)
            {
                MessageBox.Show("写入失败！");
            }
            else
            {
                MessageBox.Show("写入成功！");
            }
        }

        private void ReadIOBtn(object sender, RoutedEventArgs e)
        {
            List<bool> val;
            bool ret = _modBusClient.ReadIOs(Int32.Parse(IOAddrEdit.Text), 1, out val);
            if (!ret)
            {
                MessageBox.Show("读取失败！");
            }
            else
            {
                List<string> results = new List<string>();
                foreach (bool v in val)
                {
                    if(v)
                      results.Add("1");
                    else
                      results.Add("0");
                }
                IOValEdit.Text = string.Join(",", results);
            }
        }

        private void WriteIOBtn(object sender, RoutedEventArgs e)
        {
            List<bool> val = new List<bool>();
            int v = int.Parse(IOValEdit.Text);
            if(v<=0)
              val.Add(false);
            else
              val.Add(true);
            bool ret = _modBusClient.WriteIOs(0, val);
            if (!ret)
            {
                MessageBox.Show("写入失败！");
            }
            else
            {
                MessageBox.Show("写入成功！");
            }
        }
    }
}
