using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using DevExpress.DataAccess.Native.Sql.MasterDetail;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;

namespace QLVT
{
    public partial class fmDonDatHang : Form
    {
        int viTri = 0;
        int viTriCTDDH = 0; // Lưu vị trí riêng cho CTDDH

        bool dangThemMoi = false;
        bool dangChinhSua = false;
        bool dangThemMoiCTDDH = false;
        bool dangChinhSuaCTDDH = false;

        Stack<UndoRedoAction> undoStack = new Stack<UndoRedoAction>();
        Stack<UndoRedoAction> redoStack = new Stack<UndoRedoAction>();

        DataTable originalData = new DataTable();

        public fmDonDatHang()
        {
            InitializeComponent();
        }

        private void fmDonDatHang_Load(object sender, EventArgs e)
        {
            qLVTDataSet.EnforceConstraints = false;

            this.taDDH.Connection.ConnectionString = Program.connstr;
            this.taDDH.Fill(this.qLVTDataSet.DDH);

            this.taDSNV.Connection.ConnectionString = Program.connstr;
            this.taDSNV.Fill(this.qLVTDataSet.DSNV);

            this.taVT.Connection.ConnectionString = Program.connstr;
            this.taVT.Fill(this.qLVTDataSet.VATTU);

            this.taCTDDH.Connection.ConnectionString = Program.connstr;
            this.taCTDDH.Fill(this.qLVTDataSet.CTDDH);

            this.taPN.Connection.ConnectionString = Program.connstr;
            this.taPN.Fill(this.qLVTDataSet.PHIEUNHAP);


            originalData = qLVTDataSet.DDH.Copy();

            // Cấu hình txtHoTenNV
            txtHoTenNV.DataBindings.Clear();
            txtHoTenNV.DataSource = bdsDSNV;
            txtHoTenNV.DisplayMember = "HOTEN";
            txtHoTenNV.ValueMember = "MANV";
            txtHoTenNV.DataBindings.Add(new System.Windows.Forms.Binding("SelectedValue", bdsDDH, "MANV", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            txtHoTenNV.Enabled = true; // Ban đầu chỉ hiển thị, không cho chọn

            CapNhatTrangThaiGiaoDien();
            CapNhatMaNVDisplay(); // Dùng hàm mới
        }

        // --- Hàm cập nhật hiển thị Mã NV ---
        private void CapNhatMaNVDisplay()
        {
            if (txtHoTenNV.SelectedValue != null && !(txtHoTenNV.SelectedValue is DBNull))
            {
                txtMaNV.Text = txtHoTenNV.SelectedValue.ToString();
            }
            else
            {
                txtMaNV.Text = "";
            }
        }

        private void txtHoTenNV_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (txtHoTenNV.SelectedValue != null)
            {
                // Chỉ khi chắc chắn SelectedValue không null thì mới gọi ToString()
                txtMaNV.Text = txtHoTenNV.SelectedValue.ToString();
            }
        }


        // --- Quản lý trạng thái giao diện ---     
        private void SetGridPosition(int targetPosition)
        {
            if (bdsDDH.Count <= 0) return;
            if (targetPosition < 0) targetPosition = 0;
            if (targetPosition >= bdsDDH.Count) targetPosition = bdsDDH.Count - 1;

            bdsDDH.Position = targetPosition;
            // Sự kiện bdsDDH_PositionChanged sẽ tự động gọi CapNhatMaNVDisplay()
            gcDDH.Focus();
        }

        private void SetGridFocusByMASODDH(string targetMASODDH, int fallbackPosition)
        {
            if (bdsDDH.Count <= 0 || gridView1.DataRowCount <= 0)
            {
                SetGridPosition(fallbackPosition); return;
            }
            int gridRowHandle = gridView1.LocateByValue("MASODDH", targetMASODDH);
            if (gridRowHandle != GridControl.InvalidRowHandle)
            {
                gridView1.FocusedRowHandle = gridRowHandle;
                gridView1.MakeRowVisible(gridRowHandle, false);
                int bdsIndex = bdsDDH.Find("MASODDH", targetMASODDH);
                if (bdsIndex >= 0 && bdsDDH.Position != bdsIndex)
                {
                    bdsDDH.Position = bdsIndex; // Trigger PositionChanged -> CapNhatMaNVDisplay
                }
                else if (bdsIndex >= 0 && bdsDDH.Position == bdsIndex)
                {
                    // Nếu vị trí không đổi, gọi cập nhật thủ công
                    CapNhatMaNVDisplay();
                }
            }
            else
            {
                Console.WriteLine($"SetGridFocusByMASODDH: MASODDH '{targetMASODDH}' not found. Falling back.");
                SetGridPosition(fallbackPosition);
            }
            gcDDH.Focus();
        }

        // --- Xử lý nút ---
        private void barBtnThem_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (dangChinhSua) return;
            viTri = bdsDDH.Position;
            dangThemMoi = true;
            bdsDDH.AddNew();
            txtNgay.Value = DateTime.Now;
            txtNhaCC.Text = "";
            txtHoTenNV.SelectedIndex = -1; // Reset selection
            txtMaNV.EditValue = null; // Clear MANV
            CapNhatTrangThaiGiaoDien();
        }

        private void barBtnSua_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (dangThemMoi) { /*...*/ return; }
            if (bdsDDH.Count == 0) { /*...*/ return; }
            viTri = bdsDDH.Position;
            dangChinhSua = true;
            CapNhatTrangThaiGiaoDien();
            txtNhaCC.Focus();
        }


