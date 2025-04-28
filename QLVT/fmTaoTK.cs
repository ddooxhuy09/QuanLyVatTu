using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using QLVT;

namespace QLVT
{
    public partial class fmTaoTK : Form
    {
        public fmTaoTK()
        {
            InitializeComponent();
        }

        private void fmTaoTK_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Code xác nhận và e.Cancel nằm ở đây
            if (e.CloseReason == CloseReason.UserClosing)
            {
                DialogResult result = MessageBox.Show("Bạn có muốn thoát không?",
                                                       "Xác nhận",
                                                       MessageBoxButtons.OKCancel,
                                                       MessageBoxIcon.Question);

                if (result == DialogResult.Cancel)
                {
                    e.Cancel = true;
                }
            }
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            this.Close();
            Program.fmChinh.barBtnTaoTK.Enabled = true;
        }

        private void fmTaoTK_Load(object sender, EventArgs e)
        {
            this.taLayDSNV.Connection.ConnectionString = Program.connstr;
            this.taLayDSNV.Fill(this.qLVTDataSet.LayDSNV);

            this.txtHOTEN.SelectedIndex = -1; //ô combobox ban đầu trống
        }

        private void txtHOTEN_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (txtHOTEN.SelectedValue != null && txtHOTEN.SelectedItem is DataRowView row)
            {
                // Xóa giá trị cũ (chỉ mật khẩu)
                txtMK.Text = "";
                txtXacNhanMK.Text = "";

                // Gán MANV
                txtMANV.Text = txtHOTEN.SelectedValue.ToString();

                // Lấy LoginName (TENDN) và RoleName (TENNHOM)
                string loginName = row["TENDN"]?.ToString() ?? "";
                string roleName = row["TENNHOM"]?.ToString() ?? ""; // Lấy tên nhóm
                txtTENDN.Text = loginName;

                // Luôn reset trạng thái radio buttons trước khi xử lý
                radioBtnAd.Checked = false;
                radioBtnNV.Checked = false;

                if (!string.IsNullOrEmpty(loginName)) // Đã có tài khoản
                {
                    btnXoa.Enabled = true;
                    btnTao.Enabled = false;
                    txtTENDN.Enabled = false;
                    txtMK.Enabled = false;
                    txtXacNhanMK.Enabled = false;

                    // --- PHẦN XỬ LÝ ROLE THEO YÊU CẦU ---
                    const string TEN_ROLE_ADMIN = "Admin";     // <-- THAY TÊN ROLE ADMIN THỰC TẾ
                    const string TEN_ROLE_NHANVIEN = "NhanVien"; // <-- THAY TÊN ROLE NHÂN VIÊN THỰC TẾ

                    // Chỉ check nếu roleName khớp chính xác "Admin" hoặc "NhanVien"
                    if (roleName.Equals(TEN_ROLE_ADMIN, StringComparison.OrdinalIgnoreCase))
                    {
                        radioBtnAd.Checked = true;
                    }
                    else if (roleName.Equals(TEN_ROLE_NHANVIEN, StringComparison.OrdinalIgnoreCase))
                    {
                        radioBtnNV.Checked = true;
                    }
                    // Nếu roleName là null, rỗng, hoặc giá trị khác, không radio button nào được check (do đã reset ở trên)

                    // Vẫn vô hiệu hóa việc chọn role khi tài khoản đã tồn tại
                    radioBtnAd.Enabled = false;
                    radioBtnNV.Enabled = false;
                    // --- KẾT THÚC PHẦN XỬ LÝ ROLE ---
                }
                else // Chưa có tài khoản (chuẩn bị tạo mới)
                {
                    btnXoa.Enabled = false;
                    btnTao.Enabled = true;
                    txtTENDN.Enabled = true;
                    txtMK.Enabled = true;
                    txtXacNhanMK.Enabled = true;

                    // Kích hoạt radio buttons để người dùng chọn (vẫn không check sẵn)
                    radioBtnAd.Enabled = true;
                    radioBtnNV.Enabled = true;
                }
            }
            else // Không chọn nhân viên nào
            {
                // Reset tất cả
                txtMANV.Text = "";
                txtTENDN.Text = "";
                txtMK.Text = "";
                txtXacNhanMK.Text = "";
                btnXoa.Enabled = false;
                btnTao.Enabled = false;
                txtTENDN.Enabled = false;
                txtMK.Enabled = false;
                txtXacNhanMK.Enabled = false;
                radioBtnAd.Checked = false; // Đảm bảo không check
                radioBtnNV.Checked = false; // Đảm bảo không check
                radioBtnAd.Enabled = false;
                radioBtnNV.Enabled = false;
            }
        
        }

        private bool kiemTraDuLieuDauVao() // Giữ nguyên tên hàm theo yêu cầu
        {
            // Kiểm tra đã chọn nhân viên chưa (qua txtMANV)
            if (txtMANV.Text == "") // So sánh với chuỗi rỗng
            {
                MessageBox.Show("Chưa chọn nhân viên", "Thông báo", MessageBoxButtons.OK);
                return false;
            }

            // Kiểm tra tên đăng nhập
            if (txtTENDN.Text == "")
            {
                MessageBox.Show("Thiếu tên đăng nhập", "Thông báo", MessageBoxButtons.OK);
                return false;
            }

            // Kiểm tra mật khẩu
            if (txtMK.Text == "")
            {
                MessageBox.Show("Thiếu mật khẩu", "Thông báo", MessageBoxButtons.OK);
                return false;
            }

            // Kiểm tra xác nhận mật khẩu
            if (txtXacNhanMK.Text == "")
            {
                MessageBox.Show("Thiếu mật khẩu xác nhận", "Thông báo", MessageBoxButtons.OK);
                return false;
            }

            // Kiểm tra khớp mật khẩu
            if (txtMK.Text != txtXacNhanMK.Text)
            {
                MessageBox.Show("Mật khẩu không khớp với mật khẩu xác nhận", "Thông báo", MessageBoxButtons.OK);
                return false;
            }

            // Kiểm tra đã chọn vai trò chưa
            if (radioBtnAd.Checked == false && radioBtnNV.Checked == false) // Kiểm tra cả hai đều không được check
            {
                MessageBox.Show("Chưa chọn vai trò cho tài khoản", "Thông báo", MessageBoxButtons.OK);
                return false;
            }

            // Nếu tất cả kiểm tra đều qua
            return true;
        }

        private void btnTao_Click(object sender, EventArgs e)
        {
            if (!kiemTraDuLieuDauVao()) return;

            string taiKhoan = txtTENDN.Text.Trim();
            string matKhau = txtMK.Text;
            string maNhanVien = txtMANV.Text;
            // !!! THAY TÊN ROLE THỰC TẾ !!!
            string vaiTro = radioBtnAd.Checked ? "ADMIN" : "NHANVIEN";

            SqlCommand sqlCommand = new SqlCommand("sp_TaoTaiKhoan", Program.conn);
            sqlCommand.CommandType = CommandType.StoredProcedure;

            sqlCommand.Parameters.AddWithValue("@login", taiKhoan);
            sqlCommand.Parameters.AddWithValue("@password", matKhau);
            sqlCommand.Parameters.AddWithValue("@username", maNhanVien);
            sqlCommand.Parameters.AddWithValue("@role", vaiTro);

            try
            {
                if (Program.conn.State == ConnectionState.Closed)
                {
                    Program.conn.Open();
                }

                sqlCommand.ExecuteNonQuery();

                MessageBox.Show("Tạo tài khoản thành công cho Mã Nhân Viên: " + maNhanVien,
                                "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                try
                {
                    // 1. Gọi lại Fill để cập nhật DataSet từ DB
                    this.taLayDSNV.Connection.ConnectionString = Program.connstr;
                    this.taLayDSNV.Fill(this.qLVTDataSet.LayDSNV);

                    // 2. Reset trạng thái các control trên form
                    txtHOTEN.SelectedIndex = -1; 
                                                 

                    // 3. Xóa các ô mật khẩu một cách tường minh
                    txtMK.Text = "";
                    txtXacNhanMK.Text = "";

                    MessageBox.Show("Đã cập nhật lại danh sách nhân viên.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);


                }
                catch (Exception exReload)
                {
                    MessageBox.Show("Tạo tài khoản thành công, nhưng có lỗi khi tải lại danh sách nhân viên:\n" + exReload.Message,
                                    "Lỗi Tải Lại Dữ Liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }


            }
            catch (SqlException ex)
            {
                string msg = "Lỗi tạo tài khoản.\n";
                if (ex.Number == 15025)
                    msg += "Tên đăng nhập '" + taiKhoan + "' đã tồn tại.";
                else if (ex.Number == 15023)
                    msg += "User '" + maNhanVien + "' đã tồn tại trong database.";
                else
                    msg += ex.Message;
                MessageBox.Show(msg, "Lỗi Database", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi không xác định: " + ex.Message, "Lỗi Hệ Thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (Program.conn.State == ConnectionState.Open)
                {
                    Program.conn.Close();
                }
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            string loginToDelete = txtTENDN.Text;
            string userNameToDelete = txtMANV.Text; 

            DialogResult confirmResult = MessageBox.Show($"Bạn có chắc chắn muốn xóa tài khoản login {loginToDelete}?",
                                                       "Xác nhận xóa",
                                                       MessageBoxButtons.YesNo,
                                                       MessageBoxIcon.Warning);

            if (confirmResult == DialogResult.Yes)
            {

                SqlCommand sqlCommand = new SqlCommand("sp_XoaTaiKhoan", Program.conn);
                sqlCommand.CommandType = CommandType.StoredProcedure;


                sqlCommand.Parameters.AddWithValue("@login", loginToDelete);
                sqlCommand.Parameters.AddWithValue("@username", userNameToDelete);

                try
                {
                    if (Program.conn.State == ConnectionState.Closed) Program.conn.Open();

                    sqlCommand.ExecuteNonQuery();

                    MessageBox.Show("Xóa tài khoản thành công!", "Thông Báo", MessageBoxButtons.OK);

                    // Load lại dữ liệu
                    try
                    {
                        this.taLayDSNV.Connection.ConnectionString = Program.connstr;
                        this.taLayDSNV.Fill(this.qLVTDataSet.LayDSNV);
                        txtHOTEN.SelectedIndex = -1;
                    }
                    catch { }
                }
                catch (Exception ex) 
                {
                    MessageBox.Show("Lỗi xóa tài khoản: " + ex.Message, "Lỗi", MessageBoxButtons.OK);
                }
                finally
                {
                    if (Program.conn.State == ConnectionState.Open) 
                        Program.conn.Close();
                }
            }
        }
    }
    
}


    


