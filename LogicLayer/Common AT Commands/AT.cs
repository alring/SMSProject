using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;

namespace LogicLayer
{
    /// <summary>
    /// This Struct contains AT Commands
    /// </summary>
    public struct ATCommands 
    {
		#region Data Members (14) 

        public static string AddressType_Read = "AT+CSCA?\r";
        public static string AddressType_Set = "AT+CSCA";
        //
        // Address Type Command
        public static string AddressType_Test = "AT+CSCA=?\r";
        /// <summary>
        /// Battery Charge Command
        /// </summary>
        public static string BATTERY_CHARGE = "AT+CBC\r";
        public static string CHECK_CONNECTION = "AT\r";
        /// <summary>
        /// Request Firmware Command
        /// </summary>
        public static string FIRMWARE = "AT+CGMR\r";
        /// <summary>
        /// Request IMEI Command
        /// </summary>
        public static string IMEI = "AT+CGSN\r";
        /// <summary>
        /// Request Manufacturer Identification Command
        /// </summary>
        public static string MANUFACTURER_IDENTIFICATION = "AT+CGMI\r";
        public static string MessageFormat_Read = "AT+CMGF?\r";
        public static string MessageFormat_Set_PDU = "AT+CMGF=0\r";
        public static string MessageFormat_Set_TEXT = "AT+CMGF=1\r";
        // Message Format Command
        public static string MessageFormat_Test = "AT+CMGF=?\r";
        /// <summary>
        /// Request Model Identification Command
        /// </summary>
        public static string MODEL_IDENTIFICATION = "AT+CGMM\r";
        /// <summary>
        /// Signal Quality Command
        /// </summary>
        public static string SIGNAL_QUALITY = "AT+CSQ\r";

		#endregion Data Members 

        //
    }

    /// <summary>
    /// AT Final Result Code
    /// </summary>
    public struct ATFinalResultCode
    {
		#region Data Members (2) 

        public static string ERROR = "\r\nERROR\r\n";
        public static string OK = "\r\nOK\r\n";

		#endregion Data Members 
    }

    /// <summary>
    /// Commands need to check the device for supporting test.
    /// </summary>
    public class Test_AT_Commands
    {
		#region Fields (27) 

        public  Test_Commands AT = new Test_Commands("AT\r","AT Commands support");
        public Test_Commands BATTERY_CHARGE = new Test_Commands("AT+CBC=?\r", "Battery Charge");
        public Test_Commands DELETE_MESSAGE = new Test_Commands("AT+CMGD=?\r", "Delete Message");
        public Test_Commands FIND_PHONEBOOK_ENTRIES = new Test_Commands("AT+CPBF=?\r", "Find Phone Book Entries");
        public Test_Commands FIRMWARE = new Test_Commands("AT+CGMR=?\r", "Request Revision Identification");
        public Test_Commands IMEI = new Test_Commands("AT+CGSN=?\r", "Request Product Serial Number Identification");
        public Test_Commands LIST_MESSAGES = new Test_Commands("AT+CMGL=?\r", "List Messages");
        public Test_Commands MANUFACTURER = new Test_Commands("AT+CGMI=?\r", "Request Manufacturer Identification");
        public Test_Commands MESSAGE_FORMAT = new Test_Commands("AT+CMGF=?\r", "Message Format");
        public Test_Commands MODEL = new Test_Commands("AT+CGMM=?\r", "Request Model Identification");
        public Test_Commands NEW_MESSAGE_INDICATION_TO_TE = new Test_Commands("AT+CNMI=?\r", "New Message Indications to TE");
        public Test_Commands PREFERED_MESSAGE_STORAGE = new Test_Commands("AT+CPMS=?\r", "Preferred Message Storage");
        public Test_Commands READ_MESSAGES = new Test_Commands("AT+CMGR=?\r", "Read Message");
        public Test_Commands READ_PHONEBOOK_ENTRY = new Test_Commands("AT+CPBR=?\r", "Read Phone Book Entry");
        public Test_Commands RESTORE_SETTINGS = new Test_Commands("AT+CRES=?\r", "Restore Settings");
        public Test_Commands SAVE_SETTINGS = new Test_Commands("AT+CSAS=?\r", "Save Settings");
        public Test_Commands SELECT_CELL_BROADCAST_MESSAGE_TYPE = new Test_Commands("AT+CSCB=?\r", "Select Cell Broadcast Message Types");
        public Test_Commands SELECT_MESSAGE_SERVICE = new Test_Commands("AT+CSMS=?\r", "Select Message Service");
        public Test_Commands SELECT_PHONEBOOK_MEMORY_STORAGE = new Test_Commands("AT+CPBS=?\r", "Select Phone Book Memory Storage");
        public Test_Commands SEND_MESSAGE_FROM_STORAGE = new Test_Commands("AT+CMSS=?\r", "Send Message from Storage");
        public Test_Commands SEND_MESSAGES = new Test_Commands("AT+CMGS=?\r", "Send Message");
        public Test_Commands SERVICE_CENTER_ADDRESS = new Test_Commands("AT+CSCA=?\r", "Service Centre Address");
        public Test_Commands SET_TEXT_MODE_PARAMETER = new Test_Commands("AT+CSMP=?\r", "Set Text Mode Parameters");
        public Test_Commands SHOW_TEXT_MODE_PARAMETER = new Test_Commands("AT+CSDH=?\r", "Show Text Mode Parameters");
        public Test_Commands SIGNAL_QUALITY = new Test_Commands("AT+CSQ=?\r", "Signal Quality");
        public Test_Commands WRITE_MESSAGE_TO_MEMORY = new Test_Commands("AT+CMGW=?\r", "Write Message to Memory");
        public Test_Commands WRITE_PHONEBOOK_ENTRY = new Test_Commands("AT+CPBW=?\r", "Write Phone Book Entry");

