using DevExpress.XtraReports.UI;
using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Windows.Forms;

namespace QLVT
{
    public partial class FormChiTietSoLuongHangHoaNhapXuat : Form
    {
        private decimal tongTriGia; // Biến lưu tổng trị giá
        private string sapXepTheo = "THANG"; // Mặc định sắp xếp theo tháng
        private string thuTuSapXep = "ASC"; // Mặc định tăng dần

        public FormChiTietSoLuongHangHoaNhapXuat()
        {
            InitializeComponent();
            // Vô hiệu hóa dteToiNgay khi khởi tạo form
            dteToiNgay.Enabled = false;
            // Đặt giá trị mặc định cho cmbCHINHANH và filter
            cmbCHINHANH.SelectedIndex = 0; // Chọn "Nhập" mặc định
            filter.SelectedIndex = 0; // Chọn "Tháng: Từ thấp đến cao" mặc định
            // Gắn sự kiện click cho các nút
            button1.Click += new EventHandler(button1_Click);
            button2.Click += new EventHandler(button2_Click);
        }

        private void cmbCHINHANH_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Không cần xử lý thêm vì LoaiBangKe được ánh xạ trong GetReportData
        }

        private void dteTuNgay_EditValueChanged(object sender, EventArgs e)
        {
            if (dteTuNgay.EditValue != null && dteTuNgay.DateTime != DateTime.MinValue)
            {
                // Kích hoạt dteToiNgay
                dteToiNgay.Enabled = true;
                // Đặt ngày tối thiểu cho dteToiNgay là ngày sau dteTuNgay
                dteToiNgay.Properties.MinDate = dteTuNgay.DateTime.AddDays(1);
                // Xóa giá trị hiện tại của dteToiNgay nếu nó nhỏ hơn ngày tối thiểu
                if (dteToiNgay.DateTime < dteTuNgay.DateTime)
                {
                    dteToiNgay.EditValue = null;
                }
            }
            else
            {
                // Vô hiệu hóa dteToiNgay nếu dteTuNgay không có giá trị
                dteToiNgay.Enabled = false;
                dteToiNgay.EditValue = null;
            }
        }

        private void dteToiNgay_EditValueChanged(object sender, EventArgs e)
        {
            if (dteToiNgay.EditValue != null && dteToiNgay.DateTime != DateTime.MinValue)
            {
                // Đảm bảo ngày ở dteToiNgay không nhỏ hơn dteTuNgay
                if (dteToiNgay.DateTime < dteTuNgay.DateTime)
                {
                    MessageBox.Show("Ngày 'Tới Ngày' phải sau ngày 'Từ Ngày'.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    dteToiNgay.EditValue = null;
                }
            }
        }

        private void filter_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Xử lý sắp xếp theo các tùy chọn
            switch (filter.SelectedIndex)
            {
                case 0: // Tháng: Từ thấp đến cao
                    sapXepTheo = "THANG";
                    thuTuSapXep = "ASC";
                    break;
                case 1: // Tháng: Từ cao đến thấp
                    sapXepTheo = "THANG";
                    thuTuSapXep = "DESC";
                    break;
                case 2: // Mã vật tư: Từ thấp đến cao
                    sapXepTheo = "MAVT";
                    thuTuSapXep = "ASC";
                    break;
                case 3: // Mã vật tư: Từ cao đến thấp
                    sapXepTheo = "MAVT";
                    thuTuSapXep = "DESC";
                    break;
                case 4: // Tên vật tư: Từ thấp đến cao
                    sapXepTheo = "TENVT";
                    thuTuSapXep = "ASC";
                    break;
                case 5: // Tên vật tư: Từ cao đến thấp
                    sapXepTheo = "TENVT";
                    thuTuSapXep = "DESC";
                    break;
                case 6: // Trị giá: Từ thấp đến cao
                    sapXepTheo = "TRIGIA";
                    thuTuSapXep = "ASC";
                    break;
                case 7: // Trị giá: Từ cao đến thấp
                    sapXepTheo = "TRIGIA";
                    thuTuSapXep = "DESC";
                    break;
            }
        }

