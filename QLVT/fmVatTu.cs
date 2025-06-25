using DevExpress.XtraGrid;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace QLVT
{
    public partial class fmVatTu : Form
    {
        int viTri = 0; // Vị trí trên BindingSource

        bool dangThemMoi = false; // True khi đang thêm mới
        bool dangChinhSua = false; // True khi đang sửa

        // Sử dụng Stack chứa đối tượng UndoRedoAction (dùng kiểu string cho khóa chính)
        Stack<UndoRedoAction> undoStack = new Stack<UndoRedoAction>();
        Stack<UndoRedoAction> redoStack = new Stack<UndoRedoAction>();

        DataTable originalData = new DataTable(); // Lưu dữ liệu gốc để so sánh chỉnh sửa

        public fmVatTu()
        {
            InitializeComponent();
        }

        private void vATTUBindingNavigatorSaveItem_Click(object sender, EventArgs e)
        {
            // Hàm này thường được tạo tự động, dùng để lưu trực tiếp không qua logic Undo/Redo
        }

        private void fmVatTu_Load(object sender, EventArgs e)
        {
            // Không kiểm tra ràng buộc khóa ngoại trên DataSet (để tránh lỗi khi Fill)
            qLVTDataSet.EnforceConstraints = false;

            // --- Fill các TableAdapter ---
            this.taVT.Connection.ConnectionString = Program.connstr;
            this.taVT.Fill(this.qLVTDataSet.VATTU);

            this.taCTDDH.Connection.ConnectionString = Program.connstr;
            this.taCTDDH.Fill(this.qLVTDataSet.CTDDH);

            this.taCTPN.Connection.ConnectionString = Program.connstr;
            this.taCTPN.Fill(this.qLVTDataSet.CTPN);

            this.taCTPX.Connection.ConnectionString = Program.connstr;
            this.taCTPX.Fill(this.qLVTDataSet.CTPX);

            // Lưu dữ liệu gốc ban đầu
            originalData = qLVTDataSet.VATTU.Copy();

            // Cập nhật trạng thái nút ban đầu
            capNhatNut();
            barBtnGhi.Enabled = false; // Nút Ghi ban đầu bị vô hiệu hóa
            barBtnHoanTac.Enabled = undoStack.Count > 0;
            barBtnRedo.Enabled = redoStack.Count > 0;
        }

        // --- Các hàm cập nhật UI ---

        private void capNhatNut()
        {
            bool editing = dangThemMoi || dangChinhSua;

            // Cập nhật trạng thái ReadOnly của các ô nhập liệu
            capNhatONhap(!editing);

            if (dangThemMoi)
            {
                txtMAVT.ReadOnly = false; // Cho phép nhập Mã VT khi thêm mới
                txtMAVT.Focus();
            }
            else
            {
                txtMAVT.ReadOnly = true; // Không cho sửa Mã VT khi đang sửa
            }

            // Cập nhật trạng thái Enabled của các nút trên thanh công cụ
            barBtnThem.Enabled = !editing;
            barBtnSua.Enabled = !editing && bdsVT.Count > 0;
            barBtnXoa.Enabled = !editing && bdsVT.Count > 0;
            barBtnGhi.Enabled = editing;
            barBtnHoanTac.Enabled = !editing && undoStack.Count > 0;
            barBtnRedo.Enabled = !editing && redoStack.Count > 0;
            barBtnLamMoi.Enabled = !editing;
            barBtnThoat.Enabled = true; // Nút Thoát luôn Enabled

            // Panel nhập liệu và Grid
            panelVT.Enabled = editing; // Panel nhập liệu được enable khi đang edit
        }

        private void capNhatONhap(bool readOnly)
        {
            txtTENVT.ReadOnly = readOnly;
            txtDVT.ReadOnly = readOnly;
        }

        // --- Các hàm xử lý vị trí trên Grid ---

        private void SetGridPosition(int targetPosition)
        {
            if (bdsVT.Count <= 0) return;

            // Đảm bảo targetPosition hợp lệ
            if (targetPosition < 0) targetPosition = 0;
            if (targetPosition >= bdsVT.Count) targetPosition = bdsVT.Count - 1;

            bdsVT.Position = targetPosition;

            // DevExpress Grid focus (nên thực hiện sau khi Position đã được set)
            gcVT.DataSource = bdsVT; // Đảm bảo grid đang dùng đúng binding source
            gcVT.RefreshDataSource(); // Làm mới dữ liệu grid
            if (gridViewVT.RowCount > 0) // Kiểm tra grid có dòng nào không
            {
                // Tìm row handle tương ứng với vị trí BindingSource
                int rowHandle = gridViewVT.GetRowHandle(targetPosition);
                if (rowHandle != GridControl.InvalidRowHandle) // Kiểm tra row handle hợp lệ
                {
                    gridViewVT.FocusedRowHandle = rowHandle;
                    gridViewVT.MakeRowVisible(rowHandle, false); // Cuộn đến dòng đó
                }
                else // Nếu không tìm thấy handle (trường hợp hiếm), thử focus dòng đầu/cuối
                {
                    gridViewVT.FocusedRowHandle = (targetPosition == 0) ? 0 : gridViewVT.RowCount - 1;
                }
            }
            gcVT.Focus(); // Đưa focus về Grid Control
        }

        // Hàm tìm và focus dòng dựa trên MAVT (khóa chính kiểu string)
        private void SetGridFocusByMAVT(string targetMAVT, int fallbackPosition)
        {
            if (bdsVT.Count <= 0 || gridViewVT.DataRowCount <= 0)
            {
                SetGridPosition(fallbackPosition);
                return;
            }

            int gridRowHandle = gridViewVT.LocateByValue("MAVT", targetMAVT);

            if (gridRowHandle != GridControl.InvalidRowHandle)
            {
                gridViewVT.FocusedRowHandle = gridRowHandle;
                gridViewVT.MakeRowVisible(gridRowHandle, false);

                int bdsIndex = bdsVT.Find("MAVT", targetMAVT);
                if (bdsIndex >= 0 && bdsVT.Position != bdsIndex)
                {
                    bdsVT.Position = bdsIndex;
                }
                gcVT.Focus();
            }
            else
            {
                SetGridPosition(fallbackPosition);
            }
        }

        private void barBtnThem_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (dangChinhSua) // Nếu đang sửa thì không cho thêm
            {
                MessageBox.Show("Đang sửa dữ liệu, vui lòng Ghi hoặc Thoát.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            viTri = bdsVT.Position; // Lưu vị trí hiện tại
            dangThemMoi = true;     // Đặt trạng thái đang thêm mới

            bdsVT.AddNew();         // Thêm dòng mới vào BindingSource
            intSLT.Value = 0;       // Set số lượng tồn mặc định là 0 khi thêm mới

            capNhatNut();           // Cập nhật trạng thái nút và ô nhập liệu
            // txtMAVT đã được focus và enable trong capNhatNut()
        }

        private void barBtnSua_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (dangThemMoi) // Nếu đang thêm thì không cho sửa
            {
                MessageBox.Show("Đang thêm mới dữ liệu, vui lòng Ghi hoặc Thoát.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (bdsVT.Count == 0) // Nếu không có dữ liệu thì không sửa được
            {
                MessageBox.Show("Không có dữ liệu để sửa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            viTri = bdsVT.Position; // Lưu vị trí hiện tại
            dangChinhSua = true;    // Đặt trạng thái đang chỉnh sửa

            capNhatNut();           // Cập nhật trạng thái nút và ô nhập liệu
            txtTENVT.Focus();       // Focus vào ô Tên VT
        }

        private void barBtnXoa_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            // 1. Kiểm tra cơ bản
            if (bdsVT.Count == 0)
            {
                MessageBox.Show("Không có vật tư nào để xóa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                barBtnXoa.Enabled = false; // Disable nút Xóa nếu không còn gì
                return;
            }
            DataRowView drv = bdsVT.Current as DataRowView;
            if (drv == null)
            {
                MessageBox.Show("Không thể xác định vật tư đang chọn.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (drv.IsNew) // Không cho xóa dòng mới chưa Ghi
            {
                MessageBox.Show("Vật tư đang thêm mới chưa được ghi, không thể xóa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string maVTToDelete = drv["MAVT"].ToString(); // Lấy Mã VT cần xóa

            // 2. Kiểm tra ràng buộc khóa ngoại (quan trọng đối với Vật Tư)
            bool daPhatSinh = false;
            try
            {
                // Sử dụng Count thay vì Any để có thể tối ưu hơn trên lượng dữ liệu lớn (LINQ to DataSet)
                if (this.qLVTDataSet.CTDDH.Count(r => r.RowState != DataRowState.Deleted && r.MAVT == maVTToDelete) > 0) daPhatSinh = true;
                if (!daPhatSinh && this.qLVTDataSet.CTPN.Count(r => r.RowState != DataRowState.Deleted && r.MAVT == maVTToDelete) > 0) daPhatSinh = true;
                if (!daPhatSinh && this.qLVTDataSet.CTPX.Count(r => r.RowState != DataRowState.Deleted && r.MAVT == maVTToDelete) > 0) daPhatSinh = true;

            }
            catch (Exception exCheck)
            {
                MessageBox.Show("Lỗi khi kiểm tra ràng buộc dữ liệu: " + exCheck.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return; // Không xóa nếu kiểm tra lỗi
            }


            if (daPhatSinh)
            {
                MessageBox.Show($"Không thể xóa vật tư '{maVTToDelete}'. Vật tư này đã được sử dụng trong Đơn đặt hàng, Phiếu nhập hoặc Phiếu xuất.", "Không Thể Xóa", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

            // 3. Xác nhận xóa
            if (MessageBox.Show($"Bạn có chắc chắn muốn xóa vĩnh viễn vật tư [{maVTToDelete} - {drv["TENVT"]}]?",
                                "Xác nhận xóa", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) != DialogResult.OK)
            {
                return; // Người dùng hủy
            }

            int positionBeforeDelete = bdsVT.Position; // Lưu vị trí INDEX trước khi xóa
            string doSql = "";
            string undoSql = "";

            try
            {
                // 4. Tạo lệnh Do/Undo SQL
                // Lấy thông tin để tạo lệnh Undo (INSERT)
                string tenVT = drv["TENVT"]?.ToString()?.Replace("'", "''") ?? "";
                string dvt = drv["DVT"]?.ToString()?.Replace("'", "''") ?? "";
                decimal slt = Convert.ToDecimal(drv["SOLUONGTON"] ?? 0m); // Lấy SL tồn hiện tại

                // Undo SQL: INSERT lại vật tư đã xóa
                undoSql = string.Format(System.Globalization.CultureInfo.InvariantCulture,
                   @"INSERT INTO VATTU (MAVT, TENVT, DVT, SOLUONGTON) VALUES ('{0}', N'{1}', N'{2}', {3})",
                   maVTToDelete.Replace("'", "''"), // Đảm bảo MAVT cũng được escape nếu cần
                   tenVT,
                   dvt,
                   slt);

                // Do SQL: DELETE vật tư
                doSql = $"DELETE FROM VATTU WHERE MAVT = '{maVTToDelete.Replace("'", "''")}'"; // Escape MAVT

                // 5. Thực hiện xóa trên BindingSource (để Grid cập nhật)
                bdsVT.RemoveCurrent();

                // 6. Thực thi thay đổi vào Database (Update từ DataSet)
                this.taVT.Update(this.qLVTDataSet.VATTU);
                originalData = qLVTDataSet.VATTU.Copy(); // Cập nhật cache sau khi DB thành công

                // 7. Lưu hành động vào Undo Stack
                if (!string.IsNullOrEmpty(doSql) && !string.IsNullOrEmpty(undoSql))
                {
                    undoStack.Push(new UndoRedoAction(doSql, undoSql, positionBeforeDelete, maVTToDelete));
                    redoStack.Clear(); // Xóa Redo stack khi có hành động mới
                }

                // 8. Thông báo và cập nhật UI
                MessageBox.Show("Xóa vật tư thành công.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                capNhatNut(); // Cập nhật lại trạng thái các nút

            }
            catch (SqlException sqlEx)
            {
                MessageBox.Show("Lỗi SQL khi xóa vật tư: " + sqlEx.Message + "\nSố lỗi: " + sqlEx.Number + "\nLệnh: " + doSql, "Lỗi SQL", MessageBoxButtons.OK, MessageBoxIcon.Error);
                try
                {
                    this.qLVTDataSet.VATTU.RejectChanges();
                    RefreshDataAndPosition(positionBeforeDelete); // Thay thế đoạn Fill và SetGridPosition
                }
                catch (Exception exFill)
                {
                    MessageBox.Show("Lỗi khi tải lại dữ liệu sau khi xóa thất bại: " + exFill.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                capNhatNut();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi không xác định khi xóa vật tư: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                try
                {
                    this.qLVTDataSet.VATTU.RejectChanges();
                    RefreshDataAndPosition(positionBeforeDelete); // Thay thế
                }
                catch { }
                capNhatNut();
            }
        }

        private void barBtnGhi_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            // 1. Validate & Chuẩn bị dữ liệu
            this.panelVT.Focus(); // Đẩy dữ liệu từ control vào BindingSource
            try
            {
                bdsVT.EndEdit(); // Kết thúc chỉnh sửa trên dòng hiện tại
            }
            catch (Exception exEdit)
            {
                MessageBox.Show("Lỗi khi kết thúc nhập liệu: " + exEdit.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DataRowView drvCurrent = bdsVT.Current as DataRowView;
            if (drvCurrent == null)
            {
                MessageBox.Show("Không thể lấy dữ liệu vật tư hiện tại.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 2. Kiểm tra dữ liệu đầu vào
            if (!KiemTraDuLieuDauVao()) return; // Hàm KiemTra... sẽ hiển thị MessageBox lỗi

            // 3. Lấy Mã VT (khóa chính)
            string maVT = drvCurrent["MAVT"].ToString().Trim();
            if (string.IsNullOrEmpty(maVT)) // Mã VT là bắt buộc
            {
                MessageBox.Show("Mã vật tư không được để trống.", "Thiếu Thông Tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtMAVT.Focus();
                return;
            }


            int positionBeforeSave = bdsVT.Position; // Lưu vị trí INDEX trước khi lưu
            string doSql = "";
            string undoSql = "";

            try
            {
                // 4. Tạo lệnh Do/Undo SQL dựa trên trạng thái Thêm/Sửa
                if (dangThemMoi)
                {
                    // ----- Xử lý THÊM MỚI -----

                    // 4a. Kiểm tra trùng MAVT trong DB (sử dụng SP hoặc lệnh SQL trực tiếp)
                    // Giả sử có SP: sp_TraCuu_KiemTraMaVatTu
                    string cauTruyVanKiemTra = $"DECLARE @result int; EXEC @result = sp_KiemTraMaVatTu N'{maVT.Replace("'", "''")}'; SELECT 'Value' = @result;";
                    SqlDataReader reader = null;
                    try
                    {
                        reader = Program.ExecSqlDataReader(cauTruyVanKiemTra);
                        if (reader != null && reader.Read() && int.Parse(reader.GetValue(0).ToString()) == 1)
                        {
                            MessageBox.Show($"Mã vật tư '{maVT}' đã tồn tại!", "Lỗi Trùng Mã", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            txtMAVT.Focus();
                            return; // Dừng lại nếu mã đã tồn tại
                        }
                    }
                    catch (Exception exCheck)
                    {
                        MessageBox.Show("Lỗi khi kiểm tra mã vật tư: " + exCheck.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return; // Dừng lại nếu kiểm tra lỗi
                    }
                    finally
                    {
                        reader?.Close(); // Luôn đóng reader
                    }

                    // 4b. Tạo lệnh Do (INSERT) và Undo (DELETE)
                    string tenVTMoi = drvCurrent["TENVT"]?.ToString()?.Replace("'", "''") ?? "";
                    string dvtMoi = drvCurrent["DVT"]?.ToString()?.Replace("'", "''") ?? "";
                    decimal sltMoi = Convert.ToDecimal(drvCurrent["SOLUONGTON"] ?? 0m); // Lấy SLT từ control (thường là 0 khi thêm)

                    // Do SQL: INSERT dữ liệu mới
                    doSql = string.Format(System.Globalization.CultureInfo.InvariantCulture,
                    @"INSERT INTO VATTU (MAVT, TENVT, DVT, SOLUONGTON) VALUES (N'{0}', N'{1}', N'{2}', {3})",
                    maVT.Replace("'", "''"),         // Escape và bao trong N'...' qua string.Format
                    tenVTMoi.Replace("'", "''"),
                    dvtMoi.Replace("'", "''"),
                    sltMoi);

                    // Undo SQL: DELETE dòng vừa thêm
                    undoSql = $"DELETE FROM VATTU WHERE MAVT = N'{maVT.Replace("'", "''")}'";
                }
                else // dangChinhSua
                {
                    // ----- Xử lý CHỈNH SỬA -----

                    // 4c. Tạo lệnh Undo (UPDATE về giá trị cũ)
                    DataRow originalRow = originalData.Select($"MAVT = '{maVT.Replace("'", "''")}'").FirstOrDefault(); // Tìm dòng gốc bằng MAVT (nhớ escape)
                    if (originalRow == null)
                    {
                        // Thử tải lại nếu không thấy (dữ liệu có thể bị thay đổi ngầm)
                        this.taVT.Fill(this.qLVTDataSet.VATTU);
                        originalData = qLVTDataSet.VATTU.Copy();
                        originalRow = originalData.Select($"MAVT = '{maVT.Replace("'", "''")}'").FirstOrDefault();
                        if (originalRow == null)
                        {
                            MessageBox.Show($"Không tìm thấy dữ liệu gốc cho vật tư MAVT='{maVT}' để tạo hoàn tác.", "Lỗi Dữ Liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return; // Không lưu nếu không tạo được undo
                        }
                    }

                    // Lấy giá trị gốc
                    string tenVTGoc = originalRow["TENVT"]?.ToString()?.Replace("'", "''") ?? "";
                    string dvtGoc = originalRow["DVT"]?.ToString()?.Replace("'", "''") ?? "";
                    decimal sltGoc = Convert.ToDecimal(originalRow["SOLUONGTON"] ?? 0m); // SLT gốc (quan trọng nếu nó thay đổi)

                    // Undo SQL: UPDATE về giá trị gốc
                    undoSql = string.Format(System.Globalization.CultureInfo.InvariantCulture,
                       @"UPDATE VATTU SET TENVT=N'{1}', DVT=N'{2}', SOLUONGTON={3} WHERE MAVT=N'{0}'",
                       maVT.Replace("'", "''"), tenVTGoc, dvtGoc, sltGoc);

                    // 4d. Tạo lệnh Do (UPDATE giá trị mới)
                    string tenVTMoi = drvCurrent["TENVT"]?.ToString()?.Replace("'", "''") ?? "";
                    string dvtMoi = drvCurrent["DVT"]?.ToString()?.Replace("'", "''") ?? "";
                    decimal sltMoi = Convert.ToDecimal(drvCurrent["SOLUONGTON"] ?? 0m); // SLT hiện tại (thường không đổi khi sửa thông tin)

                    // Do SQL: UPDATE các giá trị mới
                    doSql = string.Format(System.Globalization.CultureInfo.InvariantCulture,
                       @"UPDATE VATTU SET TENVT=N'{1}', DVT=N'{2}', SOLUONGTON={3} WHERE MAVT=N'{0}'",
                       maVT.Replace("'", "''"), tenVTMoi, dvtMoi, sltMoi);
                }

                // 5. Thực thi (Update DataSet -> TableAdapter -> DB)
                this.taVT.Update(this.qLVTDataSet.VATTU);
                originalData = qLVTDataSet.VATTU.Copy(); // Cập nhật cache dữ liệu gốc sau khi lưu thành công

                // 6. Lưu thông tin hành động vào Undo Stack
                if (!string.IsNullOrEmpty(doSql) && !string.IsNullOrEmpty(undoSql))
                {
                    undoStack.Push(new UndoRedoAction(doSql, undoSql, positionBeforeSave, maVT));
                    redoStack.Clear(); // Xóa lịch sử redo
                }
                

                // 7. Cập nhật trạng thái UI
                dangThemMoi = false;
                dangChinhSua = false;
                capNhatNut(); // Cập nhật trạng thái các nút
                MessageBox.Show("Ghi dữ liệu vật tư thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
            catch (SqlException sqlEx) // Bắt lỗi SQL
            {
                string loi = $"Lỗi SQL khi ghi vật tư: {sqlEx.Message}\nSố lỗi: {sqlEx.Number}\nLệnh: {(dangThemMoi ? doSql : doSql)}";
                MessageBox.Show(loi, "Lỗi SQL", MessageBoxButtons.OK, MessageBoxIcon.Error);
                try
                {
                    // Cố gắng phục hồi trạng thái trước đó
                    this.qLVTDataSet.VATTU.RejectChanges();
                    // Không Fill lại ngay lập tức, giữ nguyên lỗi để user sửa
                    // this.taVT.Fill(this.qLVTDataSet.VATTU);
                    // originalData = qLVTDataSet.VATTU.Copy();
                    // SetGridPosition(positionBeforeSave);
                }
                catch { }
                // Không cập nhật nút, giữ trạng thái editing
            }
            catch (Exception ex) // Bắt lỗi chung
            {
                MessageBox.Show("Lỗi không xác định khi ghi vật tư: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                try
                {
                    this.qLVTDataSet.VATTU.RejectChanges();
                }
                catch { }
                // Không cập nhật nút
            }
        }

        private void barBtnHoanTac_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (undoStack.Count == 0)
            {
                MessageBox.Show("Không có hành động nào để hoàn tác.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                barBtnHoanTac.Enabled = false;
                return;
            }

            UndoRedoAction action = undoStack.Peek(); // Xem trước hành động

            try
            {
                // Thực thi lệnh Undo SQL
                int result = Program.ExecSqlNonQuery(action.UndoSql);
                if (result != 0) // Kiểm tra lỗi thực thi SQL
                {
                    MessageBox.Show($"Hoàn tác thất bại (Code: {result}). Lệnh: {action.UndoSql}", "Lỗi Thực Thi SQL", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return; // Không pop nếu lỗi
                }

                // Thành công -> Cập nhật stacks
                undoStack.Pop();
                redoStack.Push(action);

                // Nạp lại dữ liệu từ DB
                this.taVT.Fill(this.qLVTDataSet.VATTU);
                originalData = qLVTDataSet.VATTU.Copy();

                // Focus lại đúng dòng bằng MAVT
                this.BeginInvoke(new MethodInvoker(() => SetGridFocusByMAVT((string)action.AffectedKey, action.OriginalPosition)));

                MessageBox.Show("Hoàn tác thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (SqlException sqlEx)
            {
                MessageBox.Show($"Lỗi SQL khi hoàn tác: {sqlEx.Message}\nLệnh: {action.UndoSql}", "Lỗi SQL", MessageBoxButtons.OK, MessageBoxIcon.Error);
                try { RefreshDataAndPosition(0); } catch { }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi không xác định khi hoàn tác: {ex.Message}\nLệnh: {action.UndoSql}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                try { RefreshDataAndPosition(0); } catch { }
            }
            finally
            {
                dangThemMoi = false;
                dangChinhSua = false;
                capNhatNut();
            }
        }


        private void barBtnRedo_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (redoStack.Count == 0)
            {
                MessageBox.Show("Không có hành động nào để thực hiện lại (Redo).", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                barBtnRedo.Enabled = false;
                return;
            }

            UndoRedoAction action = redoStack.Peek(); // Xem trước

            try
            {
                // ----- KIỂM TRA RÀNG BUỘC TRƯỚC KHI REDO -----

                // 1. Nếu Redo là DELETE: Vật tư có bị dùng lại sau khi Undo không?
                if (action.DoSql.TrimStart().StartsWith("DELETE", StringComparison.OrdinalIgnoreCase))
                {
                    string maVTAffected = (string)action.AffectedKey;
                    bool redoHasRelatedRecords = false;
                    try
                    {
                        if (this.qLVTDataSet.CTDDH.Count(r => r.RowState != DataRowState.Deleted && r.MAVT == maVTAffected) > 0) redoHasRelatedRecords = true;
                        if (!redoHasRelatedRecords && this.qLVTDataSet.CTPN.Count(r => r.RowState != DataRowState.Deleted && r.MAVT == maVTAffected) > 0) redoHasRelatedRecords = true;
                        if (!redoHasRelatedRecords && this.qLVTDataSet.CTPX.Count(r => r.RowState != DataRowState.Deleted && r.MAVT == maVTAffected) > 0) redoHasRelatedRecords = true;
                    }
                    catch (Exception exCheck)
                    {
                        MessageBox.Show("Lỗi khi kiểm tra ràng buộc trước Redo Delete: " + exCheck.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return; // Không redo nếu kiểm tra lỗi
                    }

                    if (redoHasRelatedRecords)
                    {
                        MessageBox.Show($"Không thể Redo thao tác DELETE.\nVật tư [MAVT={maVTAffected}] đã phát sinh dữ liệu liên quan sau khi Undo.",
                                        "Lỗi Ràng Buộc Khi Redo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        redoStack.Pop(); // Xóa hành động lỗi
                        capNhatNut();
                        return;
                    }
                }
                // 2. Nếu Redo là INSERT: Mã VT có bị trùng lại không?
                else if (action.DoSql.TrimStart().StartsWith("INSERT", StringComparison.OrdinalIgnoreCase))
                {
                    string maVTAffected = (string)action.AffectedKey;
                    // Kiểm tra trong BindingSource (đã fill lại sau Undo)
                    if (bdsVT.Find("MAVT", maVTAffected) >= 0)
                    {
                        // Optional: Kiểm tra DB lần nữa nếu cần
                        MessageBox.Show($"Không thể Redo thao tác INSERT.\nMã vật tư [MAVT={maVTAffected}] đã tồn tại.",
                                        "Lỗi Trùng Khóa Khi Redo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        redoStack.Pop(); // Xóa hành động lỗi
                        capNhatNut();
                        return;
                    }
                }

                // ----- THỰC THI DOSQL -----
                int result = Program.ExecSqlNonQuery(action.DoSql);
                if (result != 0)
                {
                    MessageBox.Show($"Thực hiện lại (Redo) thất bại (Code: {result}). Lệnh: {action.DoSql}", "Lỗi Thực Thi SQL", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return; // Không pop nếu lỗi
                }

                // Thành công -> Cập nhật stacks
                redoStack.Pop();
                undoStack.Push(action);

                // Nạp lại dữ liệu từ DB
                this.taVT.Fill(this.qLVTDataSet.VATTU);
                originalData = qLVTDataSet.VATTU.Copy();

                // Focus lại đúng dòng bằng MAVT
                this.BeginInvoke(new MethodInvoker(() => SetGridFocusByMAVT((string)action.AffectedKey, action.OriginalPosition)));

                MessageBox.Show("Thực hiện lại (Redo) thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (SqlException sqlEx)
            {
                MessageBox.Show($"Lỗi SQL khi Redo: {sqlEx.Message}\nLệnh: {action.DoSql}", "Lỗi SQL", MessageBoxButtons.OK, MessageBoxIcon.Error);
                try { RefreshDataAndPosition(0); } catch { }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi không xác định khi Redo: {ex.Message}\nLệnh: {action.DoSql}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                try { RefreshDataAndPosition(0); } catch { }
            }
            finally
            {
                dangThemMoi = false;
                dangChinhSua = false;
                capNhatNut();
            }
        }

        private void barBtnLamMoi_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                // Lưu vị trí hiện tại
                int currentPosition = bdsVT.Position;

                // Tải lại dữ liệu từ DB
                this.taVT.Fill(this.qLVTDataSet.VATTU);
                // Fill lại các bảng CT... nếu cần cập nhật ràng buộc
                this.taCTDDH.Fill(this.qLVTDataSet.CTDDH);
                this.taCTPN.Fill(this.qLVTDataSet.CTPN);
                this.taCTPX.Fill(this.qLVTDataSet.CTPX);

                originalData = qLVTDataSet.VATTU.Copy(); // Cập nhật cache

                // Xóa lịch sử Undo/Redo
                undoStack.Clear();
                redoStack.Clear();

                // Đặt lại vị trí và cập nhật UI
                SetGridPosition(currentPosition); // Cố gắng giữ vị trí cũ
                dangThemMoi = false;
                dangChinhSua = false;
                capNhatNut(); // Cập nhật trạng thái nút (Undo/Redo sẽ bị disable)

                MessageBox.Show("Làm mới dữ liệu thành công.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi làm mới dữ liệu: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void barBtnThoat_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (dangThemMoi || dangChinhSua)
            {
                if (MessageBox.Show("Dữ liệu chưa lưu sẽ bị mất. Thoát?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    try { bdsVT.CancelEdit(); } catch { }

                    if (dangThemMoi)
                    {
                        DataRowView drv = bdsVT.Current as DataRowView;
                        if (drv != null && drv.Row.RowState == DataRowState.Added)
                        {
                            try { drv.Row.RejectChanges(); } catch { try { bdsVT.RemoveCurrent(); } catch { } }
                        }
                    }

                    dangThemMoi = false; dangChinhSua = false;
                    capNhatNut();

                    if (bdsVT.Count > 0)
                    {
                        if (viTri >= 0 && viTri < bdsVT.Count) { bdsVT.Position = viTri; }
                        else { bdsVT.Position = 0; }
                    }
                }
            }
            else
            {
                this.Close();
            }
        }

        // --- Hàm kiểm tra dữ liệu ---

        private bool KiemTraDuLieuDauVao()
        {
            // Sử dụng Trim() để loại bỏ khoảng trắng thừa
            string maVT = txtMAVT.Text.Trim();
            string tenVT = txtTENVT.Text.Trim();
            string dvt = txtDVT.Text.Trim();

            if (string.IsNullOrEmpty(maVT))
            {
                MessageBox.Show("Mã vật tư không được để trống!", "Thiếu Thông Tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtMAVT.Focus();
                return false;
            }
            if (maVT.Length > 6) // Ví dụ: giới hạn 6 ký tự
            {
                MessageBox.Show("Mã vật tư không được vượt quá 6 ký tự!", "Dữ Liệu Không Hợp Lệ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtMAVT.Focus();
                return false;
            }

            if (string.IsNullOrEmpty(tenVT))
            {
                MessageBox.Show("Tên vật tư không được để trống!", "Thiếu Thông Tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTENVT.Focus();
                return false;
            }
            if (!Regex.IsMatch(txtTENVT.Text, @"^[\p{L}\s]+$"))
            {
                MessageBox.Show("Tên chỉ chứa chữ cái và khoảng trắng!", "Thông báo", MessageBoxButtons.OK);
                txtTENVT.Focus();
                return false;
            }
            if (tenVT.Length > 30) // Ví dụ: giới hạn 30 ký tự
            {
                MessageBox.Show("Tên vật tư không được vượt quá 30 ký tự!", "Dữ Liệu Không Hợp Lệ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTENVT.Focus();
                return false;
            }

            if (string.IsNullOrEmpty(dvt))
            {
                MessageBox.Show("Đơn vị tính không được để trống!", "Thiếu Thông Tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtDVT.Focus();
                return false;
            }
            if (!Regex.IsMatch(txtDVT.Text, @"^[\p{L}\s]+$"))
            {
                MessageBox.Show("Tên chỉ chứa chữ cái và khoảng trắng!", "Thông báo", MessageBoxButtons.OK);
                txtDVT.Focus();
                return false;
            }
            if (dvt.Length > 15) // Ví dụ: giới hạn 15 ký tự
            {
                MessageBox.Show("Đơn vị tính không được vượt quá 15 ký tự!", "Dữ Liệu Không Hợp Lệ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtDVT.Focus();
                return false;
            }

            return true; // Tất cả kiểm tra hợp lệ
        }

        private void RefreshDataAndPosition(int position)
        {
            try
            {
                this.taVT.Fill(this.qLVTDataSet.VATTU);
                originalData = qLVTDataSet.VATTU.Copy();
                SetGridPosition(position);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi làm mới dữ liệu: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }

} 