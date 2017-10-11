using Telerik.Windows;
using Telerik.Windows.Controls;

namespace OPC_EXPLORE.View
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void RadMenuItem_OnClick(object sender, RadRoutedEventArgs e)
        {
            RadMenuItem rmi = sender as RadMenuItem;
            if(rmi == null) return;
            if (rmi.Header.Equals("连接"))
            {
                ConnectWin cw = new ConnectWin {Owner = this};
                cw.ShowDialog();
            }
        }
    }
}