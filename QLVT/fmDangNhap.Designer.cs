using System.Windows.Forms;

namespace QLVT
{
    partial class fmDangNhap
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
            this.components = new System.ComponentModel.Container();
            this.defaultLookAndFeel1 = new DevExpress.LookAndFeel.DefaultLookAndFeel(this.components);
            this.textTenDN = new System.Windows.Forms.TextBox();
            this.textMatKhau = new System.Windows.Forms.TextBox();
            this.labelDangNhap = new System.Windows.Forms.Label();
            this.labelTenDN = new System.Windows.Forms.Label();
            this.labelMatKhau = new System.Windows.Forms.Label();
            this.btnDangNhap = new System.Windows.Forms.Button();
            this.btnThoat = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // textTenDN
            // 
            this.textTenDN.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold);
            this.textTenDN.Location = new System.Drawing.Point(558, 271);
            this.textTenDN.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.textTenDN.Name = "textTenDN";
            this.textTenDN.Size = new System.Drawing.Size(402, 46);
            this.textTenDN.TabIndex = 0;
            // 
            // textMatKhau
            // 
            this.textMatKhau.CausesValidation = false;
            this.textMatKhau.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold);
            this.textMatKhau.Location = new System.Drawing.Point(558, 343);
            this.textMatKhau.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.textMatKhau.Name = "textMatKhau";
            this.textMatKhau.PasswordChar = '*';
            this.textMatKhau.Size = new System.Drawing.Size(406, 46);
            this.textMatKhau.TabIndex = 1;
            // 
            // labelDangNhap
            // 
            this.labelDangNhap.AutoSize = true;
            this.labelDangNhap.Font = new System.Drawing.Font("Tahoma", 16.125F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelDangNhap.ForeColor = System.Drawing.Color.Firebrick;
            this.labelDangNhap.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.labelDangNhap.Location = new System.Drawing.Point(494, 166);
            this.labelDangNhap.Name = "labelDangNhap";
            this.labelDangNhap.Size = new System.Drawing.Size(263, 52);
            this.labelDangNhap.TabIndex = 2;
            this.labelDangNhap.Text = "Đăng Nhập";
            this.labelDangNhap.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // labelTenDN
            // 
            this.labelTenDN.AutoSize = true;
            this.labelTenDN.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelTenDN.Location = new System.Drawing.Point(302, 276);
            this.labelTenDN.Name = "labelTenDN";
            this.labelTenDN.Size = new System.Drawing.Size(252, 39);
            this.labelTenDN.TabIndex = 3;
            this.labelTenDN.Text = "Tên đăng nhập";
            // 
            // labelMatKhau
            // 
            this.labelMatKhau.AutoSize = true;
            this.labelMatKhau.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold);
            this.labelMatKhau.Location = new System.Drawing.Point(306, 348);
            this.labelMatKhau.Name = "labelMatKhau";
            this.labelMatKhau.Size = new System.Drawing.Size(165, 39);
            this.labelMatKhau.TabIndex = 4;
            this.labelMatKhau.Text = "Mật khẩu";
            // 
            // btnDangNhap
            // 
            this.btnDangNhap.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDangNhap.Location = new System.Drawing.Point(386, 438);
            this.btnDangNhap.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnDangNhap.Name = "btnDangNhap";
            this.btnDangNhap.Size = new System.Drawing.Size(212, 48);
            this.btnDangNhap.TabIndex = 6;
            this.btnDangNhap.Text = "Đăng Nhập";
            this.btnDangNhap.UseVisualStyleBackColor = true;
            this.btnDangNhap.Click += new System.EventHandler(this.btnDangNhap_Click);
            // 
            // btnThoat
            // 
            this.btnThoat.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnThoat.Location = new System.Drawing.Point(666, 438);
            this.btnThoat.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnThoat.Name = "btnThoat";
            this.btnThoat.Size = new System.Drawing.Size(212, 48);
            this.btnThoat.TabIndex = 10;
            this.btnThoat.Text = "Thoát";
            this.btnThoat.UseVisualStyleBackColor = true;
            this.btnThoat.Click += new System.EventHandler(this.btnThoat_Click);
            // 
            // fmDangNhap
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1319, 688);
            this.Controls.Add(this.btnThoat);
            this.Controls.Add(this.btnDangNhap);
            this.Controls.Add(this.labelMatKhau);
            this.Controls.Add(this.labelTenDN);
            this.Controls.Add(this.labelDangNhap);
            this.Controls.Add(this.textMatKhau);
            this.Controls.Add(this.textTenDN);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "fmDangNhap";
            this.Text = "fmDangNhap";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.LookAndFeel.DefaultLookAndFeel defaultLookAndFeel1;
        private System.Windows.Forms.TextBox textTenDN;
        private System.Windows.Forms.TextBox textMatKhau;
        private System.Windows.Forms.Label labelDangNhap;
        private System.Windows.Forms.Label labelTenDN;
        private System.Windows.Forms.Label labelMatKhau;
        private System.Windows.Forms.Button btnDangNhap;
        private System.Windows.Forms.Button btnThoat;
    }
}