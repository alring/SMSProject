using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LogicLayer
{
    public class ServiceCenterAddress
    {
		#region Fields (4) 

        private DeviceAddress _address;
        private DeviceAddress _status;
        private bool _supportAvailable;
        private ManageSerialPort manage = ManageSerialPort.Instance;

		#endregion Fields 

		#region Constructors (1) 

        public ServiceCenterAddress()
        {

        }

		#endregion Constructors 

		#region Properties (3) 

        /// <summary>
        /// Sets the type of number
        /// </summary>
        /// <remarks>
        /// International or national
        /// </remarks>
        public DeviceAddress AddressAndType
        {
            set
            {
                if (SupportAvailable==true)
                {
                    string _type="";
                    if (value.Type==SCAddressType.International)
                    {
                        _type="145";
                    }
                    else if (value.Type==SCAddressType.Other)
                    {
                        _type="129";
                    }

                    string newSC='"'+value.Number +'"';
                    string command = string.Format("{0}={1},{2}\r", ATCommands.AddressType_Set, newSC, _type);
                    
                    if (manage.AddressType_Set(command)==true)
                    {
                        _address = value;
                    }
                    else
                    {
                        _address = null;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the status of device
        /// </summary>
        public DeviceAddress Status
        {
            get
            {
                if (SupportAvailable==true)
                {
                    _status = manage.AddressType_Read();
                }
                return _status;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the device supports AT+CSCA
        /// </summary>
        /// <value>
        /// <see langword="true" /> if ; otherwise, <see langword="false" />.
        /// </value>
        public bool SupportAvailable
        {
            get
            {
                if (manage.AddressType_Test() == true)
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
    /// This class holds information from <b>+CSCA</b> Command
    /// </summary>
    /// <remarks>
    /// +CSCA Command can read and write service center address
    /// </remarks>
    public class DeviceAddress
    {
		#region Fields (2) 

        private string _scNumber;
        private SCAddressType _type;

		#endregion Fields 

		#region Constructors (1) 

        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:LogicLayer.DeviceAddress">DeviceAddress</see> class.
        /// </summary>
        /// <param name="number"></param>
        /// <param name="type"></param>
        public DeviceAddress(string number, SCAddressType type)
        {
            this._scNumber = number;
            this._type = type;
        }

		#endregion Constructors 

		#region Properties (2) 

        /// <summary>
        /// Gets or sets Service Center Number
        /// </summary>
        public string Number
        {
            get
            {
                return _scNumber;
            }
            set
            {
                _scNumber = value;
            }
        }

        /// <summary>
        /// Gets or sets Service Center Type
        /// </summary>
        public SCAddressType Type
        {
            get
            {
                return _type;
            }
            set
            {
                _type = value;

            }
        }

		#endregion Properties 
    }

    /// <summary>
    /// Get or Set Service Center Address Type
    /// </summary>
    public enum SCAddressType : uint
    {
        NULL = 0,
        International = 145,
        Other = 129,
    }
}
