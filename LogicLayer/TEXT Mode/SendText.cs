using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LogicLayer
{
    public class SendText
    {
		#region Fields (4) 

        private string _command1;
        private string _command2;
        private string _content;
        private long _number;

		#endregion Fields 

		#region Constructors (2) 

        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:LogicLayer.SendText">SendText</see> class.
        /// </summary>
        /// <param name="number"></param>
        /// <param name="content"></param>
        public SendText(long number,string content)
        {
            _number = number;
            _content = content;
        }

        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:LogicLayer.SendText">SendText</see> class.
        /// </summary>
        public SendText()
        {

        }

		#endregion Constructors 

		#region Properties (4) 

        /// <summary>
        /// Gets first part of AT Command that must be send to device
        /// </summary>
        public string Command1
        {
            get
            {
                string number='"'+Number.ToString()+'"';
                string command = string.Format("AT+CMGS={0}\r\n", number);
                _command1 = command;
                return _command1;
            }
        }

        /// <summary>
        /// Gets second part of AT Command that must be send to device
        /// </summary>
        public string Command2
        {
            get
            {
                // 26 of ascii = Ctrl + z
                _command2 = string.Format("{0}{1}\r\n", Content, char.ConvertFromUtf32(26));
                return _command2;
            }
        }

        /// <summary>
        /// Gets or sets body of sms
        /// </summary>
        public string Content
        {
            get
            {
                return _content;
            }
            set
            {
                _content = value;
            }
        }

        /// <summary>
        /// Gets or sets the number that the sms must send to
        /// </summary>
        public long Number
        {
            get
            {
                return _number;
            }
            set
            {
                _number = value;
            }
        }

		#endregion Properties 
    }
}
