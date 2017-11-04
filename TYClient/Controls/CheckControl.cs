using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TY.SPIMS.Controllers;
using TY.SPIMS.Entities;
using TY.SPIMS.POCOs;
using TY.SPIMS.Utilities;
using TY.SPIMS.Controllers.Interfaces;

namespace TY.SPIMS.Client.Controls
{
    public partial class CheckControl : UserControl, IRefreshable
    {
        private readonly ICheckController checkController;

        public IssuerType Type { get; set; }

        public CheckControl()
        {
            this.checkController = IOC.Container.GetInstance<CheckController>();

            InitializeComponent();
        }

        #region Load

        private void CheckControl_Load(object sender, EventArgs e)
        {
            var model = ComposeSearch();
            LoadChecks(model);
        }

        private void LoadChecks(CheckFilterModel filter)
        {
            var checks = this.checkController.FetchCheckWithSearch(filter);
            checkDisplayModelBindingSource.DataSource = checks;

            var totalSChecks = checks.Sum(a => a.Amount);
            TotalReceivedTextbox.Text = totalSChecks.ToString("Php #,##0.00");
        }

        private CheckFilterModel ComposeSearch()
        {
            var model = new CheckFilterModel();

            if (!string.IsNullOrWhiteSpace(DetailTextbox.Text))
                model.CheckDetail = DetailTextbox.Text.Trim();

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

            model.Issuer = Type;

            return model;
        }

        private void SearchButton_Click(object sender, EventArgs e)
        {
            CheckFilterModel filter = ComposeSearch();
            LoadChecks(filter);
        }

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dataGridView1.Rows[e.RowIndex].Cells["ClearingColumn"].Value != null)
            {
                DateTime tom = DateTime.Now.AddDays(1).Date;
                DateTime clearDate = (DateTime)dataGridView1.Rows[e.RowIndex].Cells["ClearingColumn"].Value;

                DataGridViewCell cell = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex];
                if (clearDate < tom)
                    cell.Style.ForeColor = Color.Green;
                else
                    cell.Style.ForeColor = Color.Red;

            }
        }
        #endregion

        #region Clear

        private void ClearButton_Click(object sender, EventArgs e)
        {
            ClearFilter();
        }

        private void ClearFilter()
        {
            DetailTextbox.Clear();
            //TypeDropdown.SelectedIndex = -1;
            //TypeDropdown.ComboBox.SelectedIndex = -1;
            AllDateRB.Checked = true;
            DateFromPicker.Value = DateTime.Now;
            DateToPicker.Value = DateTime.Now;
        }

        #endregion

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                DataGridViewRow row = dataGridView1.SelectedRows[0];
                int id = (int)row.Cells["IdColumn"].Value;

                Check c = this.checkController.FetchCheckById(id);
                if (c != null)
                {
                    if (c.PaymentDetail.Any())
                    {
                        var detail = c.PaymentDetail.FirstOrDefault();
                        
                        if(detail != null)
                        {
                            VoucherNumberTextbox.Text = detail.VoucherNumber;
                            PaymentDateTextbox.Text = detail.PaymentDate.HasValue ?
                                detail.PaymentDate.Value.ToShortDateString() : "-";
                            CashTextbox.Text = detail.TotalCashPayment.HasValue ?
                                detail.TotalCashPayment.Value.ToString("Php #,##0.00") : "Php 0.00";
                            CheckTextbox.Text = detail.TotalCheckPayment.HasValue ?
                                detail.TotalCheckPayment.Value.ToString("Php #,##0.00") : "Php 0.00";

                            var total = detail.TotalCashPayment + detail.TotalCheckPayment;
                            TotalPAmountTextbox.Text = total.HasValue ? total.Value.ToString("Php #,##0.00") :
                                "Php 0.00";    
                        }
                    }
                }
            }
        }

        #region IRefreshable Members

        public void RefreshView()
        {
            CheckFilterModel filter = ComposeSearch();
            LoadChecks(filter);
        }

        #endregion
    }
}
