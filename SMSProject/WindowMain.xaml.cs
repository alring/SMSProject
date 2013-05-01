using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using LogicLayer;
using System.IO.Ports;
using System.Timers;
using System.Threading;
using System.Windows.Controls.Primitives;

namespace SMSProject
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
		#region Fields (2) 

        System.Timers.Timer _timer = new System.Timers.Timer();
        ManageSerialPort manage;

		#endregion Fields 

		#region Constructors (1) 

         public Window1()
        {
            InitializeComponent();
        }

		#endregion Constructors 

		#region Delegates and Events (1) 

		// Delegates (1) 

        public delegate void MakeThreadSafe(object state);

		#endregion Delegates and Events 

		#region Methods (54) 

		// Private Methods (54) 

        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            MakeThreadSafe dd = new MakeThreadSafe(ClearStatusbar);
            this.Dispatcher.BeginInvoke(dd, e);
        }

        private void ButtonClear_Click(object sender, RoutedEventArgs e)
        {
            StackPanelAllTestOutput.Children.Clear();
        }

        private void ButtonClearInfo_Click(object sender, RoutedEventArgs e)
        {
            TextBlockManufacturer.Text = "";
            TextBlockModel.Text = "";
            TextBlockIMEI.Text = "";
            TextBlockFirmware.Text = "";
            TextBlockSignal.Text = "";
            TextBlockBattery.Text = "";
        }

         private void ButtonConnect_Click(object sender, RoutedEventArgs e)
        {
            if (manage.IsOpen==false)
            {
                manage.Connect();
                ButtonConnect.Content = "Disconnect";
            }
            else if (manage.IsOpen==true)
            {
                manage.Disconnect2();
                manage.DestroySerialPort();
                manage.CreateNewSerialPort();
                manage.LoadSetting();
                ButtonConnect.Content = "Connect";
            }
        }

        private void ButtonConnect_MouseEnter(object sender, MouseEventArgs e)
        {
            if (manage.IsOpen == false)
            {
                ButtonConnect.Content = "Connect";
            }
            else if (manage.IsOpen == true)
            {
                ButtonConnect.Content = "Disconnect";
            }
        }

         private void ButtonGetAllInformation_Click(object sender, RoutedEventArgs e)
        {
            manage.BeginAllTestsInQueue();
        }

        private void ButtonGetCurrentServiceCenter_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ServiceCenterAddress address = new ServiceCenterAddress();
                DeviceAddress da = address.Status;
                TextBoxServiceCenter.Text = da.Number;
            }
            catch (Exception ex)
            {

            }
        }

        private void ButtonInput_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                manage.SendToTerminal(TextBoxInput.Text);
                TextBoxInput.Text = "";
            }
            catch (Exception ex)
            {
                ShowMSG(ex.Message, 5 * 1000);
            }
        }

        private void ButtonRunAllTests_Click(object sender, RoutedEventArgs e)
        {
            StackPanelAllTestOutput.Children.Clear();
            manage.TestDevice();
        }

        private void ButtonSend_Click(object sender, RoutedEventArgs e)
        {
            MessageFormat mode = new MessageFormat(SMSMode.TEXT);

            long number;
            long.TryParse(TextBoxNumber.Text, out number);
            SendText sms = new SendText(number, TextBoxSMSContent.Text);
            manage.SendSMS(sms.Command1, sms.Command2);
        }

        private void ButtonSendPDU_Click(object sender, RoutedEventArgs e)
        {
            SendPDUSMS(EncodetoPDU());
        }

        private void ButtonSetServiceCenter_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ServiceCenterAddress address = new ServiceCenterAddress();
                SCAddressType type = new SCAddressType();
                if (TextBoxServiceCenter.Text.StartsWith("+"))
                {
                    type = SCAddressType.International;
                }
                else
                {
                    type = SCAddressType.Other;
                }

                DeviceAddress da = new DeviceAddress(TextBoxServiceCenter.Text, type);
                address.AddressAndType = da;
            }
            catch (Exception ex)
            {

            }
        }

        private void ButtonShowPDU_Click(object sender, RoutedEventArgs e)
        {
            ShowPDU(EncodetoPDU());
        }

        private void ButtonTestConnection_Click(object sender, RoutedEventArgs e)
        {
            manage.CheckConnection();
        }

        /// <summary>
        /// Clears the statusbar.
        /// </summary>
        /// <param name="state">The state.</param>
        private void ClearStatusbar(object state)
        {
            TextBlockStatusBar.Text = "";
        }

        private void ComboBoxBaudRate_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int baudrate = (int)ComboBoxBaudRate.SelectedItem;
            manage.BaudRate = baudrate;
        }

        private void ComboBoxDataBits_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int databit = (int)ComboBoxDataBits.SelectedItem;
            manage.DataBits = databit;
        }

        private void ComboBoxParity_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string parity = (string)ComboBoxParity.SelectedItem;
            manage.Parity = (Parity)Enum.Parse(typeof(Parity), parity);
        }

        private void ComboBoxPort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string port = (string)ComboBoxPort.SelectedItem;
            manage.PortName = port;
        }

        private void ComboBoxReadTimeout_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int timeout = (int)ComboBoxReadTimeout.SelectedItem;
            manage.ReadTimeout = timeout;
        }

        private void ComboBoxStopBits_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string stopbit = (string)ComboBoxStopBits.SelectedItem;
            manage.StopBits = (StopBits)Enum.Parse(typeof(StopBits), stopbit);
        }

        private void ComboBoxWriteTimeout_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int timeout = (int)ComboBoxWriteTimeout.SelectedItem;
            manage.WriteTimeout = timeout;
        }

         private SendPDU EncodetoPDU()
        {
            try
            {
                MessageFormat mode = new MessageFormat(SMSMode.PDU);

                long number;
                long.TryParse(TextBoxPhoneNumberPDU.Text, out number);

                PDU _pdu = new PDU();
                _pdu._1stOctet = new FirstOctet
                {
                    Message_type_indicator = TP_MTI.SMSSUBMIT,
                    Reject_duplicates = false,
                    Reply_path = (bool)CheckBoxReplyPathExist.IsChecked,
                    Status_report_request = (bool)CheckBoxRequestDeliveryReport.IsChecked,
                    User_data_header_indicator = false,
                    Validity_Period_Format = TP_VPI.Relative
                };

                if (CheckBoxFlashSMS.IsChecked == true)
                {
                    _pdu.DataCodingScheme = TP_DSC._7bitFlashSMS;
                }
                else
                {
                    _pdu.DataCodingScheme = TP_DSC._7bit;
                }

                if (CheckBoxRequestDeliveryReport.IsChecked==false)
                {
                    _pdu.ValidityPeriod = -1;
                }
                else
                {
                    _pdu.ValidityPeriod = ComboBoxValidityPeriodPDU.SelectedIndex;
                }
                
                _pdu.PhoneNumber = number.ToString();
                _pdu.UserData = TextBoxSMSContentPDU.Text;


                string result = _pdu.EncodedTextInPDU;
                int lengthOfSMS = (result.Length - 2) / 2;
                SendPDU sms = new SendPDU(lengthOfSMS, result);

                return sms;
            }
            catch (Exception ex)
            {
                ShowMSG(ex.Message, 5 * 1000);
                return null;
            }
        }

        private void Initialize_ComboBoxBaudRate()
        {
            int[] baudrates = ManageSerialPort.GetAllBaudRate();
            foreach (int baudrate in baudrates)
            {
                ComboBoxBaudRate.Items.Add(baudrate);
            }

            ComboBoxBaudRate.SelectedItem = manage.BaudRate;
        }

        private void Initialize_ComboBoxDataBits()
        {
            int[] databits = ManageSerialPort.GetAllDataBits();
            foreach (int databit in databits)
            {
                ComboBoxDataBits.Items.Add(databit);
            }

            ComboBoxDataBits.SelectedItem = manage.DataBits;
        }

        private void Initialize_ComboBoxParity()
        {
            string[] parities = ManageSerialPort.GetAllParity();
            foreach (string parity in parities)
            {
                ComboBoxParity.Items.Add(parity);
            }

            ComboBoxParity.SelectedItem = manage.Parity.ToString();
        }

        private void Initialize_ComboBoxPort()
        {
            string[] ports = ManageSerialPort.GetAllPorts();
            foreach (string port in ports)
            {
                ComboBoxPort.Items.Add(port);
            }

            ComboBoxPort.SelectedItem = manage.PortName;
        }

        private void Initialize_ComboBoxReadTimeout()
        {
            int[] timeouts = ManageSerialPort.GetAllTimeout();
            foreach (int timeout in timeouts)
            {
                ComboBoxReadTimeout.Items.Add(timeout);
            }

            ComboBoxReadTimeout.SelectedItem=manage.ReadTimeout;
        }

        private void Initialize_ComboBoxStopBits()
        {
            string[] stopbits = ManageSerialPort.GetAllStopBits();
            foreach (string stopbit in stopbits)
            {
                ComboBoxStopBits.Items.Add(stopbit);
            }

            ComboBoxStopBits.SelectedItem = manage.StopBits.ToString();
        }

         private void Initialize_ComboBoxWriteTimeout()
        {
            int[] timeouts = ManageSerialPort.GetAllTimeout();
            foreach (int timeout in timeouts)
            {
                ComboBoxWriteTimeout.Items.Add(timeout);
            }

            ComboBoxWriteTimeout.SelectedItem=manage.WriteTimeout;
        }

        /// <summary>
        /// Initializes all comboboxes.
        /// </summary>
        private void InitializeAll()
        {
            Initialize_ComboBoxPort();
            Initialize_ComboBoxBaudRate();
            Initialize_ComboBoxDataBits();
            Initialize_ComboBoxReadTimeout();
            Initialize_ComboBoxStopBits();
            Initialize_ComboBoxWriteTimeout();
            Initialize_ComboBoxParity();
        }

        /// <summary>
        /// Initializes the serila port.
        /// </summary>
        private void InitializeSerilaPort()
        {
            manage = ManageSerialPort.Instance;
            manage.CreateNewSerialPort();
            manage.LoadDefaultSetting();
            InitializeAll();

            // Events
            manage.TestCompleted += new ResultRetievedEventHadler(manage_TestCompleted);
            manage.ShowMessageInStatusbar += new ShowMessageInStatusbarEventHandler(manage_ShowMessageInStatusbar);
            manage.ManufacturerRetieved += new ResultRetievedEventHadler(manage_ManufacturerRetieved);
            manage.ModelRetrieved += new ResultRetievedEventHadler(manage_ModelRetrieved);
            manage.IMEIRetrieved += new ResultRetievedEventHadler(manage_IMEIRetrieved);
            manage.FirmwareRetrieved += new ResultRetievedEventHadler(manage_FirmwareRetrieved);
            manage.SignalQualityRetrieved += new ResultRetievedEventHadler(manage_SignalQualityRetrieved);
            manage.BatteryChargeRetrieved += new ResultRetievedEventHadler(manage_BatteryChargeRetrieved);
            manage.TerminalResponseRetrieved += new ResultRetievedEventHadler(manage_TerminalResponseRetrieved);
            //
        }

        void manage_BatteryChargeRetrieved(object sender, ResultRetievedEventArgs e)
        {
            MakeThreadSafe dd = new MakeThreadSafe(ShowBatteryCharge);
            this.Dispatcher.BeginInvoke(dd, e);
        }

        void manage_FirmwareRetrieved(object sender, ResultRetievedEventArgs e)
        {
            MakeThreadSafe dd = new MakeThreadSafe(ShowFirmware);
            this.Dispatcher.BeginInvoke(dd, e);
        }

        void manage_IMEIRetrieved(object sender, ResultRetievedEventArgs e)
        {
            MakeThreadSafe dd = new MakeThreadSafe(ShowIMEI);
            this.Dispatcher.BeginInvoke(dd, e);
        }

        void manage_ManufacturerRetieved(object sender, ResultRetievedEventArgs e)
        {
            MakeThreadSafe dd = new MakeThreadSafe(ShowManufacturer);
            this.Dispatcher.BeginInvoke(dd, e);
        }

        void manage_ModelRetrieved(object sender, ResultRetievedEventArgs e)
        {
            MakeThreadSafe dd = new MakeThreadSafe(ShowModel);
            this.Dispatcher.BeginInvoke(dd, e);
        }

        private void manage_ShowMessageInStatusbar(object sender, ShowMessageInStatusbarEventArgs e)
        {
            MakeThreadSafe dd = new MakeThreadSafe(ShowMSG);
            this.Dispatcher.BeginInvoke(dd, e);
        }

        void manage_SignalQualityRetrieved(object sender, ResultRetievedEventArgs e)
        {
            MakeThreadSafe dd = new MakeThreadSafe(ShowSignalQuality);
            this.Dispatcher.BeginInvoke(dd, e);
        }

        void manage_TerminalResponseRetrieved(object sender, ResultRetievedEventArgs e)
        {
            MakeThreadSafe dd = new MakeThreadSafe(ShowTerminalOutput);
            this.Dispatcher.BeginInvoke(dd, e.Response);
        }

        void manage_TestCompleted(object sender, ResultRetievedEventArgs e)
        {
            MakeThreadSafe dd = new MakeThreadSafe(ShowTestResult);
            this.Dispatcher.BeginInvoke(dd, e);
        }

        private void SendPDUSMS(SendPDU sms)
        {
            if (sms == null)
            {
                return;
            }

            manage.SendSMS(sms.CommandPart1, sms.CommandPart2);
        }

        /// <summary>
        /// Shows the battery charge.
        /// </summary>
        /// <param name="state">The state.</param>
        private void ShowBatteryCharge(object state)
        {
            ResultRetievedEventArgs e = (ResultRetievedEventArgs)state;
            TextBlockBattery.Text = e.Response;
        }

        /// <summary>
        /// Shows the firmware.
        /// </summary>
        /// <param name="state">The state.</param>
        private void ShowFirmware(object state)
        {
            ResultRetievedEventArgs e = (ResultRetievedEventArgs)state;
            TextBlockFirmware.Text = e.Response;
        }

        /// <summary>
        /// Shows the IMEI.
        /// </summary>
        /// <param name="state">The state.</param>
        private void ShowIMEI(object state)
        {
            ResultRetievedEventArgs e = (ResultRetievedEventArgs)state;
            TextBlockIMEI.Text = e.Response;
        }

        /// <summary>
        /// Shows the manufacturer.
        /// </summary>
        /// <param name="manufacturer">The manufacturer.</param>
        private void ShowManufacturer(object state)
        {
            ResultRetievedEventArgs e = (ResultRetievedEventArgs)state;
            TextBlockManufacturer.Text = e.Response;
        }

        /// <summary>
        /// Shows the model.
        /// </summary>
        /// <param name="state">The state.</param>
        private void ShowModel(object state)
        {
            ResultRetievedEventArgs e = (ResultRetievedEventArgs)state;
            TextBlockModel.Text = e.Response;
        }

        /// <summary>
        /// Shows message comes from event in Statusbar
        /// </summary>
        /// <param name="state">The message.</param>
        private void ShowMSG(object state)
        {
            ShowMessageInStatusbarEventArgs e = (ShowMessageInStatusbarEventArgs)state;
            TextBlockStatusBar.Text = e.MSG;
            _timer.Stop();
            _timer.Interval = e.Duration;
            _timer.Start();
        }

        /// <summary>
        /// Shows message in Statusbar.
        /// </summary>
        /// <param name="msg">The message.</param>
        /// <param name="duration">The duration.</param>
        private void ShowMSG(string msg,double duration)
        {
            TextBlockStatusBar.Text = msg;
            _timer.Stop();
            _timer.Interval = duration;
            _timer.Start();
        }

        private void ShowPDU(SendPDU sms)
        {
            if (sms==null)
            {
                return;
            }

            TextBoxShowPDU.Text = "";
            TextBoxShowPDU.Text += sms.CommandPart1;
            TextBoxShowPDU.Text += sms.CommandPart2;
        }

        /// <summary>
        /// Shows the signal quality.
        /// </summary>
        /// <param name="state">The state.</param>
        private void ShowSignalQuality(object state)
        {
            ResultRetievedEventArgs e = (ResultRetievedEventArgs)state;
            TextBlockSignal.Text = e.Response;
        }

        private void ShowTerminalOutput(object state)
        {
            string response = (string)state;
            TextBoxOutput.Text += "\r\n";
            TextBoxOutput.Text += response;
        }

        /// <summary>
        /// Shows result of AT Commands supporting test.
        /// </summary>
        /// <param name="state">The state.</param>
        private void ShowTestResult(object state)
        {
            lock (state)
            {
                ResultRetievedEventArgs e = (ResultRetievedEventArgs)state;
                StackPanel sp = new StackPanel();
                sp.Orientation = Orientation.Horizontal;
                sp.Margin = new System.Windows.Thickness(1, 2, 5, 2);

                TextBlock txtblock1 = new TextBlock();
                txtblock1.Text = e.Response;

                TextBlock txtblock2 = new TextBlock();
                txtblock2.Margin = new Thickness(5, 0, 5, 0);
                if (e.Success == true)
                {
                    txtblock2.Foreground = Brushes.Green;
                    txtblock2.Text = "Passed";
                }
                else
                {
                    txtblock2.Foreground = Brushes.Red;
                    txtblock2.Text = "Failed";
                }

                sp.Children.Add(txtblock1);
                sp.Children.Add(txtblock2);

                StackPanelAllTestOutput.Children.Add(sp);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            InitializeSerilaPort();

            // Initialize Timer
            _timer.AutoReset = false;
            _timer.Enabled = false;
            _timer.Elapsed += new ElapsedEventHandler(_timer_Elapsed);
            //
        }

		#endregion Methods 
    }
}
