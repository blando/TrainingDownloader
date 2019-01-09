using System;
using System.Collections.Generic;
using System.Text;
using TrainingDownloader.models;

namespace TrainingDownloader.services.interfaces
{
    public interface IVendorUserService
    {
        VendorUser CreateVendorUser(VendorRecord vRecord);
    }
}
