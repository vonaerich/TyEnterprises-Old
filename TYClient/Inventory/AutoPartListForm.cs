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
    public partial class AutoPartListForm : ComponentFactory.Krypton.Toolkit.KryptonForm
    {
        private readonly IAutoPartController autoPartController;
        private readonly IBrandController brandController;

        public int PartId { get; set; }

        public AutoPartListForm()
        {
            this.autoPartController = IOC.Container.GetInstance<AutoPartController>();
            this.brandController = IOC.Container.GetInstance<BrandController>();

            InitializeComponent();
        }

        private void AutoPartListForm_Load(object sender, EventArgs e)
        {
            this.LoadBrands();
            this.LoadPartsInventory();
        }

        private AutoPartFilterModel CreateFilter()
        {
            int brandId = BrandDropdown.SelectedIndex != -1 ?
                (int)BrandDropdown.SelectedValue : 0;

            AutoPartFilterModel filter = new AutoPartFilterModel()
            {
                AutoPartId = this.PartId,
                PartNumber = PartNumberTextbox.Text,
                BrandId = brandId,
                Size = SizeTextbox.Text,
                Model = ModelTextbox.Text
            };

            return filter;
        }

        private void LoadPartsInventory()
        {
            AutoPartFilterModel filter = CreateFilter();
            autoPartDisplayModelBindingSource.DataSource = this.autoPartController.FetchAutoPartWithSearch(filter);
        }

        private void LoadBrands()
        {
            brandBindingSource.DataSource = this.brandController.FetchAllBrands();
            BrandDropdown.SelectedIndex = -1;
            brandBindingSource.Position = -1;
        }

        private void SearchButton_Click(object sender, EventArgs e)
        {
            LoadPartsInventory();
        }

        private void ClearButton_Click(object sender, EventArgs e)
        {
            PartNumberTextbox.Clear();
            //PartNameTextbox.Clear();
            BrandDropdown.ComboBox.SelectedIndex = -1;
            BrandDropdown.SelectedIndex = -1;
            ModelTextbox.Clear();
            SizeTextbox.Clear();

            LoadPartsInventory();    
        }

        private void dataGridView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                DataGridViewRow row = dataGridView1.SelectedRows[0];
                int id = (int)row.Cells["IdColumn"].Value;

                if (e.KeyCode == Keys.F12)
                    LoadRecentSales(id);
                else if (e.KeyCode == Keys.F11)
                    LoadRecentPurchases(id);
                else if (e.KeyCode == Keys.Up)
                {
                    if (row.Index == 0)
                        PartNumberTextbox.Focus();
                }
            }
        }

        private void LoadRecentPurchases(int id)
        {
            RecentPurchasesForm form = new RecentPurchasesForm();
            form.AutoPartDetailId = id;
            form.Show();
        }

        private void LoadRecentSales(int id)
        {
            RecentSalesForm form = new RecentSalesForm();
            form.AutoPartDetailId = id;
            form.Show();
        }
    }
}