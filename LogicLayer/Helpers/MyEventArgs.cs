using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LogicLayer
{
    /// <summary>
    /// this class will show messages in statusbar
    /// </summary>
    public class ShowMessageInStatusbarEventArgs:EventArgs
    {
		#region Fields (2) 

        private double _duration;
        private string _msg;

		#endregion Fields 

		#region Constructors (1) 

        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:LogicLayer.ShowMessageInStatusbarEventArgs">ShowMessageInStatusbarEventArgs</see>
        /// class.
        /// </summary>
        /// <param name="message">the message that must be displayed in GUI</param>
        /// <param name="duration">the duration of displaying message in statusbar</param>
        public ShowMessageInStatusbarEventArgs(string message,double duration)
        {
            this._msg = message;
            this._duration = duration;
        }

		#endregion Constructors 

		#region Properties (2) 

        /// <summary>
        /// Gets the duration of displaying message in GUI
        /// </summary>
        public double Duration
        {
            get
            {
                return _duration;
            }
        }

        /// <summary>
        /// Gets message that must be displayed in GUI
        /// </summary>
        public string MSG
        {
            get
            {
                return _msg;
            }
        }

		#endregion Properties 
    }

    /// <summary>
    /// a class for sending results to another layer(GUI)
    /// </summary>
    public class ResultRetievedEventArgs:EventArgs
    {
		#region Fields (2) 

        private string response;
        private bool success;

		#endregion Fields 

		#region Constructors (1) 

        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:LogicLayer.ResultRetievedEventArgs">ResultRetievedEventArgs</see> class.
        /// </summary>
        /// <param name="_success">If set to <see langword="true"/>, then ; otherwise,
        /// .</param>
        /// <param name="_response">response from device</param>
        public ResultRetievedEventArgs(bool _success,string _response)
        {
            this.success = _success;
            this.response = _response;
        }

		#endregion Constructors 

		#region Properties (2) 

        public string Response
        {
            get { return response; }
            set { response = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether that the return value of AT Command was
        /// successful or not
        /// </summary>
        /// <value>
        /// <see langword="true" /> if successful; otherwise, <see langword="false" />.
        /// </value>
        public bool Success
        {
            get { return success; }
            set { success = value; }
        }

		#endregion Properties 
    }
}
