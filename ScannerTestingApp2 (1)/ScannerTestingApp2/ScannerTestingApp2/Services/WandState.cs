using Com.Zebra.Rfid.Api3;
using ScannerTestingApp.Interfaces;
using Com.Zebra.Rfid.Api3;

namespace ScannerTestingApp.Services
{
    public class WandState : IWandState
    {
        public RFIDReader ZebraReader { get; set; }
    }
}
