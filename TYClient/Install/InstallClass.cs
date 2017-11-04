using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Configuration.Install;
using System.Windows.Forms;

namespace TY.SPIMS.Client.Install
{
    [RunInstaller(true)]
    public partial class InstallerObj : Installer
    {
        public InstallerObj()
        {
            Form f = new InstallForm();
            f.BringToFront();
            f.ShowDialog();
        }
    }
}
