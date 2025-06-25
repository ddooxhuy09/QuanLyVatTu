using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraReports.UI;
using QLVT;
using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Windows.Forms;

namespace QLVT
{
    public partial class FormDanhSachVatTu : Form
    {
        private string sapXepTheo = "TENVT"; // Mặc định sắp xếp theo tên vật tư
        private string thuTuSapXep = "ASC"; // Mặc định tăng dần

        public FormDanhSachVatTu()
        {
            InitializeComponent();
            // Thiết lập giá trị mặc định cho filter
            filter.SelectedIndex = 0; // Mặc định "Số lượng tồn: Từ thấp đến cao"
            LoadData(); // Gọi phương thức tải dữ liệu khi form khởi tạo
            // Gắn sự kiện click cho các nút
            button1.Click += new EventHandler(button1_Click);
            button2.Click += new EventHandler(button2_Click);
        }

        private void LoadData()
        {
            try
            {
                // Ensure database connection is established
                if (Program.conn.State == ConnectionState.Closed)
                    Program.conn.Open();

                // Gọi stored procedure SP_DanhMucVatTu_Filter
                using (SqlCommand command = new SqlCommand("SP_DanhMucVatTu_Filter", Program.conn))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Thêm các tham số cho stored procedure
                    command.Parameters.AddWithValue("@FilterTenVT", DBNull.Value); // Không lọc theo tên vật tư
                    command.Parameters.AddWithValue("@FilterDVT", DBNull.Value); // Không lọc theo đơn vị tính
                    command.Parameters.AddWithValue("@SoLuongTonTu", DBNull.Value); // Không lọc số lượng tồn từ
                    command.Parameters.AddWithValue("@SoLuongTonDen", DBNull.Value); // Không lọc số lượng tồn đến
                    command.Parameters.AddWithValue("@ChiLayVatTuConHang", 0); // Không chỉ lấy vật tư còn hàng
                    command.Parameters.AddWithValue("@ChiLayVatTuHetHang", 0); // Không chỉ lấy vật tư hết hàng
                    command.Parameters.AddWithValue("@SapXepTheo", sapXepTheo); // Sắp xếp theo trường được chọn
                    command.Parameters.AddWithValue("@ThuTuSapXep", thuTuSapXep); // Sắp xếp theo thứ tự được chọn

                    // Tham số OUTPUT để lấy tổng số bản ghi
                    SqlParameter tongSoBanGhiParam = new SqlParameter("@TongSoBanGhi", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    command.Parameters.Add(tongSoBanGhiParam);

                    // Tạo DataTable để lưu kết quả
                    DataTable dataTable = new DataTable();
                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        adapter.Fill(dataTable);
                    }

                    // Gán dữ liệu vào GridControl
                    gcVatTu.DataSource = dataTable;

                    // Lấy tổng số bản ghi từ tham số OUTPUT
                    int tongSoBanGhi = tongSoBanGhiParam.Value != DBNull.Value ? Convert.ToInt32(tongSoBanGhiParam.Value) : 0;
                    label1.Text = $"DANH SÁCH THÔNG TIN CHI TIẾT VẬT TƯ (Tổng: {tongSoBanGhi} bản ghi)";
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

        // Xử lý sự kiện cho nút "XEM TRƯỚC"
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                // Tạo instance của báo cáo
                ReportDanhSachVatTu report = new ReportDanhSachVatTu();

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

        // Xử lý sự kiện cho nút "XUẤT BẢN"
        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                // Tạo instance của báo cáo
                ReportDanhSachVatTu report = new ReportDanhSachVatTu();

                // Lấy dữ liệu từ stored procedure
                DataTable dt = GetReportData();

                // Gán dữ liệu cho báo cáo
                report.DataSource = dt;

                // Đường dẫn lưu file PDF
                string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "DanhSachVatTu.pdf");

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

                using (SqlCommand command = new SqlCommand("SP_DanhMucVatTu_Filter", Program.conn))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Thêm các tham số cho stored procedure
                    command.Parameters.AddWithValue("@FilterTenVT", DBNull.Value);
                    command.Parameters.AddWithValue("@FilterDVT", DBNull.Value);
                    command.Parameters.AddWithValue("@SoLuongTonTu", DBNull.Value);
                    command.Parameters.AddWithValue("@SoLuongTonDen", DBNull.Value);
                    command.Parameters.AddWithValue("@ChiLayVatTuConHang", 0);
                    command.Parameters.AddWithValue("@ChiLayVatTuHetHang", 0);
                    command.Parameters.AddWithValue("@SapXepTheo", sapXepTheo); // Sắp xếp theo trường được chọn
                    command.Parameters.AddWithValue("@ThuTuSapXep", thuTuSapXep); // Sắp xếp theo thứ tự được chọn

                    // Tham số OUTPUT để lấy tổng số bản ghi
                    SqlParameter tongSoBanGhiParam = new SqlParameter("@TongSoBanGhi", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    command.Parameters.Add(tongSoBanGhiParam);

                    // Execute và điền DataTable
                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        adapter.Fill(dt);
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

        private void filter_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Xử lý sắp xếp theo số lượng tồn
            switch (filter.SelectedIndex)
            {
                case 0: // Số lượng tồn: Từ thấp đến cao
                    sapXepTheo = "SOLUONGTON";
                    thuTuSapXep = "ASC";
                    break;
                case 1: // Số lượng tồn: Từ cao đến thấp
                    sapXepTheo = "SOLUONGTON";
                    thuTuSapXep = "DESC";
                    break;
            }
            LoadData(); // Cập nhật GridView
        }

        private void label1_Click(object sender, EventArgs e)
        {
        }
    }
}