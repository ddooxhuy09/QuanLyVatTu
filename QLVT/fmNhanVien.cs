using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.Accessibility;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid;
using QLVT;

namespace QLVT
{
    public partial class fmNhanVien : Form
    {
        int viTri = 0; //vị trí trên bảng

        bool dangThemMoi = false; // true khi đang thêm
        bool dangChinhSua = false; // True khi đang sửa

        Stack undoList = new Stack(); // danh sách hoàn tác
        DataTable originalData = new DataTable(); // Lưu dữ liệu gốc để so sánh chỉnh sửa
        public fmNhanVien()
        {
            InitializeComponent();
        }


        private void nHANVIENBindingNavigatorSaveItem_Click(object sender, EventArgs e)
        {
            this.Validate();
            this.bdsNV.EndEdit();
            this.tableAdapterManager.UpdateAll(this.qLVTDataSet);

        }

        private void fmNhanVien_Load(object sender, EventArgs e)
        {
            //không kiểm tra khóa ngoại nữa
            qLVTDataSet.EnforceConstraints = false;

            this.taNV.Connection.ConnectionString = Program.connstr;
            this.taNV.Fill(this.qLVTDataSet.NHANVIEN);

            this.taDDH.Connection.ConnectionString = Program.connstr;
            this.taDDH.Fill(this.qLVTDataSet.DDH);

            this.taPN.Connection.ConnectionString = Program.connstr;
            this.taPN.Fill(this.qLVTDataSet.PHIEUNHAP);

            this.taPX.Connection.ConnectionString = Program.connstr;
            this.taPX.Fill(this.qLVTDataSet.PHIEUXUAT);

            // Lưu dữ liệu gốc
            originalData = qLVTDataSet.NHANVIEN.Copy();


            SetEditingState(false); 
            barBtnGhi.Enabled = false;
            barBtnHoanTac.Enabled = undoList.Count > 0;

        }

        private void SetEditingState(bool editing) 
        {
            dangThemMoi = false;
            dangChinhSua = false;

            //kiểm tra coi đang trong chế độ sửa hay thêm
            if (editing == true)
            {
                if (txtMaNV.ReadOnly == false)
                {

                    dangThemMoi = true;
                }
                else 
                {
                    dangChinhSua = true;
                }
            }

            SetPanelControlsReadOnly(!editing); //nếu mà đang chỉnh sửa hoặc là thêm mới thì phải mở hết cho sửa chứ, tắt RO đi
            if (dangThemMoi)
            {
                txtMaNV.ReadOnly = false; //thêm thì mở mã nhân viên cho nhập
                txtMaNV.Focus();
            }
            else
            {
                txtMaNV.ReadOnly = true; //còn sửa thì không cho nhập mã nhân viên
            }

            //không trogn trạng thái chỉnh sửa hoặc thêm mới thì các nút mở
            barBtnThem.Enabled = !editing;
            barBtnSua.Enabled = !editing && bdsNV.Count > 0; //kiểm tra coi còn gì để xóa hoặc sửa không
            barBtnXoa.Enabled = !editing && bdsNV.Count > 0; 
            barBtnGhi.Enabled = editing;
            barBtnHoanTac.Enabled = editing || undoList.Count > 0; //kiểm tra coi còn gì để hoàn tác nữa không
            barBtnLamMoi.Enabled = !editing;
            barBtnThoat.Enabled = true; 
        }

        private void SetPanelControlsReadOnly(bool readOnly)
        {
            txtMaNV.ReadOnly = true; 
            txtHo.ReadOnly = readOnly;
            txtTen.ReadOnly = readOnly;
            txtCCCD.ReadOnly = readOnly;
            txtNgaySinh.ReadOnly = readOnly;
            //txtNgaySinh.Enabled = !readOnly; 
            txtDiaChi.ReadOnly = readOnly;
            txtLuong.ReadOnly = readOnly;
            //txtLuong.Enabled = !readOnly; 
            txtGhiChu.ReadOnly = readOnly;
            checkBoxTrangThai.ReadOnly = readOnly;
        }