        private void barBtnGhi_ItemClick(object sender, ItemClickEventArgs e)
        {
            panelDDH.Focus();
            try
            {
                bdsDDH.EndEdit();
            }
            catch (Exception exEdit)
            {
                MessageBox.Show("Lỗi kết thúc nhập liệu: " + exEdit.Message, "Lỗi"); return;
            }

            DataRowView drvCurrent = bdsDDH.Current as DataRowView;
            if (drvCurrent == null || drvCurrent.Row.RowState == DataRowState.Detached)
            {
                MessageBox.Show("Không thể lấy dữ liệu dòng hiện tại.", "Lỗi"); return;
            }
            if (!KiemTraDuLieuDauVao()) return;

            string maSoDDH = drvCurrent["MASODDH"].ToString().Trim();
            int manvMoi = Convert.ToInt32(drvCurrent["MANV"]);

            int positionBeforeSave = bdsDDH.Position;
            string doSql = "";
            string undoSql = "";

            try
            {
                if (dangThemMoi)
                {
                    string cauTruyVanKiemTra = $"DECLARE @result int; EXEC @result = sp_KiemTraMaDonDatHang '{maSoDDH.Replace("'", "''")}'; SELECT 'Value' = @result;";
                    SqlDataReader reader = null;
                    try
                    {
                        reader = Program.ExecSqlDataReader(cauTruyVanKiemTra);
                        if (reader != null && reader.Read() && int.Parse(reader.GetValue(0).ToString()) == 1)
                        {
                            MessageBox.Show($"Mã DDH '{maSoDDH}' đã tồn tại!", "Lỗi Trùng Mã"); txtMaDDH.Focus(); return;
                        }
                    }
                    catch (Exception exCheck)
                    {
                        MessageBox.Show("Lỗi kiểm tra Mã DDH: " + exCheck.Message, "Lỗi"); return;
                    }
                    finally { reader?.Close(); }
                    undoSql = $"DELETE FROM DDH WHERE MASODDH = '{maSoDDH.Replace("'", "''")}'";
                }
                else
                {
                    DataRow originalRow = originalData.Select($"MASODDH = '{maSoDDH.Replace("'", "''")}'").FirstOrDefault();
                    if (originalRow == null)
                    {
                        originalData = qLVTDataSet.DDH.Copy();
                        originalRow = originalData.Select($"MASODDH = '{maSoDDH.Replace("'", "''")}'").FirstOrDefault();
                        if (originalRow == null) { MessageBox.Show($"Không tìm thấy dữ liệu gốc của DDH '{maSoDDH}'.", "Cảnh báo"); }
                    }
                    if (originalRow != null)
                    {
                        string nhaccGoc = originalRow["NHACC"]?.ToString()?.Replace("'", "''") ?? "";
                        DateTime? ngayGocRaw = originalRow["NGAY"] as DateTime?;
                        string ngayGocSql = ngayGocRaw.HasValue ? $"'{ngayGocRaw.Value:yyyy-MM-dd}'" : "NULL";
                        int manvGoc = Convert.ToInt32(originalRow["MANV"] ?? 0);
                        undoSql = string.Format(System.Globalization.CultureInfo.InvariantCulture,
                          @"UPDATE DDH SET NGAY={1}, NHACC=N'{2}', MANV={3} WHERE MASODDH='{0}'",
                          maSoDDH.Replace("'", "''"), ngayGocSql, nhaccGoc, manvGoc);
                    }
                }

                this.taDDH.Update(this.qLVTDataSet.DDH);
                originalData = qLVTDataSet.DDH.Copy();

                string nhaccMoi = drvCurrent["NHACC"]?.ToString()?.Replace("'", "''") ?? "";
                DateTime? ngayMoiRaw = drvCurrent["NGAY"] as DateTime?;
                string ngayMoiSql = ngayMoiRaw.HasValue ? $"'{ngayMoiRaw.Value:yyyy-MM-dd}'" : "NULL";

                if (dangThemMoi)
                {
                    doSql = string.Format(System.Globalization.CultureInfo.InvariantCulture,
                      @"INSERT INTO DDH (MASODDH, NGAY, NHACC, MANV) VALUES ('{0}', {1}, N'{2}', {3})",
                      maSoDDH.Replace("'", "''"), ngayMoiSql, nhaccMoi, manvMoi);
                }
                else
                {
                    doSql = string.Format(System.Globalization.CultureInfo.InvariantCulture,
                    @"UPDATE DDH SET NGAY={1}, NHACC=N'{2}', MANV={3} WHERE MASODDH='{0}'",
                    maSoDDH.Replace("'", "''"), ngayMoiSql, nhaccMoi, manvMoi);
                }

                if (!string.IsNullOrEmpty(doSql) && !string.IsNullOrEmpty(undoSql))
                {
                    undoStack.Push(new UndoRedoAction(doSql, undoSql, positionBeforeSave, maSoDDH));
                    redoStack.Clear();
                }
                else { Console.WriteLine("Cảnh báo: DoSql/UndoSql rỗng khi ghi."); }

                dangThemMoi = false; dangChinhSua = false;
                CapNhatTrangThaiGiaoDien();
                SetGridFocusByMASODDH(maSoDDH, positionBeforeSave);
                MessageBox.Show("Ghi dữ liệu thành công!", "Thông báo");

            }
            catch (SqlException sqlEx)
            {
                MessageBox.Show($"Lỗi SQL khi ghi: {sqlEx.Message}", "Lỗi SQL");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi không xác định khi ghi: " + ex.Message, "Lỗi");
            }
        }

        private void barBtnXoa_ItemClick(object sender, ItemClickEventArgs e)
        {
            DataRowView drv = bdsDDH.Current as DataRowView;
            if (drv == null || drv.IsNew)
            {
                MessageBox.Show("Vui lòng chọn một đơn đặt hàng đã lưu hoặc ghi lại đơn hàng đang thêm trước khi xóa.",
                                "Yêu Cầu Thao Tác", // Tiêu đề
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning); // Biểu tượng cảnh báo
                return;
            }

            string maSoDDH = drv["MASODDH"].ToString();

            // --- Kiểm tra khóa ngoại ---
            bool daPhatSinh = false;
            try
            {
                // Kiểm tra CTDDH
                if (bdsCTDDH.List.OfType<DataRowView>().Any(r => r.Row.RowState != DataRowState.Deleted && r["MASODDH"].ToString() == maSoDDH))
                {
                    daPhatSinh = true;
                }
                // Kiểm tra Phiếu Nhập (chỉ kiểm tra nếu chưa phát sinh ở CTDDH)
                if (!daPhatSinh && bdsPN.List.OfType<DataRowView>().Any(r => r.Row.RowState != DataRowState.Deleted && r["MASODDH"].ToString() == maSoDDH))
                {
                    daPhatSinh = true;
                }
            }
            catch (Exception exCheck)
            {
                MessageBox.Show("Đã xảy ra lỗi khi kiểm tra dữ liệu liên quan (CTDDH/PN).\nChi tiết: " + exCheck.Message,
                                "Lỗi Kiểm Tra Dữ Liệu", // Tiêu đề
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error); // Biểu tượng lỗi
                return;
            }

            // Thông báo nếu có dữ liệu liên quan
            if (daPhatSinh)
            {
                MessageBox.Show($"Không thể xóa đơn đặt hàng '{maSoDDH}'.\nĐơn hàng này đã có Chi tiết đơn hàng (CTDDH) hoặc Phiếu nhập (PN) liên quan.",
                                "Không Thể Xóa Do Ràng Buộc", // Tiêu đề (giữ nguyên cái cũ vì đã tốt)
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Stop); // Biểu tượng Stop
                return;
            }
            // --- Hết kiểm tra khóa ngoại ---

            // --- Xác nhận xóa ---
            DialogResult confirmResult = MessageBox.Show($"Bạn có chắc chắn muốn xóa đơn đặt hàng '{maSoDDH}' không?",
                                                       "Xác Nhận Xóa",
                                                       MessageBoxButtons.YesNo, // Dùng Yes/No
                                                       MessageBoxIcon.Question); // Biểu tượng dấu hỏi
            if (confirmResult != DialogResult.Yes)
            {
                return; // Người dùng chọn No
            }

            // --- Tiến hành xóa ---
            int positionBeforeDelete = bdsDDH.Position;
            string doSql = ""; string undoSql = "";
            try
            {
                // Tạo SQL cho Undo/Redo
                string nhacc = drv["NHACC"]?.ToString()?.Replace("'", "''") ?? "";
                DateTime? ngayRaw = drv["NGAY"] as DateTime?;
                string ngaySql = ngayRaw.HasValue ? $"'{ngayRaw.Value:yyyy-MM-dd}'" : "NULL";
                int manv = Convert.ToInt32(drv["MANV"] ?? 0);
                undoSql = string.Format(System.Globalization.CultureInfo.InvariantCulture,
                   @"INSERT INTO DDH (MASODDH, NGAY, NHACC, MANV) VALUES ('{0}', {1}, N'{2}', {3})",
                   maSoDDH.Replace("'", "''"), ngaySql, nhacc, manv);
                doSql = $"DELETE FROM DDH WHERE MASODDH = '{maSoDDH.Replace("'", "''")}'";

                // Xóa trên giao diện và lưu vào DB
                bdsDDH.RemoveCurrent();
                this.taDDH.Update(this.qLVTDataSet.DDH);
                originalData = qLVTDataSet.DDH.Copy(); // Cập nhật bản gốc

                // Lưu vào stack Undo
                if (!string.IsNullOrEmpty(doSql) && !string.IsNullOrEmpty(undoSql))
                {
                    undoStack.Push(new UndoRedoAction(doSql, undoSql, positionBeforeDelete, maSoDDH));
                    redoStack.Clear();
                }

                // Thông báo thành công
                MessageBox.Show($"Đã xóa thành công đơn đặt hàng '{maSoDDH}'.",
                                "Xóa Hoàn Tất", // Tiêu đề
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information); // Biểu tượng thông tin
                CapNhatTrangThaiGiaoDien(); // Cập nhật trạng thái các nút
            }
            catch (SqlException sqlEx) // Bắt lỗi SQL cụ thể
            {
                MessageBox.Show($"Lỗi khi xóa đơn đặt hàng khỏi cơ sở dữ liệu.\nLỗi SQL: {sqlEx.Message} (Số lỗi: {sqlEx.Number})",
                                "Lỗi Cơ Sở Dữ Liệu", // Tiêu đề
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error); // Biểu tượng lỗi
                                                       // Cố gắng nạp lại dữ liệu để khôi phục
                try { this.taDDH.Fill(this.qLVTDataSet.DDH); originalData = qLVTDataSet.DDH.Copy(); SetGridPosition(positionBeforeDelete); } catch { }
                CapNhatTrangThaiGiaoDien();
            }
            catch (Exception ex) // Bắt lỗi chung
            {
                MessageBox.Show($"Đã xảy ra lỗi không xác định trong quá trình xóa.\nChi tiết: {ex.Message}",
                                "Lỗi Hệ Thống", // Tiêu đề
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error); // Biểu tượng lỗi
                                                       // Cố gắng nạp lại dữ liệu để khôi phục
                try { this.taDDH.Fill(this.qLVTDataSet.DDH); originalData = qLVTDataSet.DDH.Copy(); SetGridPosition(positionBeforeDelete); } catch { }
                CapNhatTrangThaiGiaoDien();
            }
        }

        private void barBtnHoanTac_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (undoStack.Count == 0) { return; }
            UndoRedoAction action = undoStack.Peek();
            try
            {
                int result = Program.ExecSqlNonQuery(action.UndoSql);
                if (result != 0) { return; }
                undoStack.Pop(); redoStack.Push(action);
                this.taDDH.Fill(this.qLVTDataSet.DDH); originalData = qLVTDataSet.DDH.Copy();
                this.BeginInvoke(new MethodInvoker(() => SetGridFocusByMASODDH((string)action.AffectedKey, action.OriginalPosition)));
                MessageBox.Show("Hoàn tác thành công!");
            }
            catch (SqlException sqlEx)
            {
                try { this.taDDH.Fill(this.qLVTDataSet.DDH); originalData = qLVTDataSet.DDH.Copy(); SetGridPosition(0); } catch { }
            }
            catch (Exception ex)
            {
                try { this.taDDH.Fill(this.qLVTDataSet.DDH); originalData = qLVTDataSet.DDH.Copy(); SetGridPosition(0); } catch { }
            }
            finally { dangThemMoi = false; dangChinhSua = false; CapNhatTrangThaiGiaoDien(); }
        }

        private void barBtnRedo_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (redoStack.Count == 0) { return; }
            UndoRedoAction action = redoStack.Peek();
            try
            {
                string maSoDDH = (string)action.AffectedKey;
                if (action.DoSql.TrimStart().StartsWith("DELETE", StringComparison.OrdinalIgnoreCase))
                {
                    bool redoHasRelatedRecords = false;
                    try
                    {
                        if (qLVTDataSet.DSCTDDH.AsEnumerable().Any(r => r.RowState != DataRowState.Deleted && r.Field<string>("MASODDH") == maSoDDH)) redoHasRelatedRecords = true;
                        if (!redoHasRelatedRecords && qLVTDataSet.PHIEUNHAP.AsEnumerable().Any(r => r.RowState != DataRowState.Deleted && r.Field<string>("MASODDH") == maSoDDH)) redoHasRelatedRecords = true;
                    }
                    catch (Exception exCheck) { return; }
                    if (redoHasRelatedRecords) { redoStack.Pop(); CapNhatTrangThaiGiaoDien(); return; }
                }
                else if (action.DoSql.TrimStart().StartsWith("INSERT", StringComparison.OrdinalIgnoreCase))
                {
                    if (bdsDDH.Find("MASODDH", maSoDDH) >= 0) { redoStack.Pop(); CapNhatTrangThaiGiaoDien(); return; }
                }

                int result = Program.ExecSqlNonQuery(action.DoSql);
                if (result != 0) { return; }
                redoStack.Pop(); undoStack.Push(action);
                this.taDDH.Fill(this.qLVTDataSet.DDH); originalData = qLVTDataSet.DDH.Copy();
                this.BeginInvoke(new MethodInvoker(() => SetGridFocusByMASODDH((string)action.AffectedKey, action.OriginalPosition)));
                MessageBox.Show("Redo thành công!");
            }
            catch (SqlException sqlEx)
            {
                try { this.taDDH.Fill(this.qLVTDataSet.DDH); originalData = qLVTDataSet.DDH.Copy(); SetGridPosition(0); } catch { }
            }
            catch (Exception ex)
            {
                try { this.taDDH.Fill(this.qLVTDataSet.DDH); originalData = qLVTDataSet.DDH.Copy(); SetGridPosition(0); } catch { }
            }
            finally { dangThemMoi = false; dangChinhSua = false; CapNhatTrangThaiGiaoDien(); }
        }

