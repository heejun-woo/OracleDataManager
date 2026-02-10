namespace OracleDataManager
{
    partial class ConnectionForm
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
            txtUser = new TextBox();
            txtDataSource = new TextBox();
            txtPw = new TextBox();
            btnSave = new Button();
            btnTest = new Button();
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            SuspendLayout();
            // 
            // txtUser
            // 
            txtUser.Location = new Point(75, 117);
            txtUser.Name = "txtUser";
            txtUser.Size = new Size(238, 23);
            txtUser.TabIndex = 0;
            // 
            // txtDataSource
            // 
            txtDataSource.Location = new Point(25, 43);
            txtDataSource.Multiline = true;
            txtDataSource.Name = "txtDataSource";
            txtDataSource.Size = new Size(329, 68);
            txtDataSource.TabIndex = 1;
            txtDataSource.Text = "127.0.0.1:1521/XEPDB1";
            // 
            // txtPw
            // 
            txtPw.Location = new Point(75, 148);
            txtPw.Name = "txtPw";
            txtPw.PasswordChar = '*';
            txtPw.Size = new Size(238, 23);
            txtPw.TabIndex = 2;
            // 
            // btnSave
            // 
            btnSave.Location = new Point(288, 191);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(75, 23);
            btnSave.TabIndex = 4;
            btnSave.Text = "Save";
            btnSave.UseVisualStyleBackColor = true;
            // 
            // btnTest
            // 
            btnTest.Cursor = Cursors.No;
            btnTest.Location = new Point(207, 191);
            btnTest.Name = "btnTest";
            btnTest.Size = new Size(75, 23);
            btnTest.TabIndex = 3;
            btnTest.Text = "Test";
            btnTest.TextAlign = ContentAlignment.TopCenter;
            btnTest.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(25, 25);
            label1.Name = "label1";
            label1.Size = new Size(76, 15);
            label1.TabIndex = 5;
            label1.Text = "DataSource :";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(25, 120);
            label2.Name = "label2";
            label2.Size = new Size(26, 15);
            label2.TabIndex = 6;
            label2.Text = "ID :";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(25, 151);
            label3.Name = "label3";
            label3.Size = new Size(32, 15);
            label3.TabIndex = 7;
            label3.Text = "PW :";
            // 
            // ConnectionForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(375, 226);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(btnSave);
            Controls.Add(btnTest);
            Controls.Add(txtPw);
            Controls.Add(txtDataSource);
            Controls.Add(txtUser);
            Name = "ConnectionForm";
            Text = "ConnectionForm";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox txtUser;
        private TextBox txtDataSource;
        private TextBox txtPw;
        private Button btnSave;
        private Button btnTest;
        private Label label1;
        private Label label2;
        private Label label3;
    }
}