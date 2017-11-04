using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ComponentFactory.Krypton.Toolkit;
using TY.SPIMS.Client.Controls;

namespace TY.SPIMS.Client.Inventory
{
    public partial class ViewInventoryForm : ComponentFactory.Krypton.Toolkit.KryptonForm
    {
        public ViewInventoryForm()
        {
            InitializeComponent();
        }

        private void ViewInventoryForm_Load(object sender, EventArgs e)
        {
            InventoryControl c = new InventoryControl();
            c.Dock = DockStyle.Fill;
            this.Controls.Add(c);
        }
    }
}