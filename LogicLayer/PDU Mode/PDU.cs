using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace LogicLayer
{
    public class PDU
    {
		#region Fields (12) 

        private string _data_coding_scheme = "00";
        /// <summary>
        /// Encoded Body plus Header
        /// </summary>
        private string _encoded_All;
        private FirstOctet _first_octet;
        private string _PDUencoded_user_data;
        private string _phone_number;
        private string _protocol_identifier = "00";
        private string _semioctet_phoneNumber;
        private string _smsLength = "00";
        private string _TP_Message_Reference = "00";
        private string _Type_of_Address = "81";
        private string _user_data;
        // Default value is 24 Hours
        private string _validty_period = "A7";

		#endregion Fields 

		#region Constructors (1) 

        public PDU()
        {

        }

		#endregion Constructors 

		#region Properties (6) 

        /// <summary>
        /// Sets first octet
        /// </summary>
        public FirstOctet _1stOctet
        {
            set
            {
                _first_octet = value;
            }
        }

        /// <summary>
        /// Sets Data Coding Scheme
        /// </summary>
        /// <remarks>
        /// The TP-Data-Coding-Scheme field, defined in GSM 03.40, indicates the data coding
        /// scheme of the TP-UD field, and may indicate a message class
        /// </remarks>
        public TP_DSC DataCodingScheme
        {
            set
            {
                if (value==TP_DSC._7bit)
                {
                    _data_coding_scheme = "00";
                }
                else if (value==TP_DSC._8bit)
                {
                    _data_coding_scheme = "04";
                }
                else if (value==TP_DSC._16bit)
                {
                    _data_coding_scheme = "18";
                }
                else if (value==TP_DSC._7bitFlashSMS)
                {
                    _data_coding_scheme = "10";
                }
                else if (value==TP_DSC._16bitFlashSMS)
                {
                    _data_coding_scheme = "10";
                }
            }
        }

        /// <summary>
        /// Gets text in PDU format
        /// </summary>
        public string EncodedTextInPDU
        {
            get
            {
                string result = _smsLength + _first_octet.ToString() + _TP_Message_Reference + _phone_number.Length.ToString("X2") + _Type_of_Address +
                    _semioctet_phoneNumber + _protocol_identifier + _data_coding_scheme + _validty_period + _user_data.Length.ToString("X2") + _PDUencoded_user_data;

                _encoded_All = result;
                return _encoded_All;
            }
        }

        /// <summary>
        /// Sets phone number
        /// </summary>
        public string PhoneNumber
        {
            set
            {
                _phone_number = value;
                _semioctet_phoneNumber = ConvertToSemiOctet(value);
            }
        }

        /// <summary>
        /// Sets User Data
        /// </summary>
        public string UserData
        {
            set
            {
                _user_data = value;
                _PDUencoded_user_data = EncodePDU.GetMessage(value);
            }
        }

        /// <summary>
        /// Sets Validity Period
        /// </summary>
        public int ValidityPeriod
        {
            set
            {
                if (value==-1)
                {
                    _validty_period = TP_VP._Disabled;
                }
                else if (value==0)
                {
                    _validty_period = TP_VP._1Hour;
                }
                else if (value == 1)
                {
                    _validty_period = TP_VP._6Hours;
                }
                else if (value == 2)
                {
                    _validty_period = TP_VP._24Hours;
                }
                else if (value == 3)
                {
                    _validty_period = TP_VP._3Days;
                }
                else if (value == 4)
                {
                    _validty_period = TP_VP._12Hours;
                }
                else if (value == 5)
                {
                    _validty_period = TP_VP._1Week;
                }
                else if (value == 6)
                {
                    _validty_period = TP_VP._Max;
                }
            }
        }

		#endregion Properties 

		#region Methods (1) 

		// Private Methods (1) 

        /// <summary>
        /// Convert phone number to semi octet
        /// </summary>
        /// <param name="number"></param>
        private string ConvertToSemiOctet(string number)
        {
            string number1 = "";

            if (number.Length%2!=0)
            {
                number1 = number;
                number1 += "F";
            }
            else
            {
                number1 = number;
            }

            char[] number2 = new char[number1.Length];
            number1.CopyTo(0, number2, 0, number1.Length);

            char[] number3 = new char[number1.Length];

            for (int i=0;i<number2.Length;i+=2)
            {
                number3[i] = number2[i + 1];
                number3[i + 1] = number2[i];
            }

            string finalResult = new string(number3, 0, number3.Length);
            return finalResult;
        }

		#endregion Methods 
    }


    /// <summary>
    /// First octet of message
    /// </summary>
    public class FirstOctet
    {
		#region Fields (1) 

        private int[] _firstOctet = new int[8];

		#endregion Fields 

		#region Constructors (1) 

        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:LogicLayer.FirstOctet">FirstOctet</see> class.
        /// </summary>
        public FirstOctet()
        {

        }

		#endregion Constructors 

		#region Properties (6) 

        /// <summary>
        /// Sets Message type indicator
        /// </summary>
        /// <remarks>
        /// Bits no 1 and 0 are set to 0 and 1 respectively to indicate that this PDU is an
        /// SMS-SUBMIT.
        /// Bits no 1 and 0 are both set to 0 to indicate that this PDU is an SMS-DELIVER 
        /// </remarks>
        public TP_MTI Message_type_indicator
        {
            set
            {
                if (value==TP_MTI.SMSDELIVER)
                {
                    _firstOctet[0] = 0;
                    _firstOctet[1] = 0;
                }
                else if (value==TP_MTI.SMSSUBMIT)
                {
                    _firstOctet[0] = 1;
                    _firstOctet[1] = 0;
                }
            }
        }

        /// <summary>
        /// Sets a value indicating whether SC shall reject duplicates
        /// </summary>
        /// <remarks>
        /// whether or not the SC shall accept an SMS-SUBMIT for an SM still held in the SC
        /// which has the same TP-MR and the same TP-DA as a previously submitted SM from
        /// the same OA.
        /// </remarks>
        /// <value>
        /// <see langword="true" /> if ; otherwise, <see langword="false" />.
        /// </value>
        public bool Reject_duplicates
        {
            set
            {
                if (value==true)
                {
                    _firstOctet[2] = 1;
                }
                else
                {
                    _firstOctet[2] = 0;
                }
            }
        }

        /// <summary>
        /// Sets a value indicating whether reply path exists.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if ; otherwise, <see langword="false" />.
        /// </value>
        public bool Reply_path
        {
            set
            {
                if (value == true)
                {
                    _firstOctet[7] = 1;
                }
                else
                {
                    _firstOctet[7] = 0;
                }
            }
        }

        /// <summary>
        /// Sets a value indicating whether Status report requested
        /// </summary>
        /// <value>
        /// <see langword="true" /> if ; otherwise, <see langword="false" />.
        /// </value>
        public bool Status_report_request
        {
            set
            {
                if (value==true)
                {
                    _firstOctet[5] = 1;
                }
                else
                {
                    _firstOctet[5] = 0;
                }
            }
        }

        /// <summary>
        /// Sets User data header indicator
        /// </summary>
        /// <value>
        /// <see langword="true" /> if ; otherwise, <see langword="false" />.
        /// </value>
        public bool User_data_header_indicator
        {
            set
            {
                if (value==true)
                {
                    _firstOctet[6] = 1;
                }
                else
                {
                    _firstOctet[6] = 0;
                }
            }
        }

        /// <summary>
        /// Sets Validity Period Format
        /// </summary>
        public TP_VPI Validity_Period_Format
        {
            set
            {
                if (value==TP_VPI.NULL)
                {
                    _firstOctet[3] = 0;
                    _firstOctet[4] = 0;
                }
                else if (value==TP_VPI.Enhanced)
                {
                    _firstOctet[4] = 0;
                    _firstOctet[3] = 1;
                }
                else if (value==TP_VPI.Relative)
                {
                    _firstOctet[4] = 1;
                    _firstOctet[3] = 0;
                }
                else if (value==TP_VPI.Absolute)
                {
                    _firstOctet[4] = 1;
                    _firstOctet[3] = 1;
                }
            }
        }

		#endregion Properties 

		#region Methods (2) 


        /// <summary>
        /// Converts Decimal number to Hex
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String">T:System.String</see> that represents the current
        /// <see cref="T:System.Object">T:System.Object</see>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return BinaryToDecimal().ToString("X2");
        }

        /// <summary>
        /// Converts binary number to decimal
        /// </summary>
        /// <returns>
        /// returns decimal number
        /// </returns>
        private int BinaryToDecimal()
        {
            int result = 0;

            for (int i = 0; i < 8;i++ )
            {
                result += Convert.ToInt32(_firstOctet[i] * Math.Pow(2, i));
            }

            return result;
        }

		#endregion Methods 
    }

    /// <summary>
    /// TP_MTI
    /// </summary>
    public enum TP_MTI : uint
    {
        NULL=0,
        SMSDELIVER = 1,
        SMSSUBMIT = 2,
    }

    /// <summary>
    /// TP_VPI
    /// </summary>
    public enum TP_VPI:uint
    {
        NULL=0,
        Relative=1,
        Enhanced=2,
        Absolute=3,
    }

    /// <summary>
    /// TP_DSC
    /// </summary>
    public enum TP_DSC:uint
    {
        NULL=0,
        _7bit=1,
        _8bit=2,
        _16bit=3,
        _7bitFlashSMS=4,
        _16bitFlashSMS=5,
    }
    /// <summary>
    /// TP_VP
    /// </summary>
    public struct TP_VP
    {
		#region Data Members (8) 

        /// <summary>
        /// 12 Hours = 143
        /// </summary>
        public static string _12Hours = "8F";

        /// <summary>
        /// 1 Hour = 11
        /// </summary>
        public static string _1Hour = "0B";

        /// <summary>
        /// 1 Week = 173
        /// </summary>
        public static string _1Week = "AD";

        /// <summary>
        /// 24 Hours = 167
        /// </summary>
        public static string _24Hours = "A7";

        /// <summary>
        /// 3 Days = 169
        /// </summary>
        public static string _3Days = "A9";

        /// <summary>
        /// 6 Hours = 71
        /// </summary>
        public static string _6Hours = "47";
        
        /// <summary>
        /// Disabled
        /// </summary>
        public static string _Disabled = "00";
        /// <summary>
        /// Max = 255
        /// </summary>
        public static string _Max = "FF";

		#endregion Data Members 
    }
}
