using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LogicLayer
{
    public class MessageFormat
    {
		#region Fields (3) 

        private SMSMode _mode;
        private SMSMode _status;
        private bool _supportAvailable;

		#endregion Fields 

		#region Constructors (2) 

        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:LogicLayer.MessageFormat">MessageFormat</see> class.
        /// </summary>
        /// <param name="mode">Set mode of format</param>
        public MessageFormat(SMSMode mode)
        {
            Mode = mode;
        }

        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:LogicLayer.MessageFormat">MessageFormat</see> class.
        /// </summary>
        public MessageFormat()
        {

        }

		#endregion Constructors 

		#region Properties (3) 

        /// <summary>
        /// Gets or sets Message Format
        /// </summary>
        public SMSMode Mode
        {
            get
            {
                return _mode;
            }
            set
            {
                ManageSerialPort manage = ManageSerialPort.Instance;
                if (SupportAvailable == true)
                {
                    if (value == SMSMode.PDU)
                    {
                        if (manage.MessageFormat_Set(ATCommands.MessageFormat_Set_PDU))
                        {
                            _mode = SMSMode.PDU;
                        }
                        else
                        {
                            _mode = SMSMode.NULL;
                        }
                    }
                    else if (value == SMSMode.TEXT)
                    {
                        if (manage.MessageFormat_Set(ATCommands.MessageFormat_Set_TEXT))
                        {
                            _mode = SMSMode.TEXT;
                        }
                        else
                        {
                            _mode = SMSMode.NULL;
                        }
                    }
                }
                else
                {
                    _mode = SMSMode.NULL;
                }
            }
        }

        /// <summary>
        /// Gets device status
        /// </summary>
        public SMSMode Status
        {
            get
            {
                ManageSerialPort manage=ManageSerialPort.Instance;
                if (SupportAvailable == true)
                {
                    _status = manage.MessageFormat_Read();
                }
                else
                {
                    _status = SMSMode.NULL;
                }
                return _status;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the device supports Message Format AT Command
        /// </summary>
        /// <value>
        /// <see langword="true" /> if ; otherwise, <see langword="false" />.
        /// </value>
        public bool SupportAvailable
        {
            get
            {
                ManageSerialPort manage = ManageSerialPort.Instance;
                if (manage.MessageFormat_Test() == true)
                {
                    _supportAvailable = true;
                }
                else
                {
                    _supportAvailable = false;
                }
                return _supportAvailable;
            }
        }

		#endregion Properties 
    }

    /// <summary>
    /// Get or Set Message Format
    /// </summary>
    public enum SMSMode : uint
    {
        /// <summary>
        /// null
        /// </summary>
        NULL = 0,
        /// <summary>
        ///  PDU Mode
        /// </summary>
        PDU = 1,
        /// <summary>
        /// TEXT Mode
        /// </summary>
        TEXT = 2,
    }
}
