using System;
using System.Linq;
using System.Windows.Forms;
using TY.SPIMS.Client.Returns;
using TY.SPIMS.Controllers;
using TY.SPIMS.POCOs;
using TY.SPIMS.Utilities;
using TY.SPIMS.Controllers.Interfaces;

namespace TY.SPIMS.Client.Controls
{
    public partial class SalesReturnControl : UserControl, IRefreshable
    {
        private readonly ICustomerController customerController;
        private readonly ISalesReturnController salesReturnController;

        public SalesReturnControl()
        {
            this.customerController = IOC.Container.GetInstance<CustomerController>();
            this.salesReturnController = IOC.Container.GetInstance<SalesReturnController>();

            InitializeComponent();
        }

        #region Load

        private void SalesReturnControl_Load(object sender, EventArgs e)
        {
            LoadCustomers();
            CustomerDropdown.Focus();
        }

        private void LoadCustomers()
        {
            customerDisplayModelBindingSource.DataSource = this.customerController.FetchAllCustomers();
            CustomerDropdown.SelectedIndex = -1;
        }

        private void LoadSalesReturn(SalesReturnFilterModel filter)
        {
            int records = 0;
            long elapsedTime = ClientHelper.PerformFetch(() =>
            {
                var results = this.salesReturnController.FetchSalesReturnWithSearch(filter);
                salesReturnDisplayModelBindingSource.DataSource = results;

                if (salesReturnDisplayModelBindingSource.Count == 0)
                    salesReturnDetailModelBindingSource.DataSource = null;

                records = results.Count;
            });

            ((MainForm)this.ParentForm).AttachStatus(records, elapsedTime);
        }

        private SalesReturnFilterModel ComposeSearch()
        {
            SalesReturnFilterModel model = new SalesReturnFilterModel();

            if (CustomerDropdown.SelectedIndex != -1)
                model.CustomerId = (int)CustomerDropdown.SelectedValue;

            if (!string.IsNullOrWhiteSpace(MemoTextbox.Text))
                model.MemoNumber = MemoTextbox.Text.Trim();

            if (!AllDateRB.Checked)
            {
                if (DateRangeRB.Checked)
                {
                    model.DateType = DateSearchType.DateRange;
                    model.DateFrom = DateFromPicker.Value;
                    model.DateTo = DateToPicker.Value;
                }
            }
            else
                model.DateType = DateSearchType.All;

            if (!AllStatusRB.Checked)
            {
                if (UsedStatusRB.Checked)
                    model.Status = ReturnStatusType.Used;
                else
                    model.Status = ReturnStatusType.Unused;
            }
            else
                model.Status = ReturnStatusType.All;

            return model;
        }

        private void SearchButton_Click(object sender, EventArgs e)
        {
            SalesReturnFilterModel filter = ComposeSearch();
            LoadSalesReturn(filter);
        }

        #endregion

        #region Details

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                DataGridViewRow row = dataGridView1.SelectedRows[0];
                int id = (int)row.Cells[SalesReturnIdColumn.Name].Value;

                salesReturnDetailModelBindingSource.DataSource = this.salesReturnController.FetchSalesReturnDetails(id);
                paymentCreditModelBindingSource.DataSource = this.salesReturnController.FetchPaymentDetails(id);
            }
        }

        #endregion

        #region Clear

        private void ClearButton_Click(object sender, EventArgs e)
        {
            ClearSearch();
        }

        private void ClearSearch()
        {
            CustomerDropdown.ComboBox.SelectedIndex = -1;
            CustomerDropdown.SelectedIndex = -1;
            MemoTextbox.Clear();
            AllDateRB.Checked = true;
            DateToPicker.Value = DateTime.Now;
            DateFromPicker.Value = DateTime.Now;
        }

        #endregion

        #region Add/Edit Sales Return

        private void AddButton_Click(object sender, EventArgs e)
        {
            OpenForm(0);
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex != -1 && e.ColumnIndex != -1)
            {
                int id = (int)dataGridView1.Rows[e.RowIndex].Cells[SalesReturnIdColumn.Name].Value;
                bool? credited = this.salesReturnController.IsItemCredited(id);
                if (credited != null && credited.Value)
                {
                    if(ClientHelper.ShowConfirmMessage("Sales return is already included in a payment. Do you want to continue?") != DialogResult.Yes)
                        return;
                }
                OpenForm(id);
            }
        }

        int selectedId = 0;
        private void OpenForm(int id)
        {
            selectedId = id;

            Form addForm = this.ParentForm.OwnedForms.FirstOrDefault(a => a.Name == "AddSalesReturnForm");
            if (addForm == null)
            {
                AddSalesReturnForm form = new AddSalesReturnForm();
                form.ReturnId = id;
                form.Owner = this.ParentForm;
                form.SalesReturnUpdated += new SalesReturnUpdatedEventHandler(form_SalesReturnUpdated);
                form.Show();
            }
            else
            {
                AddSalesReturnForm openedForm = (AddSalesReturnForm)addForm;
                openedForm.ReturnId = id;
                openedForm.LoadSalesReturnDetails();
                openedForm.Focus();
            }
        }

        void form_SalesReturnUpdated(object sender, EventArgs e)
        {
            SalesReturnFilterModel filter = ComposeSearch();
            LoadSalesReturn(filter);

            if (selectedId == 0)
                salesReturnDisplayModelBindingSource.Position = salesReturnDisplayModelBindingSource.Count - 1;
            else
            {
                SalesReturnDisplayModel item = ((SortableBindingList<SalesReturnDisplayModel>)salesReturnDisplayModelBindingSource.DataSource)
                    .FirstOrDefault(a => a.Id == selectedId);
                int index = salesReturnDisplayModelBindingSource.IndexOf(item);

                salesReturnDisplayModelBindingSource.Position = index;
                dataGridView1.Rows[index].Selected = true;
            }
        }

        #endregion

        #region IRefreshable Members

        public void RefreshView()
        {
            SalesReturnFilterModel filter = ComposeSearch();
            LoadSalesReturn(filter);
        }

        #endregion

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                int id = (int)dataGridView1.SelectedRows[0].Cells[SalesReturnIdColumn.Name].Value;
                bool? credited = this.salesReturnController.IsItemCredited(id);
                if (credited != null && credited.Value)
                {
                    ClientHelper.ShowErrorMessage("Sales return is already included in a payment.");
                    return;
                }

                if (ClientHelper.ShowConfirmMessage("Are you sure you want to delete this return?") == DialogResult.Yes)
                {
                    this.salesReturnController.DeleteSalesReturn(id);
                    ClientHelper.ShowSuccessMessage("Sales return deleted successfully.");
                    this.RefreshView();
                }
            }
        }
    }
}