        private void barBtnLamMoi_ItemClick(object sender, ItemClickEventArgs e)
        {
            int currentDDHPosition = bdsDDH.Position;
            int currentCTDDHPosition = bdsCTDDH.Position;

            try
            {
                // Ngắt kết nối tạm thời
                gcDDH.DataSource = null;
                gcCTDDH.DataSource = null;
                txtHoTenNV.DataSource = null;
                repoCBTenVT.DataSource = null;

                // Xóa dữ liệu cũ
                qLVTDataSet.DSNV.Clear();
                qLVTDataSet.DDH.Clear();
                qLVTDataSet.VATTU.Clear();
                qLVTDataSet.CTDDH.Clear();
                qLVTDataSet.PHIEUNHAP.Clear();

                // Nạp lại dữ liệu
                this.taDDH.Fill(this.qLVTDataSet.DDH);
                this.taDSNV.Fill(this.qLVTDataSet.DSNV);
                this.taVT.Fill(this.qLVTDataSet.VATTU);
                this.taCTDDH.Fill(this.qLVTDataSet.CTDDH);
                this.taPN.Fill(this.qLVTDataSet.PHIEUNHAP);

                // Kết nối lại BindingSource
                bdsDSNV.DataSource = qLVTDataSet.DSNV;
                bdsVT.DataSource = qLVTDataSet.VATTU;
                bdsDDH.DataSource = qLVTDataSet.DDH;
                bdsCTDDH.DataSource = bdsDDH;
                bdsCTDDH.DataMember = "FK__CTDDH__MASODDH";

                // Tái cấu hình txtHoTenNV
                txtHoTenNV.DataBindings.Clear();
                txtHoTenNV.DataSource = bdsDSNV;
                txtHoTenNV.DisplayMember = "HOTEN";
                txtHoTenNV.ValueMember = "MANV";
                txtHoTenNV.DataBindings.Add(new System.Windows.Forms.Binding("SelectedValue", bdsDDH, "MANV", true));
                txtHoTenNV.Enabled = true; // Đặt read-only sau khi làm mới

                // Kết nối lại Controls
                repoCBTenVT.DataSource = bdsVT;
                gcDDH.DataSource = bdsDDH;
                gcCTDDH.DataSource = bdsCTDDH;

                // Làm mới Lookup và ép render lại
                RefreshLookupVatTu();
                colMAVT.ColumnEdit = repoCBTenVT;

                // Ép làm mới giao diện
                gridView2.RefreshData();
                gridView2.Invalidate();
                gridView2.LayoutChanged();
                gcCTDDH.RefreshDataSource();

                // Cập nhật trạng thái
                originalData = qLVTDataSet.DDH.Copy();
                undoStack.Clear();
                redoStack.Clear();
                dangThemMoi = false;
                dangChinhSua = false;
                dangThemMoiCTDDH = false;
                dangChinhSuaCTDDH = false;

                // Khôi phục vị trí
                if (bdsDDH.Count > 0)
                {
                    bdsDDH.Position = Math.Max(0, Math.Min(currentDDHPosition, bdsDDH.Count - 1));
                    if (bdsCTDDH.Count > 0)
                    {
                        bdsCTDDH.Position = Math.Max(0, Math.Min(currentCTDDHPosition, bdsCTDDH.Count - 1));
                    }
                }

                MessageBox.Show("Làm mới thành công.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi nghiêm trọng khi làm mới: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtHoTenNV.DataSource = null;
                txtHoTenNV.Text = "";
            }
            finally
            {
                CapNhatTrangThaiGiaoDien();
                CapNhatMaNVDisplay();
            }
        }

        private void barBtnThoat_ItemClick(object sender, ItemClickEventArgs e)
        {
            // Kiểm tra xem có bất kỳ trạng thái thêm mới hoặc chỉnh sửa nào đang diễn ra không
            if (dangThemMoi || dangChinhSua || dangThemMoiCTDDH || dangChinhSuaCTDDH)
            {
                // Hỏi người dùng xác nhận
                DialogResult result = MessageBox.Show(
                    "Dữ liệu bạn đang nhập/sửa chưa được lưu.\nBạn có chắc chắn muốn thoát và hủy bỏ các thay đổi?", // Làm rõ là sẽ hủy thay đổi
                    "Xác Nhận Thoát",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning // Dùng Warning vì có thể mất dữ liệu nhập
                );

                // Nếu người dùng không chọn Yes, thì dừng lại, không thoát
                if (result != DialogResult.Yes)
                {
                    return;
                }

                // Nếu người dùng chọn Yes, tiến hành hủy các chỉnh sửa có thể có
                // Sử dụng BindingSource.CancelEdit() là đủ để hủy thay đổi trên dòng hiện tại
                // Nó cũng thường xử lý việc loại bỏ dòng mới thêm (trạng thái Added)
                try
                {
                    // Hủy thay đổi trên BindingSource của Đơn đặt hàng (nếu có)
                    if (bdsDDH.Current != null) // Chỉ gọi nếu có dòng hiện tại
                    {
                        HuyThaoTacDDH();
                    }

                    // Hủy thay đổi trên BindingSource của Chi tiết Đơn đặt hàng (nếu có)
                    if (bdsCTDDH.Current != null) // Chỉ gọi nếu có dòng hiện tại
                    {
                        HuyThaoTacCTDDH();
                    }

                    // Đặt lại các cờ trạng thái (không bắt buộc khi đóng form, nhưng sạch sẽ)
                    dangThemMoi = false;
                    dangChinhSua = false;
                    dangThemMoiCTDDH = false;
                    dangChinhSuaCTDDH = false;
                }
                catch (Exception exCancel)
                {
                    // Có thể thêm MessageBox ở đây nếu muốn thông báo lỗi hủy cho người dùng
                    MessageBox.Show("Có lỗi khi hủy thay đổi, nhưng form vẫn sẽ đóng.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                this.Close();
            }
        }

        // --- Các hàm hủy thao tác ---
        private void HuyThaoTacDDH()
        {
            try { bdsDDH.CancelEdit(); } catch { } // Hủy thay đổi trên control

            if (dangThemMoi) // Xử lý hủy dòng mới thêm
            {
                DataRowView drv = bdsDDH.Current as DataRowView;
                if (drv != null && drv.Row.RowState == DataRowState.Added)
                {
                    try { bdsDDH.RemoveCurrent(); } // Xóa dòng mới thêm
                    catch { }
                }
                // Đặt lại vị trí về trước khi Thêm
                SetGridPosition(viTri);
            }
            else if (dangChinhSua) // Nếu đang sửa, chỉ cần đặt lại vị trí
            {
                SetGridPosition(viTri);
            }

            dangThemMoi = false;
            dangChinhSua = false;
            CapNhatTrangThaiGiaoDien();
        }
        private void HuyThaoTacCTDDH()
        {
            try { bdsCTDDH.CancelEdit(); } catch { } // Hủy thay đổi trên control của CTDDH

            if (dangThemMoiCTDDH) // Xử lý hủy dòng CTDDH mới thêm
            {
                DataRowView drv = bdsCTDDH.Current as DataRowView;
                if (drv != null && drv.Row.RowState == DataRowState.Added)
                {
                    try { bdsCTDDH.RemoveCurrent(); } // Xóa dòng mới
                    catch { }
                }
                // Đặt lại vị trí CTDDH về trước khi Thêm
                if (bdsCTDDH.Count > 0)
                {
                    if (viTriCTDDH >= 0 && viTriCTDDH < bdsCTDDH.Count) bdsCTDDH.Position = viTriCTDDH;
                    else bdsCTDDH.Position = 0;
                }
            }
            else if (dangChinhSuaCTDDH) // Nếu đang sửa CTDDH, đặt lại vị trí
            {
                if (bdsCTDDH.Count > 0)
                {
                    if (viTriCTDDH >= 0 && viTriCTDDH < bdsCTDDH.Count) bdsCTDDH.Position = viTriCTDDH;
                    else bdsCTDDH.Position = 0;
                }
            }

            dangThemMoiCTDDH = false;
            dangChinhSuaCTDDH = false;
            CapNhatTrangThaiGiaoDien(); // Cập nhật lại trạng thái nút và ReadOnly cột
        }
        private bool KiemTraDuLieuDauVao()
        {
            string maDDH = txtMaDDH.Text.Trim();
            string nhaCC = txtNhaCC.Text.Trim();

            // --- Kiểm tra Mã Đơn Đặt Hàng ---
            if (string.IsNullOrEmpty(maDDH))
            {
                MessageBox.Show("Mã đơn đặt hàng không được để trống!",
                                "Thiếu Thông Tin",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                txtMaDDH.Focus();
                return false;
            }
            if (!Regex.IsMatch(maDDH, @"^[A-Za-z0-9]+$")) // Chỉ cho phép chữ cái và số
            {
                MessageBox.Show("Mã đơn đặt hàng chỉ được chứa chữ cái (A-Z, a-z) và số (0-9).",
                                "Dữ Liệu Không Hợp Lệ",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                txtMaDDH.Focus();
                return false;
            }
            if (maDDH.Length > 8)
            {
                MessageBox.Show("Mã đơn đặt hàng không được vượt quá 8 ký tự!",
                                "Dữ Liệu Không Hợp Lệ",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                txtMaDDH.Focus();
                return false;
            }

            // --- Kiểm tra Nhà Cung Cấp ---
            if (string.IsNullOrEmpty(nhaCC))
            {
                MessageBox.Show("Tên nhà cung cấp không được để trống!",
                                "Thiếu Thông Tin",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                txtNhaCC.Focus();
                return false;
            }
            if (!Regex.IsMatch(nhaCC, @"^[\p{L}\s]+$"))
            {
                MessageBox.Show("Nhà cung cấp chỉ chứa chữ cái và khoảng trắng!", "Thông báo", MessageBoxButtons.OK);
                txtNhaCC.Focus();
                return false;
            }
            if (nhaCC.Length > 100)
            {
                MessageBox.Show("Tên nhà cung cấp không được vượt quá 100 ký tự!",
                                "Dữ Liệu Không Hợp Lệ",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                txtNhaCC.Focus();
                return false;
            }

            // --- Kiểm tra Ngày Đặt Hàng ---
            if (txtNgay.Value == null) // Kiểm tra null (dù DateTimePicker thường không null)
            {
                MessageBox.Show("Vui lòng chọn ngày đặt hàng!",
                                "Thiếu Thông Tin",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                txtNgay.Focus();
                return false;
            }
            if (txtNgay.Value.Date > DateTime.Now.Date) // So sánh ngày, bỏ qua giờ
            {
                MessageBox.Show("Ngày đặt hàng không được là một ngày trong tương lai!",
                                "Dữ Liệu Không Hợp Lệ",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                txtNgay.Focus();
                return false;
            }

            // --- Kiểm tra Nhân Viên ---
            if (txtHoTenNV.SelectedValue == null || txtHoTenNV.SelectedValue is DBNull || string.IsNullOrEmpty(txtHoTenNV.SelectedValue.ToString()))
            {
                MessageBox.Show("Vui lòng chọn nhân viên lập đơn hàng!",
                                "Thiếu Thông Tin",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                txtHoTenNV.Focus();
                return false;
            }
            // Kiểm tra xem giá trị có phải là số nguyên không
            if (!int.TryParse(txtHoTenNV.SelectedValue.ToString(), out _))
            {
                MessageBox.Show("Mã nhân viên được chọn không hợp lệ. Vui lòng chọn lại.",
                                "Dữ Liệu Không Hợp Lệ",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error); // Dùng Error vì đây là lỗi dữ liệu nội bộ
                txtHoTenNV.Focus();
                return false;
            }

            // Nếu tất cả kiểm tra đều qua
            return true;
        }

        //-----------------------------popup menu CTDDH-----------------------------

        private void bdsDDH_PositionChanged(object sender, EventArgs e)
        {
            // Nếu đang thêm/sửa CTDDH mà chuyển dòng DDH -> Hủy CTDDH
            if (dangThemMoiCTDDH || dangChinhSuaCTDDH)
            {
                HuyThaoTacCTDDH(); // Hàm hủy bỏ thao tác CTDDH
            }
            CapNhatMaNVDisplay();
            CapNhatTrangThaiGiaoDien(); // Cập nhật lại enable/disable của popBtn
        }

        // --- SỰ KIỆN KHI CHUYỂN DÒNG TRÊN BINDING SOURCE CTDDH ---
        private void BdsCTDDH_CurrentChanged(object sender, EventArgs e)
        {
            // Nếu đang thêm/sửa CTDDH mà lại chuyển sang dòng CTDDH khác -> Có thể coi là hủy
            if (dangThemMoiCTDDH || dangChinhSuaCTDDH)
            {
                // Kiểm tra xem có phải dòng đang thêm/sửa không
                DataRowView drvCurrent = bdsCTDDH.Current as DataRowView;
                // Có thể thêm logic phức tạp hơn để xác định chính xác user cố tình chuyển dòng hay không
                // Tạm thời không hủy tự động khi chuyển dòng CTDDH để tránh mất dữ liệu đang nhập dở
            }
            CapNhatTrangThaiGiaoDien(); // Cập nhật nút popup
        }

        // --- Quản lý trạng thái giao diện ---
        private void CapNhatTrangThaiGiaoDien()
        {
            bool dangSuaHoacThemDDH = dangThemMoi || dangChinhSua;
            bool dangSuaHoacThemCTDDH = dangThemMoiCTDDH || dangChinhSuaCTDDH;
            bool coDDH = bdsDDH.Count > 0;
            bool coCTDDH = bdsCTDDH.Count > 0;

            // Trạng thái form chính (DDH)
            barBtnThem.Enabled = !dangSuaHoacThemDDH && !dangSuaHoacThemCTDDH;
            barBtnSua.Enabled = !dangSuaHoacThemDDH && !dangSuaHoacThemCTDDH && coDDH;
            barBtnXoa.Enabled = !dangSuaHoacThemDDH && !dangSuaHoacThemCTDDH && coDDH;
            barBtnGhi.Enabled = dangSuaHoacThemDDH;
            barBtnHoanTac.Enabled = !dangSuaHoacThemDDH && !dangSuaHoacThemCTDDH && undoStack.Count > 0;
            barBtnRedo.Enabled = !dangSuaHoacThemDDH && !dangSuaHoacThemCTDDH && redoStack.Count > 0;
            barBtnLamMoi.Enabled = !dangSuaHoacThemDDH && !dangSuaHoacThemCTDDH;
            barBtnThoat.Enabled = true;

            panelDDH.Enabled = dangSuaHoacThemDDH;
            gcDDH.Enabled = !dangSuaHoacThemDDH && !dangSuaHoacThemCTDDH;
            gcCTDDH.Enabled = !dangSuaHoacThemDDH;

            // Trạng thái lưới chi tiết (CTDDH)
            colMAVT.OptionsColumn.ReadOnly = !dangSuaHoacThemCTDDH;
            colSOLUONG.OptionsColumn.ReadOnly = !dangSuaHoacThemCTDDH;
            colDONGIA.OptionsColumn.ReadOnly = !dangSuaHoacThemCTDDH;
            colMASODDH1.OptionsColumn.ReadOnly = true;

            // Trạng thái nút popup CTDDH
            popBtnThem.Enabled = !dangSuaHoacThemDDH && !dangSuaHoacThemCTDDH && coDDH;
            popBtnSua.Enabled = !dangSuaHoacThemCTDDH && !dangSuaHoacThemDDH && coCTDDH;
            popBtnXoa.Enabled = !dangSuaHoacThemCTDDH && !dangSuaHoacThemDDH && coCTDDH;
            popBtnGhi.Enabled = dangSuaHoacThemCTDDH;

            // Xử lý riêng cho txtMaDDH
            txtMaDDH.ReadOnly = !dangThemMoi;
            if (dangThemMoi) txtMaDDH.Focus();

            // Cập nhật ô nhập liệu DDH
            capNhatONhap(!dangSuaHoacThemDDH);
        }

        private void capNhatONhap(bool readOnly)
        {
            txtNhaCC.ReadOnly = readOnly;
            txtNgay.Enabled = !readOnly;
            txtHoTenNV.Enabled = !readOnly;
        }


        // --- Xử lý Popup Menu cho CTDDH ---
        private void popDH(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            // Cập nhật trạng thái nút popup trước khi hiển thị
            CapNhatTrangThaiGiaoDien();
            popDDH.ShowPopup(Control.MousePosition);
        }

        // Hàm trợ giúp lấy Tên Vật Tư từ Mã Vật Tư
        private string GetTenVT(string maVT)
        {
            if (string.IsNullOrEmpty(maVT)) return "[Chưa chọn]";
            try
            {
                // Tìm trong bdsVT
                var view = bdsVT.List as DataView;
                if (view != null)
                {
                    view.RowFilter = $"MAVT = '{maVT.Replace("'", "''")}'";
                    if (view.Count > 0)
                    {
                        string tenVT = view[0]["TENVT"].ToString();
                        view.RowFilter = ""; // Bỏ filter
                        return tenVT;
                    }
                    view.RowFilter = ""; // Bỏ filter
                }
                // Tìm trong DataTable
                DataRow[] rows = qLVTDataSet.VATTU.Select($"MAVT = '{maVT.Replace("'", "''")}'");
                if (rows.Length > 0)
                {
                    return rows[0]["TENVT"].ToString();
                }
            }
            catch (Exception)
            {
            }
            return "[Lỗi tên VT]";
        }

        // Đổi tên các hàm xử lý ItemClick của Popup để tránh trùng với BarButton
        private void popBtnThemCTDDH_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (dangThemMoi || dangChinhSua || dangThemMoiCTDDH || dangChinhSuaCTDDH) return;
            if (bdsDDH.Count == 0 || bdsDDH.Current == null)
            {
                MessageBox.Show("Vui lòng chọn một Đơn đặt hàng trước khi thêm chi tiết.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            viTriCTDDH = bdsCTDDH.Position;
            dangThemMoiCTDDH = true;
            bdsCTDDH.AddNew();

            var drvDDH = (DataRowView)bdsDDH.Current;
            var drvCTDDH_New = (DataRowView)bdsCTDDH.Current;

            if (drvDDH != null && drvCTDDH_New != null)
            {
                drvCTDDH_New["MASODDH"] = drvDDH["MASODDH"];
                drvCTDDH_New["SOLUONG"] = 1;
                drvCTDDH_New["DONGIA"] = 0;
            }
            else
            {
                MessageBox.Show("Lỗi: Không thể lấy thông tin Đơn đặt hàng hoặc dòng chi tiết mới.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                HuyThaoTacCTDDH();
                return;
            }

            // Làm mới lookup để đảm bảo chỉ hiển thị vật tư chưa có
            RefreshLookupVatTu();
            CapNhatTrangThaiGiaoDien();
            gridView2.FocusedRowHandle = gridView2.RowCount - 1;
            gridView2.FocusedColumn = colMAVT;
            gridView2.ShowEditor();
        }

        private void popBtnSuaCTDDH_ItemClick(object sender, ItemClickEventArgs e)
        {
            // Chỉ cho phép sửa CTDDH khi không sửa DDH và không sửa CTDDH khác
            if (dangThemMoi || dangChinhSua || dangThemMoiCTDDH || dangChinhSuaCTDDH) return;
            if (bdsCTDDH.Count == 0 || bdsCTDDH.Current == null)
            {
                MessageBox.Show("Không có chi tiết nào để sửa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            // Kiểm tra xem dòng hiện tại có phải là dòng đang thêm dở không (trường hợp hiếm)
            var drv = (DataRowView)bdsCTDDH.Current;
            if (drv.IsNew)
            {
                MessageBox.Show("Dòng này đang được thêm mới, vui lòng nhấn Ghi.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }


            viTriCTDDH = bdsCTDDH.Position; // Lưu vị trí dòng đang sửa
            dangChinhSuaCTDDH = true;

            CapNhatTrangThaiGiaoDien(); // Cập nhật trạng thái nút và ReadOnly cột
            gcCTDDH.Focus(); // Focus vào lưới CTDDH
            gridView2.FocusedColumn = colMAVT; // Focus vào cột đầu tiên cho phép sửa
            gridView2.ShowEditor(); // Hiển thị editor
        }

        private void popBtnXoaCTDDH_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (dangThemMoi || dangChinhSua || dangThemMoiCTDDH || dangChinhSuaCTDDH) return;

            if (bdsCTDDH.Count == 0 || bdsCTDDH.Current == null)
            {
                MessageBox.Show("Không có chi tiết đơn đặt hàng để xóa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var drv = (DataRowView)bdsCTDDH.Current;

            string maSoDDH = drv["MASODDH"]?.ToString() ?? "[Lỗi Mã DDH]";
            string mavt = drv["MAVT"]?.ToString() ?? "[Lỗi Mã VT]";
            string tenVT = GetTenVT(mavt);

            if (MessageBox.Show($"Xác nhận xóa vật tư '{tenVT}' (Mã: {mavt})\nkhỏi đơn đặt hàng '{maSoDDH}'?",
                                "Xác Nhận Xóa Chi Tiết", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            {
                return;
            }

            int position = bdsCTDDH.Position;
            try
            {
                int positionBeforeDelete = bdsCTDDH.Position;

                bdsCTDDH.RemoveCurrent();
                taCTDDH.Update(this.qLVTDataSet.CTDDH);

                // Làm mới dữ liệu và cập nhật Grid
                gcCTDDH.BeginUpdate();
                qLVTDataSet.CTDDH.Clear(); // Xóa dữ liệu cũ
                this.taCTDDH.Fill(this.qLVTDataSet.CTDDH); // Nạp lại CTDDH

                qLVTDataSet.VATTU.Clear(); // Xóa dữ liệu cũ
                this.taVT.Fill(this.qLVTDataSet.VATTU);   // Nạp lại VATTU

                RefreshLookupVatTu(); // Làm mới lookup

                // Khôi phục vị trí
                if (positionBeforeDelete >= 0 && positionBeforeDelete < bdsCTDDH.Count)
                {
                    bdsCTDDH.Position = positionBeforeDelete;
                }
                else if (bdsCTDDH.Count > 0)
                {
                    bdsCTDDH.Position = Math.Max(0, bdsCTDDH.Count - 1);
                }

                bdsCTDDH.ResetBindings(false);
                colMAVT.ColumnEdit = this.repoCBTenVT;
                gridView2.RefreshData();
                gridView2.Invalidate();
                gridView2.LayoutChanged();
                gcCTDDH.EndUpdate();
            }
            catch (Exception ex)
            {
                string errorMsg = ex is SqlException ? $"Lỗi SQL khi xóa chi tiết đơn hàng: {ex.Message}" : $"Lỗi không xác định khi xóa chi tiết đơn hàng: {ex.Message}";
                MessageBox.Show(errorMsg, ex is SqlException ? "Lỗi Cơ Sở Dữ Liệu" : "Lỗi Hệ Thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
                try
                {
                    qLVTDataSet.CTDDH.Clear();
                    this.taCTDDH.Fill(qLVTDataSet.CTDDH);
                    qLVTDataSet.VATTU.Clear();
                    this.taVT.Fill(qLVTDataSet.VATTU);
                    RefreshLookupVatTu();
                    if (position >= 0 && position < bdsCTDDH.Count) bdsCTDDH.Position = position;
                    else if (bdsCTDDH.Count > 0) bdsCTDDH.Position = 0;
                    bdsCTDDH.ResetBindings(false);
                    gcCTDDH.RefreshDataSource();
                }
                catch (Exception exFallback)
                {
                    Console.WriteLine($"Fallback error: {exFallback.Message}");
                }
                CapNhatTrangThaiGiaoDien();
            }
        }

        private void popBtnGhiCTDDH_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (!dangThemMoiCTDDH && !dangChinhSuaCTDDH) return;

            try
            {
                if (gridView2.IsEditing)
                {
                    gridView2.CloseEditor();
                    gridView2.UpdateCurrentRow();
                }
                bdsCTDDH.EndEdit();
            }
            catch (Exception exEdit)
            {
                MessageBox.Show($"Lỗi khi kết thúc nhập liệu chi tiết: {exEdit.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var drv = (DataRowView)bdsCTDDH.Current;
            if (drv == null)
            {
                MessageBox.Show("Không lấy được dữ liệu chi tiết hiện tại để ghi.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                if (dangThemMoiCTDDH) HuyThaoTacCTDDH();
                return;
            }

            string maSoDDH = drv["MASODDH"]?.ToString();
            string mavt = drv["MAVT"]?.ToString();
            string tenVT = GetTenVT(mavt);

            if (string.IsNullOrEmpty(mavt))
            {
                MessageBox.Show("Vui lòng chọn Vật tư!", "Thiếu Thông Tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                gridView2.FocusedColumn = colMAVT;
                gridView2.ShowEditor();
                return;
            }
            int soLuong;
            if (!int.TryParse(drv["SOLUONG"]?.ToString(), out soLuong) || soLuong <= 0)
            {
                MessageBox.Show("Số lượng phải là số nguyên dương!", "Dữ Liệu Không Hợp Lệ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                gridView2.FocusedColumn = colSOLUONG;
                gridView2.ShowEditor();
                return;
            }
            int donGia;
            if (!int.TryParse(drv["DONGIA"]?.ToString(), out donGia) || donGia < 0)
            {
                MessageBox.Show("Đơn giá phải là số không âm!", "Dữ Liệu Không Hợp Lệ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                gridView2.FocusedColumn = colDONGIA;
                gridView2.ShowEditor();
                return;
            }

            if (dangThemMoiCTDDH && !string.IsNullOrEmpty(maSoDDH) && !string.IsNullOrEmpty(mavt))
            {
                var existingRows = qLVTDataSet.CTDDH.AsEnumerable()
                    .Where(row => row.RowState != DataRowState.Deleted &&
                                  row.Field<string>("MASODDH") == maSoDDH &&
                                  row.Field<string>("MAVT") == mavt &&
                                  row != drv.Row);

                if (existingRows.Any())
                {
                    MessageBox.Show($"Lỗi: Vật tư '{tenVT}' (Mã: {mavt}) đã tồn tại trong Đơn đặt hàng '{maSoDDH}' này.\nVui lòng chọn vật tư khác.",
                                    "Lỗi Trùng Vật Tư", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    gridView2.FocusedColumn = colMAVT;
                    this.BeginInvoke(new MethodInvoker(() => gridView2.ShowEditor()));
                    return;
                }
            }

            int position = bdsCTDDH.Position;

            try
            {
                this.taCTDDH.Update(this.qLVTDataSet.CTDDH);

                dangThemMoiCTDDH = false;
                dangChinhSuaCTDDH = false;

                MessageBox.Show($"Ghi chi tiết vật tư '{tenVT}' thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                try
                {
                    int savedPosition = bdsCTDDH.Position;

                    gcCTDDH.BeginUpdate();
                    bdsCTDDH.RaiseListChangedEvents = false;
                    bdsVT.RaiseListChangedEvents = false;

                    this.taCTDDH.Fill(this.qLVTDataSet.CTDDH);
                    this.taVT.Fill(this.qLVTDataSet.VATTU);

                    bdsVT.RaiseListChangedEvents = true;
                    bdsCTDDH.RaiseListChangedEvents = true;

                    RefreshLookupVatTu(); // Làm mới lookup

                    bdsVT.ResetBindings(false);
                    bdsCTDDH.ResetBindings(false);

                    if (savedPosition >= 0 && savedPosition < bdsCTDDH.Count)
                    {
                        bdsCTDDH.Position = savedPosition;
                    }
                    else if (bdsCTDDH.Count > 0)
                    {
                        bdsCTDDH.Position = 0;
                    }

                    colMAVT.ColumnEdit = this.repoCBTenVT;
                    gridView2.RefreshData();
                    gridView2.LayoutChanged(); // Đảm bảo layout được cập nhật
                    gcCTDDH.RefreshDataSource();
                }
                catch (Exception exFill)
                {
                    MessageBox.Show("Lỗi khi làm mới dữ liệu sau khi ghi: " + exFill.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                finally
                {
                    gcCTDDH.EndUpdate();
                }
            }
            catch (SqlException sqlEx)
            {
                if (sqlEx.Number == 2627 || (sqlEx.Message.Contains("PRIMARY KEY constraint") && sqlEx.Message.Contains("duplicate key")))
                {
                    tenVT = GetTenVT(drv["MAVT"]?.ToString());
                    MessageBox.Show($"Lỗi CSDL: Vật tư '{tenVT}' (Mã: {drv["MAVT"]?.ToString()}) đã có trong đơn hàng này.\nVui lòng chọn vật tư khác.",
                                    "Lỗi Trùng Vật Tư (CSDL)", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.BeginInvoke(new MethodInvoker(() => gridView2.ShowEditor()));
                    return;
                }
                else
                {
                    MessageBox.Show($"Lỗi SQL khác khi ghi chi tiết: {sqlEx.Message}", "Lỗi Cơ Sở Dữ Liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    try { bdsCTDDH.CancelEdit(); } catch { }
                    dangThemMoiCTDDH = false;
                    dangChinhSuaCTDDH = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khác khi ghi chi tiết: {ex.Message}", "Lỗi Hệ Thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
                try { bdsCTDDH.CancelEdit(); } catch { }
                dangThemMoiCTDDH = false;
                dangChinhSuaCTDDH = false;
            }
            finally
            {
                CapNhatTrangThaiGiaoDien();
            }
        }

        // --- Xử lý sự kiện của GridView CTDDH ---
        private void gridView2_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            if (dangThemMoiCTDDH || dangChinhSuaCTDDH) return;

            DataRowView drv = gridView2.GetRow(e.FocusedRowHandle) as DataRowView;
            if (drv != null)
            {
                string mavt = drv["MAVT"]?.ToString();
                if (!string.IsNullOrEmpty(mavt))
                {
                    gridView2.SetRowCellValue(e.FocusedRowHandle, colMAVT, mavt);
                    gridView2.RefreshRow(e.FocusedRowHandle); // Làm mới hiển thị
                }
            }
        }

        private void gridView2_CustomRowCellEditForEditing(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            if (e.Column == colMAVT)
            {
                e.RepositoryItem = repoCBTenVT;
            }
        }

        private void gridView2_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            if (e.Column.FieldName == "TENVT" && e.IsGetData)
            {
                DataRowView row = gridView2.GetRow(e.ListSourceRowIndex) as DataRowView;
                if (row != null)
                {
                    string mavt = row["MAVT"]?.ToString();
                    if (!string.IsNullOrEmpty(mavt))
                    {
                        string tenVT = GetTenVT(mavt);
                        e.Value = string.IsNullOrEmpty(tenVT) ? "[Lỗi tên VT]" : tenVT;
                    }
                    else
                    {
                        e.Value = "[Chưa chọn]";
                    }
                }
                else
                {
                    e.Value = "[Lỗi dữ liệu]";
                }
            }
        }

        private void repoCBTenVT_QueryPopUp(object sender, System.ComponentModel.CancelEventArgs e)
        {
            LookUpEdit lookUpEdit = sender as LookUpEdit;
            if (lookUpEdit == null) return;

            DataRowView drvDDH = bdsDDH.Current as DataRowView;
            if (drvDDH == null)
            {
                lookUpEdit.Properties.DataSource = this.bdsVT;
                return;
            }

            string currentMaSoDDH = drvDDH["MASODDH"]?.ToString();
            if (string.IsNullOrEmpty(currentMaSoDDH)) return;

            DataRowView drvCTDDH = gridView2.GetRow(gridView2.FocusedRowHandle) as DataRowView;
            string currentMAVT = drvCTDDH != null ? drvCTDDH["MAVT"]?.ToString() : null;

            // Lấy danh sách MAVT đã sử dụng trong CTDDH cho MASODDH hiện tại
            var usedMaVTs = qLVTDataSet.CTDDH.AsEnumerable()
                .Where(row => row.RowState != DataRowState.Deleted && row.Field<string>("MASODDH") == currentMaSoDDH)
                .Select(row => row.Field<string>("MAVT"))
                .Where(mavt => !string.IsNullOrEmpty(mavt))
                .Distinct()
                .ToList();

            DataView dvVattu = new DataView(qLVTDataSet.VATTU);
            if (usedMaVTs.Count > 0)
            {
                var escapedMaVTs = usedMaVTs.Select(mavt => $"'{mavt.Replace("'", "''")}'");
                string filter = $"MAVT NOT IN ({string.Join(",", escapedMaVTs)})";

                // Nếu đang thêm mới hoặc sửa, áp dụng filter để chỉ hiển thị vật tư chưa có
                if (dangThemMoiCTDDH)
                {
                    if (!string.IsNullOrEmpty(currentMAVT) && !usedMaVTs.Contains(currentMAVT))
                    {
                        dvVattu.RowFilter = filter;
                    }
                    else
                    {
                        dvVattu.RowFilter = filter;
                    }
                }
                else if (dangChinhSuaCTDDH)
                {
                    // Trong chế độ sửa, loại bỏ currentMAVT khỏi filter để nó vẫn hiển thị
                    if (!string.IsNullOrEmpty(currentMAVT) && usedMaVTs.Contains(currentMAVT))
                    {
                        var filteredMaVTs = usedMaVTs.Where(m => m != currentMAVT).Select(m => $"'{m.Replace("'", "''")}'");
                        dvVattu.RowFilter = filteredMaVTs.Any() ? $"MAVT NOT IN ({string.Join(",", filteredMaVTs)})" : "";
                    }
                    else
                    {
                        dvVattu.RowFilter = filter;
                    }
                }
                else
                {
                    // Khi không thêm mới hoặc sửa (xem), hiển thị tất cả
                    dvVattu.RowFilter = "";
                }
            }
            else
            {
                dvVattu.RowFilter = ""; // Không lọc nếu chưa có vật tư nào
            }

            lookUpEdit.Properties.DataSource = dvVattu;
            lookUpEdit.Properties.PopulateColumns();
            lookUpEdit.Properties.ValueMember = "MAVT";
            lookUpEdit.Properties.DisplayMember = "TENVT";
            lookUpEdit.Properties.ForceInitialize();
        }
        private void RefreshLookupVatTu()
        {
            try
            {
                gcCTDDH.BeginUpdate();

                // Xóa dữ liệu cũ và nạp lại
                repoCBTenVT.DataSource = null;
                qLVTDataSet.VATTU.Clear(); // Xóa dữ liệu cũ trong DataTable
                this.taVT.Fill(this.qLVTDataSet.VATTU); // Nạp lại từ DB

                // Kiểm tra dữ liệu
                if (qLVTDataSet.VATTU.Rows.Count == 0)
                {
                    MessageBox.Show("Dữ liệu VATTU rỗng, vui lòng kiểm tra cơ sở dữ liệu!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                // Gán lại DataSource cho LookupEdit
                bdsVT.ResetBindings(false);
                repoCBTenVT.DataSource = bdsVT;
                repoCBTenVT.DisplayMember = "TENVT";
                repoCBTenVT.ValueMember = "MAVT";
                repoCBTenVT.PopulateColumns();
                colMAVT.ColumnEdit = repoCBTenVT; // Gán lại ColumnEdit

                // Ép làm mới giao diện
                gridView2.RefreshData();
                gridView2.Invalidate();
                gridView2.LayoutChanged();
                gcCTDDH.RefreshDataSource();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error refreshing VatTu lookup: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                gcCTDDH.EndUpdate();
            }
        }

    }
}