        private void barBtnThem_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (dangChinhSua) 
            {
                MessageBox.Show("Đang sửa dữ liệu, vui lòng Ghi hoặc Thoát.", "Thông báo", MessageBoxButtons.OK);
                return;
            }
            viTri = bdsNV.Position;

            // Add new row
            bdsNV.AddNew();
            checkBoxTrangThai.Checked = false; // Default to not deleted
            txtNgaySinh.EditValue = new DateTime(2000, 1, 1); // Default birth date
            txtLuong.Value = 4000000; // Default salary

            // Set state for adding
            SetEditingState(true); // Enter editing mode
            txtMaNV.Properties.ReadOnly = false; // *** Allow editing MaNV ONLY when adding ***
            txtMaNV.Focus();
        }

        private void barBtnXoa_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (bdsNV.Count == 0)
            {
                MessageBox.Show("Không có nhân viên để xóa!", "Thông báo", MessageBoxButtons.OK);
                barBtnXoa.Enabled = false;
                return;
            }

            string maNV = ((DataRowView)bdsNV[bdsNV.Position])["MANV"].ToString();
            if (maNV == Program.userName)
            {
                MessageBox.Show("Không thể xóa tài khoản đang đăng nhập!", "Thông báo", MessageBoxButtons.OK);
                return;
            }

