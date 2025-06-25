using DevExpress.XtraReports.UI;
using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Windows.Forms;

namespace QLVT
{
    public partial class FormTongHopNhapXuat : Form
    {
        private string sapXepTheo = "NGAY"; // Mặc định sắp xếp theo ngày
        private string thuTuSapXep = "ASC"; // Mặc định tăng dần

        public FormTongHopNhapXuat()
        {
            InitializeComponent();
            // Vô hiệu hóa dteToiNgay khi khởi tạo
            dteToiNgay.Enabled = false;
            // Đặt giá trị mặc định cho filter
            filter.Items.AddRange(new object[]
            {
                "Ngày: Từ thấp đến cao",
                "Ngày: Từ cao đến thấp",
                "Nhập: Từ thấp đến cao",
                "Nhập: Từ cao đến thấp",
                "Xuất: Từ thấp đến cao",
                "Xuất: Từ cao đến thấp"
            });
            filter.SelectedIndex = 0; // Chọn "Ngày: Từ thấp đến cao" mặc định
            // Gắn sự kiện
            dteTuNgay.EditValueChanged += new EventHandler(dteTuNgay_EditValueChanged);
            dteToiNgay.EditValueChanged += new EventHandler(dteToiNgay_EditValueChanged);
            filter.SelectedIndexChanged += new EventHandler(filter_SelectedIndexChanged);
            button1.Click += new EventHandler(button1_Click);
            button2.Click += new EventHandler(button2_Click);
        }

        private void dteTuNgay_EditValueChanged(object sender, EventArgs e)
        {
            if (dteTuNgay.EditValue != null && dteTuNgay.DateTime != DateTime.MinValue)
            {
                dteToiNgay.Enabled = true;
                dteToiNgay.Properties.MinDate = dteTuNgay.DateTime.AddDays(1);
                if (dteToiNgay.DateTime < dteTuNgay.DateTime)
                {
                    dteToiNgay.EditValue = null;
                }
            }
            else
            {
                dteToiNgay.Enabled = false;
                dteToiNgay.EditValue = null;
            }
        }

        private void dteToiNgay_EditValueChanged(object sender, EventArgs e)
        {
            if (dteToiNgay.EditValue != null && dteToiNgay.DateTime != DateTime.MinValue)
            {
                if (dteToiNgay.DateTime < dteTuNgay.DateTime)
                {
                    MessageBox.Show("Ngày 'Tới Ngày' phải sau ngày 'Từ Ngày'.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    dteToiNgay.EditValue = null;
                }
            }
        }

        private void filter_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Xử lý sắp xếp
            switch (filter.SelectedIndex)
            {
                case 0: // Ngày: Từ thấp đến cao
                    sapXepTheo = "NGAY";
                    thuTuSapXep = "ASC";
                    break;
                case 1: // Ngày: Từ cao đến thấp
                    sapXepTheo = "NGAY";
                    thuTuSapXep = "DESC";
                    break;
                case 2: // Nhập: Từ thấp đến cao
                    sapXepTheo = "NHAP";
                    thuTuSapXep = "ASC";
                    break;
                case 3: // Nhập: Từ cao đến thấp
                    sapXepTheo = "NHAP";
                    thuTuSapXep = "DESC";
                    break;
                case 4: // Xuất: Từ thấp đến cao
                    sapXepTheo = "XUAT";
                    thuTuSapXep = "ASC";
                    break;
                case 5: // Xuất: Từ cao đến thấp
                    sapXepTheo = "XUAT";
                    thuTuSapXep = "DESC";
                    break;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (dteTuNgay.EditValue == null || dteToiNgay.EditValue == null)
                {
                    MessageBox.Show("Vui lòng chọn 'Từ Ngày' và 'Tới Ngày'.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                ReportTongHopNhapXuat report = new ReportTongHopNhapXuat();
                DataSet ds = GetReportData();

                // Lấy dữ liệu chính (tập hợp đầu tiên)
                report.DataSource = ds.Tables[0];

                // Gán giá trị cho các nhãn
                report.txtTuNgay.Text = dteTuNgay.DateTime.ToString("dd/MM/yyyy");
                report.txtToiNgay.Text = dteToiNgay.DateTime.ToString("dd/MM/yyyy");

                // Hiển thị báo cáo
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

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (dteTuNgay.EditValue == null || dteToiNgay.EditValue == null)
                {
                    MessageBox.Show("Vui lòng chọn 'Từ Ngày' và 'Tới Ngày'.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                ReportTongHopNhapXuat report = new ReportTongHopNhapXuat();
                DataSet ds = GetReportData();

                // Lấy dữ liệu chính (tập hợp đầu tiên)
                report.DataSource = ds.Tables[0];

                // Gán giá trị cho các nhãn
                report.txtTuNgay.Text = dteTuNgay.DateTime.ToString("dd/MM/yyyy");
                report.txtToiNgay.Text = dteToiNgay.DateTime.ToString("dd/MM/yyyy");

                // Xuất báo cáo ra PDF
                string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "TongHopNhapXuat.pdf");
                report.ExportToPdf(filePath);

                MessageBox.Show($"Báo cáo đã được xuất ra file PDF tại: {filePath}", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(filePath) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xuất báo cáo: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private DataSet GetReportData()
        {
            DataSet ds = new DataSet();
            try
            {
                if (Program.conn.State == ConnectionState.Closed)
                    Program.conn.Open();

                using (SqlCommand cmd = new SqlCommand("SP_Report_TongHop_NhapXuat", Program.conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Set tham số
                    cmd.Parameters.AddWithValue("@TuNgay", dteTuNgay.DateTime);
                    cmd.Parameters.AddWithValue("@DenNgay", dteToiNgay.DateTime);
                    cmd.Parameters.AddWithValue("@SapXepTheo", sapXepTheo);
                    cmd.Parameters.AddWithValue("@ThuTuSapXep", thuTuSapXep);

                    // Lấy dữ liệu
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(ds);
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
            return ds;
        }
    }
}