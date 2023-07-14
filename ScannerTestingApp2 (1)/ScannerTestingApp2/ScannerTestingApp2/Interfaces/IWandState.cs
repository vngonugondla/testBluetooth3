using Com.Zebra.Rfid.Api3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScannerTestingApp.Interfaces
{
    public interface IWandState
    {
        RFIDReader ZebraReader { get; set; }
    }
}
