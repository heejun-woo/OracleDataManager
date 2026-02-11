using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OracleDataManager
{
    public partial class PasswordPromptForm : Form
    {
        public string Password => txtPw.Text;

        public PasswordPromptForm(string profileName)
        {
            InitializeComponent();
            lblTitle.Text = $"Password: {profileName}";
            btnOk.Click += (_, __) => { DialogResult = DialogResult.OK; Close(); };
            btnCancel.Click += (_, __) => { DialogResult = DialogResult.Cancel; Close(); };
        }
    }
}
