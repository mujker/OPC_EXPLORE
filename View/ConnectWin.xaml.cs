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
using System.Windows.Shapes;
using OPCAutomation;
using OPC_EXPLORE.Commons;
using Telerik.Windows.Controls;

namespace OPC_EXPLORE.View
{
    /// <summary>
    /// ConnectWin.xaml 的交互逻辑
    /// </summary>
    public partial class ConnectWin : RadWindow
    {
        public OPCClient Opct;
        public ConnectWin()
        {
            InitializeComponent();
            InitData();
        }

        private void InitData()
        {
            Opct = new OPCClient();
            Opct.GetOPCServers();
            CbPcName.Items.Add("localhost");
            foreach (var serverName in Opct.serverNames)
            {
                LbServer.Items.Add(serverName);
            }
            LbServer.SelectionChanged += LbServerOnSelectionChanged;
            LbServer.SelectedIndex = 0;
        }

        /// <summary>
        /// 选择服务器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="selectionChangedEventArgs"></param>
        private void LbServerOnSelectionChanged(object sender, SelectionChangedEventArgs selectionChangedEventArgs)
        {
            TbServerName.Text = LbServer.SelectedValue.ToString();
        }

        /// <summary>
        /// 连接
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnConnect_OnClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(LbServer.SelectedValue.ToString())) return;
            Opct.ConnectServer(LbServer.SelectedValue.ToString());
            DialogResult = true;
            Close();
        }

        private void BtnCancel_OnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
