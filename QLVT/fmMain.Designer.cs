using static System.Net.Mime.MediaTypeNames;
using System.Drawing.Printing;
using System.Windows.Forms;
using System.Xml.Linq;



namespace QLVT
{
    partial class fmMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(fmMain));
            this.ribTTNV = new DevExpress.XtraBars.Ribbon.RibbonControl();
            this.barBtn_DN = new DevExpress.XtraBars.BarButtonItem();
            this.barBtnTaoTK = new DevExpress.XtraBars.BarButtonItem();
            this.barBtnDX = new DevExpress.XtraBars.BarButtonItem();
            this.barBtnThoatMain = new DevExpress.XtraBars.BarButtonItem();
            this.barBtnDSNV = new DevExpress.XtraBars.BarButtonItem();
            this.barBtnVatTu = new DevExpress.XtraBars.BarButtonItem();
            this.barSubLapPhieu = new DevExpress.XtraBars.BarSubItem();
            this.barBtnDonHang = new DevExpress.XtraBars.BarButtonItem();
            this.barBtnPhieuNhap = new DevExpress.XtraBars.BarButtonItem();
            this.barBtnPhieuXuat = new DevExpress.XtraBars.BarButtonItem();
            this.barBtnDMNV = new DevExpress.XtraBars.BarButtonItem();
            this.barBtnDMVT = new DevExpress.XtraBars.BarButtonItem();
            this.barBtnDDHKPN = new DevExpress.XtraBars.BarButtonItem();
            this.barBtnCTPNX = new DevExpress.XtraBars.BarButtonItem();
            this.barBtnTHNX = new DevExpress.XtraBars.BarButtonItem();
            this.barBtnBackUpRestore = new DevExpress.XtraBars.BarButtonItem();
            this.barBtnHDNV = new DevExpress.XtraBars.BarButtonItem();
            this.ribHeThong = new DevExpress.XtraBars.Ribbon.RibbonPage();
            this.ribQLTK = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.ribCSDL = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.ribDanhMuc = new DevExpress.XtraBars.Ribbon.RibbonPage();
            this.ribNghiepVu = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.ribBaoCao = new DevExpress.XtraBars.Ribbon.RibbonPage();
            this.ribBC = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.tabNhanVien = new DevExpress.XtraTabbedMdi.XtraTabbedMdiManager(this.components);
            this.TTDN = new System.Windows.Forms.StatusStrip();
            this.MaNV = new System.Windows.Forms.ToolStripStatusLabel();
            this.HoTen = new System.Windows.Forms.ToolStripStatusLabel();
            this.Nhom = new System.Windows.Forms.ToolStripStatusLabel();
            ((System.ComponentModel.ISupportInitialize)(this.ribTTNV)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tabNhanVien)).BeginInit();
            this.TTDN.SuspendLayout();
            this.SuspendLayout();
            // 
            // ribTTNV
            // 
            this.ribTTNV.EmptyAreaImageOptions.ImagePadding = new System.Windows.Forms.Padding(60, 58, 60, 58);
            this.ribTTNV.ExpandCollapseItem.Id = 0;
            this.ribTTNV.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.ribTTNV.ExpandCollapseItem,
            this.barBtn_DN,
            this.barBtnTaoTK,
            this.barBtnDX,
            this.barBtnThoatMain,
            this.barBtnDSNV,
            this.barBtnVatTu,
            this.barSubLapPhieu,
            this.barBtnDonHang,
            this.barBtnPhieuNhap,
            this.barBtnPhieuXuat,
            this.barBtnDMNV,
            this.barBtnDMVT,
            this.barBtnDDHKPN,
            this.barBtnCTPNX,
            this.barBtnTHNX,
            this.barBtnBackUpRestore,
            this.barBtnHDNV});
            this.ribTTNV.Location = new System.Drawing.Point(0, 0);
            this.ribTTNV.Margin = new System.Windows.Forms.Padding(6);
            this.ribTTNV.MaxItemId = 30;
            this.ribTTNV.Name = "ribTTNV";
            this.ribTTNV.OptionsMenuMinWidth = 660;
            this.ribTTNV.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[] {
            this.ribHeThong,
            this.ribDanhMuc,
            this.ribBaoCao});
            this.ribTTNV.Size = new System.Drawing.Size(1462, 308);
            // 
            // barBtn_DN
            // 
            this.barBtn_DN.Caption = "Đăng Nhập";
            this.barBtn_DN.Id = 1;
            this.barBtn_DN.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("barBtn_DN.ImageOptions.LargeImage")));
            this.barBtn_DN.Name = "barBtn_DN";
            this.barBtn_DN.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barBtn_DN_ItemClick);
            // 
            // barBtnTaoTK
            // 
            this.barBtnTaoTK.Caption = "Tạo Tài Khoản";
            this.barBtnTaoTK.Enabled = false;
            this.barBtnTaoTK.Id = 2;
            this.barBtnTaoTK.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("barBtnTaoTK.ImageOptions.LargeImage")));
            this.barBtnTaoTK.Name = "barBtnTaoTK";
            this.barBtnTaoTK.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barBtnTaoTK_ItemClick);
            // 
            // barBtnDX
            // 
            this.barBtnDX.Caption = "Đăng Xuất";
            this.barBtnDX.Enabled = false;
            this.barBtnDX.Id = 5;
            this.barBtnDX.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("barBtnDX.ImageOptions.LargeImage")));
            this.barBtnDX.Name = "barBtnDX";
            this.barBtnDX.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barBtnDX_ItemClick);
            // 
            // barBtnThoatMain
            // 
            this.barBtnThoatMain.Caption = "Thoát";
            this.barBtnThoatMain.Id = 6;
            this.barBtnThoatMain.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("barBtnThoatMain.ImageOptions.LargeImage")));
            this.barBtnThoatMain.Name = "barBtnThoatMain";
            this.barBtnThoatMain.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barBtnThoatMain_ItemClick);
            // 
            // barBtnDSNV
            // 
            this.barBtnDSNV.Caption = "NHÂN VIÊN";
            this.barBtnDSNV.Id = 13;
            this.barBtnDSNV.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("barBtnDSNV.ImageOptions.LargeImage")));
            this.barBtnDSNV.Name = "barBtnDSNV";
            this.barBtnDSNV.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barBtnDSNV_ItemClick);
            // 
            // barBtnVatTu
            // 
            this.barBtnVatTu.Caption = "VẬT TƯ";
            this.barBtnVatTu.Id = 14;
            this.barBtnVatTu.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("barBtnVatTu.ImageOptions.LargeImage")));
            this.barBtnVatTu.Name = "barBtnVatTu";
            this.barBtnVatTu.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barBtnVatTu_ItemClick);
            // 
            // barSubLapPhieu
            // 
            this.barSubLapPhieu.Caption = "LẬP PHIẾU";
            this.barSubLapPhieu.Id = 16;
            this.barSubLapPhieu.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("barSubLapPhieu.ImageOptions.LargeImage")));
            this.barSubLapPhieu.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.barBtnDonHang),
            new DevExpress.XtraBars.LinkPersistInfo(this.barBtnPhieuNhap),
            new DevExpress.XtraBars.LinkPersistInfo(this.barBtnPhieuXuat)});
            this.barSubLapPhieu.Name = "barSubLapPhieu";
            // 
            // barBtnDonHang
            // 
            this.barBtnDonHang.Caption = "Đơn Hàng";
            this.barBtnDonHang.Id = 17;
            this.barBtnDonHang.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("barBtnDonHang.ImageOptions.Image")));
            this.barBtnDonHang.Name = "barBtnDonHang";
            this.barBtnDonHang.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barBtnDonHang_ItemClick);
            // 
            // barBtnPhieuNhap
            // 
            this.barBtnPhieuNhap.Caption = "Phiếu Nhập";
            this.barBtnPhieuNhap.Id = 18;
            this.barBtnPhieuNhap.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("barBtnPhieuNhap.ImageOptions.Image")));
            this.barBtnPhieuNhap.Name = "barBtnPhieuNhap";
            this.barBtnPhieuNhap.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barBtnPhieuNhap_ItemClick);
            // 
            // barBtnPhieuXuat
            // 
            this.barBtnPhieuXuat.Caption = "Phiếu Xuất";
            this.barBtnPhieuXuat.Id = 19;
            this.barBtnPhieuXuat.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("barBtnPhieuXuat.ImageOptions.Image")));
            this.barBtnPhieuXuat.Name = "barBtnPhieuXuat";
            this.barBtnPhieuXuat.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barBtnPhieuXuat_ItemClick);
            // 
            // barBtnDMNV
            // 
            this.barBtnDMNV.Caption = "Danh Sách Nhân Viên";
            this.barBtnDMNV.Id = 20;
            this.barBtnDMNV.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("bar.ImageOptions.LargeImage")));
            this.barBtnDMNV.Name = "barBtnDMNV";
            // 
            // barBtnDMVT
            // 
            this.barBtnDMVT.Caption = "Danh Sách Vật Tư";
            this.barBtnDMVT.Id = 21;
            this.barBtnDMVT.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("barBtnDMVT.ImageOptions.LargeImage")));
            this.barBtnDMVT.Name = "barBtnDMVT";
            // 
            // barBtnDDHKPN
            // 
            this.barBtnDDHKPN.Caption = "Đơn Đặt Hàng Không Phiếu Nhập";
            this.barBtnDDHKPN.Id = 25;
            this.barBtnDDHKPN.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("barBtnDDH.ImageOptions.LargeImage")));
            this.barBtnDDHKPN.Name = "barBtnDDHKPN";
            // 
            // barBtnCTPNX
            // 
            this.barBtnCTPNX.Caption = "Chi Tiết Nhập Xuất";
            this.barBtnCTPNX.Id = 26;
            this.barBtnCTPNX.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("barBtnCTPNX.ImageOptions.LargeImage")));
            this.barBtnCTPNX.Name = "barBtnCTPNX";
            // 
            // barBtnTHNX
            // 
            this.barBtnTHNX.Caption = "Tổng Hợp Nhập Xuất";
            this.barBtnTHNX.Enabled = false;
            this.barBtnTHNX.Id = 27;
            this.barBtnTHNX.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("barBtnTHNX.ImageOptions.LargeImage")));
            this.barBtnTHNX.Name = "barBtnTHNX";
            // 
            // barBtnBackUpRestore
            // 
            this.barBtnBackUpRestore.Caption = "Sao Lưu - Khôi phục";
            this.barBtnBackUpRestore.Enabled = false;
            this.barBtnBackUpRestore.Id = 28;
            this.barBtnBackUpRestore.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("barBtnBackUpRestore.ImageOptions.LargeImage")));
            this.barBtnBackUpRestore.Name = "barBtnBackUpRestore";
            // 
            // barBtnHDNV
            // 
            this.barBtnHDNV.Caption = "Hoạt Động Nhân Viên";
            this.barBtnHDNV.Id = 29;
            this.barBtnHDNV.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("barButtonItem2.ImageOptions.LargeImage")));
            this.barBtnHDNV.Name = "barBtnHDNV";
            this.barBtnHDNV.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonItem2_ItemClick);
            // 
            // ribHeThong
            // 
            this.ribHeThong.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[] {
            this.ribQLTK,
            this.ribCSDL});
            this.ribHeThong.Name = "ribHeThong";
            this.ribHeThong.Text = "Hệ Thống";
            // 
            // ribQLTK
            // 
            this.ribQLTK.ItemLinks.Add(this.barBtn_DN);
            this.ribQLTK.ItemLinks.Add(this.barBtnDX);
            this.ribQLTK.ItemLinks.Add(this.barBtnTaoTK);
            this.ribQLTK.ItemLinks.Add(this.barBtnThoatMain);
            this.ribQLTK.Name = "ribQLTK";
            this.ribQLTK.Text = "Quản Lý Tài Khoản";
            // 
            // ribCSDL
            // 
            this.ribCSDL.ItemLinks.Add(this.barBtnBackUpRestore);
            this.ribCSDL.Name = "ribCSDL";
            this.ribCSDL.Text = "Cơ Sở Dữ Liệu";
            // 
            // ribDanhMuc
            // 
            this.ribDanhMuc.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[] {
            this.ribNghiepVu});
            this.ribDanhMuc.Name = "ribDanhMuc";
            this.ribDanhMuc.Text = "Danh Mục";
            this.ribDanhMuc.Visible = false;
            // 
            // ribNghiepVu
            // 
            this.ribNghiepVu.ItemLinks.Add(this.barBtnDSNV);
            this.ribNghiepVu.ItemLinks.Add(this.barBtnVatTu, true);
            this.ribNghiepVu.ItemLinks.Add(this.barSubLapPhieu, true);
            this.ribNghiepVu.Name = "ribNghiepVu";
            this.ribNghiepVu.Text = "Nghiệp Vụ";
            // 
            // ribBaoCao
            // 
            this.ribBaoCao.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[] {
            this.ribBC});
            this.ribBaoCao.Name = "ribBaoCao";
            this.ribBaoCao.Text = "Báo Cáo";
            this.ribBaoCao.Visible = false;
            // 
            // ribBC
            // 
            this.ribBC.ItemLinks.Add(this.barBtnDMNV);
            this.ribBC.ItemLinks.Add(this.barBtnDMVT);
            this.ribBC.ItemLinks.Add(this.barBtnDDHKPN);
            this.ribBC.ItemLinks.Add(this.barBtnCTPNX);
            this.ribBC.ItemLinks.Add(this.barBtnHDNV);
            this.ribBC.ItemLinks.Add(this.barBtnTHNX);
            this.ribBC.ItemsLayout = DevExpress.XtraBars.Ribbon.RibbonPageGroupItemsLayout.OneRow;
            this.ribBC.Name = "ribBC";
            this.ribBC.Text = "Báo Cáo";
            // 
            // tabNhanVien
            // 
            this.tabNhanVien.MdiParent = this;
            // 
            // TTDN
            // 
            this.TTDN.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.TTDN.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MaNV,
            this.HoTen,
            this.Nhom});
            this.TTDN.Location = new System.Drawing.Point(0, 935);
            this.TTDN.Name = "TTDN";
            this.TTDN.Size = new System.Drawing.Size(1462, 42);
            this.TTDN.TabIndex = 4;
            this.TTDN.Text = "Thông tin đăng nhập";
            // 
            // MaNV
            // 
            this.MaNV.Name = "MaNV";
            this.MaNV.Size = new System.Drawing.Size(81, 32);
            this.MaNV.Text = "MaNV";
            // 
            // HoTen
            // 
            this.HoTen.Name = "HoTen";
            this.HoTen.Size = new System.Drawing.Size(83, 32);
            this.HoTen.Text = "HoTen";
            // 
            // Nhom
            // 
            this.Nhom.Name = "Nhom";
            this.Nhom.Size = new System.Drawing.Size(81, 32);
            this.Nhom.Text = "Nhom";
            // 
            // fmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1462, 977);
            this.Controls.Add(this.TTDN);
            this.Controls.Add(this.ribTTNV);
            this.IsMdiContainer = true;
            this.Margin = new System.Windows.Forms.Padding(6);
            this.Name = "fmMain";
            this.Ribbon = this.ribTTNV;
            this.RibbonVisibility = DevExpress.XtraBars.Ribbon.RibbonVisibility.Visible;
            this.Text = "Trang Chủ Quản Lý Vật Tư";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            ((System.ComponentModel.ISupportInitialize)(this.ribTTNV)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tabNhanVien)).EndInit();
            this.TTDN.ResumeLayout(false);
            this.TTDN.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraBars.Ribbon.RibbonControl ribTTNV;
        private DevExpress.XtraBars.Ribbon.RibbonPage ribHeThong;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribQLTK;
        public DevExpress.XtraBars.BarButtonItem barBtn_DN;
        public DevExpress.XtraBars.BarButtonItem barBtnTaoTK;
        private DevExpress.XtraBars.BarButtonItem barBtnDX;
        private DevExpress.XtraBars.BarButtonItem barBtnThoatMain;
        private DevExpress.XtraBars.BarButtonItem barBtnDSNV;
        private DevExpress.XtraBars.Ribbon.RibbonPage ribDanhMuc;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribNghiepVu;
        private DevExpress.XtraTabbedMdi.XtraTabbedMdiManager tabNhanVien;
        private DevExpress.XtraBars.BarButtonItem barBtnVatTu;
        private DevExpress.XtraBars.BarButtonItem barBtnLapPhieu;
        private DevExpress.XtraBars.BarSubItem barSubLapPhieu;
        private DevExpress.XtraBars.BarButtonItem barBtnDonHang;
        private DevExpress.XtraBars.BarButtonItem barBtnPhieuNhap;
        private DevExpress.XtraBars.BarButtonItem barBtnPhieuXuat;
        private DevExpress.XtraBars.Ribbon.RibbonPage ribBaoCao;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribBC;
        private System.Windows.Forms.StatusStrip TTDN;
        public System.Windows.Forms.ToolStripStatusLabel MaNV;
        public System.Windows.Forms.ToolStripStatusLabel HoTen;
        public System.Windows.Forms.ToolStripStatusLabel Nhom;
        private DevExpress.XtraBars.BarButtonItem barBtnDMNV;
        private DevExpress.XtraBars.BarButtonItem barBtnDMVT;
        private DevExpress.XtraBars.BarButtonItem barBtnDDHKPN;
        private DevExpress.XtraBars.BarButtonItem barBtnCTPNX;
        private DevExpress.XtraBars.BarButtonItem barBtnTHNX;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup1;
        private DevExpress.XtraBars.BarButtonItem barBtnBackUpRestore;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribCSDL;
        private DevExpress.XtraBars.BarButtonItem barBtnHDNV;
    }
}

