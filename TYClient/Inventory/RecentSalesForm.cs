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
    public partial class RecentSalesForm : ComponentFactory.Krypton.Toolkit.KryptonForm
    {
        private readonly ICustomerController customerController;
        private readonly ISaleController saleController;

        public int AutoPartDetailId { get; set; }
        public string CustomerName { get; set; }

        public RecentSalesForm()
        {
            this.customerController = IOC.Container.GetInstance<CustomerController>();
            this.saleController = IOC.Container.GetInstance<SaleController>();

            InitializeComponent();
        }

        private void RecentSalesForm_Load(object sender, EventArgs e)
        {
            LoadCustomers();

            if (!string.IsNullOrWhiteSpace(CustomerName))
            {
                CustomerTextbox.Text = CustomerName;
                GetCustomerSales();
            }
            else
            {
                LoadRecentSales();
            }
            
            CustomerTextbox.Focus();
        }

        private void LoadCustomers()
        {
            List<string> customerNames = new List<string>();
            this.customerController.FetchAllCustomers().ForEach((a) => { customerNames.Add(a.CompanyName); });

            AutoCompleteStringCollection collection = new AutoCompleteStringCollection();
            collection.AddRange(customerNames.ToArray());
            CustomerTextbox.AutoCompleteCustomSource = collection;
        }

        private void LoadRecentSales(int customerId = 0)
        {
            var recentSales = this.saleController.FetchSalesPerItemPerCustomer(AutoPartDetailId, customerId);
            salesDetailViewModelBindingSource.DataSource = recentSales;
        }

        private void CustomerTextbox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                GetCustomerSales();
            }
        }

        private void GetCustomerSales()
        {
            if (!string.IsNullOrWhiteSpace(CustomerTextbox.Text))
            {
                var customer = this.customerController.FetchCustomerByName(CustomerTextbox.Text);
                if (customer != null)
                {
                    LoadRecentSales(customer.Id);
                    return;
                }
            }

            LoadRecentSales();
        }
    }
}