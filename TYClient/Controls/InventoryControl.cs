using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TY.SPIMS.Controllers;
using TY.SPIMS.Client.Inventory;
using TY.SPIMS.Utilities;
using TY.SPIMS.Entities;
using TY.SPIMS.Controllers.Interfaces;

namespace TY.SPIMS.Client.Controls
{
    public partial class InventoryControl : UserControl, IRefreshable
    {
        private readonly IAutoPartController autoPartController;
        private readonly IBrandController brandController;

        public bool AllowSelect { get; set; } //If true, when user double clicks, the item will be selected and form will be closed

        public InventoryControl()
        {
            this.autoPartController = IOC.Container.GetInstance<AutoPartController>();
            this.brandController = IOC.Container.GetInstance<BrandController>();

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

        private void InventoryControl_Load(object sender, EventArgs e)
        {
            LoadBrands();
            LoadAutoParts();
            CheckUserAccess();

            if (AllowSelect)
            {
                HideButtons();
            }

            APTextbox.Focus();
        }

        private void HideButtons()
        {
            AddButton.Visible = false;
            DeleteButton.Visible = false;

            EditItem.Visible = false;
            DeleteItem.Visible = false;
        }

        private void LoadBrands()
        {
            brandBindingSource.DataSource = this.brandController.FetchAllBrands();
            BrandDropdown.SelectedIndex = -1;
            brandBindingSource.Position = -1;
        }

        private void CheckUserAccess()
        {
            if (UserInfo.IsVisitor)
            {
                HideButtons();
            }
        }

        private void LoadAutoParts()
        {
            string s = APTextbox.Text.Trim();

            autoPartBindingSource.DataSource = !string.IsNullOrWhiteSpace(s) ? this.autoPartController.FetchAPWithSearch(s)
                : this.autoPartController.FetchAllAutoParts();
        }

        private void LoadPartsInventory(bool getPartId)
        {
            int records = 0;
            long elapsedTime = ClientHelper.PerformFetch(() =>
            {
                AutoPartFilterModel filter = CreateFilter(getPartId);

                var result = this.autoPartController.FetchAutoPartWithSearch(filter);
                autoPartDisplayModelBindingSource.DataSource = result;

                records = result.Count;
            });

            if(this.ParentForm.GetType() == typeof(MainForm))
                ((MainForm)this.ParentForm).AttachStatus(records, elapsedTime);
        }

        int autoPartId = 0;
        private void dataGridView2_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView2.SelectedRows.Count > 0)
            {
                DataGridViewRow row = dataGridView2.SelectedRows[0];

                autoPartId = (int)row.Cells["AutoPartIdColumn"].Value;
                LoadPartsInventory(true);
            }
            else
                autoPartDisplayModelBindingSource.DataSource = null;
        }

