using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Threading;

namespace LogicLayer
{
    public class ManageSerialPort
    {
		#region Fields (11) 

        private int _BaudRate;
        private int _DataBits;
        private Parity _Parity;
        private string _PortName;
        private int _ReadTimeout;
        private SerialPort _serialport;
        private StopBits _StopBits;
        private int _WriteTimeout;
        private static volatile ManageSerialPort instance;
        private EventWaitHandle receiveNow = new EventWaitHandle(false, EventResetMode.AutoReset);
        private static object syncRoot = new object();

		#endregion Fields 

		#region Constructors (1) 

        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:LogicLayer.ManageSerialPort">ManageSerialPort</see> class.
        /// </summary>
        private ManageSerialPort()
        {

        }

		#endregion Constructors 

		#region Properties (9) 

        /// <summary>
        /// Gets or sets baud rate for serial port
        /// </summary>
        public int BaudRate
        {
            get
            {
                return _BaudRate;
            }
            set
            {
                _BaudRate = value;
            }
        }

        /// <summary>
        /// Gets or sets data bits for serial port
        /// </summary>
        public int DataBits
        {
            get
            {
                return _DataBits;
            }
            set
            {
                _DataBits = value;
            }
        }

        /// <summary>
        /// Property for initializing class using singleton pattern
        /// </summary>
        public static ManageSerialPort Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        instance = new ManageSerialPort();
                    }
                }
                return instance;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the serial port is open.
        /// </summary>
        /// <value><c>true</c> if the serial port is open; otherwise, <c>false</c>.</value>
        public bool IsOpen
        {
            get
            {
                return _serialport.IsOpen;
            }
        }

        /// <summary>
        /// Gets or sets parity for serial port
        /// </summary>
        public Parity Parity
        {
            get
            {
                return _Parity;
            }
            set
            {
                _Parity = value;
            }
        }

        /// <summary>
        /// Gets or sets port name for serial port
        /// </summary>
        public string PortName
        {
            get
            {
                return _PortName;
            }

            set
            {
                try
                {
                    _PortName = value;
                }
                catch (Exception ex)
                {
                    ShowMessageInStatusbarEventArgs e = new ShowMessageInStatusbarEventArgs(ex.Message, 5 * 1000);
                    OnShowMessageInStatusbar(e);
                }
            }
        }

        /// <summary>
        /// Gets or sets read time out for serial port
        /// </summary>
        public int ReadTimeout
        {
            get
            {
                return _ReadTimeout;
            }
            set
            {
                _ReadTimeout = value;
            }
        }

        /// <summary>
        /// Gets or sets stop bits for serial port
        /// </summary>
        public StopBits StopBits
        {
            get
            {
                return _StopBits;
            }
            set
            {
                _StopBits = value;
            }
        }

        /// <summary>
        /// Gets or sets write timeout for serial port
        /// </summary>
        public int WriteTimeout
        {
            get
            {
                return _WriteTimeout;
            }
            set
            {
                _WriteTimeout = value;
            }
        }

		#endregion Properties 

		#region Delegates and Events (10) 

		// Events (10) 

        /// <summary>
        /// Occurs when [battery charge retrieved].
        /// </summary>
        public event ResultRetievedEventHadler BatteryChargeRetrieved;

        /// <summary>
        /// Occurs when [firmware retrieved].
        /// </summary>
        public event ResultRetievedEventHadler FirmwareRetrieved;

        /// <summary>
        /// Occurs when [IMEI retrieved].
        /// </summary>
        public event ResultRetievedEventHadler IMEIRetrieved;

        /// <summary>
        /// Occurs when [manufacturer identification retieved].
        /// </summary>
        public event ResultRetievedEventHadler ManufacturerRetieved;

        /// <summary>
        /// Occurs when [model identification retrieved].
        /// </summary>
        public event ResultRetievedEventHadler ModelRetrieved;

        /// <summary>
        /// General purpose event for returning the result.
        /// </summary>
        public event ResultRetievedEventHadler ResultRetieved;

        /// <summary>
        /// an event for returning the results to GUI and show in statusbar
        /// </summary>
        public event ShowMessageInStatusbarEventHandler ShowMessageInStatusbar;

        /// <summary>
        /// Occurs when [signal quality retrieved].
        /// </summary>
        public event ResultRetievedEventHadler SignalQualityRetrieved;

        /// <summary>
        /// Occurs when response from terminal is read
        /// </summary>
        public event ResultRetievedEventHadler TerminalResponseRetrieved;

        /// <summary>
        /// Occurs when [test completed].
        /// </summary>
        public event ResultRetievedEventHadler TestCompleted;

		#endregion Delegates and Events 

		#region Methods (1) 

		// Public Methods (1) 

        /// <summary>
        /// Reads Service Centre Address from device
        /// </summary>
        public DeviceAddress AddressType_Read()
        {
            try
            {
                string command = ATCommands.AddressType_Read;
                _serialport.DiscardInBuffer();
                _serialport.DiscardOutBuffer();
                receiveNow.Reset();
                _serialport.Write(command);

                string response = ReadResponse(1000);
                if (response.EndsWith(ATFinalResultCode.ERROR) || response.Length == 0)
                {
                    return null;
                }
                else if (response.EndsWith(ATFinalResultCode.OK))
                {
                    string result = ReadResult(response, command, ATFinalResultCode.OK);
                    char[] delimiter = { ',' };
                    string[] finalResult = result.Split(delimiter);
                    SCAddressType _type = new SCAddressType();
                    if (finalResult[1].Trim() == "145")
                    {
                        _type = SCAddressType.International;
                    }
                    else if (finalResult[1].Trim() == "129")
                    {
                        _type = SCAddressType.Other;
                    }

                    string returnSubCSCA = "+CSCA: ";
                    string number1 = finalResult[0].Trim();
                    string number2 = number1.Remove(0, returnSubCSCA.Length);
                    char[] triming = { '"' };
                    string number3 = number2.Trim(triming);
                    DeviceAddress da = new DeviceAddress(number3, _type);
                    return da;
                }
            }
            catch (Exception ex)
            {
                ShowMessageInStatusbarEventArgs ev = new ShowMessageInStatusbarEventArgs(ex.Message, 5 * 1000);
                OnShowMessageInStatusbar(ev);
            }

            return null;
        }

        /// <summary>
        /// Sets Service Centre Address for device
        /// </summary>
        /// <param name="command"></param>
        public bool AddressType_Set(string command)
        {
            try
            {
                _serialport.DiscardInBuffer();
                _serialport.DiscardOutBuffer();
                receiveNow.Reset();
                _serialport.Write(command);

                string response = ReadResponse(1000);
                if (response.EndsWith(ATFinalResultCode.ERROR) || response.Length == 0)
                {
                    return false;
                }
                else if (response.EndsWith(ATFinalResultCode.OK))
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                ShowMessageInStatusbarEventArgs ev = new ShowMessageInStatusbarEventArgs(ex.Message, 5 * 1000);
                OnShowMessageInStatusbar(ev);
            }

            return false;
        }

        /// <summary>
        /// Send SMS asynchronously
        /// </summary>
        /// <param name="Part1OfCommand">The first part of AT Command</param>
        /// <param name="Part2OfCommand">The second part of AT Command</param>
        public void SendSMS(string Part1OfCommand, string Part2OfCommand)
        {
            ParameterizedThreadStart pts = new ParameterizedThreadStart(SendSMS2);
            Thread t1 = new Thread(pts);

            string[] commands = { Part1OfCommand, Part2OfCommand };

            t1.Start(commands);
        }

        private void SendSMS2(object state)
        {
            try
            {
                object syncRoot = new object();
                lock (syncRoot)
                {
                    ShowMessageInStatusbarEventArgs ev1 = new ShowMessageInStatusbarEventArgs("Sending...", 10 * 1000);
                    OnShowMessageInStatusbar(ev1);
                    string[] commands = (string[])state;
                    string command1 = commands[0];
                    string command2 = commands[1];

                    //_serialport.DiscardInBuffer();
                    //_serialport.DiscardOutBuffer();
                    receiveNow.Reset();
                    _serialport.Write(command1);
                    Thread.Sleep(100);
                    _serialport.Write(command2);

                    string response = ReadResponse(1000);

                    if (response.EndsWith(ATFinalResultCode.OK))
                    {
                        ShowMessageInStatusbarEventArgs ev = new ShowMessageInStatusbarEventArgs("Message Sent", 5 * 1000);
                        OnShowMessageInStatusbar(ev);
                    }
                    else
                    {
                        ShowMessageInStatusbarEventArgs ev = new ShowMessageInStatusbarEventArgs("Sending Failed", 5 * 1000);
                        OnShowMessageInStatusbar(ev);
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessageInStatusbarEventArgs ev = new ShowMessageInStatusbarEventArgs(ex.Message, 5 * 1000);
                OnShowMessageInStatusbar(ev);
            }
        }

        /// <summary>
        /// Sends user command async to terminal.
        /// </summary>
        /// <remarks>
        /// the value will be returned by use of <b>TerminalResponseRetrieved </b>event
        /// </remarks>
        /// <param name="command"></param>
        public void SendToTerminal(string command)
        {
            ParameterizedThreadStart pts = new ParameterizedThreadStart(SendToTerminal2);
            Thread t1 = new Thread(pts);
            t1.Start(command);
        }
        public void SendToTerminal2(object state)
        {
            try
            {
                string command = (string)state;
                _serialport.DiscardInBuffer();
                _serialport.DiscardOutBuffer();
                receiveNow.Reset();
                _serialport.Write(command + "\r\n");

                string response = ReadResponse(1000);
                ResultRetievedEventArgs ev = new ResultRetievedEventArgs(true, response);
                OnTerminalResponseRetrieved(ev);
            }
            catch (Exception ex)
            {
                ShowMessageInStatusbarEventArgs ev = new ShowMessageInStatusbarEventArgs(ex.Message, 5 * 1000);
                OnShowMessageInStatusbar(ev);
            }
        }

        /// <summary>
        /// Checks if the device supports Service Centre Address AT Command
        /// </summary>
        public bool AddressType_Test()
        {
            try
            {
                string command = ATCommands.AddressType_Test;
                _serialport.DiscardInBuffer();
                _serialport.DiscardOutBuffer();
                receiveNow.Reset();
                _serialport.Write(command);

                string response = ReadResponse(1000);
                if (response.EndsWith(ATFinalResultCode.ERROR) || response.Length == 0)
                {
                    return false;
                }
                else if (response.EndsWith(ATFinalResultCode.OK))
                {
                    return true;
                }
            }
            catch (System.Exception ex)
            {
                ShowMessageInStatusbarEventArgs ev = new ShowMessageInStatusbarEventArgs(ex.Message, 5 * 1000);
                OnShowMessageInStatusbar(ev);
            }
            return false;
        }

        /// <summary>
        /// Begins all tests one by one.
        /// </summary>
        public void BeginAllTestsInQueue()
        {
            ThreadStart ts = new ThreadStart(BeginAllTestsInQueue2);
            Thread t5 = new Thread(ts);
            t5.Start();
        }

        /// <summary>
        /// Checks if current COM port is supporting AT Commands.
        /// </summary>
        public void CheckConnection()
        {
            ThreadStart ts = new ThreadStart(CheckConnection2);
            Thread t3 = new Thread(ts);
            t3.Start();
        }

        /// <summary>
        /// Connect to serial port asynchronously
        /// </summary>
        public void Connect()
        {
            ThreadStart ts = new ThreadStart(Connect2);
            Thread t1 = new Thread(ts);
            t1.Start();
        }

        public void Connect2()
        {
            try
            {
                if (_serialport.IsOpen == false)
                {
                    ShowMessageInStatusbarEventArgs e = new ShowMessageInStatusbarEventArgs("Connecting...", 10 * 1000);
                    OnShowMessageInStatusbar(e);
                    LoadSetting();
                    _serialport.Open();
                    e = new ShowMessageInStatusbarEventArgs("Connected.", 5 * 1000);
                    OnShowMessageInStatusbar(e);
                }
            }
            catch (Exception ex)
            {
                ShowMessageInStatusbarEventArgs e = new ShowMessageInStatusbarEventArgs(ex.Message, 5 * 1000);
                OnShowMessageInStatusbar(e);
            }
        }

        public void CreateNewSerialPort()
        {
            _serialport = new SerialPort();
        }

        public void DestroySerialPort()
        {
            _serialport = null;
        }

        /// <summary>
        /// Disconnects the serial port asynchronously.
        /// </summary>
        public void Disconnect()
        {
            ThreadStart ts = new ThreadStart(Disconnect2);
            Thread t2 = new Thread(ts);
            t2.Start();
        }

        public void Disconnect2()
        {
            try
            {
                ShowMessageInStatusbarEventArgs e = new ShowMessageInStatusbarEventArgs("Disconnecting...", 10 * 1000);
                OnShowMessageInStatusbar(e);
                _serialport.Close();
                _serialport = new SerialPort();
                e = new ShowMessageInStatusbarEventArgs("Disconnected.", 5 * 1000);
                OnShowMessageInStatusbar(e);
            }
            catch (System.Exception ex)
            {
                ShowMessageInStatusbarEventArgs e = new ShowMessageInStatusbarEventArgs(ex.Message, 5 * 1000);
                OnShowMessageInStatusbar(e);
            }
            finally
            {
                _serialport.Dispose();
            }
        }

        /// <summary>
        /// Get all baud rates
        /// </summary>
        /// <returns></returns>
        public static int[] GetAllBaudRate()
        {
            int[] baudrate = { 300, 1200, 2400, 4800, 9600, 19200, 38400, 57600, 115200 };
            return baudrate;
        }

        /// <summary>
        /// Get all data bits
        /// </summary>
        /// <returns></returns>
        public static int[] GetAllDataBits()
        {
            int[] databits = { 4, 5, 6, 7, 8 };
            return databits;
        }

        /// <summary>
        /// Get all parities
        /// </summary>
        /// <returns></returns>
        public static string[] GetAllParity()
        {
            string[] parity = { Parity.Even.ToString(), Parity.Mark.ToString(), Parity.None.ToString(), Parity.Odd.ToString(), Parity.Space.ToString() };
            return parity;
        }

        /// <summary>
        /// Get all available COM ports in computer
        /// </summary>
        /// <returns></returns>
        public static string[] GetAllPorts()
        {
            return SerialPort.GetPortNames();
        }

        /// <summary>
        /// Get all stop bits
        /// </summary>
        /// <returns></returns>
        public static string[] GetAllStopBits()
        {
            string[] stopbit = { StopBits.None.ToString(), StopBits.One.ToString(), StopBits.OnePointFive.ToString(), StopBits.Two.ToString() };
            return stopbit;
        }

        /// <summary>
        /// Gets all timeout.
        /// </summary>
        /// <returns></returns>
        public static int[] GetAllTimeout()
        {
            int[] timeout = { 100, 200, 300, 400, 500, 600, 800, 900, 1000 };
            return timeout;
        }

        /// <summary>
        /// Load the default setting
        /// </summary>
        public void LoadDefaultSetting()
        {
            this.PortName = SerialPort.GetPortNames()[0];
            this.DataBits = 8;
            this.Parity = Parity.None;
            this.StopBits = StopBits.One;
            this.BaudRate = 9600;
            this.ReadTimeout = 500;
            this.WriteTimeout = 500;
            LoadSetting();
        }

        /// <summary>
        /// Loads the setting.
        /// </summary>
        public void LoadSetting()
        {
            _serialport.PortName = this.PortName;
            _serialport.DataBits = this.DataBits;
            _serialport.Parity = this.Parity;
            _serialport.StopBits = this.StopBits;
            _serialport.BaudRate = this.BaudRate;
            _serialport.ReadTimeout = this.ReadTimeout;
            _serialport.WriteTimeout = this.WriteTimeout;
            _serialport.Encoding = Encoding.GetEncoding(1252); // iso-8859-1
            _serialport.DtrEnable = true;
            _serialport.DtrEnable = true;
            _serialport.DataReceived += new SerialDataReceivedEventHandler(_serialport_DataReceived);
        }

        /// <summary>
        /// Read <b>Message Format</b> setting for current device
        /// </summary>
        /// <returns>
        /// PDU Mode or TEXT Mode
        /// </returns>
        public SMSMode MessageFormat_Read()
        {
            try
            {
                string command = ATCommands.MessageFormat_Read;
                _serialport.DiscardInBuffer();
                _serialport.DiscardOutBuffer();
                receiveNow.Reset();
                _serialport.Write(command);

                string response = ReadResponse(1000);
                if (response.EndsWith(ATFinalResultCode.ERROR) || response.Length == 0)
                {
                    return SMSMode.NULL;
                }
                else if (response.EndsWith(ATFinalResultCode.OK))
                {
                    string result = ReadResult(response, command, ATFinalResultCode.OK);
                    if (result.Contains("0"))
                    {
                        return SMSMode.PDU;
                    }
                    else if (result.Contains("1"))
                    {
                        return SMSMode.TEXT;
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessageInStatusbarEventArgs ev = new ShowMessageInStatusbarEventArgs(ex.Message, 5 * 1000);
                OnShowMessageInStatusbar(ev);
            }

            return SMSMode.NULL;
        }

        /// <summary>
        /// Set Message Format for current device
        /// </summary>
        /// <param name="command">Command to set the device to PDU mode or TEXT
        /// mode.</param>
        /// <returns>
        /// Return true on successful Set otherwise false.
        /// </returns>
        public bool MessageFormat_Set(string command)
        {
            try
            {
                _serialport.DiscardInBuffer();
                _serialport.DiscardOutBuffer();
                receiveNow.Reset();
                _serialport.Write(command);

                string response = ReadResponse(1000);
                if (response.EndsWith(ATFinalResultCode.ERROR) || response.Length == 0)
                {
                    return false;
                }
                else if (response.EndsWith(ATFinalResultCode.OK))
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                ShowMessageInStatusbarEventArgs ev = new ShowMessageInStatusbarEventArgs(ex.Message, 5 * 1000);
                OnShowMessageInStatusbar(ev);
            }

            return false;
        }

        /// <summary>
        /// Test if the device support Message Format AT Command.
        /// </summary>
        /// <returns>
        /// Retun <see langword="true"/> if the device support the AT Command, otherwise
        /// <see langword="false"/>
        /// </returns>
        public bool MessageFormat_Test()
        {
            try
            {
                string command = ATCommands.MessageFormat_Test;
                _serialport.DiscardInBuffer();
                _serialport.DiscardOutBuffer();
                receiveNow.Reset();
                _serialport.Write(command);

                string response = ReadResponse(1000);
                if (response.EndsWith(ATFinalResultCode.ERROR) || response.Length == 0)
                {
                    return false;
                }
                else if (response.EndsWith(ATFinalResultCode.OK))
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                ShowMessageInStatusbarEventArgs ev = new ShowMessageInStatusbarEventArgs(ex.Message, 5 * 1000);
                OnShowMessageInStatusbar(ev);
            }

            return false;
        }

        /// <summary>
        /// Reads the response from selected COM port.
        /// </summary>
        /// <param name="timeout">The timeout.</param>
        /// <returns></returns>
        public string ReadResponse(int timeout)
        {
            string buffer = "";
            try
            {
                do
                {
                    if (receiveNow.WaitOne(timeout, false))
                    {
                        string t = _serialport.ReadExisting();
                        buffer += t;
                    }
                } while (!buffer.EndsWith(ATFinalResultCode.ERROR) && !buffer.EndsWith(ATFinalResultCode.OK));
            }
            catch (System.Exception ex)
            {
                ShowMessageInStatusbarEventArgs ev = new ShowMessageInStatusbarEventArgs(ex.Message, 5 * 1000);
                OnShowMessageInStatusbar(ev);
            }

            return buffer;
        }

        /// <summary>
        /// Request Battery Charge.
        /// </summary>
        public void Request_Battery_Charge()
        {
            ThreadStart ts = new ThreadStart(Request_Battery_Charge2);
            Thread t10 = new Thread(ts);
            t10.Start();
        }

        /// <summary>
        /// Request Revision Identification.
        /// </summary>
        public void Request_Firmware()
        {
            ThreadStart ts = new ThreadStart(Request_Firmware2);
            Thread t8 = new Thread(ts);
            t8.Start();
        }

        /// <summary>
        /// Request Product Serial Number Identification.
        /// </summary>
        public void Request_IMEI()
        {
            ThreadStart ts = new ThreadStart(Request_IMEI2);
            Thread t7 = new Thread(ts);
            t7.Start();
        }

        /// <summary>
        /// Request Manufacturer Identification.
        /// </summary>
        public void Request_Manufacturer_Identification()
        {
            ThreadStart ts = new ThreadStart(Request_Manufacturer_Identification2);
            Thread t4 = new Thread(ts);
            t4.Start();
        }

        /// <summary>
        /// Request Model Identification.
        /// </summary>
        public void Request_Model_Identification()
        {
            ThreadStart ts = new ThreadStart(Request_Model_Identification2);
            Thread t6 = new Thread(ts);
            t6.Start();
        }

        /// <summary>
        /// Request Signal Quality.
        /// </summary>
        public void Request_Signal_Quality()
        {
            ThreadStart ts = new ThreadStart(Request_Signal_Quality2);
            Thread t9 = new Thread(ts);
            t9.Start();
        }

        /// <summary>
        /// Tests the device capabilities.
        /// </summary>
        public void TestDevice()
        {
            ThreadStart ts = new ThreadStart(TestDevice2);
            Thread t11 = new Thread(ts);
            t11.Start();
        }

        protected virtual void OnBatteryChargeRetrieved(ResultRetievedEventArgs e)
        {
            if (BatteryChargeRetrieved != null)
            {
                BatteryChargeRetrieved(this, e);
            }
        }

        protected virtual void OnFirmwareRetrieved(ResultRetievedEventArgs e)
        {
            if (FirmwareRetrieved != null)
            {
                FirmwareRetrieved(this, e);
            }
        }

        protected virtual void OnIMEIRetrieved(ResultRetievedEventArgs e)
        {
            if (IMEIRetrieved != null)
            {
                IMEIRetrieved(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:ManufacturerRetieved"/> event.
        /// </summary>
        /// <param name="e">The <see cref="LogicLayer.ResultRetievedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnManufacturerRetieved(ResultRetievedEventArgs e)
        {
            if (ManufacturerRetieved != null)
            {
                ManufacturerRetieved(this, e);
            }
        }

        protected virtual void OnModelRetrieved(ResultRetievedEventArgs e)
        {
            if (ModelRetrieved != null)
            {
                ModelRetrieved(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:ResultRetieved"/> event.
        /// </summary>
        /// <param name="e">The <see cref="LogicLayer.ResultRetievedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnResultRetieved(ResultRetievedEventArgs e)
        {
            if (ResultRetieved != null)
            {
                ResultRetieved(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:ShowMessageInStatusbar"/> event.
        /// </summary>
        /// <param name="e">The <see cref="LogicLayer.ShowMessageInStatusbarEventArgs"/> instance containing the event data.</param>
        protected virtual void OnShowMessageInStatusbar(ShowMessageInStatusbarEventArgs e)
        {
            if (ShowMessageInStatusbar != null)
            {
                ShowMessageInStatusbar(this, e);
            }
        }

        protected virtual void OnSignalQualityRetrieved(ResultRetievedEventArgs e)
        {
            if (SignalQualityRetrieved != null)
            {
                SignalQualityRetrieved(this, e);
            }
        }

        protected virtual void OnTerminalResponseRetrieved(ResultRetievedEventArgs e)
        {
            if (TerminalResponseRetrieved != null)
            {
                TerminalResponseRetrieved(this, e);
            }
        }

        protected virtual void OnTestCompleted(ResultRetievedEventArgs e)
        {
            if (TestCompleted != null)
            {
                TestCompleted(this, e);
            }
        }

        private void _serialport_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                if (e.EventType == SerialData.Chars)
                {
                    receiveNow.Set();
                }
            }
            catch (Exception ex)
            {
                ShowMessageInStatusbarEventArgs ev = new ShowMessageInStatusbarEventArgs(ex.Message, 5 * 1000);
                OnShowMessageInStatusbar(ev);
            }
        }

        private void BeginAllTestsInQueue2()
        {
            Request_Manufacturer_Identification2();
            Request_Model_Identification2();
            Request_IMEI2();
            Request_Firmware2();
            Request_Signal_Quality2();
            Request_Battery_Charge2();
        }

        private void CheckConnection2()
        {
            try
            {
                _serialport.DiscardInBuffer();
                _serialport.DiscardOutBuffer();
                receiveNow.Reset();
                _serialport.Write(ATCommands.CHECK_CONNECTION);

                string response = ReadResponse(1000);
                if (response.EndsWith(ATFinalResultCode.ERROR) || response.Length == 0)
                {
                    ResultRetievedEventArgs ev = new ResultRetievedEventArgs(false, response);
                    OnResultRetieved(ev);

                    ShowMessageInStatusbarEventArgs ev2 = new ShowMessageInStatusbarEventArgs("AT Commands test:Failed", 5 * 1000);
                    OnShowMessageInStatusbar(ev2);
                }
                else if (response.EndsWith(ATFinalResultCode.OK))
                {
                    ResultRetievedEventArgs ev = new ResultRetievedEventArgs(true, response);
                    OnResultRetieved(ev);

                    ShowMessageInStatusbarEventArgs ev2 = new ShowMessageInStatusbarEventArgs("AT Commands test:Successful", 5 * 1000);
                    OnShowMessageInStatusbar(ev2);
                }
            }
            catch (Exception ex)
            {
                ShowMessageInStatusbarEventArgs ev = new ShowMessageInStatusbarEventArgs(ex.Message, 5 * 1000);
                OnShowMessageInStatusbar(ev);
            }
        }

        /// <summary>
        /// Reads the result without AT Command and Final Result Code.
        /// </summary>
        /// <param name="Response">The response.</param>
        /// <param name="ATCommand">The AT command.</param>
        /// <param name="FinalCode">The final code.</param>
        /// <returns></returns>
        private string ReadResult(string Response, string ATCommand, string FinalCode)
        {
            try
            {
                int finalcodeindex = Response.LastIndexOf(FinalCode);
                string temp = Response.Remove(finalcodeindex, FinalCode.Length);
                int atcommandindex = temp.IndexOf(ATCommand);
                temp = temp.Remove(atcommandindex, ATCommand.Length);
                string result = temp.Trim();
                return result;
            }
            catch (Exception ex)
            {
                ShowMessageInStatusbarEventArgs ev = new ShowMessageInStatusbarEventArgs(ex.Message, 5 * 1000);
                OnShowMessageInStatusbar(ev);

                return string.Empty;
            }
        }

        private void Request_Battery_Charge2()
        {
            try
            {
                _serialport.DiscardOutBuffer();
                _serialport.DiscardInBuffer();
                receiveNow.Reset();
                _serialport.Write(ATCommands.BATTERY_CHARGE);

                string response = ReadResponse(1000);

                string result;
                if (response.EndsWith(ATFinalResultCode.OK))
                {
                    result = ReadResult(response, ATCommands.BATTERY_CHARGE, ATFinalResultCode.OK);
                    ResultRetievedEventArgs ev = new ResultRetievedEventArgs(true, result);
                    OnBatteryChargeRetrieved(ev);
                }
                else if (response.EndsWith(ATFinalResultCode.ERROR))
                {
                    result = ReadResult(response, ATCommands.BATTERY_CHARGE, ATFinalResultCode.ERROR);
                    ResultRetievedEventArgs ev = new ResultRetievedEventArgs(false, result);
                    OnBatteryChargeRetrieved(ev);
                }
                else if (response.Length == 0)
                {
                    result = "";
                    ResultRetievedEventArgs ev = new ResultRetievedEventArgs(false, result);
                    OnBatteryChargeRetrieved(ev);
                }
            }
            catch (System.Exception ex)
            {
                ShowMessageInStatusbarEventArgs ev = new ShowMessageInStatusbarEventArgs(ex.Message, 5 * 1000);
                OnShowMessageInStatusbar(ev);
            }
        }

        private void Request_Firmware2()
        {
            try
            {
                _serialport.DiscardOutBuffer();
                _serialport.DiscardInBuffer();
                receiveNow.Reset();
                _serialport.Write(ATCommands.FIRMWARE);

                string response = ReadResponse(1000);

                string result;
                if (response.EndsWith(ATFinalResultCode.OK))
                {
                    result = ReadResult(response, ATCommands.FIRMWARE, ATFinalResultCode.OK);
                    ResultRetievedEventArgs ev = new ResultRetievedEventArgs(true, result);
                    OnFirmwareRetrieved(ev);
                }
                else if (response.EndsWith(ATFinalResultCode.ERROR))
                {
                    result = ReadResult(response, ATCommands.FIRMWARE, ATFinalResultCode.ERROR);
                    ResultRetievedEventArgs ev = new ResultRetievedEventArgs(false, result);
                    OnFirmwareRetrieved(ev);
                }
                else if (response.Length == 0)
                {
                    result = "";
                    ResultRetievedEventArgs ev = new ResultRetievedEventArgs(false, result);
                    OnFirmwareRetrieved(ev);
                }
            }
            catch (System.Exception ex)
            {
                ShowMessageInStatusbarEventArgs ev = new ShowMessageInStatusbarEventArgs(ex.Message, 5 * 1000);
                OnShowMessageInStatusbar(ev);
            }
        }

        private void Request_IMEI2()
        {
            try
            {
                _serialport.DiscardOutBuffer();
                _serialport.DiscardInBuffer();
                receiveNow.Reset();
                _serialport.Write(ATCommands.IMEI);

                string response = ReadResponse(1000);

                string result;
                if (response.EndsWith(ATFinalResultCode.OK))
                {
                    result = ReadResult(response, ATCommands.IMEI, ATFinalResultCode.OK);
                    ResultRetievedEventArgs ev = new ResultRetievedEventArgs(true, result);
                    OnIMEIRetrieved(ev);
                }
                else if (response.EndsWith(ATFinalResultCode.ERROR))
                {
                    result = ReadResult(response, ATCommands.IMEI, ATFinalResultCode.ERROR);
                    ResultRetievedEventArgs ev = new ResultRetievedEventArgs(false, result);
                    OnIMEIRetrieved(ev);
                }
                else if (response.Length == 0)
                {
                    result = "";
                    ResultRetievedEventArgs ev = new ResultRetievedEventArgs(false, result);
                    OnIMEIRetrieved(ev);
                }
            }
            catch (System.Exception ex)
            {
                ShowMessageInStatusbarEventArgs ev = new ShowMessageInStatusbarEventArgs(ex.Message, 5 * 1000);
                OnShowMessageInStatusbar(ev);
            }
        }

        private void Request_Manufacturer_Identification2()
        {
            try
            {
                _serialport.DiscardInBuffer();
                _serialport.DiscardOutBuffer();
                receiveNow.Reset();
                _serialport.Write(ATCommands.MANUFACTURER_IDENTIFICATION);

                string response = ReadResponse(1000);

                string result;
                if (response.EndsWith(ATFinalResultCode.OK))
                {
                    result = ReadResult(response, ATCommands.MANUFACTURER_IDENTIFICATION, ATFinalResultCode.OK);
                    ResultRetievedEventArgs ev = new ResultRetievedEventArgs(true, result);
                    OnManufacturerRetieved(ev);
                }
                else if (response.EndsWith(ATFinalResultCode.ERROR))
                {
                    result = ReadResult(response, ATCommands.MANUFACTURER_IDENTIFICATION, ATFinalResultCode.ERROR);
                    ResultRetievedEventArgs ev = new ResultRetievedEventArgs(false, result);
                    OnManufacturerRetieved(ev);
                }
                else if (response.Length == 0)
                {
                    result = "";
                    ResultRetievedEventArgs ev = new ResultRetievedEventArgs(false, result);
                    OnManufacturerRetieved(ev);
                }
            }
            catch (System.Exception ex)
            {
                ShowMessageInStatusbarEventArgs ev = new ShowMessageInStatusbarEventArgs(ex.Message, 5 * 1000);
                OnShowMessageInStatusbar(ev);
            }
        }

        private void Request_Model_Identification2()
        {
            try
            {
                _serialport.DiscardOutBuffer();
                _serialport.DiscardInBuffer();
                receiveNow.Reset();
                _serialport.Write(ATCommands.MODEL_IDENTIFICATION);

                string response = ReadResponse(1000);

                string result;
                if (response.EndsWith(ATFinalResultCode.OK))
                {
                    result = ReadResult(response, ATCommands.MODEL_IDENTIFICATION, ATFinalResultCode.OK);
                    ResultRetievedEventArgs ev = new ResultRetievedEventArgs(true, result);
                    OnModelRetrieved(ev);
                }
                else if (response.EndsWith(ATFinalResultCode.ERROR))
                {
                    result = ReadResult(response, ATCommands.MODEL_IDENTIFICATION, ATFinalResultCode.ERROR);
                    ResultRetievedEventArgs ev = new ResultRetievedEventArgs(false, result);
                    OnModelRetrieved(ev);
                }
                else if (response.Length == 0)
                {
                    result = "";
                    ResultRetievedEventArgs ev = new ResultRetievedEventArgs(false, result);
                    OnModelRetrieved(ev);
                }
            }
            catch (System.Exception ex)
            {
                ShowMessageInStatusbarEventArgs ev = new ShowMessageInStatusbarEventArgs(ex.Message, 5 * 1000);
                OnShowMessageInStatusbar(ev);
            }
        }

        private void Request_Signal_Quality2()
        {
            try
            {
                _serialport.DiscardOutBuffer();
                _serialport.DiscardInBuffer();
                receiveNow.Reset();
                _serialport.Write(ATCommands.SIGNAL_QUALITY);

                string response = ReadResponse(1000);

                string result;
                if (response.EndsWith(ATFinalResultCode.OK))
                {
                    result = ReadResult(response, ATCommands.SIGNAL_QUALITY, ATFinalResultCode.OK);
                    ResultRetievedEventArgs ev = new ResultRetievedEventArgs(true, result);
                    OnSignalQualityRetrieved(ev);
                }
                else if (response.EndsWith(ATFinalResultCode.ERROR))
                {
                    result = ReadResult(response, ATCommands.SIGNAL_QUALITY, ATFinalResultCode.ERROR);
                    ResultRetievedEventArgs ev = new ResultRetievedEventArgs(false, result);
                    OnSignalQualityRetrieved(ev);
                }
                else if (response.Length == 0)
                {
                    result = "";
                    ResultRetievedEventArgs ev = new ResultRetievedEventArgs(false, result);
                    OnSignalQualityRetrieved(ev);
                }
            }
            catch (System.Exception ex)
            {
                ShowMessageInStatusbarEventArgs ev = new ShowMessageInStatusbarEventArgs(ex.Message, 5 * 1000);
                OnShowMessageInStatusbar(ev);
            }
        }

        private void TestDevice2()
        {
            try
            {
                Test_AT_Commands atCommands = new Test_AT_Commands();
                Test_Commands[] commands = atCommands.GetAllCommands();
                foreach (Test_Commands command in commands)
                {
                    _serialport.DiscardInBuffer();
                    _serialport.DiscardOutBuffer();
                    receiveNow.Reset();
                    _serialport.Write(command.Command);

                    string response = ReadResponse(1000);

                    if (response.EndsWith(ATFinalResultCode.OK))
                    {
                        ResultRetievedEventArgs ev = new ResultRetievedEventArgs(true, command.Description);
                        OnTestCompleted(ev);
                    }
                    else
                    {
                        ResultRetievedEventArgs ev = new ResultRetievedEventArgs(false, command.Description);
                        OnTestCompleted(ev);
                    }

                    // Release Memory
                    atCommands = null;
                    //
                }
            }
            catch (Exception ex)
            {
                ShowMessageInStatusbarEventArgs ev = new ShowMessageInStatusbarEventArgs(ex.Message, 5 * 1000);
                OnShowMessageInStatusbar(ev);
            }
        }

    }

		#endregion Methods 
}
