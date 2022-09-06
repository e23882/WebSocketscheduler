using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

namespace Utility
{
    public static class Hardward
    {
        #region Declarations
        private static PerformanceCounter _CPU = new PerformanceCounter("Processor", "% Processor Time", "_Total");
        //private static PerformanceCounter _RAM = new PerformanceCounter("Memory", "% Committed Bytes in Use");
        private static PerformanceCounter _RAM = new PerformanceCounter("Memory", "% Committed Bytes in Use");
        #endregion

        #region Property
        /// <summary>
        /// 目前設備CPU使用率
        /// </summary>
        public static float CPU 
        {
            get 
            {
                List<float> allCPUValue = new List<float>();
                
                allCPUValue.Add(_CPU.NextValue());
                Thread.Sleep(50);
                allCPUValue.Add(_CPU.NextValue());
                Thread.Sleep(50);
                allCPUValue.Add(_CPU.NextValue());
                Thread.Sleep(50);
                allCPUValue.Add(_CPU.NextValue());
                Thread.Sleep(50);
                allCPUValue.Add(_CPU.NextValue());
                Thread.Sleep(50);

                return 100-allCPUValue.Sum(x=>x)/5;
            }
        }
        /// <summary>
        /// 目前設備RAM使用率
        /// </summary>
        public static float RAM 
        {
            get 
            {
                List<float> allRAM = new List<float>();
                
                allRAM.Add(_RAM.NextValue());
                Thread.Sleep(50);
                allRAM.Add(_RAM.NextValue());
                Thread.Sleep(50);
                allRAM.Add(_RAM.NextValue());
                Thread.Sleep(50);
                allRAM.Add(_RAM.NextValue());
                Thread.Sleep(50);
                allRAM.Add(_RAM.NextValue());
                Thread.Sleep(50);

                return allRAM.Sum(x=>x)/5;
            }
        }
        
        public static List<Disk> DISK 
        {
            get 
            {
                List<Disk> information = new List<Disk>();
                DriveInfo[] myDrives = DriveInfo.GetDrives();

                foreach (DriveInfo drive in myDrives)
                {
                    if (drive.IsReady == true)
                    {
                        information.Add(new Disk() 
                        {
                            DiskName = drive.Name,
                            Capacity = drive.TotalFreeSpace / 1024 / 1024 / 1024
                        });
                    }
                }
                return information;
            }
        }
        #endregion
    }
}
