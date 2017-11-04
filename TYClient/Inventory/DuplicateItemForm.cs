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
    public partial class DuplicateItemForm : ComponentFactory.Krypton.Toolkit.KryptonForm
    {
        private readonly IAutoPartController autoPartController;

        #region Props

        public string AltPartNumber { get; set; }

        #endregion

        #region Events

        public event ItemSelectedEventHandler ItemSelected;
        protected void OnItemSelected(ItemSelectedEventArgs e)
        {
            ItemSelectedEventHandler handler = ItemSelected;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        #endregion

        public DuplicateItemForm()
        {
            this.autoPartController = IOC.Container.GetInstance<AutoPartController>();

            InitializeComponent();
        }

        #region Load

        private void DuplicateItemForm_Load(object sender, EventArgs e)
        {
            LoadItems();
        }

        private void LoadItems()
        {
            if (!string.IsNullOrWhiteSpace(AltPartNumber))
            {
                AutoPartFilterModel filter = new AutoPartFilterModel() { PartNumber = AltPartNumber };
                var items = this.autoPartController.FetchAutoPartWithSearch(filter);

                autoPartDisplayModelBindingSource.DataSource = items;
            }
        }

        #endregion

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex != -1 && e.ColumnIndex != -1)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

                int id = (int)row.Cells["IdColumn"].Value;
                string p = row.Cells["PartNumberColumn"].Value != null ?
                    row.Cells["PartNumberColumn"].Value.ToString() : string.Empty;
                int qty = (int)row.Cells["QtyColumn"].Value;

                SelectRow(id, p, qty);
            }
        }

        private void dataGridView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                DataGridViewRow row = dataGridView1.SelectedRows[0];

                int id = (int)row.Cells["IdColumn"].Value;
                string p = row.Cells["PartNumberColumn"].Value != null ?
                    row.Cells["PartNumberColumn"].Value.ToString() : string.Empty;
                int qty = (int)row.Cells["QtyColumn"].Value;

                if (e.KeyCode == Keys.Enter)
                    SelectRow(id, p, qty);
                else if (e.KeyCode == Keys.F12)
                    LoadRecentSales(id);
                else if (e.KeyCode == Keys.F11)
                    LoadRecentPurchases(id);
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

        private void SelectRow(int id, string partNumber, int qty)
        {
            OnItemSelected(new ItemSelectedEventArgs() { AutoPartDetailId = id, PartNumber = partNumber, QtyLeft = qty });
            this.Close();
        }
    }
}