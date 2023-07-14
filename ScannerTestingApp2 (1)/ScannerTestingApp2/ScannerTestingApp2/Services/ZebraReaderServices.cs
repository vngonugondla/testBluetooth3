using Com.Zebra.Rfid.Api3;
using ScannerTestingApp.Interfaces;
using System.Text.RegularExpressions;

namespace ScannerTestingApp.Services
{
    public class ZebraReaderServices
    {
        private readonly IWandState _wandStatus;

        private readonly IRfidEventsListener _eventService;
        private readonly Readers.IRFIDReaderEventHandler _eventHandlerService;

        public ZebraReaderServices(IWandState wandStatus, IRfidEventsListener eventService, Readers.IRFIDReaderEventHandler handlerService)
        {
            _wandStatus = wandStatus;
            _eventService = eventService;
            _eventHandlerService = handlerService;
            Readers.Attach(_eventHandlerService);
        }

        public bool Connect()
        {
            try
            {
                // MAKE SURE TO ADD TIME OUT FEATURE
                Readers rfidReaders = new();
                var zebraDevice = rfidReaders.AvailableRFIDReaderList.FirstOrDefault(_ => RegexUtil.ZebraReaderRegex.IsMatch(_.Name), null);
                _wandStatus.ZebraReader = zebraDevice.RFIDReader;

            }
            catch
            //catch (OperationFailureException e)
            {
                //Crashes.TrackError(e);
                //return false;
            }

            InitializeZebraConfig();
            InitializeScanSettings();

            if (!_wandStatus.ZebraReader.IsConnected) return false;



            InitializeZebraEvents();
            InitializeZebraTags();



            return true;

        }
        private void InitializeZebraConfig()
        {

            try
            {
               int indexVar = 0;
                for (int i = 0; i < _wandStatus.ZebraReader.ReaderCapabilities.SupportedRegions.Length(); i++)
                {
                    if (_wandStatus.ZebraReader.ReaderCapabilities.SupportedRegions.GetRegionInfo(i).Name == "United States")
                    {
                        indexVar = i; 
                        break;
                    }
                }
                _wandStatus.ZebraReader.Connect();
                var regulatoryConfig = _wandStatus.ZebraReader.Config.RegulatoryConfig;
                var regionInfo = _wandStatus.ZebraReader.ReaderCapabilities.SupportedRegions.GetRegionInfo(indexVar);
                regulatoryConfig.Region = regionInfo.RegionCode;
                regulatoryConfig.SetIsHoppingOn(regionInfo.IsHoppingConfigurable);
                regulatoryConfig.SetEnabledChannels(regionInfo.GetSupportedChannels());
                _wandStatus.ZebraReader.Config.RegulatoryConfig = regulatoryConfig;



            }
            //catch (OperationFailureException e)
            catch
            {
                //Crashes.TrackError(e);
                //device failed to connect
            }
        }



        private void InitializeZebraEvents()
        {
            _wandStatus.ZebraReader.Events.AddEventsListener(_eventService);
            _wandStatus.ZebraReader.Events.SetInventoryStartEvent(true);
            _wandStatus.ZebraReader.Events.SetInventoryStopEvent(true);
            _wandStatus.ZebraReader.Events.SetTagReadEvent(true);
            _wandStatus.ZebraReader.Events.SetAttachTagDataWithReadEvent(false);
            _wandStatus.ZebraReader.Events.SetReaderDisconnectEvent(true);
        }

        private void InitializeScanSettings()
        {
            try
            {
                Antennas.SingulationControl singulationControl = _wandStatus.ZebraReader.Config.Antennas.GetSingulationControl(1);
                var antenna = _wandStatus.ZebraReader.Config.Antennas.GetAntennaRfConfig(1);
                //antenna.TransmitPowerIndex = (int) antenna.TransmitPowerIndex.LowPower;
                antenna.TransmitPowerIndex = 24;
                singulationControl.Action.InventoryState = INVENTORY_STATE.InventoryStateA;
                singulationControl.Action.SLFlag = SL_FLAG.SlAll;
                singulationControl.Session = SESSION.SessionS1;
                singulationControl.TagPopulation = 30;

                _wandStatus.ZebraReader.Config.SetDefaultConfigurations(antenna, singulationControl, null, true, true, null);
            }
            catch { }
            /*catch (OperationFailureExcepttion e)
            {
                Crashes.TrackError(e);
            }
            */
        }

        private void InitializeZebraTags()
        {
            try
            {
                TagAccess.ReadAccessParams readAccessParams = new(new TagAccess())
                {
                    Count = 0,
                    Offset = 0,
                    MemoryBank = MEMORY_BANK.MemoryBankTid,
                };

                _wandStatus.ZebraReader.Actions.TagAccess.ReadEvent(readAccessParams, null, null);
            }
            catch { }
        }

        //catch (OperationFailureException e){
        //Crashes.TrackError(e);
    }

}

