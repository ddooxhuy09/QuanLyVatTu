namespace QLVT
{
    partial class XfrmPhieuNvLapTrongNamTheoLoai
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
            System.Windows.Forms.Label lblHoTen;
            System.Windows.Forms.Label lblLoaiPhieu;
            System.Windows.Forms.Label lblNam;
            System.Windows.Forms.Label labelLoaiPhieu;
            this.qLVTDataSet = new QLVT.QLVTDataSet();
            this.bdsDSNVKhongMaNV = new System.Windows.Forms.BindingSource(this.components);
            this.taDSNVKhongMaNV = new QLVT.QLVTDataSetTableAdapters.DSNVKhongMaNVTableAdapter();
            this.tableAdapterManager = new QLVT.QLVTDataSetTableAdapters.TableAdapterManager();
            this.mANVSpinEdit = new DevExpress.XtraEditors.SpinEdit();
            this.cbLoaiPhieu = new DevExpress.XtraEditors.ComboBoxEdit();
            this.hOTENTextEdit = new DevExpress.XtraEditors.ComboBoxEdit();
            this.cbNam = new DevExpress.XtraEditors.ComboBoxEdit();
            this.btnXemTruoc = new System.Windows.Forms.Button();
            this.btnThoat = new System.Windows.Forms.Button();
            lblHoTen = new System.Windows.Forms.Label();
            lblLoaiPhieu = new System.Windows.Forms.Label();
            lblNam = new System.Windows.Forms.Label();
            labelLoaiPhieu = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.qLVTDataSet)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bdsDSNVKhongMaNV)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mANVSpinEdit.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbLoaiPhieu.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.hOTENTextEdit.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbNam.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // lblHoTen
            // 
            lblHoTen.AutoSize = true;
            lblHoTen.Location = new System.Drawing.Point(92, 63);
            lblHoTen.Name = "lblHoTen";
            lblHoTen.Size = new System.Drawing.Size(181, 25);
            lblHoTen.TabIndex = 0;
            lblHoTen.Text = "Họ tên nhân viên:";
            // 
            // lblLoaiPhieu
            // 
            lblLoaiPhieu.AutoSize = true;
            lblLoaiPhieu.Location = new System.Drawing.Point(908, 63);
            lblLoaiPhieu.Name = "lblLoaiPhieu";
            lblLoaiPhieu.Size = new System.Drawing.Size(0, 25);
            lblLoaiPhieu.TabIndex = 4;
            // 
            // lblNam
            // 
            lblNam.AutoSize = true;
            lblNam.Location = new System.Drawing.Point(1282, 64);
            lblNam.Name = "lblNam";
            lblNam.Size = new System.Drawing.Size(62, 25);
            lblNam.TabIndex = 6;
            lblNam.Text = "Năm:";
            // 
            // labelLoaiPhieu
            // 
            labelLoaiPhieu.AutoSize = true;
            labelLoaiPhieu.Location = new System.Drawing.Point(924, 63);
            labelLoaiPhieu.Name = "labelLoaiPhieu";
            labelLoaiPhieu.Size = new System.Drawing.Size(118, 25);
            labelLoaiPhieu.TabIndex = 10;
            labelLoaiPhieu.Text = "Loại phiếu:";
            // 
            // qLVTDataSet
            // 
            this.qLVTDataSet.DataSetName = "QLVTDataSet";
            this.qLVTDataSet.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // bdsDSNVKhongMaNV
            // 
            this.bdsDSNVKhongMaNV.DataMember = "DSNVKhongMaNV";
            this.bdsDSNVKhongMaNV.DataSource = this.qLVTDataSet;
            // 
            // taDSNVKhongMaNV
            // 
            this.taDSNVKhongMaNV.ClearBeforeFill = true;
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
            // mANVSpinEdit
            // 
            this.mANVSpinEdit.DataBindings.Add(new System.Windows.Forms.Binding("EditValue", this.bdsDSNVKhongMaNV, "MANV", true));
            this.mANVSpinEdit.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.mANVSpinEdit.Location = new System.Drawing.Point(708, 57);
            this.mANVSpinEdit.Name = "mANVSpinEdit";
            this.mANVSpinEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.mANVSpinEdit.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.mANVSpinEdit.Size = new System.Drawing.Size(110, 40);
            this.mANVSpinEdit.TabIndex = 3;
            // 
            // cbLoaiPhieu
            // 
            this.cbLoaiPhieu.Location = new System.Drawing.Point(1064, 56);
            this.cbLoaiPhieu.Name = "cbLoaiPhieu";
            this.cbLoaiPhieu.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cbLoaiPhieu.Properties.Items.AddRange(new object[] {
            "NHẬP",
            "XUẤT"});
            this.cbLoaiPhieu.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.cbLoaiPhieu.Size = new System.Drawing.Size(136, 40);
            this.cbLoaiPhieu.TabIndex = 5;
            // 
            // hOTENTextEdit
            // 
            this.hOTENTextEdit.DataBindings.Add(new System.Windows.Forms.Binding("EditValue", this.bdsDSNVKhongMaNV, "HOTEN", true));
            this.hOTENTextEdit.Location = new System.Drawing.Point(286, 57);
            this.hOTENTextEdit.Name = "hOTENTextEdit";
            this.hOTENTextEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.hOTENTextEdit.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.hOTENTextEdit.Size = new System.Drawing.Size(402, 40);
            this.hOTENTextEdit.TabIndex = 1;
            // 
            // cbNam
            // 
            this.cbNam.Location = new System.Drawing.Point(1362, 55);
            this.cbNam.Name = "cbNam";
            this.cbNam.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cbNam.Size = new System.Drawing.Size(136, 40);
            this.cbNam.TabIndex = 7;
            // 
            // btnXemTruoc
            // 
            this.btnXemTruoc.Location = new System.Drawing.Point(1680, 54);
            this.btnXemTruoc.Name = "btnXemTruoc";
            this.btnXemTruoc.Size = new System.Drawing.Size(144, 43);
            this.btnXemTruoc.TabIndex = 8;
            this.btnXemTruoc.Text = "Xem trước";
            this.btnXemTruoc.UseVisualStyleBackColor = true;
            this.btnXemTruoc.Click += new System.EventHandler(this.btnXemTruoc_Click);
            // 
            // btnThoat
            // 
            this.btnThoat.Location = new System.Drawing.Point(1880, 54);
            this.btnThoat.Name = "btnThoat";
            this.btnThoat.Size = new System.Drawing.Size(83, 43);
            this.btnThoat.TabIndex = 9;
            this.btnThoat.Text = "Thoát";
            this.btnThoat.UseVisualStyleBackColor = true;
            // 
            // XfrmPhieuNvLapTrongNamTheoLoai
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(2197, 406);
            this.Controls.Add(labelLoaiPhieu);
            this.Controls.Add(this.btnThoat);
            this.Controls.Add(this.btnXemTruoc);
            this.Controls.Add(this.cbNam);
            this.Controls.Add(lblNam);
            this.Controls.Add(this.cbLoaiPhieu);
            this.Controls.Add(lblLoaiPhieu);
            this.Controls.Add(this.mANVSpinEdit);
            this.Controls.Add(lblHoTen);
            this.Controls.Add(this.hOTENTextEdit);
            this.Name = "XfrmPhieuNvLapTrongNamTheoLoai";
            this.Text = "XfrmPhieuNvLapTrongNamTheoLoai";
            this.Load += new System.EventHandler(this.XfrmPhieuNvLapTrongNamTheoLoai_Load);
            ((System.ComponentModel.ISupportInitialize)(this.qLVTDataSet)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bdsDSNVKhongMaNV)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mANVSpinEdit.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbLoaiPhieu.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.hOTENTextEdit.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbNam.Properties)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private QLVTDataSet qLVTDataSet;
        private System.Windows.Forms.BindingSource bdsDSNVKhongMaNV;
        private QLVTDataSetTableAdapters.DSNVKhongMaNVTableAdapter taDSNVKhongMaNV;
        private QLVTDataSetTableAdapters.TableAdapterManager tableAdapterManager;
        private DevExpress.XtraEditors.SpinEdit mANVSpinEdit;
        private DevExpress.XtraEditors.ComboBoxEdit cbLoaiPhieu;
        private DevExpress.XtraEditors.ComboBoxEdit hOTENTextEdit;
        private DevExpress.XtraEditors.ComboBoxEdit cbNam;
        private System.Windows.Forms.Button btnXemTruoc;
        private System.Windows.Forms.Button btnThoat;
    }
}