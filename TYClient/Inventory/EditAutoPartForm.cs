using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ComponentFactory.Krypton.Toolkit;
using TY.SPIMS.Controllers;
using TY.SPIMS.Controllers.Interfaces;

namespace TY.SPIMS.Client.Inventory
{
    public partial class EditAutoPartForm : ComponentFactory.Krypton.Toolkit.KryptonForm
    {
        private readonly IAutoPartController autoPartController;

        public int AutoPartId { get; set; }
        public string PartName { get; set; }

        public EditAutoPartForm()
        {
            this.autoPartController = IOC.Container.GetInstance<AutoPartController>();

            InitializeComponent();
        }

        private void EditAutoPartForm_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(PartName))
            {
                AutoPartTextbox.Text = PartName;
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(AutoPartTextbox.Text))
            {
                this.autoPartController.UpdateAutoPartName(this.AutoPartId, this.AutoPartTextbox.Text);
                ClientHelper.ShowSuccessMessage("Auto part updated successfully.");
            }
        }
    }
}