using DevExpress.XtraBars;
using QLVT;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace QLVT
{
    public partial class fmMain : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        private fmDangNhap frmLogin = null;
        private fmTaoTK frmCreateLogin = null;
        public fmMain()
        {
            InitializeComponent();
            this.IsMdiContainer = true;
        }

        public void EnableButton(bool enable)
        {
            barBtnDX.Enabled = true;
            barBtnThoatMain.Enabled = false;
            ribDanhMuc.Visible = true;
            ribDanhMuc.Visible = true;
            ribBaoCao.Visible = true;


            if (Program.role == "Admin")
            {
                barBtnTaoTK.Enabled = true;
                barBtnBackUpRestore.Enabled = true;
                barBtnTHNX.Enabled = true;
            }
            
        }

        private void ResetUI()
        {
            if (frmLogin != null)
            {
                try
                {
                    frmLogin.FormClosed -= DangNhap_FormClosed;

                    if (!frmLogin.IsDisposed)
                    {
                        frmLogin.Close();
                    }
                }
                catch (ObjectDisposedException)
                { }
                catch (Exception ex)
                {
                    Console.WriteLine("Error closing login form during ResetUI: " + ex.Message);
                }
                finally
                {
                    frmLogin = null;
                }
            }

            // Xóa bar nhân viên ở form main
            this.MaNV.Text = "MaNV";
            this.HoTen.Text = "HoTen";
            this.Nhom.Text = "Nhom";

            // It's now safe to enable the login button
            barBtn_DN.Enabled = true;
            barBtnDX.Enabled = false;
            barBtnThoatMain.Enabled = true;
            ribDanhMuc.Visible = false;
            //ribDanhMuc.Visible = false; // Duplicate line, removed one
            ribBaoCao.Visible = false;

            // Vô hiệu hóa các nút dành cho Admin
            barBtnTaoTK.Enabled = false;
            barBtnBackUpRestore.Enabled = false;
            barBtnTHNX.Enabled = false;

            // Đóng tất cả MDI form con (this part is likely okay as is)
            logout(); // This handles MDI children, our explicit code above handles frmLogin
        }

        private Form CheckExists(Type ftype)
        {
            foreach (Form f in this.MdiChildren)
                if (f.GetType() == ftype)
                    return f;
            return null;
        }

        private void logout()
        {
            foreach (Form f in this.MdiChildren)
                f.Dispose();
        }

        
        private void barBtn_DN_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (frmLogin != null && !frmLogin.IsDisposed)
            {
                frmLogin.Focus();
                return;
            }

            this.barBtn_DN.Enabled = false;

            frmLogin = new fmDangNhap();


            frmLogin.FormClosed += DangNhap_FormClosed;

            frmLogin.Show();

        }

        private void DangNhap_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (string.IsNullOrEmpty(Program.userName)) // nếu username rỗng hoặc null thì mở nút đăng nhập
            {
                this.barBtn_DN.Enabled = true;
            }

            // Dọn dẹp
            if (sender is fmDangNhap closedForm)
            {
                closedForm.FormClosed -= DangNhap_FormClosed; // Gỡ sự kiện
                if (frmLogin == closedForm)
                {
                    frmLogin = null; // Reset biến tham chiếu
                }
            }
        }

        private void barBtnThoatMain_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (MessageBox.Show("Bạn có muốn thoát không?", "Thông báo", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                Application.Exit();
            }
        }

        private void barBtnDX_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (MessageBox.Show("Bạn có muốn đăng xuất không?", "Thông báo", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                try
                {
                    // Lấy đối tượng connection hiện tại (trước khi đóng)
                    SqlConnection currentConnection = Program.conn;
                    if (currentConnection != null)
                    {
                        // Đóng kết nối hiện tại (nếu đang mở)
                        if (currentConnection.State == System.Data.ConnectionState.Open)
                        {
                            currentConnection.Close();
                        }
                        // Yêu cầu xóa pool cho kết nối này
                        SqlConnection.ClearPool(currentConnection);
                        Console.WriteLine("Connection pool cleared for the logged-out user.");
                    }
                }
                catch (Exception exPool)
                {
                    // Ghi log hoặc thông báo lỗi nếu không xóa được pool (hiếm gặp)
                    Console.WriteLine("Error clearing connection pool during logout: " + exPool.Message);
                }

                Program.loginName = "";
                Program.loginPassword = "";
                Program.userName = "";
                Program.staff = "";
                Program.role = "";

                ResetUI();
            }
        }

        private void barBtnDSNV_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Form f = this.CheckExists(typeof(fmNhanVien));
            if (f != null)
            {
                f.Activate();
            }
            else
            {
                fmNhanVien form = new fmNhanVien();
                form.MdiParent = this;
                form.Show();
            }
        }

        private void barBtnVatTu_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Form f = this.CheckExists(typeof(fmVatTu));
            if (f != null)
            {
                f.Activate();
            }
            else
            {
                fmVatTu form = new fmVatTu();
                form.MdiParent = this;
                form.Show();
            }
        }

        private void barBtnTaoTK_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (frmCreateLogin != null && !frmCreateLogin.IsDisposed)
            {
                frmCreateLogin.Focus();
                return;
            }

            this.barBtnTaoTK.Enabled = false;

            frmCreateLogin = new fmTaoTK();


            frmCreateLogin.FormClosed += TaoTK_FormClosed;

            frmCreateLogin.Show();
        }

        private void TaoTK_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.barBtnTaoTK.Enabled = true;

            // Dọn dẹp
            if (sender is fmTaoTK closedForm)
            {
                closedForm.FormClosed -= TaoTK_FormClosed; // Gỡ sự kiện
                if (frmCreateLogin == closedForm)
                {
                    frmCreateLogin = null; // Reset biến tham chiếu
                }
            }
        }

        private void barBtnDonHang_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Form f = this.CheckExists(typeof(fmDonDatHang));
            if (f != null)
            {
                f.Activate();
            }
            else
            {
                fmDonDatHang form = new fmDonDatHang();
                form.MdiParent = this;
                form.Show();
            }
        }

        private void barBtnPhieuNhap_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Form f = this.CheckExists(typeof(fmPhieuNhap));
            if (f != null)
            {
                f.Activate();
            }
            else
            {
                fmPhieuNhap form = new fmPhieuNhap();
                form.MdiParent = this;
                form.Show();
            }
        }

        private void barBtnPhieuXuat_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Form f = this.CheckExists(typeof(fmPhieuXuat));
            if (f != null)
            {
                f.Activate();
            }
            else
            {
                fmPhieuXuat form = new fmPhieuXuat();
                form.MdiParent = this;
                form.Show();
            }
        }

        private void barBtnHDNV_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Form f = this.CheckExists(typeof(FormHoatDongNhanVien));
            if (f != null)
            {
                f.Activate();
            }
            else
            {
                FormHoatDongNhanVien form = new FormHoatDongNhanVien();
                form.MdiParent = this;
                form.Show();
            }
        }

        private void barBtnBackUpRestore_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Form f = this.CheckExists(typeof(fmSaoLuu));
            if (f != null)
            {
                f.Activate();
            }
            else
            {
                fmSaoLuu form = new fmSaoLuu();
                form.MdiParent = this;
                form.Show();
            }
        }

        //vừa thêm các nút của báo cáo
        private void barBtnDMNV_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (Program.KetNoi() == 0)
            {
                MessageBox.Show("Không thể kết nối đến cơ sở dữ liệu!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Form f = this.CheckExists(typeof(FormDanhSachNhanVien));
            if (f != null)
            {
                f.Activate();
            }
            else
            {
                FormDanhSachNhanVien form = new FormDanhSachNhanVien();
                form.MdiParent = this;
                form.Show();
            }
        }

        private void barBtnDMVT_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (Program.KetNoi() == 0)
            {
                MessageBox.Show("Không thể kết nối đến cơ sở dữ liệu!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Form f = this.CheckExists(typeof(FormDanhSachVatTu));
            if (f != null)
            {
                f.Activate();
            }
            else
            {
                FormDanhSachVatTu form = new FormDanhSachVatTu();
                form.MdiParent = this;
                form.Show();
            }
        }

        private void barBtnDDHKPN_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (Program.KetNoi() == 0)
            {
                MessageBox.Show("Không thể kết nối đến cơ sở dữ liệu!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Form f = this.CheckExists(typeof(FormDonHangKhongPhieuNhap));
            if (f != null)
            {
                f.Activate();
            }
            else
            {
                FormDonHangKhongPhieuNhap form = new FormDonHangKhongPhieuNhap();
                form.MdiParent = this;
                form.Show();
            }
        }

        private void barBtnCTPNX_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (Program.KetNoi() == 0)
            {
                MessageBox.Show("Không thể kết nối đến cơ sở dữ liệu!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Form f = this.CheckExists(typeof(FormChiTietSoLuongHangHoaNhapXuat));
            if (f != null)
            {
                f.Activate();
            }
            else
            {
                FormChiTietSoLuongHangHoaNhapXuat form = new FormChiTietSoLuongHangHoaNhapXuat();
                form.MdiParent = this;
                form.Show();
            }
        }

        private void barBtnTHNX_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (Program.KetNoi() == 0)
            {
                MessageBox.Show("Không thể kết nối đến cơ sở dữ liệu!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Form f = this.CheckExists(typeof(FormTongHopNhapXuat));
            if (f != null)
            {
                f.Activate();
            }
            else
            {
                FormTongHopNhapXuat form = new FormTongHopNhapXuat();
                form.MdiParent = this;
                form.Show();
            }
        }
    }
}
