using HslCommunication.Enthernet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace File.Server
{
    public partial class FormServer : Form
    {
        public FormServer()
        {
            InitializeComponent();
        }

        #region 服务器端代码
        
        private UltimateFileServer ultimateFileServer;                                            // 引擎对象

        private void UltimateFileServerInitialization()
        {
            ultimateFileServer = new UltimateFileServer();                                        // 实例化对象
            ultimateFileServer.KeyToken = new Guid("A8826745-84E1-4ED4-AE2E-D3D70A9725B5");       // 指定一个令牌
            ultimateFileServer.LogNet = new HslCommunication.LogNet.LogNetSingle(Application.StartupPath + @"\Logs\123.txt");
            ultimateFileServer.FilesDirectoryPath = Application.StartupPath + @"\UltimateFile";   // 所有文件存储的基础路径
            ultimateFileServer.ServerStart(34567);                                                // 启动一个端口的引擎

            // 订阅一个目录的信息，使用文件集容器实现
            GroupFileContainer container = ultimateFileServer.GetGroupFromFilePath(Application.StartupPath + @"\UltimateFile\Files\Personal\Admin");
            container.FileCountChanged += Container_FileCountChanged;                         // 当文件数量发生变化时触发
        }

        private void Container_FileCountChanged(int obj)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<int>(Container_FileCountChanged), obj);
                return;
            }

            label1.Text = "文件数量：" + obj.ToString();
        }

        private void userButton1_Click(object sender, EventArgs e)
        {
            // 点击了启动服务器端的文件引擎
            UltimateFileServerInitialization();
            userButton1.Enabled = false;
        }
        
        #endregion


    }
}
