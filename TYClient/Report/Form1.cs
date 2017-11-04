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

namespace TY.SPIMS.Client.Report
{
    public partial class Form1 : ComponentFactory.Krypton.Toolkit.KryptonForm
    {
        private readonly IPurchaseController purchaseController;

        public Form1()
        {
            this.purchaseController = IOC.Container.GetInstance<PurchaseController>();

            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.PurchasesViewBindingSource.DataSource = this.purchaseController.FetchAllPurchases();
            this.reportViewer1.RefreshReport();
        }
    }
}