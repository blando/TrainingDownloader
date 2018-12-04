using System;
using System.Collections.Generic;
using System.Text;

namespace CitiDownloader.wrappers
{
    public interface ISftpWrapper
    {
        void Upload(string file);
    }
}
