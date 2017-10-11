using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using OPCAutomation;

namespace OPC_EXPLORE.Commons
{
    public class OPCClient
    {
        private OPCServer KepServer;
        private OPCGroups KepGroups;
        public OPCGroup KepGroup;
        private OPCItems KepItems;
        private OPCItem KepItem;
        int itmHandleClient = 0;
        int itmHandleServer = 0;

        public object readValue;

        public List<string> serverNames = new List<string>();
        public List<string> Tags = new List<string>();

        /// <summary>
        /// 枚举本地OPC SERVER
        /// </summary>
        public void GetOPCServers()
        {
            IPHostEntry IPHost = Dns.GetHostEntry(Environment.MachineName);

            LogHelper.WriteLog("MAC Address:");
            foreach (IPAddress ip in IPHost.AddressList)
            {
                LogHelper.WriteLog(ip.ToString());
            }
            LogHelper.WriteLog("Please Enter IPHOST");

            string strHostIP = "localhost"; //Console.ReadLine();

            IPHostEntry ipHostEntry = Dns.GetHostEntry(strHostIP);
            try
            {
                KepServer = new OPCServer();
                object serverList = KepServer.GetOPCServers(ipHostEntry.HostName.ToString());
                int i = 0;
                foreach (string serverName in (Array) serverList)
                {
                    LogHelper.WriteLog(i + "." + serverName);
                    serverNames.Add(serverName);
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("Connect Error", ex);
            }
        }

        /// <summary>
        /// 连接OPC SERVER
        /// </summary>
        /// <param name="serverName">OPC SERVER名字</param>
        public void ConnectServer(string serverName)
        {
            try
            {
                KepServer.Connect(serverName, "");
                CreateGroup("");
                CreateItems();
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("Connect Error", ex);
            }
        }

        /// <summary>
        /// 创建组,组名无所谓
        /// </summary>
        private void CreateGroup(string groupName)
        {
            try
            {
                KepGroups = KepServer.OPCGroups;
                KepGroup = KepGroups.Add(groupName);
                KepServer.OPCGroups.DefaultGroupIsActive = true;
                KepServer.OPCGroups.DefaultGroupDeadband = 0;
                KepGroup.UpdateRate = 250;
                KepGroup.IsActive = true;
                KepGroup.IsSubscribed = true;
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("Create group error", ex);
            }
        }

        private void CreateItems()
        {
            KepItems = KepGroup.OPCItems;
            KepGroup.DataChange += new DIOPCGroupEvent_DataChangeEventHandler(KepGroup_DataChange);
        }

        private void KepGroup_DataChange(int TransactionID, int NumItems, ref Array ClientHandles, ref Array ItemValues,
            ref Array Qualities, ref Array TimeStamps)
        {
            for (int i = 1; i <= NumItems; i++)
            {
                readValue = ItemValues.GetValue(i).ToString();
            }
        }

        private void GetTagValue(string tagName)
        {
            try
            {
                readValue = "";
                if (itmHandleClient != 0)
                {
                    Array Errors;
                    OPCItem bItem = KepItems.GetOPCItem(itmHandleServer);
                    //注：OPC中以1为数组的基数
                    int[] temp = new int[2] {0, bItem.ServerHandle};
                    Array serverHandle = (Array) temp;
                    //移除上一次选择的项
                    KepItems.Remove(KepItems.Count, ref serverHandle, out Errors);
                }
                itmHandleClient = 12345;
                KepItem = KepItems.AddItem(tagName, itmHandleClient);
                itmHandleServer = KepItem.ServerHandle;
            }
            catch (Exception err)
            {
                //没有任何权限的项，都是OPC服务器保留的系统项，此处可不做处理。
                itmHandleClient = 0;
                LogHelper.WriteLog("Read value error", err);
            }
        }

        public void WriteValue(string tagName, object _value)
        {
            GetTagValue(tagName);
            OPCItem bItem = KepItems.GetOPCItem(itmHandleServer);
            int[] temp = new int[2] {0, bItem.ServerHandle};
            Array serverHandles = (Array) temp;
            object[] valueTemp = new object[2] {"", _value};
            Array values = (Array) valueTemp;
            Array Errors;
            int cancelID;
            KepGroup.AsyncWrite(1, ref serverHandles, ref values, out Errors, 2009, out cancelID);
            //KepItem.Write(txtWriteTagValue.Text);//这句也可以写入，但并不触发写入事件
            GC.Collect();
        }

        public object ReadValue(string tagName)
        {
            GetTagValue(tagName);
            Thread.Sleep(500);
            try
            {
                return KepItem.Value;
            }
            catch
            {
                return null;
            }
        }

        public void ReadValue(string tagName, bool wtf)
        {
            GetTagValue(tagName);
            OPCItem bItem = KepItems.GetOPCItem(itmHandleServer);
            int[] temp = new int[2] {0, bItem.ServerHandle};
            Array serverHandles = (Array) temp;
            Array Errors;
            int cancel;
            KepGroup.AsyncRead(1, ref serverHandles, out Errors, 2009, out cancel);
            GC.Collect();
        }
    }
}