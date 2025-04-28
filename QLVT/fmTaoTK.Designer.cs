namespace QLVT
{
    partial class fmTaoTK
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
            this.labelTaoTK = new System.Windows.Forms.Label();
            this.labelHoTenNV = new System.Windows.Forms.Label();
            this.labelTenDN = new System.Windows.Forms.Label();
            this.labelMK = new System.Windows.Forms.Label();
            this.labelNhom = new System.Windows.Forms.Label();
            this.txtHOTEN = new System.Windows.Forms.ComboBox();
            this.bdsLayDSNV = new System.Windows.Forms.BindingSource(this.components);
            this.qLVTDataSet = new QLVT.QLVTDataSet();
            this.txtMANV = new System.Windows.Forms.TextBox();
            this.txtTENDN = new System.Windows.Forms.TextBox();
            this.txtMK = new System.Windows.Forms.TextBox();
            this.radioBtnAd = new System.Windows.Forms.RadioButton();
            this.radioBtnNV = new System.Windows.Forms.RadioButton();
            this.btnTao = new System.Windows.Forms.Button();
            this.btnXoa = new System.Windows.Forms.Button();
            this.btnThoat = new System.Windows.Forms.Button();
            this.tableAdapterManager = new QLVT.QLVTDataSetTableAdapters.TableAdapterManager();
            this.txtXacNhanMK = new System.Windows.Forms.TextBox();
            this.labelXNMK = new System.Windows.Forms.Label();
            this.taLayDSNV = new QLVT.QLVTDataSetTableAdapters.LayDSNVTableAdapter();
            ((System.ComponentModel.ISupportInitialize)(this.bdsLayDSNV)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.qLVTDataSet)).BeginInit();
            this.SuspendLayout();
            // 
            // labelTaoTK
            // 
            this.labelTaoTK.AutoSize = true;
            this.labelTaoTK.Font = new System.Drawing.Font("Segoe UI", 12F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelTaoTK.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.labelTaoTK.Location = new System.Drawing.Point(366, 55);
            this.labelTaoTK.Name = "labelTaoTK";
            this.labelTaoTK.Size = new System.Drawing.Size(472, 45);
            this.labelTaoTK.TabIndex = 0;
            this.labelTaoTK.Text = "Tạo Tài Khoản Cho Nhân Viên";
            // 
            // labelHoTenNV
            // 
            this.labelHoTenNV.AutoSize = true;
            this.labelHoTenNV.Font = new System.Drawing.Font("Segoe UI", 10.125F, System.Drawing.FontStyle.Bold);
            this.labelHoTenNV.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.labelHoTenNV.Location = new System.Drawing.Point(123, 148);
            this.labelHoTenNV.Name = "labelHoTenNV";
            this.labelHoTenNV.Size = new System.Drawing.Size(246, 37);
            this.labelHoTenNV.TabIndex = 1;
            this.labelHoTenNV.Text = "Họ Tên Nhân Viên";
            // 
            // labelTenDN
            // 
            this.labelTenDN.AutoSize = true;
            this.labelTenDN.Font = new System.Drawing.Font("Segoe UI", 10.125F, System.Drawing.FontStyle.Bold);
            this.labelTenDN.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.labelTenDN.Location = new System.Drawing.Point(123, 215);
            this.labelTenDN.Name = "labelTenDN";
            this.labelTenDN.Size = new System.Drawing.Size(213, 37);
            this.labelTenDN.TabIndex = 2;
            this.labelTenDN.Text = "Tên Đăng Nhập";
            // 
            // labelMK
            // 
            this.labelMK.AutoSize = true;
            this.labelMK.Font = new System.Drawing.Font("Segoe UI", 10.125F, System.Drawing.FontStyle.Bold);
            this.labelMK.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.labelMK.Location = new System.Drawing.Point(123, 281);
            this.labelMK.Name = "labelMK";
            this.labelMK.Size = new System.Drawing.Size(141, 37);
            this.labelMK.TabIndex = 3;
            this.labelMK.Text = "Mật Khẩu";
            // 
            // labelNhom
            // 
            this.labelNhom.AutoSize = true;
            this.labelNhom.Font = new System.Drawing.Font("Segoe UI", 10.125F, System.Drawing.FontStyle.Bold);
            this.labelNhom.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.labelNhom.Location = new System.Drawing.Point(123, 419);
            this.labelNhom.Name = "labelNhom";
            this.labelNhom.Size = new System.Drawing.Size(96, 37);
            this.labelNhom.TabIndex = 4;
            this.labelNhom.Text = "Nhóm";
            // 
            // txtHOTEN
            // 
            this.txtHOTEN.DataSource = this.bdsLayDSNV;
            this.txtHOTEN.DisplayMember = "HOTEN";
            this.txtHOTEN.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.txtHOTEN.FormattingEnabled = true;
            this.txtHOTEN.Location = new System.Drawing.Point(403, 148);
            this.txtHOTEN.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtHOTEN.Name = "txtHOTEN";
            this.txtHOTEN.Size = new System.Drawing.Size(372, 33);
            this.txtHOTEN.TabIndex = 5;
            this.txtHOTEN.ValueMember = "MANV";
            this.txtHOTEN.SelectedIndexChanged += new System.EventHandler(this.txtHOTEN_SelectedIndexChanged);
            // 
            // bdsLayDSNV
            // 
            this.bdsLayDSNV.DataMember = "LayDSNV";
            this.bdsLayDSNV.DataSource = this.qLVTDataSet;
            // 
            // qLVTDataSet
            // 
            this.qLVTDataSet.DataSetName = "QLVTDataSet";
            this.qLVTDataSet.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // txtMANV
            // 
            this.txtMANV.Location = new System.Drawing.Point(810, 141);
            this.txtMANV.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtMANV.Name = "txtMANV";
            this.txtMANV.ReadOnly = true;
            this.txtMANV.Size = new System.Drawing.Size(77, 31);
            this.txtMANV.TabIndex = 6;
            // 
            // txtTENDN
            // 
            this.txtTENDN.Enabled = false;
            this.txtTENDN.Location = new System.Drawing.Point(403, 216);
            this.txtTENDN.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtTENDN.Name = "txtTENDN";
            this.txtTENDN.Size = new System.Drawing.Size(372, 31);
            this.txtTENDN.TabIndex = 7;
            // 
            // txtMK
            // 
            this.txtMK.Enabled = false;
            this.txtMK.Location = new System.Drawing.Point(403, 280);
            this.txtMK.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtMK.Name = "txtMK";
            this.txtMK.PasswordChar = '*';
            this.txtMK.Size = new System.Drawing.Size(372, 31);
            this.txtMK.TabIndex = 8;
            // 
            // radioBtnAd
            // 
            this.radioBtnAd.AutoSize = true;
            this.radioBtnAd.Enabled = false;
            this.radioBtnAd.Location = new System.Drawing.Point(405, 422);
            this.radioBtnAd.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.radioBtnAd.Name = "radioBtnAd";
            this.radioBtnAd.Size = new System.Drawing.Size(103, 29);
            this.radioBtnAd.TabIndex = 9;
            this.radioBtnAd.TabStop = true;
            this.radioBtnAd.Text = "Admin";
            this.radioBtnAd.UseVisualStyleBackColor = true;
            // 
            // radioBtnNV
            // 
            this.radioBtnNV.AutoSize = true;
            this.radioBtnNV.Enabled = false;
            this.radioBtnNV.Location = new System.Drawing.Point(607, 421);
            this.radioBtnNV.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.radioBtnNV.Name = "radioBtnNV";
            this.radioBtnNV.Size = new System.Drawing.Size(143, 29);
            this.radioBtnNV.TabIndex = 10;
            this.radioBtnNV.TabStop = true;
            this.radioBtnNV.Text = "Nhân Viên";
            this.radioBtnNV.UseVisualStyleBackColor = true;
            // 
            // btnTao
            // 
            this.btnTao.BackColor = System.Drawing.Color.SkyBlue;
            this.btnTao.Enabled = false;
            this.btnTao.Font = new System.Drawing.Font("Segoe UI", 10.125F, System.Drawing.FontStyle.Bold);
            this.btnTao.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.btnTao.Location = new System.Drawing.Point(123, 502);
            this.btnTao.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnTao.Name = "btnTao";
            this.btnTao.Size = new System.Drawing.Size(138, 50);
            this.btnTao.TabIndex = 11;
            this.btnTao.Text = "Tạo";
            this.btnTao.UseVisualStyleBackColor = false;
            this.btnTao.Click += new System.EventHandler(this.btnTao_Click);
            // 
            // btnXoa
            // 
            this.btnXoa.BackColor = System.Drawing.Color.SkyBlue;
            this.btnXoa.Enabled = false;
            this.btnXoa.Font = new System.Drawing.Font("Segoe UI", 10.125F, System.Drawing.FontStyle.Bold);
            this.btnXoa.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.btnXoa.Location = new System.Drawing.Point(378, 502);
            this.btnXoa.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnXoa.Name = "btnXoa";
            this.btnXoa.Size = new System.Drawing.Size(138, 50);
            this.btnXoa.TabIndex = 12;
            this.btnXoa.Text = "Xóa";
            this.btnXoa.UseVisualStyleBackColor = false;
            this.btnXoa.Click += new System.EventHandler(this.btnXoa_Click);
            // 
            // btnThoat
            // 
            this.btnThoat.BackColor = System.Drawing.Color.SkyBlue;
            this.btnThoat.Font = new System.Drawing.Font("Segoe UI", 10.125F, System.Drawing.FontStyle.Bold);
            this.btnThoat.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.btnThoat.Location = new System.Drawing.Point(637, 502);
            this.btnThoat.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnThoat.Name = "btnThoat";
            this.btnThoat.Size = new System.Drawing.Size(138, 50);
            this.btnThoat.TabIndex = 13;
            this.btnThoat.Text = "Thoát";
            this.btnThoat.UseVisualStyleBackColor = false;
            this.btnThoat.Click += new System.EventHandler(this.btnThoat_Click);
            // 
            // tableAdapterManager
            // 
            this.tableAdapterManager.BackupDataSetBeforeUpdate = false;
            this.tableAdapterManager.Connection = null;
            this.tableAdapterManager.CTDDHTableAdapter = null;
            this.tableAdapterManager.CTPNTableAdapter = null;
            this.tableAdapterManager.CTPXTableAdapter = null;
            this.tableAdapterManager.DDHTableAdapter = null;
            this.tableAdapterManager.NHANVIENTableAdapter = null;
            this.tableAdapterManager.PHIEUNHAPTableAdapter = null;
            this.tableAdapterManager.PHIEUXUATTableAdapter = null;
            this.tableAdapterManager.UpdateOrder = QLVT.QLVTDataSetTableAdapters.TableAdapterManager.UpdateOrderOption.InsertUpdateDelete;
            this.tableAdapterManager.VATTUTableAdapter = null;
            // 
            // txtXacNhanMK
            // 
            this.txtXacNhanMK.Enabled = false;
            this.txtXacNhanMK.Location = new System.Drawing.Point(403, 349);
            this.txtXacNhanMK.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtXacNhanMK.Name = "txtXacNhanMK";
            this.txtXacNhanMK.PasswordChar = '*';
            this.txtXacNhanMK.Size = new System.Drawing.Size(372, 31);
            this.txtXacNhanMK.TabIndex = 15;
            // 
            // labelXNMK
            // 
            this.labelXNMK.AutoSize = true;
            this.labelXNMK.Font = new System.Drawing.Font("Segoe UI", 10.125F, System.Drawing.FontStyle.Bold);
            this.labelXNMK.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.labelXNMK.Location = new System.Drawing.Point(123, 350);
            this.labelXNMK.Name = "labelXNMK";
            this.labelXNMK.Size = new System.Drawing.Size(269, 37);
            this.labelXNMK.TabIndex = 14;
            this.labelXNMK.Text = "Xác Nhận Mật Khẩu";
            // 
            // taLayDSNV
            // 
            this.taLayDSNV.ClearBeforeFill = true;
            // 
            // fmTaoTK
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1226, 660);
            this.Controls.Add(this.txtXacNhanMK);
            this.Controls.Add(this.labelXNMK);
            this.Controls.Add(this.btnThoat);
            this.Controls.Add(this.btnXoa);
            this.Controls.Add(this.btnTao);
            this.Controls.Add(this.radioBtnNV);
            this.Controls.Add(this.radioBtnAd);
            this.Controls.Add(this.txtMK);
            this.Controls.Add(this.txtTENDN);
            this.Controls.Add(this.txtMANV);
            this.Controls.Add(this.txtHOTEN);
            this.Controls.Add(this.labelNhom);
            this.Controls.Add(this.labelMK);
            this.Controls.Add(this.labelTenDN);
            this.Controls.Add(this.labelHoTenNV);
            this.Controls.Add(this.labelTaoTK);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "fmTaoTK";
            this.Text = "Tạo Tài Khoản";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.fmTaoTK_FormClosing);
            this.Load += new System.EventHandler(this.fmTaoTK_Load);
            ((System.ComponentModel.ISupportInitialize)(this.bdsLayDSNV)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.qLVTDataSet)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelTaoTK;
        private System.Windows.Forms.Label labelHoTenNV;
        private System.Windows.Forms.Label labelTenDN;
        private System.Windows.Forms.Label labelMK;
        private System.Windows.Forms.Label labelNhom;
        private System.Windows.Forms.ComboBox txtHOTEN;
        private System.Windows.Forms.TextBox txtMANV;
        private System.Windows.Forms.TextBox txtTENDN;
        private System.Windows.Forms.TextBox txtMK;
        private System.Windows.Forms.RadioButton radioBtnAd;
        private System.Windows.Forms.RadioButton radioBtnNV;
        private System.Windows.Forms.Button btnTao;
        private System.Windows.Forms.Button btnXoa;
        private System.Windows.Forms.Button btnThoat;
        private QLVTDataSet qLVTDataSet;
        private QLVTDataSetTableAdapters.TableAdapterManager tableAdapterManager;
        private System.Windows.Forms.TextBox txtXacNhanMK;
        private System.Windows.Forms.Label labelXNMK;
        private System.Windows.Forms.BindingSource bdsLayDSNV;
        private QLVTDataSetTableAdapters.LayDSNVTableAdapter taLayDSNV;
    }
}