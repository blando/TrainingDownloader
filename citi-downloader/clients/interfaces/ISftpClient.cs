using System;
using System.Collections.Generic;
using System.Text;

namespace CitiDownloader.wrappers
{
    public interface ISftpClient
    {
        void Upload(string file);
    }
}
