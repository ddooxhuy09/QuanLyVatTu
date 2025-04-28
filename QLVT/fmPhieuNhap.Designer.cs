namespace QLVT
{
    partial class fmPhieuNhap
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
            System.Windows.Forms.Label labelMAPN;
            System.Windows.Forms.Label labelNgay;
            System.Windows.Forms.Label mANVLabel;
            System.Windows.Forms.Label labelMaDDH;
            System.Windows.Forms.Label labelHOTENNV;
            System.Windows.Forms.Label labelNCC;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(fmPhieuNhap));
            this.barPN = new DevExpress.XtraBars.BarManager(this.components);
            this.bar1 = new DevExpress.XtraBars.Bar();
            this.bar2 = new DevExpress.XtraBars.Bar();
            this.barBtnThem = new DevExpress.XtraBars.BarButtonItem();
            this.barBtnGhi = new DevExpress.XtraBars.BarButtonItem();
            this.barBtnXoa = new DevExpress.XtraBars.BarButtonItem();
            this.barBtnHoanTac = new DevExpress.XtraBars.BarButtonItem();
            this.barBtnLamMoi = new DevExpress.XtraBars.BarButtonItem();
            this.bar3 = new DevExpress.XtraBars.Bar();
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            this.barMdiChildrenListItem1 = new DevExpress.XtraBars.BarMdiChildrenListItem();
            this.barButtonItem1 = new DevExpress.XtraBars.BarButtonItem();
            this.barButtonItem2 = new DevExpress.XtraBars.BarButtonItem();
            this.barButtonItem3 = new DevExpress.XtraBars.BarButtonItem();
            this.qLVTDataSet = new QLVT.QLVTDataSet();
            this.bdsPN = new System.Windows.Forms.BindingSource(this.components);
            this.taPN = new QLVT.QLVTDataSetTableAdapters.PHIEUNHAPTableAdapter();
            this.tableAdapterManager = new QLVT.QLVTDataSetTableAdapters.TableAdapterManager();
            this.gcPN = new DevExpress.XtraGrid.GridControl();
            this.gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colMAPN = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colNGAY = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colMASODDH = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colMANV = new DevExpress.XtraGrid.Columns.GridColumn();
            this.bdsDSNV = new System.Windows.Forms.BindingSource(this.components);
            this.panelPN = new DevExpress.XtraEditors.PanelControl();
            this.txtNCC = new DevExpress.XtraEditors.TextEdit();
            this.txtHOTENNV = new System.Windows.Forms.ComboBox();
            this.txtMANV = new DevExpress.XtraEditors.SpinEdit();
            this.txtMaDDH = new DevExpress.XtraEditors.TextEdit();
            this.txtNgay = new DevExpress.XtraEditors.DateEdit();
            this.txtMaPN = new DevExpress.XtraEditors.TextEdit();
            this.bdsCTPN = new System.Windows.Forms.BindingSource(this.components);
            this.taCTPN = new QLVT.QLVTDataSetTableAdapters.CTPNTableAdapter();
            this.gcCTPN_PN = new DevExpress.XtraGrid.GridControl();
            this.gridView2 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colMAPN1 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colMAVT = new DevExpress.XtraGrid.Columns.GridColumn();
            this.cbCTPN_VT = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
            this.bdsVT = new System.Windows.Forms.BindingSource(this.components);
            this.colSOLUONG = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colDONGIA = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemCBTenVT = new DevExpress.XtraEditors.Repository.RepositoryItemComboBox();
            this.repositoryItemComboBox1 = new DevExpress.XtraEditors.Repository.RepositoryItemComboBox();
            this.bdsDSCTPN = new System.Windows.Forms.BindingSource(this.components);
            this.taDSNV = new QLVT.QLVTDataSetTableAdapters.DSNVTableAdapter();
            this.taDSCTPN = new QLVT.QLVTDataSetTableAdapters.taDSCTPN();
            this.popCTPN = new DevExpress.XtraBars.PopupMenu(this.components);
            this.taVT = new QLVT.QLVTDataSetTableAdapters.VATTUTableAdapter();
            labelMAPN = new System.Windows.Forms.Label();
            labelNgay = new System.Windows.Forms.Label();
            mANVLabel = new System.Windows.Forms.Label();
            labelMaDDH = new System.Windows.Forms.Label();
            labelHOTENNV = new System.Windows.Forms.Label();
            labelNCC = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.barPN)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.qLVTDataSet)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bdsPN)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gcPN)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bdsDSNV)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelPN)).BeginInit();
            this.panelPN.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtNCC.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMANV.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMaDDH.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtNgay.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtNgay.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMaPN.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bdsCTPN)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gcCTPN_PN)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbCTPN_VT)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bdsVT)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCBTenVT)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemComboBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bdsDSCTPN)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.popCTPN)).BeginInit();
            this.SuspendLayout();
            // 
            // labelMAPN
            // 
            labelMAPN.AutoSize = true;
            labelMAPN.Location = new System.Drawing.Point(53, 103);
            labelMAPN.Name = "labelMAPN";
            labelMAPN.Size = new System.Drawing.Size(154, 25);
            labelMAPN.TabIndex = 0;
            labelMAPN.Text = "Mã Phiếu Nhập";
            // 
            // labelNgay
            // 
            labelNgay.AutoSize = true;
            labelNgay.Location = new System.Drawing.Point(562, 103);
            labelNgay.Name = "labelNgay";
            labelNgay.Size = new System.Drawing.Size(59, 25);
            labelNgay.TabIndex = 2;
            labelNgay.Text = "Ngày";
            // 
            // mANVLabel
            // 
            mANVLabel.AutoSize = true;
            mANVLabel.Location = new System.Drawing.Point(676, 333);
            mANVLabel.Name = "mANVLabel";
            mANVLabel.Size = new System.Drawing.Size(143, 25);
            mANVLabel.TabIndex = 10;
            mANVLabel.Text = "Mã Nhân Viên";
            // 
            // labelMaDDH
            // 
            labelMaDDH.AutoSize = true;
            labelMaDDH.Location = new System.Drawing.Point(53, 212);
            labelMaDDH.Name = "labelMaDDH";
            labelMaDDH.Size = new System.Drawing.Size(173, 25);
            labelMaDDH.TabIndex = 8;
            labelMaDDH.Text = "Mã đơn đặt hàng";
            // 
            // labelHOTENNV
            // 
            labelHOTENNV.AutoSize = true;
            labelHOTENNV.Location = new System.Drawing.Point(53, 332);
            labelHOTENNV.Name = "labelHOTENNV";
            labelHOTENNV.Size = new System.Drawing.Size(190, 25);
            labelHOTENNV.TabIndex = 12;
            labelHOTENNV.Text = "Họ Tên Nhân Viên:";
            // 
            // labelNCC
            // 
            labelNCC.AutoSize = true;
            labelNCC.Location = new System.Drawing.Point(602, 212);
            labelNCC.Name = "labelNCC";
            labelNCC.Size = new System.Drawing.Size(155, 25);
            labelNCC.TabIndex = 14;
            labelNCC.Text = "Nhà Cung Cấp:";
            // 
            // barPN
            // 
            this.barPN.Bars.AddRange(new DevExpress.XtraBars.Bar[] {
            this.bar1,
            this.bar2,
            this.bar3});
            this.barPN.DockControls.Add(this.barDockControlTop);
            this.barPN.DockControls.Add(this.barDockControlBottom);
            this.barPN.DockControls.Add(this.barDockControlLeft);
            this.barPN.DockControls.Add(this.barDockControlRight);
            this.barPN.Form = this;
            this.barPN.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.barBtnThem,
            this.barBtnGhi,
            this.barBtnXoa,
            this.barBtnHoanTac,
            this.barBtnLamMoi,
            this.barMdiChildrenListItem1,
            this.barButtonItem1,
            this.barButtonItem2,
            this.barButtonItem3});
            this.barPN.MainMenu = this.bar2;
            this.barPN.MaxItemId = 17;
            this.barPN.StatusBar = this.bar3;
            // 
            // bar1
            // 
            this.bar1.BarName = "Tools";
            this.bar1.DockCol = 0;
            this.bar1.DockRow = 1;
            this.bar1.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            this.bar1.Text = "Tools";
            // 
            // bar2
            // 
            this.bar2.BarName = "Main menu";
            this.bar2.DockCol = 0;
            this.bar2.DockRow = 0;
            this.bar2.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            this.bar2.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this.barBtnThem, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph),
            new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this.barBtnGhi, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph),
            new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this.barBtnXoa, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph),
            new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this.barBtnHoanTac, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph),
            new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this.barBtnLamMoi, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph)});
            this.bar2.OptionsBar.MultiLine = true;
            this.bar2.OptionsBar.UseWholeRow = true;
            this.bar2.Text = "Main menu";
            // 
            // barBtnThem
            // 
            this.barBtnThem.Caption = "Thêm";
            this.barBtnThem.Id = 0;
            this.barBtnThem.ImageOptions.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("barBtnThem.ImageOptions.SvgImage")));
            this.barBtnThem.Name = "barBtnThem";
            // 
            // barBtnGhi
            // 
            this.barBtnGhi.Caption = "Ghi";
            this.barBtnGhi.Id = 1;
            this.barBtnGhi.ImageOptions.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("barBtnGhi.ImageOptions.SvgImage")));
            this.barBtnGhi.Name = "barBtnGhi";
            // 
            // barBtnXoa
            // 
            this.barBtnXoa.Caption = "Xóa";
            this.barBtnXoa.Id = 2;
            this.barBtnXoa.ImageOptions.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("barBtnXoa.ImageOptions.SvgImage")));
            this.barBtnXoa.Name = "barBtnXoa";
            // 
            // barBtnHoanTac
            // 
            this.barBtnHoanTac.Caption = "Hoàn Tác";
            this.barBtnHoanTac.Id = 3;
            this.barBtnHoanTac.ImageOptions.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("barBtnHoanTac.ImageOptions.SvgImage")));
            this.barBtnHoanTac.Name = "barBtnHoanTac";
            // 
            // barBtnLamMoi
            // 
            this.barBtnLamMoi.Caption = "Làm Mới";
            this.barBtnLamMoi.Id = 4;
            this.barBtnLamMoi.ImageOptions.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("barBtnLamMoi.ImageOptions.SvgImage")));
            this.barBtnLamMoi.Name = "barBtnLamMoi";
            // 
            // bar3
            // 
            this.bar3.BarName = "Status bar";
            this.bar3.CanDockStyle = DevExpress.XtraBars.BarCanDockStyle.Bottom;
            this.bar3.DockCol = 0;
            this.bar3.DockRow = 0;
            this.bar3.DockStyle = DevExpress.XtraBars.BarDockStyle.Bottom;
            this.bar3.OptionsBar.AllowQuickCustomization = false;
            this.bar3.OptionsBar.DrawDragBorder = false;
            this.bar3.OptionsBar.UseWholeRow = true;
            this.bar3.Text = "Status bar";
            // 
            // barDockControlTop
            // 
            this.barDockControlTop.CausesValidation = false;
            this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
            this.barDockControlTop.Manager = this.barPN;
            this.barDockControlTop.Size = new System.Drawing.Size(2063, 68);
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 1064);
            this.barDockControlBottom.Manager = this.barPN;
            this.barDockControlBottom.Size = new System.Drawing.Size(2063, 22);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 68);
            this.barDockControlLeft.Manager = this.barPN;
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 996);
            // 
            // barDockControlRight
            // 
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Location = new System.Drawing.Point(2063, 68);
            this.barDockControlRight.Manager = this.barPN;
            this.barDockControlRight.Size = new System.Drawing.Size(0, 996);
            // 
            // barMdiChildrenListItem1
            // 
            this.barMdiChildrenListItem1.Id = 13;
            this.barMdiChildrenListItem1.Name = "barMdiChildrenListItem1";
            // 
            // barButtonItem1
            // 
            this.barButtonItem1.Caption = "Thêm";
            this.barButtonItem1.Id = 14;
            this.barButtonItem1.Name = "barButtonItem1";
            // 
            // barButtonItem2
            // 
            this.barButtonItem2.Caption = "Ghi";
            this.barButtonItem2.Id = 15;
            this.barButtonItem2.Name = "barButtonItem2";
            // 
            // barButtonItem3
            // 
            this.barButtonItem3.Caption = "Xóa";
            this.barButtonItem3.Id = 16;
            this.barButtonItem3.Name = "barButtonItem3";
            // 
            // qLVTDataSet
            // 
            this.qLVTDataSet.DataSetName = "QLVTDataSet";
            this.qLVTDataSet.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // bdsPN
            // 
            this.bdsPN.DataMember = "PHIEUNHAP";
            this.bdsPN.DataSource = this.qLVTDataSet;
            // 
            // taPN
            // 
            this.taPN.ClearBeforeFill = true;
            // 
            // tableAdapterManager
            // 
            this.tableAdapterManager.BackupDataSetBeforeUpdate = false;
            this.tableAdapterManager.CTDDHTableAdapter = null;
            this.tableAdapterManager.CTPNTableAdapter = null;
            this.tableAdapterManager.CTPXTableAdapter = null;
            this.tableAdapterManager.DDHTableAdapter = null;
            this.tableAdapterManager.NHANVIENTableAdapter = null;
            this.tableAdapterManager.PHIEUNHAPTableAdapter = this.taPN;
            this.tableAdapterManager.PHIEUXUATTableAdapter = null;
            this.tableAdapterManager.UpdateOrder = QLVT.QLVTDataSetTableAdapters.TableAdapterManager.UpdateOrderOption.InsertUpdateDelete;
            this.tableAdapterManager.VATTUTableAdapter = null;
            // 
            // gcPN
            // 
            this.gcPN.DataSource = this.bdsPN;
            this.gcPN.Dock = System.Windows.Forms.DockStyle.Top;
            this.gcPN.Location = new System.Drawing.Point(0, 68);
            this.gcPN.MainView = this.gridView1;
            this.gcPN.MenuManager = this.barPN;
            this.gcPN.Name = "gcPN";
            this.gcPN.Size = new System.Drawing.Size(2063, 510);
            this.gcPN.TabIndex = 5;
            this.gcPN.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView1});
            // 
            // gridView1
            // 
            this.gridView1.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colMAPN,
            this.colNGAY,
            this.colMASODDH,
            this.colMANV});
            this.gridView1.GridControl = this.gcPN;
            this.gridView1.Name = "gridView1";
            // 
            // colMAPN
            // 
            this.colMAPN.Caption = "Mã Phiếu Nhập";
            this.colMAPN.FieldName = "MAPN";
            this.colMAPN.MinWidth = 40;
            this.colMAPN.Name = "colMAPN";
            this.colMAPN.OptionsColumn.ReadOnly = true;
            this.colMAPN.Visible = true;
            this.colMAPN.VisibleIndex = 0;
            this.colMAPN.Width = 150;
            // 
            // colNGAY
            // 
            this.colNGAY.Caption = "Ngày Nhập";
            this.colNGAY.FieldName = "NGAY";
            this.colNGAY.MinWidth = 40;
            this.colNGAY.Name = "colNGAY";
            this.colNGAY.Visible = true;
            this.colNGAY.VisibleIndex = 1;
            this.colNGAY.Width = 150;
            // 
            // colMASODDH
            // 
            this.colMASODDH.Caption = "Mã Đơn Đặt Hàng";
            this.colMASODDH.FieldName = "MASODDH";
            this.colMASODDH.MinWidth = 40;
            this.colMASODDH.Name = "colMASODDH";
            this.colMASODDH.Visible = true;
            this.colMASODDH.VisibleIndex = 2;
            this.colMASODDH.Width = 150;
            // 
            // colMANV
            // 
            this.colMANV.Caption = "Mã Nhân Viên";
            this.colMANV.FieldName = "MANV";
            this.colMANV.MinWidth = 40;
            this.colMANV.Name = "colMANV";
            this.colMANV.Visible = true;
            this.colMANV.VisibleIndex = 3;
            this.colMANV.Width = 150;
            // 
            // bdsDSNV
            // 
            this.bdsDSNV.DataMember = "DSNV";
            this.bdsDSNV.DataSource = this.qLVTDataSet;
            // 
            // panelPN
            // 
            this.panelPN.Controls.Add(this.txtNCC);
            this.panelPN.Controls.Add(labelNCC);
            this.panelPN.Controls.Add(this.txtHOTENNV);
            this.panelPN.Controls.Add(labelHOTENNV);
            this.panelPN.Controls.Add(mANVLabel);
            this.panelPN.Controls.Add(this.txtMANV);
            this.panelPN.Controls.Add(labelMaDDH);
            this.panelPN.Controls.Add(this.txtMaDDH);
            this.panelPN.Controls.Add(labelNgay);
            this.panelPN.Controls.Add(this.txtNgay);
            this.panelPN.Controls.Add(labelMAPN);
            this.panelPN.Controls.Add(this.txtMaPN);
            this.panelPN.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelPN.Location = new System.Drawing.Point(0, 578);
            this.panelPN.Name = "panelPN";
            this.panelPN.Size = new System.Drawing.Size(1210, 486);
            this.panelPN.TabIndex = 6;
            // 
            // txtNCC
            // 
            this.txtNCC.Location = new System.Drawing.Point(794, 197);
            this.txtNCC.MenuManager = this.barPN;
            this.txtNCC.Name = "txtNCC";
            this.txtNCC.Size = new System.Drawing.Size(299, 40);
            this.txtNCC.TabIndex = 15;
            // 
            // txtHOTENNV
            // 
            this.txtHOTENNV.DataBindings.Add(new System.Windows.Forms.Binding("SelectedValue", this.bdsPN, "MANV", true));
            this.txtHOTENNV.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.bdsDSNV, "HOTEN", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.txtHOTENNV.DataSource = this.bdsDSNV;
            this.txtHOTENNV.DisplayMember = "HOTEN";
            this.txtHOTENNV.FormattingEnabled = true;
            this.txtHOTENNV.Location = new System.Drawing.Point(273, 333);
            this.txtHOTENNV.Name = "txtHOTENNV";
            this.txtHOTENNV.Size = new System.Drawing.Size(287, 33);
            this.txtHOTENNV.TabIndex = 13;
            this.txtHOTENNV.ValueMember = "MANV";
            this.txtHOTENNV.SelectedIndexChanged += new System.EventHandler(this.txtHOTENNV_SelectedIndexChanged);
            // 
            // txtMANV
            // 
            this.txtMANV.DataBindings.Add(new System.Windows.Forms.Binding("EditValue", this.bdsPN, "MANV", true));
            this.txtMANV.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txtMANV.Location = new System.Drawing.Point(840, 326);
            this.txtMANV.MenuManager = this.barPN;
            this.txtMANV.Name = "txtMANV";
            this.txtMANV.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.txtMANV.Properties.ReadOnly = true;
            this.txtMANV.Size = new System.Drawing.Size(145, 40);
            this.txtMANV.TabIndex = 11;
            // 
            // txtMaDDH
            // 
            this.txtMaDDH.DataBindings.Add(new System.Windows.Forms.Binding("EditValue", this.bdsPN, "MASODDH", true));
            this.txtMaDDH.Location = new System.Drawing.Point(290, 205);
            this.txtMaDDH.MenuManager = this.barPN;
            this.txtMaDDH.Name = "txtMaDDH";
            this.txtMaDDH.Size = new System.Drawing.Size(226, 40);
            this.txtMaDDH.TabIndex = 9;
            // 
            // txtNgay
            // 
            this.txtNgay.DataBindings.Add(new System.Windows.Forms.Binding("EditValue", this.bdsPN, "NGAY", true));
            this.txtNgay.EditValue = null;
            this.txtNgay.Location = new System.Drawing.Point(659, 97);
            this.txtNgay.Name = "txtNgay";
            this.txtNgay.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.txtNgay.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.txtNgay.Size = new System.Drawing.Size(242, 40);
            this.txtNgay.TabIndex = 3;
            // 
            // txtMaPN
            // 
            this.txtMaPN.DataBindings.Add(new System.Windows.Forms.Binding("EditValue", this.bdsPN, "MAPN", true));
            this.txtMaPN.Location = new System.Drawing.Point(229, 97);
            this.txtMaPN.MenuManager = this.barPN;
            this.txtMaPN.Name = "txtMaPN";
            this.txtMaPN.Size = new System.Drawing.Size(249, 40);
            this.txtMaPN.TabIndex = 1;
            // 
            // bdsCTPN
            // 
            this.bdsCTPN.DataMember = "FK_CHITIETPHIEUNHAP_MAPN";
            this.bdsCTPN.DataSource = this.bdsPN;
            // 
            // taCTPN
            // 
            this.taCTPN.ClearBeforeFill = true;
            // 
            // gcCTPN_PN
            // 
            this.gcCTPN_PN.DataSource = this.bdsCTPN;
            this.gcCTPN_PN.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gcCTPN_PN.Location = new System.Drawing.Point(1210, 578);
            this.gcCTPN_PN.MainView = this.gridView2;
            this.gcCTPN_PN.MenuManager = this.barPN;
            this.gcCTPN_PN.Name = "gcCTPN_PN";
            this.gcCTPN_PN.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemCBTenVT,
            this.cbCTPN_VT,
            this.repositoryItemComboBox1});
            this.gcCTPN_PN.Size = new System.Drawing.Size(853, 486);
            this.gcCTPN_PN.TabIndex = 10;
            this.gcCTPN_PN.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView2});
            this.gcCTPN_PN.Click += new System.EventHandler(this.gcCTPN_PN_Click);
            // 
            // gridView2
            // 
            this.gridView2.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colMAPN1,
            this.colMAVT,
            this.colSOLUONG,
            this.colDONGIA});
            this.gridView2.GridControl = this.gcCTPN_PN;
            this.gridView2.Name = "gridView2";
            this.gridView2.PopupMenuShowing += new DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventHandler(this.popCTCPN);
            // 
            // colMAPN1
            // 
            this.colMAPN1.Caption = "Mã Phiếu Nhập";
            this.colMAPN1.FieldName = "MAPN";
            this.colMAPN1.MinWidth = 40;
            this.colMAPN1.Name = "colMAPN1";
            this.colMAPN1.Visible = true;
            this.colMAPN1.VisibleIndex = 0;
            this.colMAPN1.Width = 150;
            // 
            // colMAVT
            // 
            this.colMAVT.Caption = "Tên Vật Tư";
            this.colMAVT.ColumnEdit = this.cbCTPN_VT;
            this.colMAVT.FieldName = "MAVT";
            this.colMAVT.MinWidth = 40;
            this.colMAVT.Name = "colMAVT";
            this.colMAVT.Visible = true;
            this.colMAVT.VisibleIndex = 1;
            this.colMAVT.Width = 150;
            // 
            // cbCTPN_VT
            // 
            this.cbCTPN_VT.AutoHeight = false;
            this.cbCTPN_VT.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cbCTPN_VT.DataSource = this.bdsVT;
            this.cbCTPN_VT.DisplayMember = "TENVT";
            this.cbCTPN_VT.Name = "cbCTPN_VT";
            this.cbCTPN_VT.ValueMember = "MAVT";
            // 
            // bdsVT
            // 
            this.bdsVT.DataMember = "VATTU";
            this.bdsVT.DataSource = this.qLVTDataSet;
            // 
            // colSOLUONG
            // 
            this.colSOLUONG.Caption = "Số Lượng";
            this.colSOLUONG.FieldName = "SOLUONG";
            this.colSOLUONG.MinWidth = 40;
            this.colSOLUONG.Name = "colSOLUONG";
            this.colSOLUONG.Visible = true;
            this.colSOLUONG.VisibleIndex = 2;
            this.colSOLUONG.Width = 150;
            // 
            // colDONGIA
            // 
            this.colDONGIA.Caption = "Đơn Giá";
            this.colDONGIA.DisplayFormat.FormatString = "n0";
            this.colDONGIA.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.colDONGIA.FieldName = "DONGIA";
            this.colDONGIA.GroupFormat.FormatString = "n0";
            this.colDONGIA.GroupFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.colDONGIA.MinWidth = 40;
            this.colDONGIA.Name = "colDONGIA";
            this.colDONGIA.Visible = true;
            this.colDONGIA.VisibleIndex = 3;
            this.colDONGIA.Width = 150;
            // 
            // repositoryItemCBTenVT
            // 
            this.repositoryItemCBTenVT.AutoHeight = false;
            this.repositoryItemCBTenVT.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.repositoryItemCBTenVT.Name = "repositoryItemCBTenVT";
            // 
            // repositoryItemComboBox1
            // 
            this.repositoryItemComboBox1.AutoHeight = false;
            this.repositoryItemComboBox1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.repositoryItemComboBox1.Name = "repositoryItemComboBox1";
            // 
            // bdsDSCTPN
            // 
            this.bdsDSCTPN.DataMember = "FK_PHIEUNHAP_DSCTPN";
            this.bdsDSCTPN.DataSource = this.bdsPN;
            // 
            // taDSNV
            // 
            this.taDSNV.ClearBeforeFill = true;
            // 
            // taDSCTPN
            // 
            this.taDSCTPN.ClearBeforeFill = true;
            // 
            // popCTPN
            // 
            this.popCTPN.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.barButtonItem1),
            new DevExpress.XtraBars.LinkPersistInfo(this.barButtonItem2),
            new DevExpress.XtraBars.LinkPersistInfo(this.barButtonItem3)});
            this.popCTPN.Manager = this.barPN;
            this.popCTPN.Name = "popCTPN";
            // 
            // taVT
            // 
            this.taVT.ClearBeforeFill = true;
            // 
            // fmPhieuNhap
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(2063, 1086);
            this.Controls.Add(this.gcCTPN_PN);
            this.Controls.Add(this.panelPN);
            this.Controls.Add(this.gcPN);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.Name = "fmPhieuNhap";
            this.Text = "Phiếu Nhập";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.fmPhieuNhap_Load);
            ((System.ComponentModel.ISupportInitialize)(this.barPN)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.qLVTDataSet)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bdsPN)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gcPN)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bdsDSNV)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelPN)).EndInit();
            this.panelPN.ResumeLayout(false);
            this.panelPN.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtNCC.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMANV.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMaDDH.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtNgay.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtNgay.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMaPN.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bdsCTPN)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gcCTPN_PN)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbCTPN_VT)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bdsVT)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCBTenVT)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemComboBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bdsDSCTPN)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.popCTPN)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraBars.BarManager barPN;
        private DevExpress.XtraBars.Bar bar1;
        private DevExpress.XtraBars.Bar bar2;
        private DevExpress.XtraBars.Bar bar3;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
        private System.Windows.Forms.BindingSource bdsPN;
        private QLVTDataSet qLVTDataSet;
        private QLVTDataSetTableAdapters.PHIEUNHAPTableAdapter taPN;
        private QLVTDataSetTableAdapters.TableAdapterManager tableAdapterManager;
        private DevExpress.XtraGrid.GridControl gcPN;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView1;
        private DevExpress.XtraGrid.Columns.GridColumn colMAPN;
        private DevExpress.XtraGrid.Columns.GridColumn colNGAY;
        private DevExpress.XtraGrid.Columns.GridColumn colMASODDH;
        private DevExpress.XtraGrid.Columns.GridColumn colMANV;
        private DevExpress.XtraEditors.PanelControl panelPN;
        private DevExpress.XtraEditors.DateEdit txtNgay;
        private DevExpress.XtraEditors.TextEdit txtMaPN;
        private DevExpress.XtraBars.BarButtonItem barBtnThem;
        private DevExpress.XtraBars.BarButtonItem barBtnGhi;
        private DevExpress.XtraBars.BarButtonItem barBtnXoa;
        private DevExpress.XtraBars.BarButtonItem barBtnHoanTac;
        private DevExpress.XtraBars.BarButtonItem barBtnLamMoi;
        private DevExpress.XtraBars.BarMdiChildrenListItem barMdiChildrenListItem1;
        private System.Windows.Forms.ComboBox txtHOTENNV;
        private DevExpress.XtraEditors.SpinEdit txtMANV;
        private DevExpress.XtraEditors.TextEdit txtMaDDH;
        private System.Windows.Forms.BindingSource bdsCTPN;
        private QLVTDataSetTableAdapters.CTPNTableAdapter taCTPN;
        private DevExpress.XtraGrid.GridControl gcCTPN_PN;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView2;
        private System.Windows.Forms.BindingSource bdsDSNV;
        private QLVTDataSetTableAdapters.DSNVTableAdapter taDSNV;
        private System.Windows.Forms.BindingSource bdsDSCTPN;
        private QLVTDataSetTableAdapters.taDSCTPN taDSCTPN;
        private DevExpress.XtraEditors.TextEdit txtNCC;
        private DevExpress.XtraBars.PopupMenu popCTPN;
        private DevExpress.XtraBars.BarButtonItem barButtonItem1;
        private DevExpress.XtraBars.BarButtonItem barButtonItem2;
        private DevExpress.XtraBars.BarButtonItem barButtonItem3;
        private DevExpress.XtraEditors.Repository.RepositoryItemComboBox repositoryItemCBTenVT;
        private DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit cbCTPN_VT;
        private System.Windows.Forms.BindingSource bdsVT;
        private QLVTDataSetTableAdapters.VATTUTableAdapter taVT;
        private DevExpress.XtraEditors.Repository.RepositoryItemComboBox repositoryItemComboBox1;
        private DevExpress.XtraGrid.Columns.GridColumn colMAPN1;
        private DevExpress.XtraGrid.Columns.GridColumn colMAVT;
        private DevExpress.XtraGrid.Columns.GridColumn colSOLUONG;
        private DevExpress.XtraGrid.Columns.GridColumn colDONGIA;
    }
}