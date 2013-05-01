using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LogicLayer
{

    /// <summary>
    /// Create AT Command for sending PDU Encoded data.
    /// </summary>
    public class SendPDU
    {
		#region Fields (4) 

        private int _length;
        private string _part1;
        private string _part2;
        private string _pdu;

		#endregion Fields 

		#region Constructors (1) 

        /// <summary>
        /// Initializes a new instance of the <see cref="T:LogicLayer.SendPDU">SendPDU</see>
        /// class.
        /// </summary>
        /// <param name="length">The legth of PDU Encoded data.</param>
        /// <param name="pdu">PDU Encoded data</param>
        public SendPDU(int length,string pdu)
        {
            _length = length;
            _pdu = pdu;
        }

		#endregion Constructors 

		#region Properties (2) 

        /// <summary>
        /// Gets first part of command
        /// </summary>
        public string CommandPart1
        {
            get
            {
                _part1 = string.Format("AT+CMGS={0}\r\n",_length);
                return _part1;
            }
        }

        /// <summary>
        /// Gets second part of command.
        /// </summary>
        public string CommandPart2
        {
            get
            {
                _part2 = string.Format("{0}{1}\r\n", _pdu, char.ConvertFromUtf32(26));
                return _part2;
            }
        }

		#endregion Properties 
    }
}
