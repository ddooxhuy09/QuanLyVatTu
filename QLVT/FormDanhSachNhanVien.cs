using DevExpress.XtraReports.UI;
using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Windows.Forms;
using DevExpress.XtraGrid.Views.Grid;

namespace QLVT
{
    public partial class FormDanhSachNhanVien : Form
    {
        private double? luongTu = null;
        private double? luongDen = null;
        private double? tempLuongDen = null; // Lưu tạm giá trị textEdit2
        private bool? trangThaiXoa = null; // Lưu trạng thái xóa (0: Đang làm, 1: Đã nghỉ, null: Cả 2)
        private string sapXepTheo = "TEN"; // Mặc định sắp xếp theo tên
        private string thuTuSapXep = "ASC"; // Mặc định tăng dần

        public FormDanhSachNhanVien()
        {
            InitializeComponent();
            // Ban đầu ẩn textEdit2 và checkButton1
            textEdit2.Enabled = false;
            checkButton1.Enabled = false;
            // Thiết lập giá trị mặc định cho comboBox1 và filter
            comboBox1.SelectedIndex = 2; // Mặc định "Cả 2"
            filter.SelectedIndex = 0; // Mặc định "Tên: Từ thấp đến cao"
            LoadData();
            // Gắn sự kiện click cho các nút
            button1.Click += new EventHandler(button1_Click);
            button2.Click += new EventHandler(button2_Click);
            // Gắn sự kiện CustomColumnDisplayText cho gridView1
            gridView1.CustomColumnDisplayText += GridView1_CustomColumnDisplayText;
        }

        private void GridView1_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            if (e.Column.FieldName == "TRANGTHAIXOA")
            {
                if (e.Value == DBNull.Value)
                {
                    e.DisplayText = "Không xác định";
                }
                else
                {
                    bool trangThaiXoa = Convert.ToBoolean(e.Value);
                    e.DisplayText = trangThaiXoa ? "Đã nghỉ việc" : "Đang làm việc";
                }
            }
            else if (e.Column.FieldName == "NGAYSINH")
            {
                if (e.Value != DBNull.Value && e.Value is DateTime date)
                {
                    e.DisplayText = date.ToString("dd/MM/yyyy");
                }
                else
                {
                    e.DisplayText = "Không xác định";
                }
            }
        }

