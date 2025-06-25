using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using QLVT;

namespace QLVT
{
    public partial class fmNhanVien : Form
    {
        int viTri = 0; //vị trí trên bảng

        bool dangThemMoi = false; // true khi đang thêm
        bool dangChinhSua = false; // True khi đang sửa

        //--- Sử dụng Stack chứa đối tượng UndoRedoAction ---
        Stack<UndoRedoAction> undoStack = new Stack<UndoRedoAction>();
        Stack<UndoRedoAction> redoStack = new Stack<UndoRedoAction>();

        DataTable originalData = new DataTable(); // Lưu dữ liệu gốc để so sánh chỉnh sửa
        public fmNhanVien()
        {
            InitializeComponent();
        }


        private void nHANVIENBindingNavigatorSaveItem_Click(object sender, EventArgs e)
        {
            this.Validate();
            this.bdsNV.EndEdit();
            this.tableAdapterManager.UpdateAll(this.qLVTDataSet);

        }

        private void fmNhanVien_Load(object sender, EventArgs e)
        {
            //không kiểm tra khóa ngoại nữa
            qLVTDataSet.EnforceConstraints = false;

            this.taNV.Connection.ConnectionString = Program.connstr;
            this.taNV.Fill(this.qLVTDataSet.NHANVIEN);

            this.taDDH.Connection.ConnectionString = Program.connstr;
            this.taDDH.Fill(this.qLVTDataSet.DDH);

            this.taPN.Connection.ConnectionString = Program.connstr;
            this.taPN.Fill(this.qLVTDataSet.PHIEUNHAP);

            this.taPX.Connection.ConnectionString = Program.connstr;
            this.taPX.Fill(this.qLVTDataSet.PHIEUXUAT);

            // Lưu dữ liệu gốc
            originalData = qLVTDataSet.NHANVIEN.Copy();

            capNhatNut();
            barBtnGhi.Enabled = false;
            barBtnHoanTac.Enabled = undoStack.Count > 0;
            barBtnRedo.Enabled = redoStack.Count > 0;

        }

        private void capNhatNut()
        {
            bool editing = dangThemMoi || dangChinhSua;

            capNhatONhap(!editing); //nếu mà đang chỉnh sửa hoặc là thêm mới thì phải mở hết cho sửa chứ, tắt RO đi
            if (dangThemMoi)
            {
                txtMaNV.ReadOnly = false; //thêm thì mở mã nhân viên cho nhập
                txtMaNV.Focus();
                gcNV.Enabled = false;
            }
            else
            {
                txtMaNV.ReadOnly = true; //còn sửa thì không cho nhập mã nhân viên
            }

            //không trogn trạng thái chỉnh sửa hoặc thêm mới thì các nút mở
            barBtnThem.Enabled = !editing;
            barBtnSua.Enabled = !editing && bdsNV.Count > 0; //kiểm tra coi còn gì để xóa hoặc sửa không
            barBtnXoa.Enabled = !editing && bdsNV.Count > 0;
            barBtnGhi.Enabled = editing;
            barBtnHoanTac.Enabled = !editing && undoStack.Count > 0; //kiểm tra coi còn gì để hoàn tác nữa không
            barBtnRedo.Enabled = !editing && redoStack.Count > 0;
            barBtnLamMoi.Enabled = !editing;
            barBtnThoat.Enabled = true;
            gcNV.Enabled = true;
            panelNV.Enabled = editing;
        }

        private void capNhatONhap(bool readOnly)
        {
            txtMaNV.ReadOnly = true;
            txtHo.ReadOnly = readOnly;
            txtTen.ReadOnly = readOnly;
            txtCCCD.ReadOnly = readOnly;
            txtNgaySinh.Enabled = !readOnly;
            txtDiaChi.ReadOnly = readOnly;
            txtLuong.ReadOnly = readOnly;
            //txtLuong.Enabled = !readOnly; 
            txtGhiChu.ReadOnly = readOnly;
            checkBoxTrangThai.ReadOnly = true;
        }

        private void SetGridPosition(int targetPosition)
        {
            if (bdsNV.Count <= 0) return;
            if (targetPosition >= 0 && targetPosition < bdsNV.Count) { bdsNV.Position = targetPosition; }
            else { bdsNV.Position = bdsNV.Count - 1; } // Fallback về cuối

            gcNV.RefreshDataSource();
            if (gridViewNV.FocusedRowHandle >= 0 && gridViewNV.FocusedRowHandle < gridViewNV.DataRowCount) { gridViewNV.MakeRowVisible(gridViewNV.FocusedRowHandle, false); }
            gcNV.Focus();
        }

        private void SetGridFocusByMANV(int targetMANV, int fallbackPosition)
        {
            // Ensure grid and binding source are populated
            if (bdsNV.Count <= 0 || gridViewNV.DataRowCount <= 0)
            {
                SetGridPosition(fallbackPosition); // Use original fallback if grid is empty
                return;
            }

            // Use DevExpress GridView's method to find the row handle efficiently
            int gridRowHandle = gridViewNV.LocateByValue("MANV", targetMANV);

            if (gridRowHandle != DevExpress.XtraGrid.GridControl.InvalidRowHandle)
            {
                // Found the row in the grid view
                gridViewNV.FocusedRowHandle = gridRowHandle;
                gridViewNV.MakeRowVisible(gridRowHandle, false); // Scroll to the row

                // Optional: Sync BindingSource position for consistency
                int bdsIndex = bdsNV.Find("MANV", targetMANV);
                if (bdsIndex >= 0 && bdsNV.Position != bdsIndex)
                {
                    bdsNV.Position = bdsIndex;
                }
            }
            else
            {
                // MANV not found in grid (e.g., undoing an add)
                Console.WriteLine($"SetGridFocusByMANV: MANV {targetMANV} not found. Falling back to position {fallbackPosition}.");
                SetGridPosition(fallbackPosition); // Use the original index-based positioning
            }

            gcNV.Focus(); // Ensure the grid control itself has focus
        }

