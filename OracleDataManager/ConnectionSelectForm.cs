using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using Oracle.ManagedDataAccess.Client;

namespace OracleDataManager
{
    public partial class ConnectionSelectForm : Form
    {
        private DbConnectionStore _store = new();
        private BindingList<DbConnectionProfile> _binding = new();

        public ConnectionSelectForm()
        {
            InitializeComponent();

            InitGrid();

            Load += (_, __) => Reload();
            btnConnect.Click += (_, __) => ConnectSelected();
            btnAdd.Click += (_, __) => AddProfile();
            btnEdit.Click += (_, __) => EditProfile();
            btnDelete.Click += (_, __) => DeleteProfile();
            btnClose.Click += (_, __) => Close();

            gridProfiles.CellDoubleClick += (_, e) =>
            {
                if (e.RowIndex >= 0) ConnectSelected();
            };
        }

        private void InitGrid()
        {
            gridProfiles.AutoGenerateColumns = false;
            gridProfiles.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            gridProfiles.MultiSelect = false;
            gridProfiles.AllowUserToAddRows = false;
            gridProfiles.AllowUserToDeleteRows = false;
            gridProfiles.ReadOnly = true;
            gridProfiles.RowHeadersVisible = false;

            gridProfiles.Columns.Clear();
            gridProfiles.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(DbConnectionProfile.Name),
                HeaderText = "Name",
                Width = 180
            });
            gridProfiles.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(DbConnectionProfile.UserId),
                HeaderText = "User",
                Width = 140
            });
            gridProfiles.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(DbConnectionProfile.DataSource),
                HeaderText = "Data Source",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            });
            gridProfiles.Columns.Add(new DataGridViewCheckBoxColumn
            {
                DataPropertyName = nameof(DbConnectionProfile.IsProduction),
                HeaderText = "PROD",
                Width = 60
            });
            gridProfiles.Columns.Add(new DataGridViewCheckBoxColumn
            {
                DataPropertyName = nameof(DbConnectionProfile.AskPasswordEveryTime),
                HeaderText = "Ask PW",
                Width = 70
            });

            // 운영 라인 색칠
            gridProfiles.RowPrePaint += (_, e) =>
            {
                var row = gridProfiles.Rows[e.RowIndex];
                if (row.DataBoundItem is DbConnectionProfile p && p.IsProduction)
                {
                    row.DefaultCellStyle.BackColor = System.Drawing.Color.MistyRose;
                }
            };
        }

        private void Reload()
        {
            _store = ConfigStore.LoadStore();
            _binding = new BindingList<DbConnectionProfile>(_store.Profiles);
            gridProfiles.DataSource = _binding;

            // 마지막 선택 복원
            if (!string.IsNullOrWhiteSpace(_store.LastSelectedName))
            {
                var idx = _binding.ToList().FindIndex(p => p.Name == _store.LastSelectedName);
                if (idx >= 0)
                {
                    gridProfiles.ClearSelection();
                    gridProfiles.Rows[idx].Selected = true;
                    gridProfiles.CurrentCell = gridProfiles.Rows[idx].Cells[0];
                }
            }
            if (gridProfiles.Rows.Count > 0 && gridProfiles.SelectedRows.Count == 0)
            {
                gridProfiles.Rows[0].Selected = true;
                gridProfiles.CurrentCell = gridProfiles.Rows[0].Cells[0];
            }
        }

        private DbConnectionProfile? GetSelected()
        {
            if (gridProfiles.SelectedRows.Count == 0) return null;
            return gridProfiles.SelectedRows[0].DataBoundItem as DbConnectionProfile;
        }

        private void ConnectSelected()
        {
            var p = GetSelected();
            if (p == null) { MessageBox.Show("프로필을 선택하세요."); return; }

            // 운영이면 경고
            if (p.IsProduction)
            {
                var ok = MessageBox.Show(
                    $"[운영 DB] {p.Name}\n정말 접속할까요?",
                    "경고",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);
                if (ok != DialogResult.Yes) return;
            }

            // 비번 매번 묻기 옵션
            string? pwOverride = null;
            if (p.AskPasswordEveryTime)
            {
                using var pwForm = new PasswordPromptForm(p.Name);
                if (pwForm.ShowDialog(this) != DialogResult.OK) return;
                pwOverride = pwForm.Password;
            }

            var connStr = ConfigStore.BuildConnStr(p, pwOverride);

            // 접속 테스트
            try
            {
                using var con = new OracleConnection(connStr);
                con.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show("접속 실패: " + ex.Message);
                return;
            }

            // 마지막 선택 저장
            _store.LastSelectedName = p.Name;
            _store.Profiles = _binding.ToList();
            ConfigStore.SaveStore(_store);

            // 메인 툴 실행
            Hide();
            using (var main = new OracleCrudGridForm(connStr))
            {
                main.ShowDialog(this);
            }
            Show();
        }

        private void AddProfile()
        {
            using var f = new ConnectionEditForm(); // 아래에서 설명
            if (f.ShowDialog(this) != DialogResult.OK) return;

            _binding.Add(f.Result);
            SaveNow();
        }

        private void EditProfile()
        {
            var p = GetSelected();
            if (p == null) return;

            using var f = new ConnectionEditForm(p);
            if (f.ShowDialog(this) != DialogResult.OK) return;

            // 선택 행 갱신 (BindingList라 참조 수정이면 자동 반영되지만 안전하게)
            var idx = _binding.ToList().FindIndex(x => ReferenceEquals(x, p));
            if (idx >= 0) _binding[idx] = f.Result;

            SaveNow();
        }

        private void DeleteProfile()
        {
            var p = GetSelected();
            if (p == null) return;

            if (MessageBox.Show($"삭제할까요?\n{p.Name}", "삭제 확인",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                return;

            _binding.Remove(p);
            SaveNow();
        }

        private void SaveNow()
        {
            _store.Profiles = _binding.ToList();
            ConfigStore.SaveStore(_store);
        }
    }

}
