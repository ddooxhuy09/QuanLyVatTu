using DevExpress.XtraExport.Helpers;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraReports.UI;
using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Windows.Forms;

namespace QLVT
{
    public partial class FormDonHangKhongPhieuNhap : Form
    {
        public FormDonHangKhongPhieuNhap()
        {
            InitializeComponent();
            ConfigureGridView(); // Cấu hình GridView để gộp ô
            LoadData();
            // Gắn sự kiện click cho các nút
            button1.Click += new EventHandler(button1_Click);
            button2.Click += new EventHandler(button2_Click);
            // Gắn sự kiện CustomColumnDisplayText
            gridView1.CustomColumnDisplayText += GridView1_CustomColumnDisplayText;
        }

        private void ConfigureGridView()
        {
            // Bật tính năng gộp ô
            gridView1.OptionsView.AllowCellMerge = true;

            // Cấu hình gộp ô cho các cột cụ thể
            colMASODDH.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.True;
            colNgayDatHang.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.True;
            colNHACC.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.True;
            colMANV.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.True;
            colTenNhanVien.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.True;
            colSoNgayTuKhiDat.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.True;

            // Không gộp ô cho các cột vật tư
            colMAVT.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
            colTENVT.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
            colDVT.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
            colSOLUONG.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
            colDONGIA.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;

            // Sắp xếp dữ liệu theo NGAY DESC để đồng bộ với stored procedure
            gridView1.SortInfo.ClearAndAddRange(new[]
            {
                new GridColumnSortInfo(colNgayDatHang, DevExpress.Data.ColumnSortOrder.Descending),
                new GridColumnSortInfo(colMASODDH, DevExpress.Data.ColumnSortOrder.Ascending)
            });

            // Sự kiện để kiểm soát logic gộp ô
            gridView1.CellMerge += GridView1_CellMerge;
        }

        private void GridView1_CellMerge(object sender, CellMergeEventArgs e)
        {
            // Chỉ gộp các cột được phép (MASODDH, NgayDatHang, NHACC, MANV, TenNhanVien, SoNgayTuKhiDat)
            if (e.Column.FieldName != "MASODDH" && e.Column.FieldName != "NgayDatHang" &&
                e.Column.FieldName != "NHACC" && e.Column.FieldName != "MANV" &&
                e.Column.FieldName != "TenNhanVien" && e.Column.FieldName != "SoNgayTuKhiDat")
            {
                e.Merge = false;
                e.Handled = true;
                return;
            }

            // Lấy giá trị của hai dòng so sánh
            object value1 = gridView1.GetRowCellValue(e.RowHandle1, e.Column);
            object value2 = gridView1.GetRowCellValue(e.RowHandle2, e.Column);

            // Gộp ô nếu giá trị giống nhau và cùng MASODDH
            if (value1 != null && value2 != null && value1.Equals(value2))
            {
                string masoddh1 = gridView1.GetRowCellValue(e.RowHandle1, colMASODDH)?.ToString();
                string masoddh2 = gridView1.GetRowCellValue(e.RowHandle2, colMASODDH)?.ToString();
                e.Merge = masoddh1 == masoddh2;
            }
            else
            {
                e.Merge = false;
            }

            e.Handled = true;
        }

        private void GridView1_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            if (e.Column.FieldName == "MAVT" || e.Column.FieldName == "TENVT" ||
                e.Column.FieldName == "DVT" || e.Column.FieldName == "SOLUONG" ||
                e.Column.FieldName == "DONGIA")
            {
                if (e.Value == DBNull.Value)
                {
                    e.DisplayText = "Không có chi tiết";
                }
            }
            else if (e.Column.FieldName == "NgayDatHang" && e.Value != DBNull.Value && e.Value is DateTime date)
            {
                e.DisplayText = date.ToString("dd/MM/yyyy");
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
                using (SqlCommand cmd = new SqlCommand("SP_DonDatHang_ChuaNhap_Filter", Program.conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

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
                    gcVatTu.DataSource = dt;

                    // Display total records in label
                    int tongSoBanGhi = tongSoBanGhiParam.Value != DBNull.Value ? Convert.ToInt32(tongSoBanGhiParam.Value) : 0;
                    label1.Text = $"ĐƠN HÀNG KHÔNG CÓ PHIẾU NHẬP (Tổng: {tongSoBanGhi} đơn hàng)";
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
                ReportDonHangKhongPhieuNhap report = new ReportDonHangKhongPhieuNhap();

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
                ReportDonHangKhongPhieuNhap report = new ReportDonHangKhongPhieuNhap();

                // Lấy dữ liệu từ stored procedure
                DataTable dt = GetReportData();

                // Gán dữ liệu cho báo cáo
                report.DataSource = dt;

                // Đường dẫn lưu file PDF
                string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "DonHangKhongPhieuNhap.pdf");

                // Xuất báo cáo ra file PDF
                report.ExportToPdf(filePath);

                MessageBox.Show($"Báo cáo đã được xuất ra file PDF tại: {filePath}", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Mở file PDF (tùy chọn)
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

                using (SqlCommand cmd = new SqlCommand("SP_DonDatHang_ChuaNhap_Filter", Program.conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

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
    }
}