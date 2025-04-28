using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QLVT
{
    internal static class Program
    {
        public static SqlConnection conn = new SqlConnection(); // conn  
        public static String connstr = ""; // connstr  
        public static String connstrPublisher = "Data Source=DOHUY;Initial Catalog=QLVT;Integrated Security=true";
        public static SqlDataReader myReader; // myReader  

        public static String serverName = "DOHUY"; // servername  
        public static String userName = ""; // username  

        public static String loginName = ""; // mlogin  
        public static String loginPassword = ""; // password  

        public static String database = "QLVT";

        public static String role = ""; // mGroup  
        public static String staff = ""; // mHoten  

        public static string maVatTuDuocChon = "";
        public static int soLuongVatTu = 0;
        public static string maDonDatHangDuocChon = "";
        public static string maDonDatHangDuocChonChiTiet = "";
        public static int donGia = 0;

        public static string maNhanVienDuocChon = "";
        public static string hoTen = "";
        public static string diaChi = "";
        public static string ngaySinh = "";

        //binding đến các form khác

        public static BindingSource bindingSource = new BindingSource();

        public static fmDangNhap formDangNhap;
        public static fmMain fmChinh;

        // kết nối csdl  
        public static int KetNoi()
        {
            if (Program.conn != null && Program.conn.State == ConnectionState.Open)
                Program.conn.Close();
            try
            {
                Program.connstr = "Data Source=" + Program.serverName + ";Initial Catalog=" +
                       Program.database + ";User ID=" +
                       Program.loginName + ";password=" + Program.loginPassword;
                Program.conn.ConnectionString = Program.connstr;

                Program.conn.Open();
                return 1;
            }

            catch (Exception )
            {
                MessageBox.Show("Tài khoản & mật khẩu không chính xác! Vui lòng nhập lại", "Thông Báo", MessageBoxButtons.OK);
                // Console.WriteLine(e.Message);  
                return 0;
            }
        }

        // thực hiện câu lệnh sql trả về dữ liêu chỉ cho phép đọc  

        public static SqlDataReader ExecSqlDataReader(String strLenh)
        {
            SqlDataReader myreader;
            SqlCommand sqlcmd = new SqlCommand(strLenh, Program.conn);
            sqlcmd.CommandType = CommandType.Text;
            if (Program.conn.State == ConnectionState.Closed)
                Program.conn.Open();
            try
            {
                myreader = sqlcmd.ExecuteReader(); return myreader;

            }
            catch (SqlException ex)
            {
                Program.conn.Close();
                MessageBox.Show(ex.Message);
                return null;
            }
        }

        // có thể thêm, xóa, sửa, cập nhật, đi lên đi xuống, nên load SqlDataReader cho nhanh sau đó  
        // thì dùng cái DataAdapter để chỉnh sửa  
        public static DataTable ExecSqlDataTable(String cmd)
        {
            DataTable dt = new DataTable();
            if (Program.conn.State == ConnectionState.Closed) Program.conn.Open();
            SqlDataAdapter da = new SqlDataAdapter(cmd, conn);
            da.Fill(dt);
            conn.Close();
            return dt;
        }

        // cập nhật trên 1 sp nhưng không trả về dữ liệu  
        public static int ExecSqlNonQuery(String strlenh)
        {
            SqlCommand Sqlcmd = new SqlCommand(strlenh, conn);
            Sqlcmd.CommandType = CommandType.Text;
            Sqlcmd.CommandTimeout = 600; // 10 phut  
            if (conn.State == ConnectionState.Closed) conn.Open();
            try
            {
                Sqlcmd.ExecuteNonQuery(); conn.Close();
                return 0;
            }
            catch (SqlException ex)
            {
                if (ex.Message.Contains("Error converting data type varchar to int"))
                    MessageBox.Show("Bạn format Cell lại cột \"Ngày Thi\" qua kiểu Number hoặc mở File Excel.");
                else MessageBox.Show(ex.Message);
                conn.Close();
                return ex.State;

            }
        }

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Program.fmChinh = new fmMain();
            Application.Run(fmChinh);
        }
    }   
}
