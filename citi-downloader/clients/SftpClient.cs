using Renci.SshNet.Sftp;
using Renci.SshNet;
using Renci;
using System;
using System.Collections.Generic;
using System.Text;
using CitiDownloader.configurations;
using System.IO;

namespace CitiDownloader.wrappers
{
    public class SftpClient : ISftpClient
    {
        private ApplicationConfiguration config;

        public SftpClient(ApplicationConfiguration config)
        {
            this.config = config;
        }

        public void Upload(string file)
        {
            string remotePath = string.Format("{0}/{1}", config.SftpRemotePath, config.SftpUploadFileName);

            PasswordAuthenticationMethod password = new PasswordAuthenticationMethod(config.SftpUserName, config.SftpPassword);

            ConnectionInfo connectionInfo = new ConnectionInfo(
                config.SftpServer,
                config.SftpUserName, 
                password
            );

            using (Renci.SshNet.SftpClient client = new Renci.SshNet.SftpClient(connectionInfo))
            {
                client.Connect();
                using (var fileStream = new FileStream(file, FileMode.Open))
                {
                    client.UploadFile(fileStream, remotePath, null);
                }
            }

        }
    }
}
