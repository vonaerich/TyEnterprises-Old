using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ComponentFactory.Krypton.Toolkit;
using TY.SPIMS.Client.Controls;

namespace TY.SPIMS.Client.Inventory
{
    public partial class SearchItemForm : ComponentFactory.Krypton.Toolkit.KryptonForm
    {
        public int PartId { get; set; }
        public int Quantity { get; set; }
        public string PartNumber 
        { 
            set 
            {
                OnItemSelected(
                    new ItemSelectedEventArgs() 
                    { 
                        AutoPartDetailId = this.PartId, 
                        QtyLeft = this.Quantity,
                        PartNumber = value 
                    });
                this.Close();
            } 
        }

        #region Event

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

        public SearchItemForm()
        {
            InitializeComponent();
        }

        #region Load

        private void SearchItemForm_Load(object sender, EventArgs e)
        {
            InventoryControl c = new InventoryControl();
            c.Dock = DockStyle.Fill;
            c.AllowSelect = true;

            kryptonPanel.Controls.Add(c);
        }

        #endregion
    }

    public class ItemSelectedEventArgs : EventArgs
    {
        public int AutoPartDetailId { get; set; }
        public string PartNumber { get; set; }
        public int QtyLeft { get; set; }
    }

    public delegate void ItemSelectedEventHandler(object sender, ItemSelectedEventArgs e);
}