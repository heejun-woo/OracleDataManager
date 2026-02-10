namespace OracleDataManager
{
    partial class OracleCrudGridForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            btnLoad = new Button();
            btnSave = new Button();
            cbTables = new ComboBox();
            splitContainer1 = new SplitContainer();
            grid = new Zuby.ADGV.AdvancedDataGridView();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)grid).BeginInit();
            SuspendLayout();
            // 
            // btnLoad
            // 
            btnLoad.Location = new Point(290, 12);
            btnLoad.Name = "btnLoad";
            btnLoad.Size = new Size(75, 23);
            btnLoad.TabIndex = 1;
            btnLoad.Text = "Select";
            btnLoad.UseVisualStyleBackColor = true;
            // 
            // btnSave
            // 
            btnSave.Location = new Point(371, 12);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(75, 23);
            btnSave.TabIndex = 2;
            btnSave.Text = "Save";
            btnSave.UseVisualStyleBackColor = true;
            // 
            // cbTables
            // 
            cbTables.FormattingEnabled = true;
            cbTables.Location = new Point(16, 12);
            cbTables.Name = "cbTables";
            cbTables.Size = new Size(264, 23);
            cbTables.TabIndex = 3;
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.FixedPanel = FixedPanel.Panel1;
            splitContainer1.IsSplitterFixed = true;
            splitContainer1.Location = new Point(0, 0);
            splitContainer1.Name = "splitContainer1";
            splitContainer1.Orientation = Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(btnLoad);
            splitContainer1.Panel1.Controls.Add(btnSave);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(grid);
            splitContainer1.Size = new Size(999, 593);
            splitContainer1.SplitterDistance = 49;
            splitContainer1.TabIndex = 4;
            // 
            // grid
            // 
            grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            grid.Dock = DockStyle.Fill;
            grid.FilterAndSortEnabled = true;
            grid.FilterStringChangedInvokeBeforeDatasourceUpdate = true;
            grid.Location = new Point(0, 0);
            grid.MaxFilterButtonImageHeight = 23;
            grid.Name = "grid";
            grid.RightToLeft = RightToLeft.No;
            grid.Size = new Size(999, 540);
            grid.SortStringChangedInvokeBeforeDatasourceUpdate = true;
            grid.TabIndex = 0;
            // 
            // OracleCrudGridForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(999, 593);
            Controls.Add(cbTables);
            Controls.Add(splitContainer1);
            Name = "OracleCrudGridForm";
            Text = "OracleDataManager";
            Load += OracleCrudGridForm_Load;
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)grid).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Zuby.ADGV.AdvancedDataGridView grid;
        private Button btnLoad;
        private Button btnSave;
        private ComboBox cbTables;
        private SplitContainer splitContainer1;
        private Zuby.ADGV.AdvancedDataGridViewSearchToolBar advancedDataGridViewSearchToolBar1;
    }
}