            // Kiểm tra ràng buộc với DDH, PHIEUNHAP, PHIEUXUAT
            if (bdsDDH.Count > 0 || bdsPN.Count > 0 || bdsPX.Count > 0)
            {
                MessageBox.Show("Nhân viên này đã lập đơn đặt hàng, phiếu nhập hoặc phiếu xuất, không thể xóa!", "Thông báo", MessageBoxButtons.OK);
                return;
            }
        }

        private void barBtnGhi_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (!KiemTraDuLieuDauVao())
                return;

            if (dangThemMoi)
            {
                // Kiểm tra mã nhân viên trùng
                string maNV = txtMaNV.Text.Trim();
                string cauTruyVan =
                    "DECLARE @result int " +
                    "EXEC @result = sp_TraCuu_KiemTraMaNhanVien '" + maNV + "' " +
                    "SELECT 'Value' = @result";
                try
                {
                    Program.myReader = Program.ExecSqlDataReader(cauTruyVan);
                    if (Program.myReader == null)
                        return;
                    Program.myReader.Read();
                    int result = int.Parse(Program.myReader.GetValue(0).ToString());
                    Program.myReader.Close();

                    if (result == 1)
                    {
                        MessageBox.Show("Mã nhân viên đã tồn tại!", "Thông báo", MessageBoxButtons.OK);
                        txtMaNV.Focus();
                        return;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi kiểm tra mã nhân viên: " + ex.Message, "Thông báo", MessageBoxButtons.OK);
                    return;
                }

                // Ghi dữ liệu thêm mới
                if (MessageBox.Show("Bạn có chắc muốn ghi dữ liệu?", "Thông báo", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    try
                    {
                        bdsNV.EndEdit();
                        taNV.Update(qLVTDataSet.NHANVIEN);
                        originalData = qLVTDataSet.NHANVIEN.Copy();

                        // Lưu câu hoàn tác
                        string cauTruyVanHoanTac = "DELETE FROM NHANVIEN WHERE MANV = " + maNV;
                        undoList.Push(cauTruyVanHoanTac);

                        // Cập nhật giao diện
                        barBtnThem.Enabled = true;
                        barBtnGhi.Enabled = false;
                        barBtnXoa.Enabled = true;
                        barBtnHoanTac.Enabled = true;
                        barBtnLamMoi.Enabled = true;
                        barBtnThoat.Enabled = true;
                        txtMaNV.Enabled = false;
                        panelNV.Enabled = true;
                        gcNV.Enabled = true;
                        dangThemMoi = false;

                        MessageBox.Show("Ghi thành công!", "Thông báo", MessageBoxButtons.OK);
                    }
                    catch (Exception ex)
                    {
                        bdsNV.CancelEdit();
                        MessageBox.Show("Lỗi ghi dữ liệu: " + ex.Message, "Thông báo", MessageBoxButtons.OK);
                        return;
                    }
                }
            }
            else if (dangChinhSua)
            {
                // Ghi dữ liệu chỉnh sửa
                string maNV = txtMaNV.Text.Trim();
                DataRowView drv = (DataRowView)bdsNV[bdsNV.Position];
                DataRow originalRow = originalData.Select($"MANV = {maNV}").FirstOrDefault();
                if (originalRow == null)
                {
                    MessageBox.Show("Không tìm thấy dữ liệu gốc!", "Thông báo", MessageBoxButtons.OK);
                    return;
                }

                if (MessageBox.Show("Bạn có chắc muốn ghi các thay đổi?", "Thông báo", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    try
                    {
                        // Lưu câu hoàn tác
                        string ho = originalRow["HO"].ToString();
                        string ten = originalRow["TEN"].ToString();
                        string cccd = originalRow["CCCD"].ToString();
                        DateTime ngaySinh = (DateTime)originalRow["NGAYSINH"];
                        string diaChi = originalRow["DIACHI"].ToString();
                        int luong = int.Parse(originalRow["LUONG"].ToString());
                        string ghiChu = originalRow["GHICHU"].ToString();

                        string cauTruyVanHoanTac =
                            "UPDATE NHANVIEN SET " +
                            "HO = N'" + ho + "', " +
                            "TEN = N'" + ten + "', " +
                            "CCCD = '" + cccd + "', " +
                            "NGAYSINH = CAST('" + ngaySinh.ToString("yyyy-MM-dd") + "' AS DATETIME), " +
                            "DIACHI = N'" + diaChi + "', " +
                            "LUONG = " + luong + ", " +
                            "GHICHU = N'" + ghiChu + "', " +
                            "TRANGTHAIXOA = 0 " +
                            "WHERE MANV = " + maNV;
                        undoList.Push(cauTruyVanHoanTac);

                        // Ghi dữ liệu
                        bdsNV.EndEdit();
                        taNV.Update(qLVTDataSet.NHANVIEN);
                        originalData = qLVTDataSet.NHANVIEN.Copy();

                        // Cập nhật giao diện
                        barBtnThem.Enabled = true;
                        barBtnGhi.Enabled = false;
                        barBtnXoa.Enabled = true;
                        barBtnHoanTac.Enabled = true;
                        barBtnLamMoi.Enabled = true;
                        barBtnThoat.Enabled = true;
                        txtMaNV.Enabled = false;
                        dangChinhSua = false;
                        gcNV.Enabled = true;

                        MessageBox.Show("Ghi thành công!", "Thông báo", MessageBoxButtons.OK);
                    }
                    catch (Exception ex)
                    {
                        bdsNV.CancelEdit();
                        MessageBox.Show("Lỗi ghi dữ liệu: " + ex.Message, "Thông báo", MessageBoxButtons.OK);
                        return;
                    }
                }
            }
        }






        private bool KiemTraDuLieuDauVao()
        {
            if (txtMaNV.Text == "")
            {
                MessageBox.Show("Mã nhân viên không được để trống!", "Thông báo", MessageBoxButtons.OK);
                txtMaNV.Focus();
                return false;
            }
            if (!Regex.IsMatch(txtMaNV.Text, @"^\d+$"))
            {
                MessageBox.Show("Mã nhân viên chỉ chứa số!", "Thông báo", MessageBoxButtons.OK);
                txtMaNV.Focus();
                return false;
            }
            if (txtHo.Text == "")
            {
                MessageBox.Show("Họ không được để trống!", "Thông báo", MessageBoxButtons.OK);
                txtHo.Focus();
                return false;
            }
            if (!Regex.IsMatch(txtHo.Text, @"^[A-Za-z\s]+$"))
            {
                MessageBox.Show("Họ chỉ chứa chữ cái và khoảng trắng!", "Thông báo", MessageBoxButtons.OK);
                txtHo.Focus();
                return false;
            }
            if (txtHo.Text.Length > 40)
            {
                MessageBox.Show("Họ không quá 40 ký tự!", "Thông báo", MessageBoxButtons.OK);
                txtHo.Focus();
                return false;
            }
            if (txtTen.Text == "")
            {
                MessageBox.Show("Tên không được để trống!", "Thông báo", MessageBoxButtons.OK);
                txtTen.Focus();
                return false;
            }
            if (!Regex.IsMatch(txtTen.Text, @"^[A-Za-z\s]+$"))
            {
                MessageBox.Show("Tên chỉ chứa chữ cái và khoảng trắng!", "Thông báo", MessageBoxButtons.OK);
                txtTen.Focus();
                return false;
            }
            if (txtTen.Text.Length > 10)
            {
                MessageBox.Show("Tên không quá 10 ký tự!", "Thông báo", MessageBoxButtons.OK);
                txtTen.Focus();
                return false;
            }
            if (txtCCCD.Text == "")
            {
                MessageBox.Show("CCCD không được để trống!", "Thông báo", MessageBoxButtons.OK);
                txtCCCD.Focus();
                return false;
            }
            if (!Regex.IsMatch(txtCCCD.Text, @"^\d{12}$"))
            {
                MessageBox.Show("CCCD phải là 12 số!", "Thông báo", MessageBoxButtons.OK);
                txtCCCD.Focus();
                return false;
            }
            if (txtNgaySinh.EditValue == null || CalculateAge((DateTime)txtNgaySinh.EditValue) < 18)
            {
                MessageBox.Show("Nhân viên phải từ 18 tuổi trở lên!", "Thông báo", MessageBoxButtons.OK);
                txtNgaySinh.Focus();
                return false;
            }
            if (txtDiaChi.Text == "")
            {
                MessageBox.Show("Địa chỉ không được để trống!", "Thông báo", MessageBoxButtons.OK);
                txtDiaChi.Focus();
                return false;
            }
            if (txtDiaChi.Text.Length > 100)
            {
                MessageBox.Show("Địa chỉ không quá 100 ký tự!", "Thông báo", MessageBoxButtons.OK);
                txtDiaChi.Focus();
                return false;
            }
            if (txtLuong.Value < 4000000)
            {
                MessageBox.Show("Lương tối thiểu là 4.000.000!", "Thông báo", MessageBoxButtons.OK);
                txtLuong.Focus();
                return false;
            }
            return true;
        }

        private static int CalculateAge(DateTime dateOfBirth)
        {
            int age = DateTime.Now.Year - dateOfBirth.Year;
            if (DateTime.Now.DayOfYear < dateOfBirth.DayOfYear)
                age--;
            return age;
        }

        private void barBtnThoat_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            // Hủy thao tác thêm mới hoặc chỉnh sửa
            if (dangThemMoi || dangChinhSua)
            {
                bdsNV.CancelEdit();
                if (dangThemMoi)
                    bdsNV.RemoveCurrent();
                bdsNV.Position = -1;

                dangThemMoi = false;
                dangChinhSua = false;
                barBtnThem.Enabled = true;
                barBtnGhi.Enabled = false;
                barBtnXoa.Enabled = true;
                barBtnHoanTac.Enabled = undoList.Count > 0;
                barBtnLamMoi.Enabled = true;
                barBtnThoat.Enabled = true;
                txtMaNV.Enabled = false;
                txtMaNV.ReadOnly = true;
                panelNV.Enabled = true;
                gcNV.Enabled = true;
            }
        }

        private void fmNhanVien_Shown(object sender, EventArgs e)
        {

        }

        private void barBtnSua_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

        }
    }
}
