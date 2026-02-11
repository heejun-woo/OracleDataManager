namespace OracleDataManager
{
    partial class ConnectionEditForm
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
            label3 = new Label();
            label2 = new Label();
            label1 = new Label();
            btnSave = new Button();
            btnTest = new Button();
            txtPw = new TextBox();
            txtDataSource = new TextBox();
            txtUser = new TextBox();
            chkAskPwEveryTime = new CheckBox();
            chkSavePassword = new CheckBox();
            chkIsProduction = new CheckBox();
            btnOk = new Button();
            btnCancel = new Button();
            lblTestStatus = new Label();
            label4 = new Label();
            txtName = new TextBox();
            SuspendLayout();
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(12, 138);
            label3.Name = "label3";
            label3.Size = new Size(32, 15);
            label3.TabIndex = 15;
            label3.Text = "PW :";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(12, 107);
            label2.Name = "label2";
            label2.Size = new Size(26, 15);
            label2.TabIndex = 14;
            label2.Text = "ID :";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 12);
            label1.Name = "label1";
            label1.Size = new Size(76, 15);
            label1.TabIndex = 13;
            label1.Text = "DataSource :";
            // 
            // btnSave
            // 
            btnSave.Location = new Point(173, 235);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(75, 23);
            btnSave.TabIndex = 12;
            btnSave.Text = "Save";
            btnSave.UseVisualStyleBackColor = true;
            // 
            // btnTest
            // 
            btnTest.Cursor = Cursors.No;
            btnTest.Location = new Point(92, 235);
            btnTest.Name = "btnTest";
            btnTest.Size = new Size(75, 23);
            btnTest.TabIndex = 11;
            btnTest.Text = "Test";
            btnTest.TextAlign = ContentAlignment.TopCenter;
            btnTest.UseVisualStyleBackColor = true;
            // 
            // txtPw
            // 
            txtPw.Location = new Point(62, 135);
            txtPw.Name = "txtPw";
            txtPw.PasswordChar = '*';
            txtPw.Size = new Size(178, 23);
            txtPw.TabIndex = 10;
            // 
            // txtDataSource
            // 
            txtDataSource.Location = new Point(12, 30);
            txtDataSource.Multiline = true;
            txtDataSource.Name = "txtDataSource";
            txtDataSource.Size = new Size(407, 68);
            txtDataSource.TabIndex = 9;
            txtDataSource.Text = "127.0.0.1:1521/XEPDB1";
            // 
            // txtUser
            // 
            txtUser.Location = new Point(62, 104);
            txtUser.Name = "txtUser";
            txtUser.Size = new Size(178, 23);
            txtUser.TabIndex = 8;
            // 
            // chkAskPwEveryTime
            // 
            chkAskPwEveryTime.AutoSize = true;
            chkAskPwEveryTime.Location = new Point(262, 108);
            chkAskPwEveryTime.Name = "chkAskPwEveryTime";
            chkAskPwEveryTime.Size = new Size(157, 19);
            chkAskPwEveryTime.TabIndex = 16;
            chkAskPwEveryTime.Text = "Ask Password EveryTime";
            chkAskPwEveryTime.UseVisualStyleBackColor = true;
            // 
            // chkSavePassword
            // 
            chkSavePassword.AutoSize = true;
            chkSavePassword.Location = new Point(262, 139);
            chkSavePassword.Name = "chkSavePassword";
            chkSavePassword.Size = new Size(105, 19);
            chkSavePassword.TabIndex = 17;
            chkSavePassword.Text = "Save Password";
            chkSavePassword.UseVisualStyleBackColor = true;
            // 
            // chkIsProduction
            // 
            chkIsProduction.AutoSize = true;
            chkIsProduction.Location = new Point(262, 168);
            chkIsProduction.Name = "chkIsProduction";
            chkIsProduction.Size = new Size(97, 19);
            chkIsProduction.TabIndex = 18;
            chkIsProduction.Text = "Is Production";
            chkIsProduction.UseVisualStyleBackColor = true;
            // 
            // btnOk
            // 
            btnOk.Cursor = Cursors.No;
            btnOk.Location = new Point(254, 235);
            btnOk.Name = "btnOk";
            btnOk.Size = new Size(75, 23);
            btnOk.TabIndex = 19;
            btnOk.Text = "OK";
            btnOk.TextAlign = ContentAlignment.TopCenter;
            btnOk.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            btnCancel.Cursor = Cursors.No;
            btnCancel.Location = new Point(335, 235);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(75, 23);
            btnCancel.TabIndex = 20;
            btnCancel.Text = "Cancel";
            btnCancel.TextAlign = ContentAlignment.TopCenter;
            btnCancel.UseVisualStyleBackColor = true;
            // 
            // lblTestStatus
            // 
            lblTestStatus.AutoSize = true;
            lblTestStatus.Font = new Font("휴먼둥근헤드라인", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 129);
            lblTestStatus.Location = new Point(12, 205);
            lblTestStatus.Name = "lblTestStatus";
            lblTestStatus.Size = new Size(128, 16);
            lblTestStatus.TabIndex = 21;
            lblTestStatus.Text = "Test Status";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(12, 167);
            label4.Name = "label4";
            label4.Size = new Size(50, 15);
            label4.TabIndex = 23;
            label4.Text = "Name : ";
            // 
            // txtName
            // 
            txtName.Location = new Point(62, 164);
            txtName.Name = "txtName";
            txtName.Size = new Size(178, 23);
            txtName.TabIndex = 22;
            // 
            // ConnectionEditForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(421, 266);
            Controls.Add(label4);
            Controls.Add(txtName);
            Controls.Add(lblTestStatus);
            Controls.Add(btnCancel);
            Controls.Add(btnOk);
            Controls.Add(chkIsProduction);
            Controls.Add(chkSavePassword);
            Controls.Add(chkAskPwEveryTime);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(btnSave);
            Controls.Add(btnTest);
            Controls.Add(txtPw);
            Controls.Add(txtDataSource);
            Controls.Add(txtUser);
            FormBorderStyle = FormBorderStyle.SizableToolWindow;
            Name = "ConnectionEditForm";
            Text = "ConnectionEditForm";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label3;
        private Label label2;
        private Label label1;
        private Button btnSave;
        private Button btnTest;
        private TextBox txtPw;
        private TextBox txtDataSource;
        private TextBox txtUser;
        private CheckBox chkAskPwEveryTime;
        private CheckBox chkSavePassword;
        private CheckBox chkIsProduction;
        private Button btnOk;
        private Button btnCancel;
        private Label lblTestStatus;
        private Label label4;
        private TextBox txtName;
    }
}