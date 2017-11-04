using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ComponentFactory.Krypton.Toolkit;
using TY.SPIMS.Controllers;

namespace TY.SPIMS.Client.ActionLog
{
    public partial class ActionLogForm : ComponentFactory.Krypton.Toolkit.KryptonForm
    {
        public ActionLogForm()
        {
            InitializeComponent();
        }

        private void ActionLogForm_Load(object sender, EventArgs e)
        {
            LoadActions(false);
        }

        private void LoadActions(bool filter)
        {
            actionDisplayModelBindingSource.DataSource = ActionLogController
                .Instance
                .FetchActionsWithSearch(filter, DateFromPicker.Value.Date, DateToPicker.Value.Date);
        }

        private void FilterButton_Click(object sender, EventArgs e)
        {
            LoadActions(true);
        }
    }
}