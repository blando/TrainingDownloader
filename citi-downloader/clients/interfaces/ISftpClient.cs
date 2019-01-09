using System;
using System.Collections.Generic;
using System.Text;

namespace TrainingDownloader.wrappers
{
    public interface ISftpClient
    {
        void Upload(string file);
    }
}
