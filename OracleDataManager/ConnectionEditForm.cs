using Oracle.ManagedDataAccess.Client;
using System;
using System.Windows.Forms;
using System.Xml.Linq;

namespace OracleDataManager
{
    public partial class ConnectionEditForm : Form
    {
        public DbConnectionProfile Result { get; private set; } = new DbConnectionProfile();

        private bool _testOk = false;

        public ConnectionEditForm(DbConnectionProfile? existing = null)
        {
            InitializeComponent();

            // 기본값
            chkAskPwEveryTime.Checked = true;
            chkSavePassword.Checked = false;
            chkIsProduction.Checked = false;

            btnOk.Enabled = false;            // ✅ TEST 전에는 저장 불가
            lblTestStatus.Text = "TEST 필요";

            // 편집 모드 초기값
            if (existing != null)
            {
                txtName.Text = existing.Name;
                txtUser.Text = existing.UserId;
                txtDataSource.Text = existing.DataSource;
                chkAskPwEveryTime.Checked = existing.AskPasswordEveryTime;
                chkIsProduction.Checked = existing.IsProduction;

                // 비번은 보안상 기본으로 빈칸 (필요하면 다시 입력)
                // 저장된 비번이 있어도 표시 안 하는 게 안전
                chkSavePassword.Checked = !string.IsNullOrWhiteSpace(existing.PasswordEnc);
            }

            // 이벤트
            btnTest.Click += (_, __) => RunTest();
            btnOk.Click += (_, __) => SaveAndClose();
            btnCancel.Click += (_, __) => { DialogResult = DialogResult.Cancel; Close(); };

            // 입력 바뀌면 TEST 다시 요구
            txtName.TextChanged += (_, __) => MarkDirty();
            txtUser.TextChanged += (_, __) => MarkDirty();
            txtPw.TextChanged += (_, __) => MarkDirty();
            txtDataSource.TextChanged += (_, __) => MarkDirty();
            chkAskPwEveryTime.CheckedChanged += (_, __) => MarkDirty();
            chkSavePassword.CheckedChanged += (_, __) => MarkDirty();
            chkIsProduction.CheckedChanged += (_, __) => MarkDirty();

            // UX: 비번 저장 체크하면 "매번 묻기" 끌지 선택(원하면 둘 다 가능하게 둬도 됨)
            chkSavePassword.CheckedChanged += (_, __) =>
            {
                if (chkSavePassword.Checked)
                    chkAskPwEveryTime.Checked = false;
            };
        }

        private void MarkDirty()
        {
            _testOk = false;
            btnOk.Enabled = false;
            lblTestStatus.Text = "변경됨 → TEST 필요";
        }

        private void RunTest()
        {
            var name = txtName.Text.Trim();
            var user = txtUser.Text.Trim();
            var ds = txtDataSource.Text.Trim();
            var pw = txtPw.Text; // 빈 비번 가능 여부는 정책에 따라

            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Name은 필수입니다.");
                txtName.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(user))
            {
                MessageBox.Show("User는 필수입니다.");
                txtUser.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(ds))
            {
                MessageBox.Show("Data Source는 필수입니다.");
                txtDataSource.Focus();
                return;
            }

            // 운영 체크 시 경고(테스트도 조심)
            if (chkIsProduction.Checked)
            {
                var ok = MessageBox.Show(
                    "운영 DB로 표시되었습니다.\n정말 TEST(접속)할까요?",
                    "운영 DB 경고",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (ok != DialogResult.Yes) return;
            }

            try
            {
                lblTestStatus.Text = "TEST 중...";
                lblTestStatus.Refresh();

                var tmp = new DbConnectionProfile
                {
                    Name = name,
                    UserId = user,
                    DataSource = ds,
                    AskPasswordEveryTime = chkAskPwEveryTime.Checked,
                    IsProduction = chkIsProduction.Checked,
                    PasswordEnc = ConfigStore.Encrypt(pw) // 테스트용
                };

                var connStr = ConfigStore.BuildConnStr(tmp);

                using var con = new OracleConnection(connStr);
                con.Open();

                // 간단 ping 쿼리(선택)
                using var cmd = new OracleCommand("SELECT 1 FROM DUAL", con);
                cmd.ExecuteScalar();

                _testOk = true;
                btnOk.Enabled = true;
                lblTestStatus.Text = "✅ TEST 성공";
            }
            catch (Exception ex)
            {
                _testOk = false;
                btnOk.Enabled = false;
                lblTestStatus.Text = "❌ TEST 실패";
                MessageBox.Show("접속 실패: " + ex.Message);
            }
        }

        private void SaveAndClose()
        {
            if (!_testOk)
            {
                MessageBox.Show("TEST 성공 후 저장할 수 있습니다.");
                return;
            }

            var name = txtName.Text.Trim();
            var user = txtUser.Text.Trim();
            var ds = txtDataSource.Text.Trim();
            var pw = txtPw.Text;

            // 저장 정책
            // - SavePassword 체크면 암호화 저장
            // - 아니면 비번 저장 안 함(빈값)
            // - AskPasswordEveryTime가 true면 실행 시 비번 입력
            var encPw = chkSavePassword.Checked ? ConfigStore.Encrypt(pw) : "";

            Result = new DbConnectionProfile
            {
                Name = name,
                UserId = user,
                DataSource = ds,
                PasswordEnc = encPw,
                AskPasswordEveryTime = chkAskPwEveryTime.Checked || !chkSavePassword.Checked,
                IsProduction = chkIsProduction.Checked
            };

            DialogResult = DialogResult.OK;
            Close();
        }
    }

}
