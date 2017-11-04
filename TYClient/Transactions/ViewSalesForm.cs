using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ComponentFactory.Krypton.Toolkit;
using TY.SPIMS.Client.Controls;
using TY.SPIMS.Controllers;

namespace TY.SPIMS.Client.Transactions
{
    public partial class ViewSalesForm : ComponentFactory.Krypton.Toolkit.KryptonForm
    {
        public ViewSalesForm()
        {
            InitializeComponent();
        }

        private void ViewSalesForm_Load(object sender, EventArgs e)
        {
            //SalesControl c = new SalesControl();
            //c.Dock = DockStyle.Fill;
            
            //this.Controls.Add(c);
            //this.WindowState = FormWindowState.Maximized;
        }
    }
}