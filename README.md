# FileManagment
一个使用HslCommunication组件实现了文件管理引擎，在客户端块演示了文件的上传，下载，删除，文件集获取操作。

## 特性支持
* 支持本地文件的上传，下载，删除，重复上传即为更新。
* 支持服务器的文件多种信息存储，包括上传人，文件名，文件大小，文件标记，下载次数，上传日期。
* 支持三种数据的操作，本地文件，流数据，bitmap位图。
* 在服务器端实现了无锁的文件读写，多客户端同时操作（上传，下载，删除）互不影响
* 支持进度报告，针对上传和下载操作，允许传入一个进度报告的方法，详细情况参考代码。

## 代码示例
<pre>
<code>
        private void userButton2_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(textBox1.Text))
            {
                // 点击开始上传，此处按照实际项目需求放到了后台线程处理，事实上这种耗时的操作就应该放到后台线程
                System.Threading.Thread thread = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart((ThreadUploadFile)));
                thread.IsBackground = true;
                thread.Start(textBox1.Text);
                userButton2.Enabled = false;
                progressBar1.Value = 0;
            }
        }

        private void ThreadUploadFile(object filename)
        {
            if (filename is string fileName)
            {
                System.IO.FileInfo fileInfo = new System.IO.FileInfo(fileName);
                // 开始正式上传，关于三级分类，下面只是举个例子，上传成功后去服务器端寻找文件就能明白
                OperateResult result = integrationFileClient.UploadFile(
                    fileName,                   // 需要上传的原文件的完整路径，上传成功还需要个条件，该文件不能被占用
                    fileInfo.Name,              // 在服务器存储的文件名，带后缀，一般设置为原文件的文件名
                    "Files",                    // 第一级分类，指示文件存储的类别，对应在服务器端存储的路径不一致
                    "Personal",                 // 第二级分类，指示文件存储的类别，对应在服务器端存储的路径不一致
                    "Admin",                    // 第三级分类，指示文件存储的类别，对应在服务器端存储的路径不一致
                    "这个文件非常重要",         // 这个文件的额外描述文本，可以为空（""）
                    "张三",                     // 文件的上传人，当然你也可以不使用
                    UpdateReportProgress        // 文件上传时的进度报告，如果你不需要，指定为NULL就行，一般文件比较大，带宽比较小，都需要进度提示
                    );

                // 切换到UI前台显示结果
                Invoke(new Action<OperateResult>(operateResult =>
                {
                    userButton2.Enabled = true;
                    if (result.IsSuccess)
                    {
                        MessageBox.Show("文件上传成功！");
                    }
                    else
                    {
                        // 失败原因多半来自网络异常，还有文件不存在，分类名称填写异常
                        MessageBox.Show("文件上传失败：" + result.ToMessageShowString());
                    }
                }), result);
            }
        }

        /// &lt;summary>
        /// 用于更新上传进度的方法，该方法是线程安全的
        /// &lt;/summary>
        /// <param name="sended">已经上传的字节数</param>
        /// <param name="totle">总字节数</param>
        private void UpdateReportProgress(long sended, long totle)
        {
            if (progressBar1.InvokeRequired)
            {
                progressBar1.Invoke(new Action<long, long>(UpdateReportProgress), sended, totle);
                return;
            }


            // 此处代码是安全的
            int value = (int)(sended * 100L / totle);
            progressBar1.Value = value;
        }
</code>
</pre>

## 截图示例
![](https://github.com/dathlin/FileManagment/raw/master/img/file001.png)

![](https://github.com/dathlin/FileManagment/raw/master/img/file002.png)