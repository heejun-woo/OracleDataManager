using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Windows.Forms;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace OracleDataManager
{
    public partial class OracleCrudGridForm : Form
    {
        //private readonly string _connStr = "User Id=C##HASED;Password=1234;Data Source = (DESCRIPTION =  (ADDRESS = (PROTOCOL = TCP)(HOST = localhost)(PORT = 1521))  (CONNECT_DATA =    (SERVICE_NAME = XEPDB1)    (SERVER = DEDICATED)  ));        Pooling=false;";

        //private readonly string _connStr = "User Id=C##HASED;Password=1234;Data Source = (DESCRIPTION =  (ADDRESS = (PROTOCOL = TCP)(HOST = localhost)(PORT = 1521))  (CONNECT_DATA =    (SID =XE)    (SERVER = DEDICATED)  ));        Pooling=false;";

        string _connStr = string.Empty;

        private string _tableName = "";
        private DataTable _table;

        // 메타
        private Dictionary<string, ColMeta> _metaByCol = new(StringComparer.OrdinalIgnoreCase);

        // ✅ 테이블별 시퀀스 매핑(실무용)
        // 예: {"MY_TABLE", new Dictionary<string,string>{{"ID","SEQ_MY_TABLE_ID"}}}
        private readonly Dictionary<string, Dictionary<string, string>> _sequenceMap
            = new(StringComparer.OrdinalIgnoreCase)
            {
                // ["EMP"] = new(StringComparer.OrdinalIgnoreCase) { ["EMP_ID"] = "SEQ_EMP_ID" },
            };

        public OracleCrudGridForm()
        {
            InitializeComponent();

            grid.AutoGenerateColumns = true;
            grid.AllowUserToAddRows = true;
            grid.AllowUserToDeleteRows = true;
            btnSave.CausesValidation = false;

            Load += (_, __) => LoadTableList();
            btnLoad.Click += (_, __) => LoadSelectedTable();
            btnSave.Click += (_, __) => SaveAllChanges();

            // 실사용 이벤트
            grid.DataError += (_, e) =>
            {
                e.ThrowException = false;
                MessageBox.Show($"데이터 형식 오류: {e.Exception?.Message}");
            };
            grid.CellValidating += Grid_CellValidating;
            grid.EditingControlShowing += Grid_EditingControlShowing;
            grid.CellDoubleClick += Grid_CellDoubleClick_DatePicker;
            grid.KeyDown += Grid_KeyDown_Paste;
            grid.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
            grid.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
        }

        #region  UI: 테이블 목록
        private void LoadTableList()
        {
            try
            {
                using var con = new OracleConnection(_connStr);
                con.Open();

                using var cmd = new OracleCommand(@"
                    SELECT table_name
                    FROM user_tables
                    ORDER BY table_name", con);

                using var rdr = cmd.ExecuteReader();
                cbTables.Items.Clear();
                while (rdr.Read())
                    cbTables.Items.Add(rdr.GetString(0));

                if (cbTables.Items.Count > 0) cbTables.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine("=== ex.ToString() ===");
                Console.WriteLine(ex.ToString());

                // InnerException 체인 끝까지 훑기
                Exception? cur = ex;
                int depth = 0;
                while (cur != null && depth++ < 10)
                {
                    Console.WriteLine($"\n--- Exception Level {depth} ---");
                    Console.WriteLine($"Type: {cur.GetType().FullName}");
                    Console.WriteLine($"Message: {cur.Message}");

                    if (cur is OracleException oex)
                    {
                        Console.WriteLine($"OracleException.Number: {oex.Number}");
                        Console.WriteLine($"OracleException.Message: {oex.Message}");

                        for (int i = 0; i < oex.Errors.Count; i++)
                        {
                            var err = oex.Errors[i];
                            Console.WriteLine($"  [{i}] ORA-{err.Number}: {err.Message}");
                        }
                    }

                    cur = cur.InnerException;
                }
            }
        }
        #endregion

        #region 조회 + 메타 로드 
        private void LoadSelectedTable()
        {
            if (cbTables.SelectedItem == null) return;
            _tableName = cbTables.SelectedItem.ToString()!;

            using var con = new OracleConnection(_connStr);
            con.Open();

            _metaByCol = LoadColumnMeta(con, _tableName);

            // ROWID 포함 (PK 없어도 Update/Delete 가능)
            var selectSql = $"SELECT ROWID AS \"__ROWID\", t.* FROM {_tableName} t";
            using var da = new OracleDataAdapter(selectSql, con);

            _table = new DataTable();
            da.Fill(_table);

            grid.DataSource = _table;

            // ROWID 숨김
            if (grid.Columns.Contains("__ROWID"))
                grid.Columns["__ROWID"].Visible = false;



            ApplyGridMetaUI();
        }

        private Dictionary<string, ColMeta> LoadColumnMeta(OracleConnection con, string tableName)
        {
            // user_tab_columns: data_type, nullable, data_default, char_length, data_precision, data_scale 등
            using var cmd = new OracleCommand(@"
            SELECT column_name, data_type, nullable, data_default,
                   char_length, data_precision, data_scale
            FROM user_tab_columns
            WHERE table_name = :t
            ORDER BY column_id", con);
            cmd.Parameters.Add(new OracleParameter("t", tableName));

            using var rdr = cmd.ExecuteReader();
            var dict = new Dictionary<string, ColMeta>(StringComparer.OrdinalIgnoreCase);

            while (rdr.Read())
            {
                var name = rdr.GetString(0);
                var dataType = rdr.IsDBNull(1) ? "" : rdr.GetString(1);
                var nullable = rdr.IsDBNull(2) ? "Y" : rdr.GetString(2);
                var dataDefault = rdr.IsDBNull(3) ? null : rdr.GetString(3);
                var charLen = rdr.IsDBNull(4) ? (int?)null : Convert.ToInt32(rdr.GetDecimal(4));
                var prec = rdr.IsDBNull(5) ? (int?)null : Convert.ToInt32(rdr.GetDecimal(5));
                var scale = rdr.IsDBNull(6) ? (int?)null : Convert.ToInt32(rdr.GetDecimal(6));

                dict[name] = new ColMeta
                {
                    Name = name,
                    DataType = dataType,
                    IsNullable = nullable.Equals("Y", StringComparison.OrdinalIgnoreCase),
                    DefaultSql = dataDefault?.Trim(),
                    CharLength = charLen,
                    Precision = prec,
                    Scale = scale,
                    IsLob = dataType.Equals("CLOB", StringComparison.OrdinalIgnoreCase)
                            || dataType.Equals("BLOB", StringComparison.OrdinalIgnoreCase)
                            || dataType.Equals("NCLOB", StringComparison.OrdinalIgnoreCase),
                    IsDateLike = dataType.Equals("DATE", StringComparison.OrdinalIgnoreCase)
                                || dataType.StartsWith("TIMESTAMP", StringComparison.OrdinalIgnoreCase),
                    IsNumber = dataType.Equals("NUMBER", StringComparison.OrdinalIgnoreCase)
                };
            }

            return dict;
        }

        private void ApplyGridMetaUI()
        {
            if (_table == null) return;

            foreach (DataGridViewColumn col in grid.Columns)
            {
                var name = col.DataPropertyName;
                if (string.IsNullOrWhiteSpace(name)) name = col.Name;

                if (!_metaByCol.TryGetValue(name, out var meta)) continue;

                // LOB ReadOnly + 표시
                if (meta.IsLob)
                {
                    col.ReadOnly = false;
                    col.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                }

                // NOT NULL 표시: 헤더에 *
                if (!meta.IsNullable && !meta.IsLob && !name.Equals("__ROWID", StringComparison.OrdinalIgnoreCase))
                {
                    if (!col.HeaderText.EndsWith("*")) col.HeaderText = col.HeaderText + " *";
                }

                // 문자열 길이 힌트
                if (meta.CharLength.HasValue && meta.CharLength.Value > 0)
                {
                    col.ToolTipText = $"{meta.DataType}({meta.CharLength.Value})"
                        + (meta.IsNullable ? "" : " NOT NULL");
                }
            }
        }
        #endregion

        #region 입력 검증
        private void Grid_CellValidating(object? sender, DataGridViewCellValidatingEventArgs e)
        {
            if (e.RowIndex >= 0 && grid.Rows[e.RowIndex].IsNewRow) return;

            if (_table == null) return;
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            var col = grid.Columns[e.ColumnIndex];
            var colName = col.DataPropertyName;

            if (string.IsNullOrEmpty(colName) || colName.Equals("__ROWID", StringComparison.OrdinalIgnoreCase))
                return;

            if (!_metaByCol.TryGetValue(colName, out var meta)) return;

            var text = (e.FormattedValue?.ToString() ?? "").Trim();

            // NOT NULL 체크 (단, INSERT 시 DB DEFAULT/SEQ로 채울 수 있으면 여기서 허용)
            if (!meta.IsNullable)
            {
                bool willAutoFillBySeq = TryGetSequenceName(_tableName, colName, out _);
                bool hasDefault = !string.IsNullOrWhiteSpace(meta.DefaultSql);

                if (string.IsNullOrEmpty(text) && !(willAutoFillBySeq || hasDefault))
                {
                    grid.Rows[e.RowIndex].ErrorText = $"{colName} 는 필수입니다.";
                    e.Cancel = true;
                    return;
                }
            }

            // 문자열 길이 체크
            if (meta.CharLength.HasValue && meta.CharLength.Value > 0 && text.Length > meta.CharLength.Value)
            {
                grid.Rows[e.RowIndex].ErrorText = $"{colName} 길이 초과 (최대 {meta.CharLength.Value})";
                e.Cancel = true;
                return;
            }

            // 숫자 체크
            if (meta.IsNumber && !string.IsNullOrEmpty(text))
            {
                if (!decimal.TryParse(text, out _))
                {
                    grid.Rows[e.RowIndex].ErrorText = $"{colName} 는 숫자여야 합니다.";
                    e.Cancel = true;
                    return;
                }
            }

            if (meta.IsLob)
            {
                var strTest = e.FormattedValue?.ToString() ?? "";

                if (strTest.Length > 1000)
                {
                    grid.Rows[e.RowIndex].ErrorText = $"{colName} 는 1000자 이하로 입력하세요.";
                    e.Cancel = true;
                    return;
                }

            }

            grid.Rows[e.RowIndex].ErrorText = "";
        }

        private void Grid_EditingControlShowing(object? sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (grid.CurrentCell == null) return;

            var colName = grid.Columns[grid.CurrentCell.ColumnIndex].DataPropertyName;
            if (string.IsNullOrEmpty(colName) || !_metaByCol.TryGetValue(colName, out var meta)) return;

            // NUMBER 컬럼이면 키 입력 제한(실사용)
            if (meta.IsNumber && e.Control is TextBox tb)
            {
                tb.KeyPress -= NumberTextBox_KeyPress;
                tb.KeyPress += NumberTextBox_KeyPress;
            }
            else if (e.Control is TextBox tb2)
            {
                tb2.KeyPress -= NumberTextBox_KeyPress;
            }
        }

        private void NumberTextBox_KeyPress(object? sender, KeyPressEventArgs e)
        {
            // 숫자/백스페이스/마이너스/소수점만 허용
            if (char.IsControl(e.KeyChar)) return;
            if (char.IsDigit(e.KeyChar)) return;
            if (e.KeyChar == '-' || e.KeyChar == '.') return;
            e.Handled = true;
        }

        // DATE/TIMESTAMP: 더블클릭하면 DateTimePicker로 입력
        private void Grid_CellDoubleClick_DatePicker(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            var colName = grid.Columns[e.ColumnIndex].DataPropertyName;
            if (string.IsNullOrEmpty(colName) || !_metaByCol.TryGetValue(colName, out var meta)) return;
            if (!meta.IsDateLike) return;

            var rect = grid.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, true);

            var dtp = new DateTimePicker
            {
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "yyyy-MM-dd HH:mm:ss",
                Visible = true
            };

            // 현재값 반영
            var cur = grid.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
            if (cur is DateTime d) dtp.Value = d;
            else dtp.Value = DateTime.Now;

            dtp.CloseUp += (_, __) =>
            {
                grid.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = dtp.Value;
                dtp.Visible = false;
                dtp.Dispose();
            };

            dtp.LostFocus += (_, __) =>
            {
                dtp.Visible = false;
                dtp.Dispose();
            };

            grid.Controls.Add(dtp);
            dtp.Bounds = rect;
            dtp.BringToFront();
            dtp.Focus();
        }
        #endregion

        #region 프리뷰
        private void ShowSqlPreview(string text)
        {
            var f = new Form
            {
                Text = "Commit Preview (SQL + Params)",
                Width = 1000,
                Height = 700,
                StartPosition = FormStartPosition.CenterParent
            };

            var tb = new TextBox
            {
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Both,
                Dock = DockStyle.Fill,
                Font = new System.Drawing.Font("Consolas", 10),
                Text = text
            };

            f.Controls.Add(tb);
            f.ShowDialog(this);
        }

        private string BuildSqlPreviewText(int maxRowsPerType = 200, int clobPreviewChars = 200)
        {
            var insRows = _table.Rows.Cast<DataRow>().Where(r => r.RowState == DataRowState.Added).Take(maxRowsPerType).ToList();
            var updRows = _table.Rows.Cast<DataRow>().Where(r => r.RowState == DataRowState.Modified).Take(maxRowsPerType).ToList();
            var delRows = _table.Rows.Cast<DataRow>().Where(r => r.RowState == DataRowState.Deleted).Take(maxRowsPerType).ToList();

            var sb = new System.Text.StringBuilder();

            sb.AppendLine($"TABLE: {_tableName}");
            sb.AppendLine($"TIME : {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            sb.AppendLine();

            // DELETE
            sb.AppendLine($"--- DELETE ({delRows.Count}{(delRows.Count == maxRowsPerType ? "+" : "")}) ---");
            foreach (var r in delRows)
            {
                var rowid = SafeGetRowIdOriginal(r);
                sb.AppendLine($"DELETE FROM {_tableName} WHERE ROWID = :rid;");
                sb.AppendLine($"  :rid = {rowid ?? "<null>"}");
                sb.AppendLine();
            }

            // UPDATE
            sb.AppendLine($"--- UPDATE ({updRows.Count}{(updRows.Count == maxRowsPerType ? "+" : "")}) ---");
            foreach (var r in updRows)
            {
                var cols = GetEditableColumnsForUpdate(_table);
                var setClauses = cols.Select(c => $"{c} = :p_{c}");
                sb.AppendLine($"UPDATE {_tableName} SET {string.Join(", ", setClauses)} WHERE ROWID = :rid;");
                foreach (var c in cols)
                    sb.AppendLine($"  :p_{c} = {FormatPreviewValue(c, r[c], clobPreviewChars)}");
                sb.AppendLine($"  :rid  = {r["__ROWID"]}");
                sb.AppendLine();
            }

            // INSERT
            sb.AppendLine($"--- INSERT ({insRows.Count}{(insRows.Count == maxRowsPerType ? "+" : "")}) ---");
            foreach (var r in insRows)
            {
                var cols = GetInsertColumns(r);

                var colNames = new List<string>();
                var valExprs = new List<string>();

                foreach (var c in cols)
                {
                    colNames.Add(c);

                    // 시퀀스면 NEXTVAL로 표시
                    if (TryGetSequenceName(_tableName, c, out var seq) && IsEmpty(r[c]))
                    {
                        valExprs.Add($"{seq}.NEXTVAL");
                        continue;
                    }

                    valExprs.Add($":p_{c}");
                }

                sb.AppendLine($"INSERT INTO {_tableName} ({string.Join(", ", colNames)}) VALUES ({string.Join(", ", valExprs)});");

                foreach (var c in cols)
                {
                    if (TryGetSequenceName(_tableName, c, out var seq) && IsEmpty(r[c]))
                        continue;

                    sb.AppendLine($"  :p_{c} = {FormatPreviewValue(c, r[c], clobPreviewChars)}");
                }
                sb.AppendLine();
            }

            // 더 많은 변경이 있으면 안내
            int totalIns = _table.Rows.Cast<DataRow>().Count(r => r.RowState == DataRowState.Added);
            int totalUpd = _table.Rows.Cast<DataRow>().Count(r => r.RowState == DataRowState.Modified);
            int totalDel = _table.Rows.Cast<DataRow>().Count(r => r.RowState == DataRowState.Deleted);

            if (totalIns > maxRowsPerType || totalUpd > maxRowsPerType || totalDel > maxRowsPerType)
            {
                sb.AppendLine($"(미리보기 제한: 타입별 최대 {maxRowsPerType}행만 표시됨)");
                sb.AppendLine($"실제 변경: INSERT {totalIns}, UPDATE {totalUpd}, DELETE {totalDel}");
            }

            return sb.ToString();
        }

        private (int insert, int update, int delete) GetChangeSummary()
        {
            int ins = 0, upd = 0, del = 0;

            foreach (DataRow r in _table.Rows)
            {
                switch (r.RowState)
                {
                    case DataRowState.Added:
                        ins++;
                        break;

                    case DataRowState.Modified:
                        upd++;
                        break;

                    case DataRowState.Deleted:
                        del++;
                        break;
                }
            }

            return (ins, upd, del);
        }


        private string? SafeGetRowIdOriginal(DataRow deletedRow)
        {
            try
            {
                if (deletedRow.Table.Columns.Contains("__ROWID"))
                {
                    var v = deletedRow["__ROWID", DataRowVersion.Original];
                    return v == DBNull.Value ? null : v?.ToString();
                }
            }
            catch { }
            return null;
        }

        private string FormatPreviewValue(string colName, object val, int clobPreviewChars)
        {
            if (val == null || val == DBNull.Value) return "NULL";

            // 메타 있으면 타입에 따라 보기 좋게
            if (_metaByCol.TryGetValue(colName, out var meta))
            {
                if (meta.IsLob)
                {
                    var s = val.ToString() ?? "";
                    if (s.Length > clobPreviewChars) s = s.Substring(0, clobPreviewChars) + $"... (len={val.ToString()!.Length})";
                    // 줄바꿈 보기 좋게
                    s = s.Replace("\r", "\\r").Replace("\n", "\\n");
                    return $"<CLOB> \"{s}\"";
                }
                if (meta.IsDateLike && val is DateTime dt)
                    return $"TO_DATE('{dt:yyyy-MM-dd HH:mm:ss}','YYYY-MM-DD HH24:MI:SS')";
                if (meta.IsNumber)
                    return val.ToString()!;
            }

            // 기본: 문자열은 따옴표로
            var str = val.ToString() ?? "";
            str = str.Replace("\r", "\\r").Replace("\n", "\\n");
            return $"\"{str}\"";
        }
        #endregion

        #region 저장: CRUD
        private void SaveAllChanges()
        {
            if (_table == null) return;

            try
            {
                var (ins, upd, del) = GetChangeSummary();
                if (ins == 0 && upd == 0 && del == 0)
                {
                    MessageBox.Show("변경된 내용이 없습니다.");
                    return;
                }

                // 1) 미리보기 생성 + 표시
                var preview = BuildSqlPreviewText(maxRowsPerType: 50, clobPreviewChars: 200);
                ShowSqlPreview(preview);

                // 2) 사용자 최종 확인
                var msg = $"INSERT {ins}, UPDATE {upd}, DELETE {del}\nDB에 반영(Commit)할까요?";
                if (MessageBox.Show(msg, "Commit 확인", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                    return;

                // 3) 여기부터 실제 트랜잭션/쿼리 실행 후 Commit
                grid.EndEdit();
                var cm = (CurrencyManager)BindingContext[grid.DataSource];
                cm?.EndCurrentEdit();

                ValidateBeforeSave(); // ✅ 저장 전 전체 검증

                using var con = new OracleConnection(_connStr);
                con.Open();
                using var tx = con.BeginTransaction();

                // 삭제 -> 수정 -> 추가
                foreach (DataRow r in _table.Rows.Cast<DataRow>().Where(r => r.RowState == DataRowState.Deleted))
                    DeleteRow(con, tx, r);

                foreach (DataRow r in _table.Rows.Cast<DataRow>().Where(r => r.RowState == DataRowState.Modified))
                    UpdateRow(con, tx, r);

                foreach (DataRow r in _table.Rows.Cast<DataRow>().Where(r => r.RowState == DataRowState.Added))
                    InsertRow(con, tx, r);

                tx.Commit();
                _table.AcceptChanges();
                MessageBox.Show("저장 완료");
            }
            catch (Exception ex)
            {
                MessageBox.Show("저장 실패: " + ex.Message);
            }
        }

        private void ValidateBeforeSave()
        {
            // Added/Modified 행에 대해 NOT NULL/길이/숫자 등 한번 더 체크
            foreach (DataRow row in _table.Rows)
            {
                if (row.RowState is not (DataRowState.Added or DataRowState.Modified)) continue;

                foreach (var meta in _metaByCol.Values)
                {
                    if (meta.Name.Equals("__ROWID", StringComparison.OrdinalIgnoreCase)) continue;
                    if (meta.IsLob) continue;

                    var val = row.Table.Columns.Contains(meta.Name) ? row[meta.Name] : DBNull.Value;
                    var text = val == DBNull.Value ? "" : val?.ToString()?.Trim() ?? "";

                    // NOT NULL
                    if (!meta.IsNullable)
                    {
                        bool autoSeq = TryGetSequenceName(_tableName, meta.Name, out _);
                        bool hasDefault = !string.IsNullOrWhiteSpace(meta.DefaultSql);

                        if ((val == DBNull.Value || string.IsNullOrEmpty(text)) && !(autoSeq || hasDefault))
                            throw new InvalidOperationException($"필수 컬럼 누락: {meta.Name}");
                    }

                    // 길이
                    if (meta?.CharLength.Value > 0)     
                        if (meta.CharLength.HasValue && text.Length > meta.CharLength.Value)
                            throw new InvalidOperationException($"길이 초과: {meta.Name} (최대 {meta.CharLength.Value})");

                    // 숫자
                    if (meta.IsNumber && !string.IsNullOrEmpty(text) && !decimal.TryParse(text, out _))
                        throw new InvalidOperationException($"숫자 형식 오류: {meta.Name}");
                }
            }
        }

        private void DeleteRow(OracleConnection con, OracleTransaction tx, DataRow deletedRow)
        {
            // ✅ 새로 추가됐다가(Added) 저장 전에 삭제된 행은 DB에 없으니 DELETE할 필요 없음
            // 이 케이스는 Original ROWID 자체가 없음.
            object ridObj = null;

            // Deleted 상태에서는 Current 접근 불가 → Original로 시도
            if (deletedRow.Table.Columns.Contains("__ROWID"))
            {
                try { ridObj = deletedRow["__ROWID", DataRowVersion.Original]; }
                catch { ridObj = null; }
            }

            var rowid = ridObj == null || ridObj == DBNull.Value ? null : ridObj.ToString();

            if (string.IsNullOrWhiteSpace(rowid))
            {
                // DB에 존재하지 않던 행(또는 ROWID를 못 가져온 행) → DB DELETE 스킵
                return;
            }

            using var cmd = new OracleCommand($"DELETE FROM {_tableName} WHERE ROWID = :rid", con);
            cmd.Transaction = tx;
            cmd.Parameters.Add(new OracleParameter("rid", rowid));
            cmd.ExecuteNonQuery();
        }

        private void UpdateRow(OracleConnection con, OracleTransaction tx, DataRow row)
        {
            var rowid = row["__ROWID"]?.ToString();
            if (string.IsNullOrWhiteSpace(rowid))
                throw new InvalidOperationException("ROWID가 없어 수정할 수 없습니다.");

            var cols = GetEditableColumnsForUpdate(row.Table);

            var setClauses = cols.Select(c => $"{c} = :p_{c}").ToArray();
            var sql = $"UPDATE {_tableName} SET {string.Join(", ", setClauses)} WHERE ROWID = :rid";

            using var cmd = new OracleCommand(sql, con);
            cmd.Transaction = tx;

            foreach (var c in cols)
            {
                if (_metaByCol.TryGetValue(c, out var meta) && meta.IsLob)
                {
                    cmd.Parameters.Add(
                        $"p_{c}",
                        OracleDbType.Clob
                    ).Value = NormalizeValue(row[c]);
                }
                else
                {
                    cmd.Parameters.Add(
                        $"p_{c}",
                        NormalizeValue(row[c])
                    );
                }
            }

            cmd.Parameters.Add(new OracleParameter("rid", rowid));
            cmd.ExecuteNonQuery();
        }

        private void InsertRow(OracleConnection con, OracleTransaction tx, DataRow row)
        {
            // Insert는 "DB DEFAULT/시퀀스"를 고려해서
            // 값이 비어있고 DEFAULT/SEQ가 있으면 컬럼을 INSERT 목록에서 제외하거나 SEQ.NEXTVAL을 넣는다.
            var cols = GetInsertColumns(row);

            var colNames = new List<string>();
            var valuesSql = new List<string>();
            var parameters = new List<OracleParameter>();

            foreach (var c in cols)
            {
                colNames.Add(c);

                // 시퀀스 자동
                if (TryGetSequenceName(_tableName, c, out var seqName) && IsEmpty(row[c]))
                {
                    valuesSql.Add($"{seqName}.NEXTVAL");
                    continue;
                }

                // DEFAULT가 있고 값 비어있으면 -> INSERT에서 제외되게 여기서 skip 처리 가능
                // 하지만 cols 단계에서 이미 제외시키는 방식이 더 깔끔해서 여기서는 파라미터만 처리

                var pName = $"p_{c}";
                valuesSql.Add($":{pName}");

                if (_metaByCol.TryGetValue(c, out var meta) && meta.IsLob)
                {
                    parameters.Add(new OracleParameter(pName, OracleDbType.Clob)
                    {
                        Value = NormalizeValue(row[c])
                    });
                }
                else
                {
                    parameters.Add(new OracleParameter(pName, NormalizeValue(row[c])));
                }
            }

            var sql = $"INSERT INTO {_tableName} ({string.Join(", ", colNames)}) VALUES ({string.Join(", ", valuesSql)})";

            using var cmd = new OracleCommand(sql, con);
            cmd.Transaction = tx;
            cmd.Parameters.AddRange(parameters.ToArray());
            cmd.ExecuteNonQuery();
        }

        private List<string> GetEditableColumnsForUpdate(DataTable dt)
        {
            return dt.Columns.Cast<DataColumn>()
                .Select(c => c.ColumnName)
                .Where(n => !n.Equals("__ROWID", StringComparison.OrdinalIgnoreCase))
                .Where(n => _metaByCol.TryGetValue(n, out var m))
                .ToList();
        }

        private List<string> GetInsertColumns(DataRow row)
        {
            var cols = new List<string>();

            foreach (DataColumn dc in row.Table.Columns)
            {
                var name = dc.ColumnName;
                if (name.Equals("__ROWID", StringComparison.OrdinalIgnoreCase)) continue;
                if (!_metaByCol.TryGetValue(name, out var meta)) continue;
                if (meta.IsLob) continue; // LOB 제외

                var val = row[name];

                // 값이 비어있고 DEFAULT가 있으면 -> INSERT 컬럼에서 제외(=DB 기본값 적용)
                // 단, 시퀀스 매핑이 있으면 DEFAULT보다 시퀀스가 우선(다루기 쉬움)
                bool hasSeq = TryGetSequenceName(_tableName, name, out _);
                bool hasDefault = !string.IsNullOrWhiteSpace(meta.DefaultSql);

                if (IsEmpty(val) && hasDefault && !hasSeq)
                    continue;

                cols.Add(name);
            }

            return cols;
        }

        private bool TryGetSequenceName(string tableName, string colName, out string seqName)
        {
            seqName = "";
            if (_sequenceMap.TryGetValue(tableName, out var map) && map.TryGetValue(colName, out var seq))
            {
                seqName = seq;
                return true;
            }
            return false;
        }

        private static bool IsEmpty(object? val)
        {
            if (val == null || val == DBNull.Value) return true;
            var s = val.ToString()?.Trim();
            return string.IsNullOrEmpty(s);
        }

        private static object NormalizeValue(object val)
        {
            // 그리드가 빈 문자열을 주는 경우 DBNull로 변환(오라클에 더 안정적)
            if (val == null) return DBNull.Value;
            if (val == DBNull.Value) return DBNull.Value;
            if (val is string s && string.IsNullOrWhiteSpace(s)) return DBNull.Value;
            return val;
        }
        #endregion

        #region 클립보드 붙여넣기
        private void Grid_KeyDown_Paste(object? sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.V)
            {
                PasteFromClipboard();
                e.Handled = true;
            }
        }

        private void PasteFromClipboard()
        {
            if (grid.CurrentCell == null) return;
            if (!Clipboard.ContainsText()) return;

            string text = Clipboard.GetText();
            if (string.IsNullOrWhiteSpace(text)) return;

            var startRow = grid.CurrentCell.RowIndex;
            var startCol = grid.CurrentCell.ColumnIndex;

            var lines = text
                .Split(new[] { "\r\n", "\n" }, StringSplitOptions.None)
                .Where(l => !string.IsNullOrWhiteSpace(l))
                .ToArray();

            int rowIndex = startRow;

            foreach (var line in lines)
            {
                // 행이 부족하면 새 행 추가 (INSERT 지원)
                if (rowIndex >= grid.Rows.Count)
                    _table.Rows.Add(_table.NewRow());

                var cells = line.Split('\t');
                int colIndex = startCol;

                for (int i = 0; i < cells.Length && colIndex < grid.Columns.Count; i++, colIndex++)
                {
                    var col = grid.Columns[colIndex];

                    // ReadOnly / LOB / ROWID 스킵
                    if (col.ReadOnly) continue;
                    if (col.DataPropertyName.Equals("__ROWID", StringComparison.OrdinalIgnoreCase))
                        continue;

                    if (!_metaByCol.TryGetValue(col.DataPropertyName, out var meta))
                        continue;

                    if (meta.IsLob) continue;

                    var value = cells[i]?.Trim();

                    // 빈 문자열 → DBNull
                    if (string.IsNullOrEmpty(value))
                    {
                        grid.Rows[rowIndex].Cells[colIndex].Value = DBNull.Value;
                    }
                    else
                    {
                        // 숫자
                        if (meta.IsNumber)
                        {
                            if (decimal.TryParse(value, out var d))
                                grid.Rows[rowIndex].Cells[colIndex].Value = d;
                        }
                        // 날짜
                        else if (meta.IsDateLike)
                        {
                            if (DateTime.TryParse(value, out var dt))
                                grid.Rows[rowIndex].Cells[colIndex].Value = dt;
                        }
                        else
                        {
                            // 문자열
                            grid.Rows[rowIndex].Cells[colIndex].Value = value;
                        }
                    }
                }

                rowIndex++;
            }

        }
        #endregion


        private void OracleCrudGridForm_Load(object sender, EventArgs e)
        {
            var cfg = ConfigStore.Load();
            if (cfg == null)
            {
                using var f = new ConnectionForm();
                if (f.ShowDialog(this) != DialogResult.OK)
                {
                    Close(); // 사용자가 취소하면 종료
                    return;
                }
                cfg = f.ResultConfig;
                ConfigStore.Save(cfg);
            }

            _connStr = ConfigStore.BuildConnStr(cfg); 
            LoadTableList();
        }
    }

}
