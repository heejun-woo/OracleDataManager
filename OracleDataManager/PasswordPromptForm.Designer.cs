namespace OracleDataManager
{
    partial class PasswordPromptForm
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
            lblTitle = new Label();
            txtPw = new TextBox();
            btnOk = new Button();
            btnCancel = new Button();
            SuspendLayout();
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(12, 17);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(39, 15);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "label1";
            // 
            // txtPw
            // 
            txtPw.Location = new Point(117, 14);
            txtPw.Name = "txtPw";
            txtPw.Size = new Size(200, 23);
            txtPw.TabIndex = 1;
            // 
            // btnOk
            // 
            btnOk.Location = new Point(161, 43);
            btnOk.Name = "btnOk";
            btnOk.Size = new Size(75, 23);
            btnOk.TabIndex = 2;
            btnOk.Text = "OK";
            btnOk.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            btnCancel.Location = new Point(242, 43);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(75, 23);
            btnCancel.TabIndex = 3;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = true;
            // 
            // PasswordPromptForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(327, 74);
            Controls.Add(btnCancel);
            Controls.Add(btnOk);
            Controls.Add(txtPw);
            Controls.Add(lblTitle);
            FormBorderStyle = FormBorderStyle.SizableToolWindow;
            Name = "PasswordPromptForm";
            Text = "PasswordPromptForm";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblTitle;
        private TextBox txtPw;
        private Button btnOk;
        private Button btnCancel;
    }
}