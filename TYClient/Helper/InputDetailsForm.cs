using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ComponentFactory.Krypton.Toolkit;

namespace TY.SPIMS.Client.Helper
{
    public class InputDetailsForm : KryptonForm
    {
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            base.OnClosing(e);

            if (ClientHelper.ShowConfirmMessage("Unsaved information will be lost. Are you sure?") != 
                System.Windows.Forms.DialogResult.Yes)
            {
                e.Cancel = true;
            }
        }
    }
}
