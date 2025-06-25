using DevExpress.XtraGrid.Views.Grid;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraReports.UI;

namespace QLVT
{
    public partial class FormHoatDongNhanVien : Form
    {
        private string filterHoTen = null;
        private string filterCCCD = null;
        private string filterDiaChi = null;
        private string selectedFilter = "HOTEN"; // Mặc định tìm kiếm theo Họ + Tên
        private string loaiPhieu = "CAHAI"; // Mặc định lấy cả hai loại phiếu
        private int? maNV = null; // Mã nhân viên (NULL để lấy tất cả)
        private string sapXepTheo = "TEN"; // Mặc định sắp xếp theo tên
        private string thuTuSapXep = "ASC"; // Mặc định tăng dần
        private bool? trangThaiXoa = null;

        public FormHoatDongNhanVien()
        {
            InitializeComponent();

            dteToiNgay.Enabled = false;

            dteTuNgay.EditValueChanged += new EventHandler(dteTuNgay_EditValueChanged);
            dteToiNgay.EditValueChanged += new EventHandler(dteToiNgay_EditValueChanged);
            // Thêm các mục vào cmbLoaiPhieu
            cmbLoaiPhieu.Items.AddRange(new[] { "NHAP", "XUAT", "CAHAI" });
            // Thiết lập giá trị mặc định
            filter.SelectedIndex = 0; // Họ + Tên
            if (cmbLoaiPhieu.Items.Count > 2)
            {
                cmbLoaiPhieu.SelectedIndex = 2; // CAHAI
            }
            else
            {
                cmbLoaiPhieu.SelectedIndex = 0; // Mặc định chọn mục đầu tiên nếu không đủ
            }
            // Gắn sự kiện
            filter.SelectedIndexChanged += filter_SelectedIndexChanged_1;
            cmbLoaiPhieu.SelectedIndexChanged += CmbLoaiPhieu_SelectedIndexChanged;
            // Tải danh sách nhân viên ban đầu
            LoadNhanVienData();

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
        }

