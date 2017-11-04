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
    public partial class PurchaseReturnControl : UserControl, IRefreshable
    {
        private readonly ICustomerController customerController;
        private readonly IPurchaseReturnController purchaseReturnController;

        public PurchaseReturnControl()
        {
            this.customerController = IOC.Container.GetInstance<CustomerController>();
            this.purchaseReturnController = IOC.Container.GetInstance<PurchaseReturnController>();

            InitializeComponent();
        }

        #region Load

        ComboBoxEx ex = new ComboBoxEx();
        private void PurchaseReturnControl_Load(object sender, EventArgs e)
        {
            LoadCustomers();
            //LoadPurchaseReturn(null);
        }

        private void LoadCustomers()
        {
            var customers = this.customerController.FetchAllCustomers();

            customerDisplayModelBindingSource.DataSource = customers;
            CustomerDropdown.SelectedIndex = -1;
        }

        private void LoadPurchaseReturn(PurchaseReturnFilterModel filter)
        {
            int records = 0;
            long elapsedTime = ClientHelper.PerformFetch(() =>
            {
                var results = this.purchaseReturnController.FetchPurchaseReturnWithSearch(filter);
                purchaseReturnDisplayModelBindingSource.DataSource = results;

                if (purchaseReturnDisplayModelBindingSource.Count == 0)
                    purchaseReturnDetailModelBindingSource.DataSource = null;

                records = results.Count;
            });

            ((MainForm)this.ParentForm).AttachStatus(records, elapsedTime);
        }

        private PurchaseReturnFilterModel ComposeSearch()
        {
            PurchaseReturnFilterModel model = new PurchaseReturnFilterModel();

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
            PurchaseReturnFilterModel filter = ComposeSearch();
            LoadPurchaseReturn(filter);
        }

        #endregion

        #region Details

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                DataGridViewRow row = dataGridView1.SelectedRows[0];
                int id = (int)row.Cells[PurchaseReturnIdColumn.Name].Value;

                purchaseReturnDetailModelBindingSource.DataSource = this.purchaseReturnController.FetchPurchaseReturnDetails(id);
                paymentDebitModelBindingSource.DataSource = this.purchaseReturnController.FetchPaymentDetails(id);
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

        #region Add Purchase Return

        private void AddPRButton_Click(object sender, EventArgs e)
        {
            OpenForm(0);
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex != -1 && e.ColumnIndex != -1)
            {
                int id = (int)dataGridView1.Rows[e.RowIndex].Cells[PurchaseReturnIdColumn.Name].Value;
                bool? debited = this.purchaseReturnController.IsItemDebited(id);
                if (debited != null && debited.Value)
                {
                    if (ClientHelper.ShowConfirmMessage("Purchase return is already included in a payment. Do you want to continue?") != DialogResult.Yes)
                        return;
                }
                OpenForm(id);
            }
        }

        int selectedId = 0;
        private void OpenForm(int id)
        {
            selectedId = id;

            Form addForm = this.ParentForm.OwnedForms.FirstOrDefault(a => a.Name == "AddPurchaseReturnForm");
            if (addForm == null)
            {
                AddPurchaseReturnForm form = new AddPurchaseReturnForm();
                form.ReturnId = id;
                form.Owner = this.ParentForm;
                form.PurchaseReturnUpdated += new PurchaseReturnUpdatedEventHandler(form_PurchaseReturnUpdated);
                form.Show();
            }
            else
            {
                AddPurchaseReturnForm openedForm = (AddPurchaseReturnForm)addForm;
                openedForm.ReturnId = id;
                openedForm.LoadPurchaseReturnDetails();
                openedForm.Focus();
            }
        }

        void form_PurchaseReturnUpdated(object sender, EventArgs e)
        {
            PurchaseReturnFilterModel filter = ComposeSearch();
            LoadPurchaseReturn(filter);

            if (selectedId == 0)
                purchaseReturnDisplayModelBindingSource.Position = purchaseReturnDisplayModelBindingSource.Count - 1;
            else
            {
                PurchaseReturnDisplayModel item = ((SortableBindingList<PurchaseReturnDisplayModel>)purchaseReturnDisplayModelBindingSource.DataSource)
                    .FirstOrDefault(a => a.Id == selectedId);
                int index = purchaseReturnDisplayModelBindingSource.IndexOf(item);

                purchaseReturnDisplayModelBindingSource.Position = index;
                dataGridView1.Rows[index].Selected = true;
            }
        }

        #endregion

        #region IRefreshable Members

        public void RefreshView()
        {
            PurchaseReturnFilterModel filter = ComposeSearch();
            LoadPurchaseReturn(filter);
        }

        #endregion

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                int id = (int)dataGridView1.SelectedRows[0].Cells[PurchaseReturnIdColumn.Name].Value;
                bool? debited = this.purchaseReturnController.IsItemDebited(id);
                if (debited != null && debited.Value)
                {
                    ClientHelper.ShowErrorMessage("Purchase return is already included in a payment.");
                    return;
                }

                if (ClientHelper.ShowConfirmMessage("Are you sure you want to delete this return?") == DialogResult.Yes)
                {
                    this.purchaseReturnController.DeletePurchaseReturn(id);

                    ClientHelper.ShowSuccessMessage("Purchase return deleted successfully.");
                    this.RefreshView();
                }
            }
        }
    }
}
