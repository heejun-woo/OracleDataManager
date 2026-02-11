namespace OracleDataManager
{
    partial class ConnectionSelectForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            gridProfiles = new Zuby.ADGV.AdvancedDataGridView();
            btnConnect = new Button();
            btnAdd = new Button();
            btnEdit = new Button();
            btnDelete = new Button();
            btnClose = new Button();
            ((System.ComponentModel.ISupportInitialize)gridProfiles).BeginInit();
            SuspendLayout();
            // 
            // gridProfiles
            // 
            gridProfiles.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            gridProfiles.FilterAndSortEnabled = true;
            gridProfiles.FilterStringChangedInvokeBeforeDatasourceUpdate = true;
            gridProfiles.Location = new Point(3, 2);
            gridProfiles.MaxFilterButtonImageHeight = 23;
            gridProfiles.Name = "gridProfiles";
            gridProfiles.RightToLeft = RightToLeft.No;
            gridProfiles.Size = new Size(1218, 290);
            gridProfiles.SortStringChangedInvokeBeforeDatasourceUpdate = true;
            gridProfiles.TabIndex = 0;
            // 
            // btnConnect
            // 
            btnConnect.Location = new Point(811, 298);
            btnConnect.Name = "btnConnect";
            btnConnect.Size = new Size(75, 23);
            btnConnect.TabIndex = 1;
            btnConnect.Text = "Connect";
            btnConnect.UseVisualStyleBackColor = true;
            // 
            // btnAdd
            // 
            btnAdd.Location = new Point(892, 298);
            btnAdd.Name = "btnAdd";
            btnAdd.Size = new Size(75, 23);
            btnAdd.TabIndex = 2;
            btnAdd.Text = "Add";
            btnAdd.UseVisualStyleBackColor = true;
            // 
            // btnEdit
            // 
            btnEdit.Location = new Point(973, 298);
            btnEdit.Name = "btnEdit";
            btnEdit.Size = new Size(75, 23);
            btnEdit.TabIndex = 3;
            btnEdit.Text = "Edit";
            btnEdit.UseVisualStyleBackColor = true;
            // 
            // btnDelete
            // 
            btnDelete.Location = new Point(1054, 298);
            btnDelete.Name = "btnDelete";
            btnDelete.Size = new Size(75, 23);
            btnDelete.TabIndex = 4;
            btnDelete.Text = "Delete";
            btnDelete.UseVisualStyleBackColor = true;
            // 
            // btnClose
            // 
            btnClose.Location = new Point(1135, 298);
            btnClose.Name = "btnClose";
            btnClose.Size = new Size(75, 23);
            btnClose.TabIndex = 5;
            btnClose.Text = "Close";
            btnClose.UseVisualStyleBackColor = true;
            // 
            // ConnectionSelectForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1226, 325);
            Controls.Add(btnClose);
            Controls.Add(btnDelete);
            Controls.Add(btnEdit);
            Controls.Add(btnAdd);
            Controls.Add(btnConnect);
            Controls.Add(gridProfiles);
            FormBorderStyle = FormBorderStyle.SizableToolWindow;
            Name = "ConnectionSelectForm";
            Text = "ConnectionSelectForm";
            ((System.ComponentModel.ISupportInitialize)gridProfiles).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Zuby.ADGV.AdvancedDataGridView gridProfiles;
        private Button btnConnect;
        private Button btnAdd;
        private Button btnEdit;
        private Button btnDelete;
        private Button btnClose;
    }
}