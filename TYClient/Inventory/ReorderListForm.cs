using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ComponentFactory.Krypton.Toolkit;
using TY.SPIMS.Controllers;
using Microsoft.Office.Interop.Excel;
using TY.SPIMS.Client.Helper;
using TY.SPIMS.Controllers.Interfaces;

namespace TY.SPIMS.Client.Inventory
{
    public partial class ReorderListForm : KryptonForm
    {
        private readonly IAutoPartController autoPartController;

        public ReorderListForm()
        {
            this.autoPartController = IOC.Container.GetInstance<AutoPartController>();

            InitializeComponent();
        }

        #region Load

        private void ReorderListForm_Load(object sender, EventArgs e)
        {
            LoadItemsForReorder(true);
        }

        private void LoadItemsForReorder(bool reorderList)
        {
            AutoPartFilterModel filter = new AutoPartFilterModel() { ForReorder = true, ReorderList = reorderList };

            var itemsForReorder = this.autoPartController.FetchAutoPartWithSearch(filter);
            autoPartDisplayModelBindingSource.DataSource = itemsForReorder;
        }

        #endregion

        private void SaveButton_Click(object sender, EventArgs e)
        {
            SaveReorders();
        }

        private void SaveReorders()
        {
            if (dataGridView1.Rows.Count > 0)
            {
                List<int> itemsForReorder = new List<int>();
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    int id = (int)row.Cells["IdColumn"].Value;

                    itemsForReorder.Add(id);
                    this.autoPartController.UpdateReorderDates(itemsForReorder);
                }

                ClientHelper.ShowSuccessMessage("Reorder saved successfully.");
                LoadItemsForReorder(true);
            }
        }

        private void ForReorderRB_CheckedChanged(object sender, EventArgs e)
        {
            ToggleReorder();
            SaveButton.Visible = true;
        }

        private void ReorderRB_CheckedChanged(object sender, EventArgs e)
        {
            ToggleReorder();
            SaveButton.Visible = false;
        }

        private void ToggleReorder()
        {
            if (ForReorderRB.Checked)
                LoadItemsForReorder(true);
            else if (ReorderRB.Checked)
                LoadItemsForReorder(false);
        }
    }
}