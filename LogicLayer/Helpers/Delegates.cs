using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LogicLayer
{
    /// <summary>
    /// a delegate for displaying messages in statusbar
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">An <see
    /// cref="T:LogicLayer.ShowMessageInStatusbarEventArgs">ShowMessageInStatusbarEventArgs</see>
    /// that contains the event data.</param>
    public delegate void ShowMessageInStatusbarEventHandler(object sender,ShowMessageInStatusbarEventArgs e);
    /// <summary>
    /// a delegate for sending the result to GUI
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">An <see
    /// cref="T:LogicLayer.ResultRetievedEventArgs">ResultRetievedEventArgs</see> that
    /// contains the event data.</param>
    public delegate void ResultRetievedEventHadler(object sender,ResultRetievedEventArgs e);
}
