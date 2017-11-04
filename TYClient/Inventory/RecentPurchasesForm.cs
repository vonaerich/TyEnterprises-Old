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
    public partial class RecentPurchasesForm : ComponentFactory.Krypton.Toolkit.KryptonForm
    {
        private readonly ICustomerController customerController;
        private readonly IPurchaseController purchaseController;

        public int AutoPartDetailId { get; set; }
        public string SupplierName { get; set; }

        public RecentPurchasesForm()
        {
            this.customerController = IOC.Container.GetInstance<CustomerController>();
            this.purchaseController = IOC.Container.GetInstance<PurchaseController>();

            InitializeComponent();
        }

        private void RecentSalesForm_Load(object sender, EventArgs e)
        {
            LoadSuppliers();

            if (!string.IsNullOrWhiteSpace(SupplierName))
            {
                SupplierTextbox.Text = SupplierName;
                GetSupplierPurchases();
            }
            else
            {
                LoadRecentPurchases();
            }

            SupplierTextbox.Focus();
        }

        private void LoadSuppliers()
        {
            List<string> supplierNames = new List<string>();
            this.customerController.FetchAllCustomers()
                .ForEach((a) => { supplierNames.Add(a.CompanyName); });

            AutoCompleteStringCollection collection = new AutoCompleteStringCollection();
            collection.AddRange(supplierNames.ToArray());
            SupplierTextbox.AutoCompleteCustomSource = collection;
        }

        private void LoadRecentPurchases(int supplierId = 0)
        {
            var recentPurchases = this.purchaseController.FetchPurchasesPerItemPerSupplier(AutoPartDetailId, supplierId);
            purchaseDetailViewModelBindingSource.DataSource = recentPurchases;
        }

        private void SupplierTextbox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                GetSupplierPurchases();
            }
        }

        private void GetSupplierPurchases()
        {
            if (!string.IsNullOrWhiteSpace(SupplierTextbox.Text))
            {
                var supplier = this.customerController.FetchCustomerByName(SupplierTextbox.Text);
                if (supplier != null)
                {
                    LoadRecentPurchases(supplier.Id);
                    return;
                }
            }

            LoadRecentPurchases();
        }
    }
}