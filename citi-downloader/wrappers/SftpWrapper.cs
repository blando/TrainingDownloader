using Rebex.Net;
using System;
using System.Collections.Generic;
using System.Text;

namespace CitiDownloader.wrappers
{
    public class SftpWrapper : ISftpWrapper
    {
        private ApplicationConfiguration config;

        public SftpWrapper(ApplicationConfiguration config)
        {
            this.config = config;
        }

        public void Upload(string file)
        {
            string remotePath = string.Format("{0}/{1}", config.SftpRemotePath, config.SftpUploadFileName);
            using (var client = new Sftp())
            {
                client.Connect(config.SftpServer);
                client.Login(config.SftpUserName, config.SftpPassword);
                client.Upload(file, remotePath);
            }
        }
    }
}
