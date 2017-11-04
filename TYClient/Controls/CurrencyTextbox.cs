using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TY.SPIMS.Client.Controls
{
    public partial class CurrencyTextbox : UserControl
    {
        public CurrencyTextbox()
        {
            InitializeComponent();
        }

        #region Properties

        public bool AlwaysActive
        {
            get { return textBox1.AlwaysActive; }
            set { textBox1.AlwaysActive = value; }
        }

        public override string Text
        {
            get
            { return textBox1.Text; }
            set
            { textBox1.Text = value; }
        }

        private Color _backColor;
        public Color BackgroundColor
        {
            get { return _backColor; }
            set
            {
                _backColor = value;
                textBox1.StateCommon.Back.Color1 = _backColor;
            }
        }

        private bool _readOnly;
        public bool ReadOnly
        {
            get { return _readOnly; }
            set
            {
                _readOnly = value;
                textBox1.ReadOnly = _readOnly;
            }
        }

        private string[] _autoCompleteValues;
        public string[] AutoCompleteValues
        {
            get { return _autoCompleteValues; }
            set
            {
                _autoCompleteValues = value;

                if (_autoCompleteValues != null)
                {
                    textBox1.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                    textBox1.AutoCompleteSource = AutoCompleteSource.CustomSource;

                    AutoCompleteStringCollection collection = new AutoCompleteStringCollection();
                    foreach (string s in _autoCompleteValues)
                    {
                        collection.Add(s);
                    }
                    
                    textBox1.AutoCompleteCustomSource = collection;
                }
            }
        }

        #endregion

        public void Clear()
        {
            textBox1.Clear();
        }

        // Boolean flag used to determine when a character other than a number is entered.
        private bool nonNumberEntered = false;

        // Handle the KeyDown event to determine the type of character entered into the control.
        private void textBox1_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            // Initialize the flag to false.
            nonNumberEntered = false;

            // Determine whether the keystroke is a number from the top of the keyboard.
            if (e.KeyCode < Keys.D0 || e.KeyCode > Keys.D9)
            {
                // Determine whether the keystroke is a number from the keypad.
                if (e.KeyCode < Keys.NumPad0 || e.KeyCode > Keys.NumPad9)
                {
                    // Determine whether the keystroke is a backspace.
                    if (e.KeyCode != Keys.Back)
                    {
                        if (e.KeyCode != Keys.Decimal && e.KeyCode != Keys.OemPeriod)
                        {
                            // A non-numerical keystroke was pressed.
                            // Set the flag to true and evaluate in KeyPress event.
                            nonNumberEntered = true;
                        }
                        else
                        {
                            if (textBox1.Text.Contains('.'))
                                nonNumberEntered = true;
                        }
                    }
                }
            }
            //If shift key was pressed, it's not a number.
            if (Control.ModifierKeys == Keys.Shift)
            {
                nonNumberEntered = true;
            }
        }

        // This event occurs after the KeyDown event and can be used to prevent
        // characters from entering the control.
        private void textBox1_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            // Check for the flag being set in the KeyDown event.
            if (nonNumberEntered == true)
            {
                // Stop the character from being entered into the control since it is non-numerical.
                e.Handled = true;
            }
        }

        private void CurrencyTextbox_Load(object sender, EventArgs e)
        {
            textBox1.StateCommon.Content.Font = this.Font;
        }
    }
}