        private void dataGridView2_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            OpenAutoPartListForm();
        }

        private void OpenAutoPartListForm()
        {
            AutoPartListForm form = new AutoPartListForm();
            form.PartId = autoPartId;
            form.Show();
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
                OpenSelectedAutoPartDetail(e.RowIndex);
            }
        }

        private void OpenSelectedAutoPartDetail(int rowIndex)
        {
            int id = (int)dataGridView1.Rows[rowIndex].Cells["IdColumn"].Value;
            selectedId = id;

            if (!AllowSelect)
            {
                ViewItemForm form = new ViewItemForm();
                form.AutoPartDetailId = id;
                form.FormClosed += new FormClosedEventHandler(form_FormClosed);
                form.Owner = this.ParentForm;
                form.Show();
            }
            else
            {
                DataGridViewRow row = dataGridView1.Rows[rowIndex];
                if(row != null)
                {
                    string partNumber = row.Cells["PartNumberColumn"].Value.ToString();
                    int qty = (int)row.Cells["QtyColumn"].Value;

                    SearchItemForm f = (SearchItemForm)this.ParentForm;
                    f.PartId = id;
                    f.Quantity = qty;
                    f.PartNumber = partNumber;
                }
            }
        }

        void form_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (ClientHelper.IsEdit)
            {
                ClientHelper.IsEdit = false;
                OpenAddForm(selectedId);
            }
        }

        int selectedId = 0;
        private void OpenAddForm(int id)
        {
            selectedId = id;

            Form addForm = this.ParentForm.OwnedForms.FirstOrDefault(a => a.Name == "AddItemForm");
            if (addForm == null)
            {
                AddItemForm form = new AddItemForm();
                form.AutoPartId = id;
                form.Owner = this.ParentForm;
                form.AutoPartUpdated += new AutoPartUpdatedEventHandler(form_AutoPartUpdated);
                form.Show();
            }
            else
            {
                AddItemForm openedForm = (AddItemForm)addForm;
                openedForm.AutoPartId = id;
                openedForm.LoadPartDetails();
                openedForm.Focus();
            }
        }

        void form_AutoPartUpdated(object sender, EventArgs e)
        {
            LoadAutoParts();
            //LoadPartsInventory();

            if (selectedId == 0)
                autoPartDisplayModelBindingSource.MoveLast();
            else
            {
                var item = ((SortableBindingList<AutoPartDisplayModel>)autoPartDisplayModelBindingSource.DataSource)
                    .FirstOrDefault(a => a.Id == selectedId);
                autoPartDisplayModelBindingSource.Position = autoPartDisplayModelBindingSource.IndexOf(item);
            }
        }

        #endregion

        #region Search

        private void APTextbox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Down)
                dataGridView2.Focus();
            else
                LoadAutoParts();
        }

        private AutoPartFilterModel CreateFilter(bool getPartId)
        {
            int brandId = BrandDropdown.SelectedIndex != -1 ?
                (int)BrandDropdown.SelectedValue : 0;

            AutoPartFilterModel filter = new AutoPartFilterModel() { 
                PartNumber = PartNumberTextbox.Text,
                BrandId = brandId,
                Size = SizeTextbox.Text,
                Model = ModelTextbox.Text
            };

            if (getPartId)
                filter.AutoPartId = this.autoPartId;
           
            return filter;
        }

        private void SearchButton_Click(object sender, EventArgs e)
        {
            LoadPartsInventory(false);
        }

        private void SearchTextboxKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                LoadPartsInventory(false);
            else if (e.KeyCode == Keys.Down)
                dataGridView1.Focus();
        }

        #endregion

        #region Clear

        private void ClearButton_Click(object sender, EventArgs e)
        {
            ClearFilter();
        }

        private void ClearFilter()
        {
            PartNumberTextbox.Clear();
            //PartNameTextbox.Clear();
            BrandDropdown.ComboBox.SelectedIndex = -1;
            BrandDropdown.SelectedIndex = -1;
            ModelTextbox.Clear();
            SizeTextbox.Clear();

            LoadPartsInventory(true);
        }

        #endregion

        #region Delete

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                DataGridViewRow row = dataGridView1.SelectedRows[0];
                int id = (int)row.Cells["IdColumn"].Value;

                if (this.autoPartController.AutoPartHasSalesOrPurchases(id))
                {
                    ClientHelper.ShowErrorMessage("Cannot delete auto parts that have corresponding sales or purchases.");
                    return;
                }
                else if (ClientHelper.ShowConfirmMessage("Are you sure you want to delete this auto part?") == DialogResult.Yes)
                {
                    this.autoPartController.DeleteAutoPart(id);
                    ClientHelper.ShowSuccessMessage("Auto part deleted successfully.");

                    LoadPartsInventory(true);
                }
            }
        }

        #endregion

        #region IRefreshable Members

        public void RefreshView()
        {
            LoadPartsInventory(true);
        }

        #endregion

        #region Edit/Delete Auto Part

        private void EditItem_Click(object sender, EventArgs e)
        {
            if (dataGridView2.SelectedRows.Count > 0)
            {
                DataGridViewRow row = dataGridView2.SelectedRows[0];
                int id = (int)row.Cells["AutoPartIdColumn"].Value;
                string partName = row.Cells["PartnameColumn"].Value.ToString();

                EditAutoPartForm form = new EditAutoPartForm();
                form.PartName = partName;
                form.AutoPartId = id;
                form.Show();
            }   
        }

        private void DeleteItem_Click(object sender, EventArgs e)
        {
            if (dataGridView2.SelectedRows.Count > 0)
            {
                DataGridViewRow row = dataGridView2.SelectedRows[0];
                int id = (int)row.Cells["AutoPartIdColumn"].Value;

                if (this.autoPartController.AutoPartHasDetails(id))
                {
                    ClientHelper.ShowErrorMessage("Cannot delete auto part with auto parts under this category.");
                    return;
                }
                else if (ClientHelper.ShowConfirmMessage("Deleting this auto part will delete all auto parts under it. Are you sure?") == DialogResult.Yes)
                {
                    this.autoPartController.DeleteAllAutoPart(id);

                    ClientHelper.ShowSuccessMessage("Auto part deleted successfully.");
                    LoadAutoParts();
                }
            }
        }

        #endregion

        #region Recent Transactions

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
                else if (e.KeyCode == Keys.Enter)
                    OpenSelectedAutoPartDetail(row.Index);
                else if (e.KeyCode == Keys.Up)
                {
                    if (row.Index == 0)
                        PartNumberTextbox.Focus();
                }
            }

            if (e.KeyCode == Keys.Left)
                dataGridView2.Focus();
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

        #endregion

        #region Navigation

        private void dataGridView2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Right)
                dataGridView1.Focus();
            else if (e.KeyCode == Keys.Enter)
                OpenAutoPartListForm();
            else if (e.KeyCode == Keys.Up)
            {
                if (dataGridView2.SelectedRows.Count > 0)
                {
                    DataGridViewRow row = dataGridView2.SelectedRows[0];
                    if (row.Index == 0)
                        APTextbox.Focus();
                }
                else
                    APTextbox.Focus();
            }
        }

        #endregion

    }
}
