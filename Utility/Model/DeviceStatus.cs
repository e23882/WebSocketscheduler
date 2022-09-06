using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility
{
    public class DeviceStatus
    {
        #region Property
        public float CPU { get; set; }
        public float RAM { get; set; }

        public List<Disk> DISK{get;set;}
        #endregion

    }

    public class Disk 
    {
        #region Property
        public string DiskName { get; set; }
        public float Capacity { get; set; }
        #endregion
    }
}
