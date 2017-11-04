using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ComponentFactory.Krypton.Toolkit;
using TY.SPIMS.Client.Controls;

namespace TY.SPIMS.Client.Transactions
{
    public partial class ViewPurchasesForm : ComponentFactory.Krypton.Toolkit.KryptonForm
    {
        public ViewPurchasesForm()
        {
            InitializeComponent();
        }

        private void ViewPurchasesForm_Load(object sender, EventArgs e)
        {
            PurchaseControl c = new PurchaseControl();
            c.Dock = DockStyle.Fill;

            this.Controls.Add(c);
            this.WindowState = FormWindowState.Maximized;
        }
    }
}