using System;
using System.Linq;
using System.Windows.Forms;
using TY.SPIMS.Client.Brands;
using TY.SPIMS.Controllers;
using TY.SPIMS.POCOs;
using TY.SPIMS.Utilities;
using TY.SPIMS.Controllers.Interfaces;

namespace TY.SPIMS.Client.Controls
{
    public partial class BrandControl : UserControl, IRefreshable
    {
        private readonly IAutoPartController autoPartController;
        private readonly IBrandController brandController;

        public BrandControl()
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

        private void BrandControl_Load(object sender, EventArgs e)
        {
            LoadBrands();
        }

        private void LoadBrands()
        {
            brandDisplayModelBindingSource.DataSource = this.brandController.FetchBrandWithSearch(BrandTextbox.Text);
        }

        #endregion

        #region Add/Edit

        private void AddButton_Click(object sender, EventArgs e)
        {
            OpenAddForm(0);
        }

        void form_BrandUpdated(object sender, EventArgs e)
        {
            LoadBrands();

            var source = (SortableBindingList<BrandDisplayModel>)brandDisplayModelBindingSource.DataSource;
            if (source != null)
            {
                if (selectedId == 0)
                    selectedId = source.Max(a => a.Id);

                if (selectedId != 0)
                {
                    BrandDisplayModel item = source.FirstOrDefault(a => a.Id == selectedId);
                    int index = brandDisplayModelBindingSource.IndexOf(item);

                    brandDisplayModelBindingSource.Position = index;
                    dataGridView1.Rows[index].Selected = true;
                }
            }
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex != -1 && e.ColumnIndex != -1)
            {
                int id = (int)dataGridView1.Rows[e.RowIndex].Cells["IdColumn"].Value;
                OpenAddForm(id);
            }
        }

        int selectedId;
        private void OpenAddForm(int id)
        {
            selectedId = id;

            Form addForm = this.ParentForm.OwnedForms.FirstOrDefault(a => a.Name == "AddBrandForm");
            if (addForm == null)
            {
                AddBrandForm form = new AddBrandForm();
                form.BrandId = id;
                form.Owner = this.ParentForm;
                form.BrandUpdated += new BrandUpdatedEventHandler(form_BrandUpdated);
                form.Show();
            }
            else
            {
                AddBrandForm openedForm = (AddBrandForm)addForm;
                openedForm.BrandId = id;
                openedForm.LoadBrandDetails();
                openedForm.Focus();
            }
        }

        #endregion

        #region Search

        private void SearchButton_Click(object sender, EventArgs e)
        {
            LoadBrands();
        }

        private void BrandTextbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                LoadBrands();
        }

        #endregion

        #region Auto Part Panel

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                DataGridViewRow row = dataGridView1.SelectedRows[0];
                int id = (int)row.Cells["IdColumn"].Value;

                AutoPartFilterModel filter = new AutoPartFilterModel() { BrandId = id };
                autoPartDisplayModelBindingSource.DataSource = this.autoPartController.FetchAutoPartWithSearch(filter);
            }
        }

        #endregion

        #region Delete

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            if (ClientHelper.ShowConfirmMessage("Are you sure you want to delete this brand?") == DialogResult.Yes)
            {
                if (dataGridView1.SelectedRows.Count > 0)
                {
                    DataGridViewRow row = dataGridView1.SelectedRows[0];
                    int id = (int)row.Cells["IdColumn"].Value;

                    this.brandController.DeleteBrand(id);
                    ClientHelper.ShowSuccessMessage("Brand deleted successfully.");

                    LoadBrands();
                }
            }
        }

        #endregion

        #region IRefreshable Members

        public void RefreshView()
        {
            LoadBrands();
        }

        #endregion

    }
}
