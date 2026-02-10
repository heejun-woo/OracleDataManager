using Oracle.ManagedDataAccess.Client;
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
    public partial class ConnectionForm : Form
    {
        public DbConfig ResultConfig { get; private set; } = new DbConfig();

        public ConnectionForm(DbConfig? existing = null)
        {
            InitializeComponent();

            if (existing != null)
            {
                txtUser.Text = existing.UserId;
                txtDataSource.Text = existing.DataSource;
                // 비번은 보통 빈칸으로 두고 다시 입력하게 하는 게 안전
            }

            btnTest.Click += (_, __) => TestConnection();
            btnSave.Click += (_, __) => SaveAndClose();
        }

        private void TestConnection()
        {
            try
            {
                var cfg = new DbConfig
                {
                    UserId = txtUser.Text.Trim(),
                    PasswordEnc = ConfigStore.Encrypt(txtPw.Text),
                    DataSource = txtDataSource.Text.Trim()
                };

                var connStr = ConfigStore.BuildConnStr(cfg);

                using var con = new OracleConnection(connStr);
                con.Open();

                MessageBox.Show("접속 성공");
            }
            catch (Exception ex)
            {
                MessageBox.Show("접속 실패: " + ex.Message);
            }
        }

        private void SaveAndClose()
        {
            var user = txtUser.Text.Trim();
            var ds = txtDataSource.Text.Trim();
            var pw = txtPw.Text; // 빈 비번 허용 여부는 정책에 따라

            if (string.IsNullOrWhiteSpace(user) || string.IsNullOrWhiteSpace(ds))
            {
                MessageBox.Show("UserId / Data Source는 필수입니다.");
                return;
            }

            ResultConfig = new DbConfig
            {
                UserId = user,
                PasswordEnc = ConfigStore.Encrypt(pw),
                DataSource = ds
            };

            DialogResult = DialogResult.OK;
            Close();
        }

    }

}
