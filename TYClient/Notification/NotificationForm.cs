using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ComponentFactory.Krypton.Toolkit;

namespace TY.SPIMS.Client.Notification
{
    public partial class NotificationForm : ComponentFactory.Krypton.Toolkit.KryptonForm
    {
        public NotificationForm()
        {
            InitializeComponent();
        }

        private void NotificationForm_Load(object sender, EventArgs e)
        {
            notificationBindingSource.DataSource = TY.SPIMS.Utilities.Helper.GetAllNotifications();
        }
    }
}