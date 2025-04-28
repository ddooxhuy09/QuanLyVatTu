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
using DevExpress.CodeParser;
using QLVT;

namespace QLVT
{
    public partial class fmDangNhap : Form
    {
        public static SqlConnection conn = new SqlConnection();

        public fmDangNhap()
        {
            InitializeComponent();
        }

        public static int KetNoi()
        {
            if (conn != null && conn.State == ConnectionState.Open)
                conn.Close();
            try
            {
                conn.ConnectionString = Program.connstrPublisher;
                conn.Open();
                return 1;
            }

            catch (Exception e)
            {
                MessageBox.Show("Lỗi kết nối cơ sở dữ liệu.\nXem lại tài khoản và mật khẩu.\n " + e.Message, "", MessageBoxButtons.OK);
                //Console.WriteLine(e.Message);
                return 0;
            }
        }

        private Form CheckExists(Type ftype)
        {
            foreach (Form f in this.MdiChildren)
                if (f.GetType() == ftype)
                    return f;
            return null;
        }

        private void btnDangNhap_Click(object sender, EventArgs e)
        {
            //Kiểm tra tính hợp lệ của User Name và Password
            if (textTenDN.Text.Trim() == "" || textMatKhau.Text.Trim() == "")
            {
                MessageBox.Show("Vui lòng điền đầy đủ thông tin đăng nhập!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            //Kiểm tra user name và password có đúng không
            Program.loginName = textTenDN.Text.Trim();
            Program.loginPassword = textMatKhau.Text.Trim();

            if (Program.KetNoi() == 0)
                return;
            //else
            //{
            //    MessageBox.Show("Đăng nhập thành công!", "Thông Báo", MessageBoxButtons.OK);
            //}

            //Chạy sp Đăng Nhập
            String statement = "EXEC sp_DangNhap '" + Program.loginName + "'";
            Program.myReader = Program.ExecSqlDataReader(statement);
            if (Program.myReader == null)
                return;
            // đọc một dòng của myReader - điều này là hiển nhiên vì kết quả chỉ có 1 dùng duy nhất
            Program.myReader.Read();

            //có thể tài khoản được tạo mà không mapping với user nào cả
            Program.userName = Program.myReader.GetString(0);// lấy userName
            if (Convert.IsDBNull(Program.userName))
            {
                MessageBox.Show("Tài khoản này không có quyền truy cập \n Hãy thử tài khoản khác", "Thông Báo", MessageBoxButtons.OK);
            }

            //lưu thông tin đăng nhập
            Program.staff = Program.myReader.GetString(1);
            Program.role = Program.myReader.GetString(2);

            Program.myReader.Close();
            Program.conn.Close();

            //đưa dữ liệu vào bar
            Program.fmChinh.MaNV.Text = "MÃ NHÂN VIÊN: " + Program.userName;
            Program.fmChinh.HoTen.Text = "TÊN NHÂN VIÊN: " + Program.staff;
            Program.fmChinh.Nhom.Text = "NHÓM: " + Program.role;


            //bật các nút trên form chính
            this.Visible = false;
            Program.fmChinh.EnableButton(true);
        }

        //hàm kiểm tra đóng form
        private void fmDangNhap_FormClosing(object sender, FormClosingEventArgs e)
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
            Program.fmChinh.barBtn_DN.Enabled = true;

        }
    }
}