        private void LoadData()
        {
            try
            {
                // Ensure database connection is established
                if (Program.conn.State == ConnectionState.Closed)
                    Program.conn.Open();

                // Create SQL command to execute the stored procedure
                using (SqlCommand cmd = new SqlCommand("SP_DanhSachNhanVien_Filter", Program.conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Set parameters for the stored procedure
                    cmd.Parameters.AddWithValue("@FilterHoTen", DBNull.Value); // NULL for no filter
                    cmd.Parameters.AddWithValue("@FilterDiaChi", DBNull.Value); // NULL for no filter
                    cmd.Parameters.AddWithValue("@FilterCCCD", DBNull.Value); // NULL for no filter
                    cmd.Parameters.AddWithValue("@LuongTu", luongTu.HasValue ? (object)luongTu : DBNull.Value); // Filter by LuongTu
                    cmd.Parameters.AddWithValue("@LuongDen", luongDen.HasValue ? (object)luongDen : DBNull.Value); // Filter by LuongDen
                    cmd.Parameters.AddWithValue("@TrangThaiXoa", trangThaiXoa.HasValue ? (object)trangThaiXoa : DBNull.Value); // Filter by TrangThaiXoa
                    cmd.Parameters.AddWithValue("@SapXepTheo", sapXepTheo); // Sorting field
                    cmd.Parameters.AddWithValue("@ThuTuSapXep", thuTuSapXep); // Sorting order

                    // Output parameter for total records
                    SqlParameter tongSoBanGhiParam = new SqlParameter("@TongSoBanGhi", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(tongSoBanGhiParam);

                    // Execute the stored procedure and load data into a DataTable
                    DataTable dt = new DataTable();
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }

                    // Bind the DataTable to the grid control
                    gcNhanVien.DataSource = dt;

                    // Display total records in label
                    int tongSoBanGhi = tongSoBanGhiParam.Value != DBNull.Value ? Convert.ToInt32(tongSoBanGhiParam.Value) : 0;
                    label1.Text = $"DANH SÁCH THÔNG TIN CHI TIẾT NHÂN VIÊN (Tổng: {tongSoBanGhi} nhân viên)";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (Program.conn.State == ConnectionState.Open)
                    Program.conn.Close();
            }
        }

        // Xử lý nút "XEM TRƯỚC"
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                // Tạo instance của báo cáo
                ReportDanhSachNhanVien report = new ReportDanhSachNhanVien();

                // Lấy dữ liệu từ stored procedure
                DataTable dt = GetReportData();

                // Gán dữ liệu cho báo cáo
                report.DataSource = dt;

                // Hiển thị cửa sổ xem trước báo cáo
                using (ReportPrintTool printTool = new ReportPrintTool(report))
                {
                    printTool.ShowPreviewDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xem trước báo cáo: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Xử lý nút "XUẤT BẢN"
        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                // Tạo instance của báo cáo
                ReportDanhSachNhanVien report = new ReportDanhSachNhanVien();

                // Lấy dữ liệu từ stored procedure
                DataTable dt = GetReportData();

                // Gán dữ liệu cho báo cáo
                report.DataSource = dt;

                // Đường dẫn lưu file PDF
                string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "DanhSachNhanVien.pdf");

                // Xuất báo cáo ra file PDF
                report.ExportToPdf(filePath);

                MessageBox.Show($"Báo cáo đã được xuất ra file PDF tại: {filePath}", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Mở file PDF
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(filePath) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xuất báo cáo: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Hàm lấy dữ liệu cho báo cáo
        private DataTable GetReportData()
        {
            DataTable dt = new DataTable();
            try
            {
                if (Program.conn.State == ConnectionState.Closed)
                    Program.conn.Open();

                using (SqlCommand cmd = new SqlCommand("SP_DanhSachNhanVien_Filter", Program.conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Set parameters for the stored procedure
                    cmd.Parameters.AddWithValue("@FilterHoTen", DBNull.Value);
                    cmd.Parameters.AddWithValue("@FilterDiaChi", DBNull.Value);
                    cmd.Parameters.AddWithValue("@FilterCCCD", DBNull.Value);
                    cmd.Parameters.AddWithValue("@LuongTu", luongTu.HasValue ? (object)luongTu : DBNull.Value); // Filter by LuongTu
                    cmd.Parameters.AddWithValue("@LuongDen", luongDen.HasValue ? (object)luongDen : DBNull.Value); // Filter by LuongDen
                    cmd.Parameters.AddWithValue("@TrangThaiXoa", trangThaiXoa.HasValue ? (object)trangThaiXoa : DBNull.Value); // Filter by TrangThaiXoa
                    cmd.Parameters.AddWithValue("@SapXepTheo", sapXepTheo); // Sorting field
                    cmd.Parameters.AddWithValue("@ThuTuSapXep", thuTuSapXep); // Sorting order

                    // Output parameter for total records
                    SqlParameter tongSoBanGhiParam = new SqlParameter("@TongSoBanGhi", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(tongSoBanGhiParam);

                    // Execute and fill DataTable
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy dữ liệu báo cáo: {ex.Message}");
            }
            finally
            {
                if (Program.conn.State == ConnectionState.Open)
                    Program.conn.Close();
            }
            return dt;
        }

        private void textEdit1_EditValueChanged(object sender, EventArgs e)
        {
            // Kiểm tra giá trị nhập vào textEdit1
            if (string.IsNullOrWhiteSpace(textEdit1.Text))
            {
                luongTu = null;
                textEdit2.Enabled = false;
                textEdit2.Text = string.Empty;
                tempLuongDen = null;
                checkButton1.Enabled = false;
            }
            else if (double.TryParse(textEdit1.Text, out double value) && value >= 0)
            {
                luongTu = value;
                textEdit2.Enabled = true; // Kích hoạt textEdit2
                checkButton1.Enabled = true; // Kích hoạt checkButton1
            }
            else
            {
                MessageBox.Show("Vui lòng nhập số không âm cho Lương từ.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textEdit1.Text = string.Empty;
                luongTu = null;
                textEdit2.Enabled = false;
                textEdit2.Text = string.Empty;
                tempLuongDen = null;
                checkButton1.Enabled = false;
            }
            LoadData(); // Cập nhật GridView
        }

        private void textEdit2_EditValueChanged(object sender, EventArgs e)
        {
            // Chỉ lưu giá trị tạm thời, không kiểm tra ngay
            if (string.IsNullOrWhiteSpace(textEdit2.Text))
            {
                tempLuongDen = null;
            }
            else if (double.TryParse(textEdit2.Text, out double value) && value >= 0)
            {
                tempLuongDen = value;
            }
            else
            {
                MessageBox.Show("Vui lòng nhập số không âm cho Lương đến.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textEdit2.Text = string.Empty;
                tempLuongDen = null;
            }
        }

        private void checkButton1_CheckedChanged(object sender, EventArgs e)
        {
            // Kiểm tra khi checkButton1 được nhấn
            if (checkButton1.Checked)
            {
                if (tempLuongDen.HasValue)
                {
                    if (luongTu.HasValue && tempLuongDen.Value <= luongTu.Value)
                    {
                        MessageBox.Show("Lương đến phải lớn hơn Lương từ.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        textEdit2.Text = string.Empty;
                        tempLuongDen = null;
                        checkButton1.Checked = false;
                    }
                    else
                    {
                        luongDen = tempLuongDen;
                        LoadData(); // Áp dụng bộ lọc vào GridView
                    }
                }
                else
                {
                    MessageBox.Show("Vui lòng nhập giá trị cho Lương đến.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    checkButton1.Checked = false;
                }
            }
            else
            {
                // Nếu bỏ check, xóa bộ lọc Lương đến
                luongDen = null;
                tempLuongDen = null;
                textEdit2.Text = string.Empty;
                LoadData(); // Cập nhật GridView
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Xử lý bộ lọc trạng thái
            switch (comboBox1.SelectedIndex)
            {
                case 0: // Đang đi làm
                    trangThaiXoa = false; // TRANGTHAIXOA = 0
                    break;
                case 1: // Đã nghỉ
                    trangThaiXoa = true; // TRANGTHAIXOA = 1
                    break;
                case 2: // Cả 2
                    trangThaiXoa = null; // TRANGTHAIXOA = NULL
                    break;
            }
            LoadData(); // Cập nhật GridView
        }

        private void filter_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Xử lý sắp xếp
            switch (filter.SelectedIndex)
            {
                case 0: // Tên: Từ thấp đến cao
                    sapXepTheo = "TEN";
                    thuTuSapXep = "ASC";
                    break;
                case 1: // Tên: Từ cao đến thấp
                    sapXepTheo = "TEN";
                    thuTuSapXep = "DESC";
                    break;
                case 2: // Lương: Từ thấp đến cao
                    sapXepTheo = "LUONG";
                    thuTuSapXep = "ASC";
                    break;
                case 3: // Lương: Từ cao đến thấp
                    sapXepTheo = "LUONG";
                    thuTuSapXep = "DESC";
                    break;
            }
            LoadData(); // Cập nhật GridView
        }

        private void label3_Click(object sender, EventArgs e)
        {
        }

        private void label2_Click(object sender, EventArgs e)
        {
        }

        private void label4_Click(object sender, EventArgs e)
        {
        }
    }
}