        private void barBtnThem_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (dangChinhSua)
            {
                MessageBox.Show("Đang sửa dữ liệu, vui lòng Ghi hoặc Thoát.", "Thông báo", MessageBoxButtons.OK);
                return;
            }
            viTri = bdsNV.Position;



            // Add new row
            bdsNV.AddNew();
            checkBoxTrangThai.Checked = false; // Default to not deleted
            txtNgaySinh.Value = DateTime.Now; // Default to current date
            txtLuong.Value = 7000000; // Default salary

            txtMaNV.ReadOnly = false;
            txtMaNV.Focus();
            dangThemMoi = true;
            dangChinhSua = false;
            capNhatNut();

        }

        private void barBtnXoa_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            // 1. Các kiểm tra cơ bản
            if (bdsNV.Count == 0)
            {
                MessageBox.Show("Không có nhân viên để xóa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            DataRowView drv = (DataRowView)bdsNV.Current;
            if (drv == null)
            {
                MessageBox.Show("Không thể xác định dòng hiện tại để xóa.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            // Kiểm tra nếu dòng đang được thêm mới (chưa ghi)
            if (drv.IsNew)
            {
                MessageBox.Show("Dữ liệu đang thêm mới chưa được ghi, không thể xóa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            // Kiểm tra trạng thái xóa hiện tại
            if (drv["TRANGTHAIXOA"] != DBNull.Value && Convert.ToInt32(drv["TRANGTHAIXOA"]) == 1)
            {
                // Tùy chọn: Có thể cho phép xóa hẳn ở đây nếu muốn, hoặc thông báo như hiện tại
                if (MessageBox.Show("Nhân viên này đã được đánh dấu xóa.\nBạn có muốn xóa vĩnh viễn khỏi CSDL không?\n(Thao tác này không thể hoàn tác dễ dàng)",
                                    "Xác nhận xóa vĩnh viễn", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                {
                    return; // Người dùng không muốn xóa hẳn
                }
                // Nếu Yes, sẽ đi vào logic Hard Delete bên dưới
            }

            // Lấy MANV từ dòng được chọn
            if (!int.TryParse(drv["MANV"]?.ToString(), out int maNVToDelete))
            {
                MessageBox.Show("Mã nhân viên không hợp lệ trên dòng được chọn.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            // Kiểm tra không cho xóa chính mình
            if (drv["MANV"].ToString() == Program.userName)
            {
                MessageBox.Show("Không thể xóa chính tài khoản đang đăng nhập.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 2. Kiểm tra ràng buộc khóa ngoại (dùng dữ liệu đã tải)
            bool hasRelatedRecords = false;
            try
            {
                // Kiểm tra trong các BindingSource đã được Fill
                if (bdsDDH.List.OfType<DataRowView>().Any(r => r.Row.RowState != DataRowState.Deleted && Convert.ToInt32(r["MANV"]) == maNVToDelete)) hasRelatedRecords = true;
                if (!hasRelatedRecords && bdsPN.List.OfType<DataRowView>().Any(r => r.Row.RowState != DataRowState.Deleted && Convert.ToInt32(r["MANV"]) == maNVToDelete)) hasRelatedRecords = true;
                if (!hasRelatedRecords && bdsPX.List.OfType<DataRowView>().Any(r => r.Row.RowState != DataRowState.Deleted && Convert.ToInt32(r["MANV"]) == maNVToDelete)) hasRelatedRecords = true;

            }
            catch (Exception exCheck)
            {
                MessageBox.Show("Lỗi khi kiểm tra ràng buộc khóa ngoại: " + exCheck.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return; // Không xóa nếu không kiểm tra được
            }

            int positionBeforeDelete = bdsNV.Position; // Lưu vị trí INDEX
            string doSql = "";
            string undoSql = "";
            string ho = drv["HO"]?.ToString() ?? "";
            string ten = drv["TEN"]?.ToString() ?? "";
            bool isSoftDelete = false;

            try
            {
                // 3. Xác nhận và tạo lệnh Do/Undo SQL
                // Ưu tiên Soft Delete nếu có phiếu liên quan
                if (hasRelatedRecords && !(drv["TRANGTHAIXOA"] != DBNull.Value && Convert.ToInt32(drv["TRANGTHAIXOA"]) == 1)) // Chỉ soft delete nếu chưa bị soft delete
                {
                    isSoftDelete = true;
                    if (MessageBox.Show($"Nhân viên [{maNVToDelete} - {ho} {ten}] đã có phiếu liên quan.\nBạn có muốn đánh dấu xóa (chuyển trạng thái) nhân viên này?",
                                        "Xác nhận đánh dấu xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                    {
                        return; // Hủy bỏ
                    }
                    doSql = $"UPDATE NHANVIEN SET TRANGTHAIXOA = 1 WHERE MANV = {maNVToDelete}";
                    undoSql = $"UPDATE NHANVIEN SET TRANGTHAIXOA = 0 WHERE MANV = {maNVToDelete}";
                    // Cập nhật trạng thái trên giao diện ngay lập tức
                    drv.BeginEdit();
                    drv["TRANGTHAIXOA"] = 1;
                    drv.EndEdit(); // Kết thúc sửa trên dòng hiện tại của bds
                }
                else // Hard Delete (Không có phiếu liên quan HOẶC đã bị soft delete và xác nhận xóa hẳn)
                {
                    isSoftDelete = false;
                    string confirmMsg = (drv["TRANGTHAIXOA"] != DBNull.Value && Convert.ToInt32(drv["TRANGTHAIXOA"]) == 1)
                        ? $"Nhân viên [{maNVToDelete} - {ho} {ten}] đang ở trạng thái xóa.\nBạn có chắc chắn muốn xóa vĩnh viễn khỏi CSDL không?"
                        : $"Nhân viên [{maNVToDelete} - {ho} {ten}] chưa có phiếu liên quan.\nBạn có chắc chắn muốn xóa vĩnh viễn nhân viên này?";

                    if (MessageBox.Show(confirmMsg, "Xác nhận xóa vĩnh viễn", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) != DialogResult.OK)
                    {
                        return; // Hủy bỏ
                    }

                    // Tạo lệnh Undo (INSERT lại) trước khi xóa khỏi bds
                    string cccd = drv["CCCD"]?.ToString() ?? "";
                    DateTime? ngaySinhRaw = drv["NGAYSINH"] as DateTime?;
                    string ngaySinhSql = ngaySinhRaw.HasValue ? $"CAST('{ngaySinhRaw.Value:yyyy-MM-dd}' AS DATETIME)" : "NULL";
                    string diaChi = drv["DIACHI"]?.ToString()?.Replace("'", "''") ?? "";
                    decimal luong = Convert.ToDecimal(drv["LUONG"] ?? 0m);
                    string ghiChu = drv["GHICHU"]?.ToString()?.Replace("'", "''") ?? "";
                    // Dù là xóa hẳn, khi undo ta nên trả về trạng thái TRANGTHAIXOA = 0
                    undoSql = string.Format(System.Globalization.CultureInfo.InvariantCulture,
                       @"INSERT INTO NHANVIEN (MANV, HO, TEN, CCCD, NGAYSINH, DIACHI, LUONG, GHICHU, TRANGTHAIXOA) VALUES ({0}, N'{1}', N'{2}', '{3}', {4}, N'{5}', {6}, N'{7}', 0)",
                       maNVToDelete, ho.Replace("'", "''"), ten.Replace("'", "''"), cccd, ngaySinhSql, diaChi, luong, ghiChu);

                    // Lệnh Do là DELETE
                    doSql = $"DELETE FROM NHANVIEN WHERE MANV = {maNVToDelete}";

                    // Xóa khỏi BindingSource để phản ánh lên GridView
                    bdsNV.RemoveCurrent();
                }

                // 4. Thực thi thay đổi vào Database (Update từ DataSet)
                // TableAdapter sẽ tự động biết cần UPDATE (cho soft delete) hay DELETE (cho hard delete)
                this.taNV.Update(this.qLVTDataSet.NHANVIEN);
                originalData = qLVTDataSet.NHANVIEN.Copy(); // Cập nhật cache sau khi DB thành công

                // 5. Lưu hành động vào Undo Stack và xóa Redo Stack
                if (!string.IsNullOrEmpty(doSql) && !string.IsNullOrEmpty(undoSql))
                {
                    // *** Pass maNVToDelete to the constructor ***
                    undoStack.Push(new UndoRedoAction(doSql, undoSql, positionBeforeDelete, maNVToDelete));
                    redoStack.Clear();
                }

                // 6. Thông báo và cập nhật UI
                MessageBox.Show(isSoftDelete ? "Đánh dấu xóa nhân viên thành công." : "Xóa nhân viên thành công.",
                                "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                capNhatNut(); // Cập nhật trạng thái các nút
            }
            catch (SqlException sqlEx)
            {
                MessageBox.Show("Lỗi SQL khi xóa: " + sqlEx.Message + "\nSố lỗi: " + sqlEx.Number + "\nLệnh thử thực thi: " + doSql, "Lỗi SQL", MessageBoxButtons.OK, MessageBoxIcon.Error);
                // Khôi phục lại dữ liệu nếu lỗi DB
                try
                {
                    this.taNV.Fill(this.qLVTDataSet.NHANVIEN); // Nạp lại từ DB
                    originalData = qLVTDataSet.NHANVIEN.Copy();
                    SetGridPosition(positionBeforeDelete >= 0 ? positionBeforeDelete : 0); // Cố gắng về vị trí cũ
                }
                catch (Exception exFill)
                {
                    MessageBox.Show("Lỗi khi tải lại dữ liệu sau khi xóa thất bại: " + exFill.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                capNhatNut(); // Cập nhật nút sau khi load lại
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi không xác định khi xóa: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                try
                {
                    this.taNV.Fill(this.qLVTDataSet.NHANVIEN); // Nạp lại từ DB
                    originalData = qLVTDataSet.NHANVIEN.Copy();
                    SetGridPosition(positionBeforeDelete >= 0 ? positionBeforeDelete : 0);
                }
                catch (Exception exFill)
                {
                    MessageBox.Show("Lỗi khi tải lại dữ liệu sau khi xóa thất bại: " + exFill.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                capNhatNut(); // Cập nhật nút sau khi load lại
            }
        }

        private void barBtnHoanTac_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (undoStack.Count == 0)
            {
                MessageBox.Show("Không có hành động để hoàn tác.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                barBtnHoanTac.Enabled = false; // Disable if stack empty
                return;
            }

            UndoRedoAction action = undoStack.Peek(); // Xem hành động trên cùng

            try
            {
                // Thực thi UndoSql vào Database
                int result = Program.ExecSqlNonQuery(action.UndoSql);
                if (result != 0) // Kiểm tra kết quả trả về từ ExecSqlNonQuery (nếu nó trả về số dòng ảnh hưởng hoặc mã lỗi)
                {
                    // Thông báo lỗi chi tiết hơn nếu có thể
                    MessageBox.Show($"Hoàn tác thất bại (Code: {result}). Lệnh đã thử: {action.UndoSql}", "Lỗi Thực Thi SQL", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    // Không Pop khỏi stack nếu lỗi, cho phép thử lại hoặc xử lý khác
                    return;
                }

                // Nếu thành công, cập nhật Stacks
                undoStack.Pop();         // Xóa khỏi undo stack
                redoStack.Push(action);  // Thêm vào redo stack

                // Nạp lại dữ liệu từ Database để đồng bộ giao diện
                this.taNV.Fill(this.qLVTDataSet.NHANVIEN);
                originalData = qLVTDataSet.NHANVIEN.Copy(); // Cập nhật cache

                // Di chuyển đến vị trí/row chính xác sau khi nạp lại dữ liệu
                // Sử dụng BeginInvoke để đảm bảo grid đã được cập nhật trước khi tìm và focus
                this.BeginInvoke(new MethodInvoker(() =>
                {
                    // Gọi hàm focus bằng MANV, dùng OriginalPosition làm fallback
                    SetGridFocusByMANV((int)action.AffectedKey, action.OriginalPosition);
                }));


                MessageBox.Show("Hoàn tác thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (SqlException sqlEx)
            {
                MessageBox.Show("Lỗi SQL khi hoàn tác: " + sqlEx.Message + "\nSố lỗi: " + sqlEx.Number + "\nLệnh: " + action.UndoSql, "Lỗi SQL", MessageBoxButtons.OK, MessageBoxIcon.Error);
                // Cố gắng nạp lại dữ liệu để đảm bảo nhất quán dù hoàn tác lỗi
                try
                {
                    this.taNV.Fill(this.qLVTDataSet.NHANVIEN);
                    originalData = qLVTDataSet.NHANVIEN.Copy();
                    SetGridPosition(0); // Về đầu danh sách nếu có lỗi
                }
                catch { }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi không xác định khi hoàn tác: " + ex.Message + "\nLệnh: " + action.UndoSql, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                try
                {
                    this.taNV.Fill(this.qLVTDataSet.NHANVIEN);
                    originalData = qLVTDataSet.NHANVIEN.Copy();
                    SetGridPosition(0);
                }
                catch { }
            }
            finally
            {
                // Đảm bảo trạng thái chỉnh sửa được reset và nút được cập nhật
                dangThemMoi = false;
                dangChinhSua = false;
                capNhatNut(); // Cập nhật lại trạng thái các nút (Undo/Redo enable/disable)
            }
        }

        private void barBtnRedo_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (redoStack.Count == 0)
            {
                MessageBox.Show("Không có hành động để thực hiện lại (Redo).", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                barBtnRedo.Enabled = false; // Disable if stack empty
                return;
            }

            UndoRedoAction action = redoStack.Peek(); // Xem hành động trên cùng

            try
            {
                // ----- KIỂM TRA RÀNG BUỘC TRƯỚC KHI THỰC THI LẠI -----
                // (Quan trọng để tránh lỗi khi trạng thái DB đã thay đổi)

                // 1. Kiểm tra nếu Redo là DELETE: Nhân viên đó có phát sinh phiếu mới không?
                if (action.DoSql.TrimStart().StartsWith("DELETE", StringComparison.OrdinalIgnoreCase))
                {
                    // Lấy MANV từ action (đã lưu trong action khi Xóa)
                    int manvAffectedByDelete = (int)action.AffectedKey;

                    bool redoHasRelatedRecords = false;
                    // Kiểm tra lại trong các BindingSource (hoặc DB nếu cần chính xác tuyệt đối)
                    if (bdsDDH.List.OfType<DataRowView>().Any(r => r.Row.RowState != DataRowState.Deleted && Convert.ToInt32(r["MANV"]) == manvAffectedByDelete)) redoHasRelatedRecords = true;
                    if (!redoHasRelatedRecords && bdsPN.List.OfType<DataRowView>().Any(r => r.Row.RowState != DataRowState.Deleted && Convert.ToInt32(r["MANV"]) == manvAffectedByDelete)) redoHasRelatedRecords = true;
                    if (!redoHasRelatedRecords && bdsPX.List.OfType<DataRowView>().Any(r => r.Row.RowState != DataRowState.Deleted && Convert.ToInt32(r["MANV"]) == manvAffectedByDelete)) redoHasRelatedRecords = true;

                    if (redoHasRelatedRecords)
                    {
                        MessageBox.Show($"Không thể thực hiện lại (Redo) thao tác DELETE.\nNhân viên [MANV={manvAffectedByDelete}] đã phát sinh dữ liệu (Phiếu) liên quan sau khi Undo.",
                                        "Lỗi Ràng Buộc Khi Redo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        redoStack.Pop(); // Xóa hành động không hợp lệ khỏi Redo stack
                        capNhatNut();    // Cập nhật lại trạng thái nút
                        return;          // Dừng thực hiện
                    }
                }
                // 2. Kiểm tra nếu Redo là INSERT: Mã NV đó có bị tạo lại (trùng) không?
                else if (action.DoSql.TrimStart().StartsWith("INSERT", StringComparison.OrdinalIgnoreCase))
                {
                    // Lấy MANV từ action (đã lưu trong action khi Thêm)
                    int manvAffectedByInsert = (int)action.AffectedKey;

                    // Kiểm tra trong BindingSource xem MANV này đã tồn tại chưa
                    if (bdsNV.Find("MANV", manvAffectedByInsert) >= 0)
                    {
                        // Optional: Double check in DB?
                        // string checkExistQuery = $"IF EXISTS (SELECT 1 FROM NHANVIEN WHERE MANV = {manvAffectedByInsert}) SELECT 1 ELSE SELECT 0";
                        // if (Program.ExecSqlScalar(checkExistQuery) == 1) { ... }

                        MessageBox.Show($"Không thể thực hiện lại (Redo) thao tác INSERT.\nMã nhân viên [MANV={manvAffectedByInsert}] đã tồn tại trong danh sách.",
                                        "Lỗi Trùng Khóa Khi Redo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        redoStack.Pop(); // Xóa hành động không hợp lệ
                        capNhatNut();
                        return;         // Dừng thực hiện
                    }
                }
                // 3. Kiểm tra nếu Redo là UPDATE TRANGTHAIXOA = 1 (Soft Delete): Có phiếu mới không?
                else if (action.DoSql.TrimStart().StartsWith("UPDATE NHANVIEN SET TRANGTHAIXOA = 1", StringComparison.OrdinalIgnoreCase))
                {
                    // Tương tự kiểm tra khóa ngoại như khi Redo DELETE, nhưng chỉ cảnh báo thay vì chặn hoàn toàn?
                    // Hoặc có thể cho phép Redo Soft Delete bất kể có phiếu mới hay không, tùy quy tắc nghiệp vụ.
                    // Ví dụ: Vẫn cho Redo Soft Delete
                }


                // ----- THỰC THI DOSQL NẾU KIỂM TRA OK -----
                int result = Program.ExecSqlNonQuery(action.DoSql);
                if (result != 0)
                {
                    MessageBox.Show($"Thực hiện lại (Redo) thất bại (Code: {result}). Lệnh đã thử: {action.DoSql}", "Lỗi Thực Thi SQL", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    // Không Pop khỏi stack nếu lỗi
                    return;
                }

                // Nếu thành công, cập nhật Stacks
                redoStack.Pop();       // Xóa khỏi redo stack
                undoStack.Push(action); // Thêm lại vào undo stack

                // Nạp lại dữ liệu từ Database
                this.taNV.Fill(this.qLVTDataSet.NHANVIEN);
                originalData = qLVTDataSet.NHANVIEN.Copy(); // Cập nhật cache

                // Di chuyển đến vị trí/row chính xác
                this.BeginInvoke(new MethodInvoker(() =>
                {
                    // Gọi hàm focus bằng MANV, dùng OriginalPosition làm fallback
                    SetGridFocusByMANV((int)action.AffectedKey, action.OriginalPosition);
                }));

                MessageBox.Show("Thực hiện lại (Redo) thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (SqlException sqlEx)
            {
                MessageBox.Show("Lỗi SQL khi thực hiện lại (Redo): " + sqlEx.Message + "\nSố lỗi: " + sqlEx.Number + "\nLệnh: " + action.DoSql, "Lỗi SQL", MessageBoxButtons.OK, MessageBoxIcon.Error);
                try
                {
                    this.taNV.Fill(this.qLVTDataSet.NHANVIEN);
                    originalData = qLVTDataSet.NHANVIEN.Copy();
                    SetGridPosition(0);
                }
                catch { }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi không xác định khi thực hiện lại (Redo): " + ex.Message + "\nLệnh: " + action.DoSql, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                try
                {
                    this.taNV.Fill(this.qLVTDataSet.NHANVIEN);
                    originalData = qLVTDataSet.NHANVIEN.Copy();
                    SetGridPosition(0);
                }
                catch { }
            }
            finally
            {
                // Đảm bảo trạng thái chỉnh sửa được reset và nút được cập nhật
                dangThemMoi = false;
                dangChinhSua = false;
                capNhatNut(); // Cập nhật lại trạng thái các nút
            }
        }

        private void barBtnSua_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            // Kiểm tra nếu đang thêm thì không cho sửa
            if (dangThemMoi)
            {
                MessageBox.Show("Đang thêm mới dữ liệu, vui lòng Ghi hoặc Thoát.", "Thông báo", MessageBoxButtons.OK);
                return;
            }
            viTri = bdsNV.Position;

            txtMaNV.ReadOnly = true;

            dangThemMoi = false;
            dangChinhSua = true;

            capNhatNut();
        }

        private void barBtnGhi_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            // 1. Validate & Chuẩn bị dữ liệu
            this.panelNV.Focus(); // Đảm bảo dữ liệu từ controls được đẩy vào BindingSource
            try
            {
                bdsNV.EndEdit(); // Kết thúc chỉnh sửa hiện tại
            }
            catch (Exception exEdit)
            {
                MessageBox.Show("Lỗi khi kết thúc chỉnh sửa: " + exEdit.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DataRowView drvCurrent = bdsNV.Current as DataRowView;
            if (drvCurrent == null)
            {
                MessageBox.Show("Không thể lấy dữ liệu dòng hiện tại.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Trim dữ liệu từ các ô nhập (TextBox/SpinEdit) thay vì từ DataRowView
            string maNVText = txtMaNV.Text.Trim();
            string hoText = txtHo.Text.Trim();
            string tenText = txtTen.Text.Trim();
            string cccdText = txtCCCD.Text.Trim();
            string diaChiText = txtDiaChi.Text.Trim();
            string ghiChuText = txtGhiChu.Text.Trim();

            // Cập nhật giá trị đã trim vào DataRowView để chuẩn bị lưu
            drvCurrent["HO"] = hoText;
            drvCurrent["TEN"] = tenText;
            drvCurrent["CCCD"] = cccdText;
            drvCurrent["DIACHI"] = diaChiText;
            drvCurrent["GHICHU"] = ghiChuText;
            if (dangThemMoi)
            {
                drvCurrent["MANV"] = maNVText;
            }

            // Thực hiện kiểm tra dữ liệu đầu vào
            if (!KiemTraDuLieuDauVao()) return; // Hàm KiemTraDuLieuDauVao sẽ hiện MessageBox

            // Lấy và kiểm tra MANV
            if (!int.TryParse(maNVText, out int maNV))
            {
                MessageBox.Show("Mã nhân viên không hợp lệ hoặc chưa được nhập.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtMaNV.Focus(); // Đưa focus về ô Mã NV
                return;
            }

            int positionBeforeSave = bdsNV.Position; // Lưu vị trí INDEX trước khi lưu
            string doSql = "";
            string undoSql = "";

            try
            {
                // 2. Tạo lệnh Do/Undo SQL dựa trên trạng thái Thêm/Sửa
                if (dangThemMoi)
                {
                    // ----- Xử lý THÊM MỚI -----

                    // 2a. Kiểm tra trùng MANV trong DB
                    string cauTruyVanKiemTra = $"DECLARE @result int; EXEC @result = sp_TraCuu_KiemTraMaNhanVien {maNV}; SELECT 'Value' = @result;";
                    SqlDataReader reader = null;
                    try
                    {
                        reader = Program.ExecSqlDataReader(cauTruyVanKiemTra);
                        if (reader != null && reader.Read() && int.Parse(reader.GetValue(0).ToString()) == 1)
                        {
                            MessageBox.Show($"Mã nhân viên [{maNV}] đã tồn tại trong cơ sở dữ liệu!", "Lỗi Trùng Mã", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            txtMaNV.Focus();
                            return; // Dừng lại nếu mã đã tồn tại
                        }
                    }
                    catch (Exception exCheck)
                    {
                        MessageBox.Show("Lỗi khi kiểm tra mã nhân viên: " + exCheck.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return; // Dừng lại nếu kiểm tra lỗi
                    }
                    finally
                    {
                        reader?.Close(); // Luôn đóng reader
                    }

                    // 2b. Tạo lệnh Do (INSERT) và Undo (DELETE)
                    string hoMoi = hoText.Replace("'", "''");
                    string tenMoi = tenText.Replace("'", "''");
                    string cccdMoi = cccdText;
                    DateTime? ngaySinhMoiRaw = txtNgaySinh.Value; // Lấy từ control
                    string ngaySinhMoiSql = ngaySinhMoiRaw.HasValue ? $"CAST('{ngaySinhMoiRaw.Value:yyyy-MM-dd}' AS DATETIME)" : "NULL";
                    string diaChiMoi = diaChiText.Replace("'", "''");
                    decimal luongMoi = Convert.ToDecimal(txtLuong.Value); // Lấy từ control
                    string ghiChuMoi = ghiChuText.Replace("'", "''");

                    doSql = string.Format(System.Globalization.CultureInfo.InvariantCulture,
                        @"INSERT INTO NHANVIEN (MANV, HO, TEN, CCCD, NGAYSINH, DIACHI, LUONG, GHICHU, TRANGTHAIXOA) VALUES ({0}, N'{1}', N'{2}', '{3}', {4}, N'{5}', {6}, N'{7}', 0)",
                        maNV, hoMoi, tenMoi, cccdMoi, ngaySinhMoiSql, diaChiMoi, luongMoi, ghiChuMoi);

                    undoSql = $"DELETE FROM NHANVIEN WHERE MANV = {maNV}";
                }
                else // dangChinhSua
                {
                    // ----- Xử lý CHỈNH SỬA -----

                    // 2c. Tạo lệnh Undo (UPDATE về giá trị cũ)
                    DataRow originalRow = originalData.Select($"MANV = {maNV}").FirstOrDefault();
                    if (originalRow == null)
                    {
                        this.taNV.Fill(this.qLVTDataSet.NHANVIEN); // Nạp lại nếu cần
                        originalData = qLVTDataSet.NHANVIEN.Copy();
                        originalRow = originalData.Select($"MANV = {maNV}").FirstOrDefault();
                        if (originalRow == null)
                        {
                            MessageBox.Show($"Không tìm thấy dữ liệu gốc cho nhân viên MANV={maNV}.", "Lỗi Dữ Liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                    }

                    string hoGoc = originalRow["HO"]?.ToString()?.Replace("'", "''") ?? "";
                    string tenGoc = originalRow["TEN"]?.ToString()?.Replace("'", "''") ?? "";
                    string cccdGoc = originalRow["CCCD"]?.ToString() ?? "";
                    DateTime? ngaySinhGocRaw = originalRow["NGAYSINH"] as DateTime?;
                    string ngaySinhGocSql = ngaySinhGocRaw.HasValue ? $"CAST('{ngaySinhGocRaw.Value:yyyy-MM-dd}' AS DATETIME)" : "NULL";
                    string diaChiGoc = originalRow["DIACHI"]?.ToString()?.Replace("'", "''") ?? "";
                    decimal luongGoc = Convert.ToDecimal(originalRow["LUONG"] ?? 0m);
                    string ghiChuGoc = originalRow["GHICHU"]?.ToString()?.Replace("'", "''") ?? "";
                    int trangThaiXoaGoc = Convert.ToInt32(originalRow["TRANGTHAIXOA"] ?? 0);

                    undoSql = string.Format(System.Globalization.CultureInfo.InvariantCulture,
                        @"UPDATE NHANVIEN SET HO=N'{1}', TEN=N'{2}', CCCD='{3}', NGAYSINH={4}, DIACHI=N'{5}', LUONG={6}, GHICHU=N'{7}', TRANGTHAIXOA={8} WHERE MANV={0}",
                        maNV, hoGoc, tenGoc, cccdGoc, ngaySinhGocSql, diaChiGoc, luongGoc, ghiChuGoc, trangThaiXoaGoc);

                    // 2d. Tạo lệnh Do (UPDATE giá trị mới)
                    string hoMoi = hoText.Replace("'", "''");
                    string tenMoi = tenText.Replace("'", "''");
                    string cccdMoi = cccdText;
                    DateTime? ngaySinhMoiRaw = txtNgaySinh.Value;
                    string ngaySinhMoiSql = ngaySinhMoiRaw.HasValue ? $"CAST('{ngaySinhMoiRaw.Value:yyyy-MM-dd}' AS DATETIME)" : "NULL";
                    string diaChiMoi = diaChiText.Replace("'", "''");
                    decimal luongMoi = Convert.ToDecimal(txtLuong.Value);
                    string ghiChuMoi = ghiChuText.Replace("'", "''");
                    int trangThaiXoaMoi = Convert.ToInt32(drvCurrent["TRANGTHAIXOA"] ?? 0);

                    doSql = string.Format(System.Globalization.CultureInfo.InvariantCulture,
                        @"UPDATE NHANVIEN SET HO=N'{1}', TEN=N'{2}', CCCD='{3}', NGAYSINH={4}, DIACHI=N'{5}', LUONG={6}, GHICHU=N'{7}', TRANGTHAIXOA={8} WHERE MANV={0}",
                        maNV, hoMoi, tenMoi, cccdMoi, ngaySinhMoiSql, diaChiMoi, luongMoi, ghiChuMoi, trangThaiXoaMoi);
                }

                // 3. Thực thi (Update DataSet -> TableAdapter -> DB)
                this.taNV.Update(this.qLVTDataSet.NHANVIEN);
                originalData = qLVTDataSet.NHANVIEN.Copy(); // Cập nhật cache dữ liệu gốc

                // 4. Lưu thông tin hành động vào Undo Stack, Xóa Redo Stack
                if (!string.IsNullOrEmpty(doSql) && !string.IsNullOrEmpty(undoSql))
                {
                    undoStack.Push(new UndoRedoAction(doSql, undoSql, positionBeforeSave, maNV));
                    redoStack.Clear();
                }
                else
                {
                    Console.WriteLine("Lỗi: Không thể tạo DoSql hoặc UndoSql.");
                }

                // 5. Cập nhật trạng thái UI
                dangThemMoi = false;
                dangChinhSua = false;
                capNhatNut();
                MessageBox.Show("Ghi dữ liệu thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
            catch (SqlException sqlEx)
            {
                MessageBox.Show("Lỗi SQL khi ghi dữ liệu: " + sqlEx.Message + "\nSố lỗi: " + sqlEx.Number + "\nLệnh thử thực thi: " + (dangThemMoi ? doSql : doSql), "Lỗi SQL", MessageBoxButtons.OK, MessageBoxIcon.Error);
                try
                {
                    this.taNV.Fill(this.qLVTDataSet.NHANVIEN);
                    originalData = qLVTDataSet.NHANVIEN.Copy();
                    SetGridPosition(positionBeforeSave > 0 ? positionBeforeSave : 0);
                }
                catch (Exception exFill)
                {
                    MessageBox.Show("Lỗi khi tải lại dữ liệu sau khi ghi thất bại: " + exFill.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi không xác định khi ghi dữ liệu: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                try
                {
                    this.taNV.Fill(this.qLVTDataSet.NHANVIEN);
                    originalData = qLVTDataSet.NHANVIEN.Copy();
                    SetGridPosition(positionBeforeSave > 0 ? positionBeforeSave : 0);
                }
                catch (Exception exFill)
                {
                    MessageBox.Show("Lỗi khi tải lại dữ liệu sau khi ghi thất bại: " + exFill.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void barBtnThoat_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (dangThemMoi || dangChinhSua)
            {
                if (MessageBox.Show("Dữ liệu chưa lưu sẽ bị mất. Thoát?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    try { bdsNV.CancelEdit(); } catch { } // Cancel pending edits

                    // If adding, remove the row if it's still marked as Added
                    if (dangThemMoi)
                    {
                        DataRowView drv = bdsNV.Current as DataRowView;
                        if (drv != null && drv.Row.RowState == DataRowState.Added)
                        {
                            try { drv.Row.RejectChanges(); } catch { try { bdsNV.RemoveCurrent(); } catch { } }
                        }
                    }

                    dangThemMoi = false; dangChinhSua = false;
                    capNhatNut();

                    // Restore original position before entering edit/add mode
                    if (bdsNV.Count > 0)
                    {
                        if (viTri >= 0 && viTri < bdsNV.Count) { bdsNV.Position = viTri; }
                        else { bdsNV.Position = 0; }
                    }
                    // this.taNV.Fill(this.qLVTDataSet.NHANVIEN);
                }
            }
            else
            {
                this.Close(); 
            }
        }

        private void barBtnLamMoi_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                int currentPosition = bdsNV.Position;
                // Tải lại data
                this.taNV.Fill(this.qLVTDataSet.NHANVIEN);
                this.taDDH.Fill(this.qLVTDataSet.DDH);
                this.taPN.Fill(this.qLVTDataSet.PHIEUNHAP);
                this.taPX.Fill(this.qLVTDataSet.PHIEUXUAT);
                originalData = qLVTDataSet.NHANVIEN.Copy();
                // Xóa lịch sử
                undoStack.Clear(); redoStack.Clear();
                // Đặt lại vị trí & UI
                SetGridPosition(currentPosition); 
                dangThemMoi = false; dangChinhSua = false; capNhatNut();
                MessageBox.Show("Làm mới thành công.", "Thông báo");
            }
            catch (Exception ex) { /* msg lỗi */ }
        }
        private bool KiemTraDuLieuDauVao()
        {
            if (txtMaNV.Text == "")
            {
                MessageBox.Show("Mã nhân viên không được để trống!", "Thông báo", MessageBoxButtons.OK);
                txtMaNV.Focus();
                return false;
            }
            if (txtHo.Text == "")
            {
                MessageBox.Show("Họ không được để trống!", "Thông báo", MessageBoxButtons.OK);
                txtHo.Focus();
                return false;
            }
            if (!Regex.IsMatch(txtHo.Text, @"^[\p{L}\s]+$"))
            {
                MessageBox.Show("Họ chỉ chứa chữ cái và khoảng trắng!", "Thông báo", MessageBoxButtons.OK);
                txtHo.Focus();
                return false;
            }
            if (txtHo.Text.Length > 40)
            {
                MessageBox.Show("Họ không quá 40 ký tự!", "Thông báo", MessageBoxButtons.OK);
                txtHo.Focus();
                return false;
            }
            if (txtTen.Text == "")
            {
                MessageBox.Show("Tên không được để trống!", "Thông báo", MessageBoxButtons.OK);
                txtTen.Focus();
                return false;
            }
            if (!Regex.IsMatch(txtTen.Text, @"^[\p{L}\s]+$"))
            {
                MessageBox.Show("Tên chỉ chứa chữ cái và khoảng trắng!", "Thông báo", MessageBoxButtons.OK);
                txtTen.Focus();
                return false;
            }
            if (txtTen.Text.Length > 10)
            {
                MessageBox.Show("Tên không quá 10 ký tự!", "Thông báo", MessageBoxButtons.OK);
                txtTen.Focus();
                return false;
            }
            if (txtCCCD.Text == "")
            {
                MessageBox.Show("CCCD không được để trống!", "Thông báo", MessageBoxButtons.OK);
                txtCCCD.Focus();
                return false;
            }
            if (!Regex.IsMatch(txtCCCD.Text, @"^\d{12}$"))
            {
                MessageBox.Show("CCCD phải là 12 số!", "Thông báo", MessageBoxButtons.OK);
                txtCCCD.Focus();
                return false;
            }
            if (txtNgaySinh.Value == null || CalculateAge((DateTime)txtNgaySinh.Value) < 18)
            {
                MessageBox.Show("Nhân viên phải từ 18 tuổi trở lên!", "Thông báo", MessageBoxButtons.OK);
                txtNgaySinh.Focus();
                return false;
            }
            if (txtDiaChi.Text == "")
            {
                MessageBox.Show("Địa chỉ không được để trống!", "Thông báo", MessageBoxButtons.OK);
                txtDiaChi.Focus();
                return false;
            }
            if (!Regex.IsMatch(txtDiaChi.Text, @"^[\p{L}\s]+$"))
            {
                MessageBox.Show("Địa chỉ chỉ chứa chữ cái và khoảng trắng!", "Thông báo", MessageBoxButtons.OK);
                txtDiaChi.Focus();
                return false;
            }
            if (txtDiaChi.Text.Length > 100)
            {
                MessageBox.Show("Địa chỉ không quá 100 ký tự!", "Thông báo", MessageBoxButtons.OK);
                txtDiaChi.Focus();
                return false;
            }
            if (txtLuong.Value < 4000000)
            {
                MessageBox.Show("Lương tối thiểu là 7.000.000!", "Thông báo", MessageBoxButtons.OK);
                txtLuong.Focus();
                return false;
            }
            if (!Regex.IsMatch(txtGhiChu.Text, @"^[\p{L}\s]+$"))
            {
                MessageBox.Show("Ghi chú chỉ chứa chữ cái và khoảng trắng!", "Thông báo", MessageBoxButtons.OK);
                txtGhiChu.Focus();
                return false;
            }
            return true;
        }

        private static int CalculateAge(DateTime dateOfBirth)
        {
            int age = DateTime.Now.Year - dateOfBirth.Year;
            if (DateTime.Now.DayOfYear < dateOfBirth.DayOfYear)
                age--;
            return age;
        }


        private void fmNhanVien_Shown(object sender, EventArgs e)
        {

        }

    }
}