		#endregion Fields 

		#region Methods (1) 

		// Public Methods (1) 

        /// <summary>
        /// Gets all commands.
        /// </summary>
        /// <returns></returns>
        public Test_Commands[] GetAllCommands()
        {
            Test_Commands[] result ={AT,BATTERY_CHARGE,MANUFACTURER,MODEL,FIRMWARE,IMEI,FIND_PHONEBOOK_ENTRIES,
                            READ_PHONEBOOK_ENTRY,SELECT_PHONEBOOK_MEMORY_STORAGE,WRITE_PHONEBOOK_ENTRY,SIGNAL_QUALITY,
                             SELECT_MESSAGE_SERVICE,PREFERED_MESSAGE_STORAGE,MESSAGE_FORMAT,SERVICE_CENTER_ADDRESS,
                             SET_TEXT_MODE_PARAMETER,SHOW_TEXT_MODE_PARAMETER,SELECT_CELL_BROADCAST_MESSAGE_TYPE,
                             SAVE_SETTINGS,RESTORE_SETTINGS,NEW_MESSAGE_INDICATION_TO_TE,LIST_MESSAGES,
                             READ_MESSAGES,SEND_MESSAGES,SEND_MESSAGE_FROM_STORAGE,WRITE_MESSAGE_TO_MEMORY,DELETE_MESSAGE};

            return result;
        }

		#endregion Methods 
    }

    /// <summary>
    /// Save Commands and their descriptions in supporting test
    /// </summary>
    public class Test_Commands
    {
		#region Fields (2) 

        private string _Command;
        private string _Description;

		#endregion Fields 

		#region Constructors (3) 

        /// <summary>
        /// Initializes a new instance of the <see cref="Test_Commands"/> class.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="description">The description.</param>
        public Test_Commands(string command, string description)
        {
            this._Command = command;
            this._Description = description;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Test_Commands"/> class.
        /// </summary>
        public Test_Commands()
        {

        }

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="Test_Commands"/> is reclaimed by garbage collection.
        /// </summary>
        ~Test_Commands()
        {

        }

		#endregion Constructors 

		#region Properties (2) 

        /// <summary>
        /// Gets or sets <b>AT </b>Command
        /// </summary>
        public string Command
        {
            get
            {
                return _Command;
            }
            set
            {
                _Command = value;
            }
        }

        /// <summary>
        /// Gets or sets the decription of AT Command
        /// </summary>
        public string Description
        {
            get
            {
                return _Description;
            }
            set
            {
                _Description = value;
            }
        }

		#endregion Properties 
    }

}