        // Xử lý nút "XEM TRƯỚC"
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                // Kiểm tra giá trị đầu vào
                if (dteTuNgay.EditValue == null || dteToiNgay.EditValue == null)
                {
                    MessageBox.Show("Vui lòng chọn 'Từ Ngày' và 'Tới Ngày'.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Tạo instance của báo cáo
                ReportChiTietSoLuongTriGiaHangHoa report = new ReportChiTietSoLuongTriGiaHangHoa();

                // Lấy dữ liệu từ stored procedure
                DataTable dt = GetReportData();

                // Gán dữ liệu cho báo cáo
                report.DataSource = dt;

                // Gán giá trị cho các tham số với kiểm tra null
                if (report.Parameters["TuNgay"] != null)
                {
                    report.Parameters["TuNgay"].Value = dteTuNgay.DateTime;
                }
                else
                {
                    report.TuNgayLabel.Text = dteTuNgay.DateTime.ToString("dd/MM/yyyy");
                }

                if (report.Parameters["DenNgay"] != null)
                {
                    report.Parameters["DenNgay"].Value = dteToiNgay.DateTime;
                }
                else
                {
                    report.DenNgayLabel.Text = dteToiNgay.DateTime.ToString("dd/MM/yyyy");
                }

                if (report.Parameters["LoaiPhieu"] != null)
                {
                    report.Parameters["LoaiPhieu"].Value = cmbCHINHANH.SelectedItem?.ToString() ?? "Cả hai";
                }
                else
                {
                    report.LoaiPhieuLabel.Text = cmbCHINHANH.SelectedItem?.ToString() ?? "Cả hai";
                }

                if (report.Parameters["TongTriGia"] != null)
                {
                    report.Parameters["TongTriGia"].Value = tongTriGia;
                }
                else
                {
                    report.TongTriGiaLabel.Text = tongTriGia.ToString("#,##0");
                }

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
                // Kiểm tra giá trị đầu vào
                if (dteTuNgay.EditValue == null || dteToiNgay.EditValue == null)
                {
                    MessageBox.Show("Vui lòng chọn 'Từ Ngày' và 'Tới Ngày'.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Tạo instance của báo cáo
                ReportChiTietSoLuongTriGiaHangHoa report = new ReportChiTietSoLuongTriGiaHangHoa();

                // Lấy dữ liệu từ stored procedure
                DataTable dt = GetReportData();

                // Gán dữ liệu cho báo cáo
                report.DataSource = dt;

                // Gán giá trị cho các tham số với kiểm tra null
                if (report.Parameters["TuNgay"] != null)
                {
                    report.Parameters["TuNgay"].Value = dteTuNgay.DateTime;
                }
                else
                {
                    report.TuNgayLabel.Text = dteTuNgay.DateTime.ToString("dd/MM/yyyy");
                }

                if (report.Parameters["DenNgay"] != null)
                {
                    report.Parameters["DenNgay"].Value = dteToiNgay.DateTime;
                }
                else
                {
                    report.DenNgayLabel.Text = dteToiNgay.DateTime.ToString("dd/MM/yyyy");
                }

                if (report.Parameters["LoaiPhieu"] != null)
                {
                    report.Parameters["LoaiPhieu"].Value = cmbCHINHANH.SelectedItem?.ToString() ?? "Cả hai";
                }
                else
                {
                    report.LoaiPhieuLabel.Text = cmbCHINHANH.SelectedItem?.ToString() ?? "Cả hai";
                }

                if (report.Parameters["TongTriGia"] != null)
                {
                    report.Parameters["TongTriGia"].Value = tongTriGia;
                }
                else
                {
                    report.TongTriGiaLabel.Text = tongTriGia.ToString("#,##0");
                }

                // Đường dẫn lưu file PDF
                string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "ChiTietSoLuongTriGiaHangHoa.pdf");

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

                using (SqlCommand cmd = new SqlCommand("SP_BangKe_NhapXuat_TheoThang", Program.conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Ánh xạ giá trị từ combobox sang tham số @LoaiBangKe
                    string loaiBangKe = "NHAP";
                    if (cmbCHINHANH.SelectedItem != null)
                    {
                        switch (cmbCHINHANH.SelectedItem.ToString())
                        {
                            case "Nhập":
                                loaiBangKe = "NHAP";
                                break;
                            case "Xuất":
                                loaiBangKe = "XUAT";
                                break;
                            case "Cả hai":
                                loaiBangKe = "BOTH";
                                break;
                        }
                    }

                    // Set tham số cho stored procedure
                    cmd.Parameters.AddWithValue("@LoaiBangKe", loaiBangKe);
                    cmd.Parameters.AddWithValue("@TuNgay", dteTuNgay.DateTime);
                    cmd.Parameters.AddWithValue("@DenNgay", dteToiNgay.DateTime);
                    cmd.Parameters.AddWithValue("@SapXepTheo", sapXepTheo);
                    cmd.Parameters.AddWithValue("@ThuTuSapXep", thuTuSapXep);

                    // Tham số đầu ra
                    SqlParameter tongSoBanGhiParam = new SqlParameter("@TongSoBanGhi", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(tongSoBanGhiParam);

                    SqlParameter tongSoLuongParam = new SqlParameter("@TongSoLuong", SqlDbType.Decimal)
                    {
                        Direction = ParameterDirection.Output,
                        Precision = 18,
                        Scale = 2
                    };
                    cmd.Parameters.Add(tongSoLuongParam);

                    SqlParameter tongTriGiaParam = new SqlParameter("@TongTriGia", SqlDbType.Decimal)
                    {
                        Direction = ParameterDirection.Output,
                        Precision = 18,
                        Scale = 2
                    };
                    cmd.Parameters.Add(tongTriGiaParam);

                    // Thực thi và lấy dữ liệu vào DataTable
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }

                    // Lưu tổng trị giá
                    tongTriGia = tongTriGiaParam.Value != DBNull.Value ? Convert.ToDecimal(tongTriGiaParam.Value) : 0;
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

        private void FormChiTietSoLuongHangHoaNhapXuat_Load(object sender, EventArgs e)
        {
        }

        private void label5_Click(object sender, EventArgs e)
        {
        }
    }
}