        private void LoadNhanVienData()
        {
            try
            {
                if (Program.conn.State == ConnectionState.Closed)
                    Program.conn.Open();

                using (SqlCommand cmd = new SqlCommand("SP_DanhSachNhanVien_Filter", Program.conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Thiết lập tham số cho stored procedure
                    cmd.Parameters.AddWithValue("@FilterHoTen", string.IsNullOrWhiteSpace(filterHoTen) ? (object)DBNull.Value : filterHoTen);
                    cmd.Parameters.AddWithValue("@FilterDiaChi", string.IsNullOrWhiteSpace(filterDiaChi) ? (object)DBNull.Value : filterDiaChi);
                    cmd.Parameters.AddWithValue("@FilterCCCD", string.IsNullOrWhiteSpace(filterCCCD) ? (object)DBNull.Value : filterCCCD);
                    cmd.Parameters.AddWithValue("@LuongTu", DBNull.Value);
                    cmd.Parameters.AddWithValue("@LuongDen", DBNull.Value);
                    cmd.Parameters.AddWithValue("@TrangThaiXoa", trangThaiXoa.HasValue ? (object)trangThaiXoa : DBNull.Value);
                    cmd.Parameters.AddWithValue("@SapXepTheo", sapXepTheo);
                    cmd.Parameters.AddWithValue("@ThuTuSapXep", thuTuSapXep);

                    // Tham số đầu ra cho tổng số bản ghi
                    SqlParameter tongSoBanGhiParam = new SqlParameter("@TongSoBanGhi", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(tongSoBanGhiParam);

                    // Thực thi và đổ dữ liệu vào DataTable
                    DataTable dt = new DataTable();
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }

                    // Gán dữ liệu vào GridControl
                    gcNhanVien.DataSource = dt;

                    // Cập nhật tiêu đề với tổng số bản ghi
                    int tongSoBanGhi = tongSoBanGhiParam.Value != DBNull.Value ? Convert.ToInt32(tongSoBanGhiParam.Value) : 0;
                    label1.Text = $"DANH SÁCH NHÂN VIÊN (Tổng: {tongSoBanGhi} nhân viên)";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải danh sách nhân viên: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (Program.conn.State == ConnectionState.Open)
                    Program.conn.Close();
            }
        }

        private void filter_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            // Cập nhật tiêu chí tìm kiếm dựa trên ComboBox
            switch (filter.SelectedIndex)
            {
                case 0: selectedFilter = "HOTEN"; break;
                case 1: selectedFilter = "CCCD"; break;
                case 2: selectedFilter = "DIACHI"; break;
                default: selectedFilter = "HOTEN"; break;
            }
            // Xóa giá trị tìm kiếm cũ khi thay đổi tiêu chí
            textEdit2.Text = string.Empty;
            filterHoTen = null;
            filterCCCD = null;
            filterDiaChi = null;
            // Tải lại danh sách nhân viên
            LoadNhanVienData();
        }

        private void textEdit2_EditValueChanged(object sender, EventArgs e)
        {
            // Cập nhật giá trị tìm kiếm
            string searchValue = string.IsNullOrWhiteSpace(textEdit2.Text) ? null : textEdit2.Text.Trim();
            filterHoTen = selectedFilter == "HOTEN" ? searchValue : null;
            filterCCCD = selectedFilter == "CCCD" ? searchValue : null;
            filterDiaChi = selectedFilter == "DIACHI" ? searchValue : null;
            // Không tải dữ liệu ngay để chờ nhấn nút Tìm
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // Kích hoạt tìm kiếm khi nhấn nút Tìm
            if (string.IsNullOrWhiteSpace(textEdit2.Text))
            {
                MessageBox.Show("Vui lòng nhập giá trị tìm kiếm.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            // Tải danh sách nhân viên với bộ lọc
            LoadNhanVienData();
        }

        private void gcNhanVien_Click(object sender, EventArgs e)
        {
            try
            {
                int focusedRowHandle = gridView1.FocusedRowHandle;
                if (focusedRowHandle < 0)
                {
                    txtMaNhanVien.Text = string.Empty;
                    txtHoVaTen.Text = string.Empty;
                    txtNgaySinh.Text = string.Empty;
                    txtDiaChi.Text = string.Empty;
                    maNV = null;
                    LoadNhanVienData(); // Tải lại danh sách nhân viên
                    return;
                }

                object maNVValue = gridView1.GetRowCellValue(focusedRowHandle, "MANV");
                object ho = gridView1.GetRowCellValue(focusedRowHandle, "HO");
                object ten = gridView1.GetRowCellValue(focusedRowHandle, "TEN");
                object ngaySinh = gridView1.GetRowCellValue(focusedRowHandle, "NGAYSINH");
                object diaChi = gridView1.GetRowCellValue(focusedRowHandle, "DIACHI");

                txtMaNhanVien.Text = maNVValue != DBNull.Value ? maNVValue.ToString() : string.Empty;
                txtHoVaTen.Text = (ho != DBNull.Value && ten != DBNull.Value) ? $"{ho} {ten}" : string.Empty;
                txtNgaySinh.Text = ngaySinh != DBNull.Value ? Convert.ToDateTime(ngaySinh).ToString("dd/MM/yyyy") : string.Empty;
                txtDiaChi.Text = diaChi != DBNull.Value ? diaChi.ToString() : string.Empty;

                maNV = maNVValue != DBNull.Value ? Convert.ToInt32(maNVValue) : (int?)null;
                // Không gọi LoadData vì hiện tại chỉ tải danh sách nhân viên
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi hiển thị thông tin nhân viên: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CmbLoaiPhieu_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (cmbLoaiPhieu.SelectedIndex)
            {
                case 0: loaiPhieu = "NHAP"; break;
                case 1: loaiPhieu = "XUAT"; break;
                case 2: loaiPhieu = "CAHAI"; break;
                default: loaiPhieu = "CAHAI"; break;
            }
            // Không gọi LoadData vì hiện tại chỉ tải danh sách nhân viên
        }

        private void label9_Click(object sender, EventArgs e)
        {
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // TODO: Xử lý xuất báo cáo
        }

        private void button1_Click(object sender, EventArgs e)
        {
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

        private void FormHoatDongNhanVien_Load(object sender, EventArgs e)
        {
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            try
            {
                // Kiểm tra tham số bắt buộc
                if (!maNV.HasValue)
                {
                    MessageBox.Show("Vui lòng chọn một nhân viên.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                if (dteTuNgay.EditValue == null || dteToiNgay.EditValue == null)
                {
                    MessageBox.Show("Vui lòng chọn khoảng thời gian.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                if (Program.conn.State == ConnectionState.Closed)
                    Program.conn.Open();

                using (SqlCommand cmd = new SqlCommand("SP_HoatDong_NhanVien", Program.conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Thiết lập tham số cho stored procedure
                    cmd.Parameters.AddWithValue("@TuNgay", dteTuNgay.EditValue);
                    cmd.Parameters.AddWithValue("@DenNgay", dteToiNgay.EditValue);
                    cmd.Parameters.AddWithValue("@LoaiPhieu", loaiPhieu);
                    cmd.Parameters.AddWithValue("@MaNV", maNV.Value);
                    cmd.Parameters.AddWithValue("@FilterHoTen", DBNull.Value); // Không sử dụng tìm kiếm theo tên trong báo cáo
                    cmd.Parameters.AddWithValue("@SapXepTheo", "NGAY");
                    cmd.Parameters.AddWithValue("@ThuTuSapXep", "ASC");

                    // Tham số đầu ra
                    SqlParameter tongSoBanGhiParam = new SqlParameter("@TongSoBanGhi", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(tongSoBanGhiParam);
                    SqlParameter tongTriGiaParam = new SqlParameter("@TongTriGia", SqlDbType.Decimal)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(tongTriGiaParam);

                    // Thực thi và lấy dữ liệu vào DataTable
                    DataTable dt = new DataTable();
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }

                    // Tạo báo cáo
                    ReportHoatDongNhanVien report = new ReportHoatDongNhanVien();

                    // Gán dữ liệu cho báo cáo
                    report.DataSource = dt;

                    // Gán các giá trị từ form vào báo cáo
                    report.txtMaNhanVien.Text = txtMaNhanVien.Text;
                    report.txtHoTen.Text = txtHoVaTen.Text;
                    report.txtNgaySinh.Text = txtNgaySinh.Text;
                    report.txtDiaChi.Text = txtDiaChi.Text;
                    report.txtTuNgay.Text = dteTuNgay.DateTime.ToString("dd/MM/yyyy");
                    report.txtToiNgay.Text = dteToiNgay.DateTime.ToString("dd/MM/yyyy");
                    report.txtLoaiPhieu.Text = loaiPhieu == "CAHAI" ? "CẢ HAI" : loaiPhieu;

                    // Hiển thị báo cáo
                    using (ReportPrintTool printTool = new ReportPrintTool(report))
                    {
                        printTool.ShowPreviewDialog();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tạo báo cáo: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (Program.conn.State == ConnectionState.Open)
                    Program.conn.Close();
            }
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            // TODO: Xử lý xuất báo cáo
        }

        private void cmbLoaiPhieu_SelectedIndexChanged_1(object sender, EventArgs e)
        {

        }
    }
}