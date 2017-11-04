using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TY.SISystem.Controllers;
using TY.SISystem.Utilities;
using TYEnterprises.SI.Client.Suppliers;

namespace TYEnterprises.SI.Client.Controls
{
    public partial class SupplierControl : UserControl, IRefreshable
    {
        public SupplierControl()
        {
            InitializeComponent();
        }

        #region Next / Previous

        private void NextButton_Click(object sender, EventArgs e)
        {
            // If nothing is selected
            if (dataGridView1.SelectedRows.Count == 0)
            {
                // If there are rows in the grid
                if (dataGridView1.Rows.Count > 0)
                {
                    // Select the first row
                    dataGridView1.Rows[0].Selected = true;
                }
            }
            else
            {
                // Find index of next row
                int index = dataGridView1.SelectedRows[0].Index + 1;

                // If past end of list then go back to the start
                if (index >= dataGridView1.Rows.Count)
                    index = 0;

                // Select the row
                dataGridView1.Rows[index].Selected = true;
            }

            dataGridView1.Refresh();
        }

        private void PreviousButton_Click(object sender, EventArgs e)
        {
            // If nothing is selected
            if (dataGridView1.SelectedRows.Count == 0)
            {
                // If there are rows in the grid
                if (dataGridView1.Rows.Count > 0)
                {
                    // Select the last row
                    dataGridView1.Rows[dataGridView1.Rows.Count - 1].Selected = true;
                }
            }
            else
            {
                // Find index of previous row
                int index = dataGridView1.SelectedRows[0].Index - 1;

                // If past start of list then go back to the end
                if (index < 0)
                    index = dataGridView1.Rows.Count - 1;

                // Select the row
                dataGridView1.Rows[index].Selected = true;
            }

            dataGridView1.Refresh();
        }

        #endregion

        #region Load

        private void SupplierControl_Load(object sender, EventArgs e)
        {
            LoadSuppliers();
        }

        private void LoadSuppliers()
        {
            supplierDisplayModelBindingSource.DataSource = SupplierController.Instance
                .FetchSupplierWithSearch(SupplierTextbox.Text.Trim());
        }

        #endregion

        #region Add/Edit

        private void AddButton_Click(object sender, EventArgs e)
        {
            OpenAddForm(0);
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex != -1 && e.ColumnIndex != -1)
            {
                int id = (int)dataGridView1.Rows[e.RowIndex].Cells["IdColumn"].Value;

                OpenAddForm(id);
            }
        }

        int selectedId = 0;
        private void OpenAddForm(int id)
        {
            selectedId = id;

            Form addForm = this.ParentForm.OwnedForms.FirstOrDefault(a => a.Name == "AddSupplierForm");
            if (addForm == null)
            {
                AddSupplierForm form = new AddSupplierForm();
                form.SupplierId = id;
                form.Owner = this.ParentForm;
                form.SupplierUpdated += new SupplierUpdatedEventHandler(form_SupplierUpdated);
                form.Show();
            }
            else
            {
                AddSupplierForm openedForm = (AddSupplierForm)addForm;
                openedForm.SupplierId = id;
                openedForm.LoadSupplierDetails();
                openedForm.Focus();
            }
        }

        void form_SupplierUpdated(object sender, EventArgs e)
        {
            LoadSuppliers();

            var source = (SortableBindingList<SupplierDisplayModel>)supplierDisplayModelBindingSource.DataSource;
            if (source != null)
            {
                if (selectedId == 0)
                    selectedId = source.Max(a => a.Id);

                if (selectedId != 0)
                {
                    SupplierDisplayModel item = source.FirstOrDefault(a => a.Id == selectedId);
                    int index = supplierDisplayModelBindingSource.IndexOf(item);

                    supplierDisplayModelBindingSource.Position = index;
                    dataGridView1.Rows[index].Selected = true;
                }
            }
        }

        #endregion

        #region Search

        private void SearchButton_Click(object sender, EventArgs e)
        {
            LoadSuppliers();
        }

        private void SupplierNameTextbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                LoadSuppliers();
        }

        #endregion

        #region Delete

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            if (ClientHelper.ShowConfirmMessage("Are you sure you want to delete this supplier?") == DialogResult.Yes)
            {
                if (dataGridView1.SelectedRows.Count > 0)
                {
                    DataGridViewRow row = dataGridView1.SelectedRows[0];
                    int id = (int)row.Cells["IdColumn"].Value;

                    SupplierController.Instance.DeleteSupplier(id);
                    ClientHelper.ShowSuccessMessage("Supplier deleted successfully.");

                    LoadSuppliers();
                }
            }
        }

        #endregion

        #region IRefreshable Members

        public void RefreshView()
        {
            LoadSuppliers();
        }

        #endregion
    }
}
