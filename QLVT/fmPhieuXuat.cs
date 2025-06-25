using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QLVT
{
    public partial class fmPhieuXuat : Form
    {
        int viTri = 0;
        int viTriCTPX = 0; // Lưu vị trí riêng cho CTPX

        bool dangThemMoi = false;
        bool dangChinhSua = false;
        bool dangThemMoiCTPX = false;
        bool dangChinhSuaCTPX = false;

        Stack<UndoRedoAction> undoStack = new Stack<UndoRedoAction>();
        Stack<UndoRedoAction> redoStack = new Stack<UndoRedoAction>();

        DataTable originalData = new DataTable();
        public fmPhieuXuat()
        {
            InitializeComponent();
        }

        private void pHIEUXUATBindingNavigatorSaveItem_Click(object sender, EventArgs e)
        {
            this.Validate();
            this.bdsPX.EndEdit();
            this.tableAdapterManager.UpdateAll(this.qLVTDataSet);

        }

        private void PhieuXuat_Load(object sender, EventArgs e)
        {
            qLVTDataSet.EnforceConstraints = false;

            this.taDSNV.Connection.ConnectionString = Program.connstr;
            this.taDSNV.Fill(this.qLVTDataSet.DSNV);

            this.taPX.Connection.ConnectionString = Program.connstr;
            this.taPX.Fill(this.qLVTDataSet.PHIEUXUAT);

            this.taCTPX.Connection.ConnectionString = Program.connstr;
            this.taCTPX.Fill(this.qLVTDataSet.CTPX);

            this.taDSPTPX.Connection.ConnectionString = Program.connstr;
            this.taDSPTPX.Fill(this.qLVTDataSet.DSCTPX);

            this.taVT.Connection.ConnectionString = Program.connstr;
            this.taVT.Fill(this.qLVTDataSet.VATTU);

            originalData = qLVTDataSet.PHIEUXUAT.Copy();


            // Cấu hình txtHOTENNV
            txtHOTENNV.DataBindings.Clear();
            txtHOTENNV.DataSource = bdsDSNV;
            txtHOTENNV.DisplayMember = "HOTEN";
            txtHOTENNV.ValueMember = "MANV";
            txtHOTENNV.DataBindings.Add(new System.Windows.Forms.Binding("SelectedValue", bdsPX, "MANV", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            txtHOTENNV.Enabled = true; // Ban đầu chỉ hiển thị, không cho chọn

            CapNhatTrangThaiGiaoDien();
            CapNhatMaNVDisplay();

        }

        private void CapNhatTrangThaiGiaoDien()
        {
            bool dangSuaHoacThemPX = dangThemMoi || dangChinhSua;
            bool dangSuaHoacThemCTPX = dangThemMoiCTPX || dangChinhSuaCTPX;
            bool coPX = bdsPX.Count > 0;
            bool coCTPX = bdsCTPX.Count > 0;

            barBtnThem.Enabled = !dangSuaHoacThemPX && !dangSuaHoacThemCTPX;
            barBtnSua.Enabled = !dangSuaHoacThemPX && !dangSuaHoacThemCTPX && coPX;
            barBtnXoa.Enabled = !dangSuaHoacThemPX && !dangSuaHoacThemCTPX && coPX;
            barBtnGhi.Enabled = dangSuaHoacThemPX;
            barBtnHoanTac.Enabled = !dangSuaHoacThemPX && !dangSuaHoacThemCTPX && undoStack.Count > 0;
            barBtnRedo.Enabled = !dangSuaHoacThemPX && !dangSuaHoacThemCTPX && redoStack.Count > 0;
            barBtnLamMoi.Enabled = !dangSuaHoacThemPX && !dangSuaHoacThemCTPX;
            barBtnThoat.Enabled = true;

            panelPX.Enabled = dangSuaHoacThemPX;
            gcPX.Enabled = !dangSuaHoacThemPX && !dangSuaHoacThemCTPX;
            gcCTPX_PX.Enabled = !dangSuaHoacThemPX;

            colMAVT.OptionsColumn.ReadOnly = !dangSuaHoacThemCTPX;
            colSOLUONG.OptionsColumn.ReadOnly = !dangSuaHoacThemCTPX;
            colDONGIA.OptionsColumn.ReadOnly = !dangSuaHoacThemCTPX;
            colMAPX1.OptionsColumn.ReadOnly = true;

            popBtnThem.Enabled = !dangSuaHoacThemPX && !dangSuaHoacThemCTPX && coPX;
            popBtnSua.Enabled = !dangSuaHoacThemCTPX && !dangSuaHoacThemPX && coCTPX;
            popBtnXoa.Enabled = !dangSuaHoacThemCTPX && !dangSuaHoacThemPX && coCTPX;
            popBtnGhi.Enabled = dangSuaHoacThemCTPX;

            txtMAPX.ReadOnly = !dangThemMoi;
            if (dangThemMoi) txtMAPX.Focus();

            capNhatONhap(!dangSuaHoacThemPX);
        }

        private void capNhatONhap(bool readOnly)
        {
            txtHOTENKH.ReadOnly = readOnly;
            txtNGAY.Enabled = !readOnly;
            txtHOTENNV.Enabled = !readOnly;
        }

        private void CapNhatMaNVDisplay()
        {
            if (txtHOTENNV.SelectedValue != null && !(txtHOTENNV.SelectedValue is DBNull))
            {
                txtMANV.Text = txtHOTENNV.SelectedValue.ToString();
            }
            else
            {
                txtMANV.Text = "";
            }
        }

        private void txtHOTENNV_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (txtHOTENNV.SelectedValue != null)
            {
                // Chỉ khi chắc chắn SelectedValue không null thì mới gọi ToString()
                txtMANV.Text = txtHOTENNV.SelectedValue.ToString();
            }
        }

        private void SetGridPosition(int targetPosition)
        {
            if (bdsPX.Count <= 0) return;
            if (targetPosition < 0) targetPosition = 0;
            if (targetPosition >= bdsPX.Count) targetPosition = bdsPX.Count - 1;
            bdsPX.Position = targetPosition;
            gcPX.Focus();
        }

        private void SetGridFocusByMAPX(string targetMAPX, int fallbackPosition)
        {
            if (bdsPX.Count <= 0 || gridView1.DataRowCount <= 0)
            {
                SetGridPosition(fallbackPosition); return;
            }
            int gridRowHandle = gridView1.LocateByValue("MAPX", targetMAPX);
            if (gridRowHandle != GridControl.InvalidRowHandle)
            {
                gridView1.FocusedRowHandle = gridRowHandle;
                gridView1.MakeRowVisible(gridRowHandle, false);
                int bdsIndex = bdsPX.Find("MAPX", targetMAPX);
                if (bdsIndex >= 0 && bdsPX.Position != bdsIndex)
                {
                    bdsPX.Position = bdsIndex;
                }
            }
            else
            {
                SetGridPosition(fallbackPosition);
            }
            gcPX.Focus();
        }

        private void barBtnThem_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (dangChinhSua) return;
            viTri = bdsPX.Position;
            dangThemMoi = true;
            bdsPX.AddNew();
            txtNGAY.Value = DateTime.Now;
            txtHOTENKH.Text = "";
            CapNhatTrangThaiGiaoDien();
            txtMAPX.Focus();
        }

        private void barBtnSua_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (dangThemMoi) return;
            if (bdsPX.Count == 0) return;
            viTri = bdsPX.Position;
            dangChinhSua = true;
            CapNhatTrangThaiGiaoDien();
            txtHOTENKH.Focus();
        }

        private void barBtnGhi_ItemClick(object sender, ItemClickEventArgs e)
        {
            panelPX.Focus();
            try { bdsPX.EndEdit(); }
            catch (Exception exEdit) { MessageBox.Show("Lỗi kết thúc nhập liệu: " + exEdit.Message, "Lỗi"); return; }

            DataRowView drvCurrent = bdsPX.Current as DataRowView;
            if (drvCurrent == null || drvCurrent.Row.RowState == DataRowState.Detached)
            {
                MessageBox.Show("Không thể lấy dữ liệu dòng hiện tại.", "Lỗi"); return;
            }
            if (!KiemTraDuLieuDauVao()) return;

            string maPX = drvCurrent["MAPX"].ToString().Trim();
            int manvMoi = Convert.ToInt32(drvCurrent["MANV"]);

            int positionBeforeSave = bdsPX.Position;
            string doSql = "";
            string undoSql = "";

            try
            {
                if (dangThemMoi)
                {
                    string cauTruyVanKiemTra = $"DECLARE @result int; EXEC @result = sp_KiemTraMaPhieuXuat '{maPX.Replace("'", "''")}'; SELECT 'Value' = @result;";
                    SqlDataReader reader = null;
                    try
                    {
                        reader = Program.ExecSqlDataReader(cauTruyVanKiemTra);
                        if (reader != null && reader.Read() && int.Parse(reader.GetValue(0).ToString()) == 1)
                        {
                            MessageBox.Show($"Mã PX '{maPX}' đã tồn tại!", "Lỗi Trùng Mã"); txtMAPX.Focus(); return;
                        }
                    }
                    catch (Exception exCheck) { MessageBox.Show("Lỗi kiểm tra Mã PX: " + exCheck.Message, "Lỗi"); return; }
                    finally { reader?.Close(); }
                    undoSql = $"DELETE FROM PHIEUXUAT WHERE MAPX = '{maPX.Replace("'", "''")}'";
                }
                else
                {
                    DataRow originalRow = originalData.Select($"MAPX = '{maPX.Replace("'", "''")}'").FirstOrDefault();
                    if (originalRow != null)
                    {
                        string hotenKHGoc = originalRow["HOTENKH"]?.ToString()?.Replace("'", "''") ?? "";
                        DateTime? ngayGocRaw = originalRow["NGAY"] as DateTime?;
                        string ngayGocSql = ngayGocRaw.HasValue ? $"'{ngayGocRaw.Value:yyyy-MM-dd}'" : "NULL";
                        int manvGoc = Convert.ToInt32(originalRow["MANV"] ?? 0);
                        undoSql = string.Format(System.Globalization.CultureInfo.InvariantCulture,
                            @"UPDATE PHIEUXUAT SET NGAY={1}, HOTENKH=N'{2}', MANV={3} WHERE MAPX='{0}'",
                            maPX.Replace("'", "''"), ngayGocSql, hotenKHGoc, manvGoc);
                    }
                }

                this.taPX.Update(this.qLVTDataSet.PHIEUXUAT);
                originalData = qLVTDataSet.PHIEUXUAT.Copy();

                string hotenKHMoi = drvCurrent["HOTENKH"]?.ToString()?.Replace("'", "''") ?? "";
                DateTime? ngayMoiRaw = drvCurrent["NGAY"] as DateTime?;
                string ngayMoiSql = ngayMoiRaw.HasValue ? $"'{ngayMoiRaw.Value:yyyy-MM-dd}'" : "NULL";

                if (dangThemMoi)
                {
                    doSql = string.Format(System.Globalization.CultureInfo.InvariantCulture,
                        @"INSERT INTO PHIEUXUAT (MAPX, NGAY, HOTENKH, MANV) VALUES ('{0}', {1}, N'{2}', {3})",
                        maPX.Replace("'", "''"), ngayMoiSql, hotenKHMoi, manvMoi);
                }
                else
                {
                    doSql = string.Format(System.Globalization.CultureInfo.InvariantCulture,
                        @"UPDATE PHIEUXUAT SET NGAY={1}, HOTENKH=N'{2}', MANV={3} WHERE MAPX='{0}'",
                        maPX.Replace("'", "''"), ngayMoiSql, hotenKHMoi, manvMoi);
                }

                if (!string.IsNullOrEmpty(doSql) && !string.IsNullOrEmpty(undoSql))
                {
                    undoStack.Push(new UndoRedoAction(doSql, undoSql, positionBeforeSave, maPX));
                    redoStack.Clear();
                }

                dangThemMoi = false; dangChinhSua = false;
                CapNhatTrangThaiGiaoDien();
                SetGridFocusByMAPX(maPX, positionBeforeSave);
                MessageBox.Show("Ghi dữ liệu thành công!", "Thông báo");
            }
            catch (SqlException sqlEx) { MessageBox.Show($"Lỗi SQL khi ghi: {sqlEx.Message}", "Lỗi SQL"); }
            catch (Exception ex) { MessageBox.Show("Lỗi không xác định khi ghi: " + ex.Message, "Lỗi"); }
        }

        private void barBtnXoa_ItemClick(object sender, ItemClickEventArgs e)
        {
            DataRowView drv = bdsPX.Current as DataRowView;
            if (drv == null || drv.IsNew)
            {
                MessageBox.Show("Vui lòng chọn một phiếu xuất đã lưu hoặc ghi lại phiếu đang thêm trước khi xóa.",
                                "Yêu Cầu Thao Tác", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string maPX = drv["MAPX"].ToString();

            bool daPhatSinh = false;
            try
            {
                if (bdsCTPX.List.OfType<DataRowView>().Any(r => r.Row.RowState != DataRowState.Deleted && r["MAPX"].ToString() == maPX))
                {
                    daPhatSinh = true;
                }
            }
            catch (Exception exCheck) { MessageBox.Show("Lỗi kiểm tra dữ liệu liên quan (CTPX): " + exCheck.Message, "Lỗi Kiểm Tra Dữ Liệu", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            if (daPhatSinh)
            {
                MessageBox.Show($"Không thể xóa phiếu xuất '{maPX}'.\nPhiếu này đã có chi tiết phiếu xuất (CTPX) liên quan.",
                                "Không Thể Xóa Do Ràng Buộc", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

            DialogResult confirmResult = MessageBox.Show($"Bạn có chắc chắn muốn xóa phiếu xuất '{maPX}' không?",
                                                        "Xác Nhận Xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirmResult != DialogResult.Yes) return;

            int positionBeforeDelete = bdsPX.Position;
            string doSql = ""; string undoSql = "";
            try
            {
                string hotenKH = drv["HOTENKH"]?.ToString()?.Replace("'", "''") ?? "";
                DateTime? ngayRaw = drv["NGAY"] as DateTime?;
                string ngaySql = ngayRaw.HasValue ? $"'{ngayRaw.Value:yyyy-MM-dd}'" : "NULL";
                int manv = Convert.ToInt32(drv["MANV"] ?? 0);
                undoSql = string.Format(System.Globalization.CultureInfo.InvariantCulture,
                    @"INSERT INTO PHIEUXUAT (MAPX, NGAY, HOTENKH, MANV) VALUES ('{0}', {1}, N'{2}', {3})",
                    maPX.Replace("'", "''"), ngaySql, hotenKH, manv);
                doSql = $"DELETE FROM PHIEUXUAT WHERE MAPX = '{maPX.Replace("'", "''")}'";

                bdsPX.RemoveCurrent();
                this.taPX.Update(this.qLVTDataSet.PHIEUXUAT);
                originalData = qLVTDataSet.PHIEUXUAT.Copy();

                if (!string.IsNullOrEmpty(doSql) && !string.IsNullOrEmpty(undoSql))
                {
                    undoStack.Push(new UndoRedoAction(doSql, undoSql, positionBeforeDelete, maPX));
                    redoStack.Clear();
                }

                MessageBox.Show($"Đã xóa thành công phiếu xuất '{maPX}'.", "Xóa Hoàn Tất", MessageBoxButtons.OK, MessageBoxIcon.Information);
                CapNhatTrangThaiGiaoDien();
            }
            catch (SqlException sqlEx)
            {
                MessageBox.Show($"Lỗi khi xóa phiếu xuất khỏi cơ sở dữ liệu.\nLỗi SQL: {sqlEx.Message}", "Lỗi Cơ Sở Dữ Liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
                try { this.taPX.Fill(this.qLVTDataSet.PHIEUXUAT); originalData = qLVTDataSet.PHIEUXUAT.Copy(); SetGridPosition(positionBeforeDelete); } catch { }
                CapNhatTrangThaiGiaoDien();
            }
            catch (Exception ex) { MessageBox.Show($"Lỗi không xác định khi xóa: {ex.Message}", "Lỗi Hệ Thống", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void barBtnHoanTac_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (undoStack.Count == 0) return;
            UndoRedoAction action = undoStack.Peek();
            try
            {
                int result = Program.ExecSqlNonQuery(action.UndoSql);
                if (result != 0) return;
                undoStack.Pop(); redoStack.Push(action);
                this.taPX.Fill(this.qLVTDataSet.PHIEUXUAT); originalData = qLVTDataSet.PHIEUXUAT.Copy();
                this.BeginInvoke(new MethodInvoker(() => SetGridFocusByMAPX((string)action.AffectedKey, action.OriginalPosition)));
                MessageBox.Show("Hoàn tác thành công!");
            }
            catch (SqlException sqlEx) { try { this.taPX.Fill(this.qLVTDataSet.PHIEUXUAT); originalData = qLVTDataSet.PHIEUXUAT.Copy(); SetGridPosition(0); } catch { } }
            catch (Exception ex) { try { this.taPX.Fill(this.qLVTDataSet.PHIEUXUAT); originalData = qLVTDataSet.PHIEUXUAT.Copy(); SetGridPosition(0); } catch { } }
            finally { dangThemMoi = false; dangChinhSua = false; CapNhatTrangThaiGiaoDien(); }
        }

        private void barBtnRedo_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (redoStack.Count == 0) return;
            UndoRedoAction action = redoStack.Peek();
            try
            {
                string maPX = (string)action.AffectedKey;
                if (action.DoSql.TrimStart().StartsWith("DELETE", StringComparison.OrdinalIgnoreCase))
                {
                    bool redoHasRelatedRecords = false;
                    try { if (qLVTDataSet.CTPX.AsEnumerable().Any(r => r.RowState != DataRowState.Deleted && r.Field<string>("MAPX") == maPX)) redoHasRelatedRecords = true; }
                    catch (Exception exCheck) { return; }
                    if (redoHasRelatedRecords) { redoStack.Pop(); CapNhatTrangThaiGiaoDien(); return; }
                }
                else if (action.DoSql.TrimStart().StartsWith("INSERT", StringComparison.OrdinalIgnoreCase))
                {
                    if (bdsPX.Find("MAPX", maPX) >= 0) { redoStack.Pop(); CapNhatTrangThaiGiaoDien(); return; }
                }

                int result = Program.ExecSqlNonQuery(action.DoSql);
                if (result != 0) return;
                redoStack.Pop(); undoStack.Push(action);
                this.taPX.Fill(this.qLVTDataSet.PHIEUXUAT); originalData = qLVTDataSet.PHIEUXUAT.Copy();
                this.BeginInvoke(new MethodInvoker(() => SetGridFocusByMAPX((string)action.AffectedKey, action.OriginalPosition)));
                MessageBox.Show("Redo thành công!");
            }
            catch (SqlException sqlEx) { try { this.taPX.Fill(this.qLVTDataSet.PHIEUXUAT); originalData = qLVTDataSet.PHIEUXUAT.Copy(); SetGridPosition(0); } catch { } }
            catch (Exception ex) { try { this.taPX.Fill(this.qLVTDataSet.PHIEUXUAT); originalData = qLVTDataSet.PHIEUXUAT.Copy(); SetGridPosition(0); } catch { } }
            finally { dangThemMoi = false; dangChinhSua = false; CapNhatTrangThaiGiaoDien(); }
        }

        private void barBtnLamMoi_ItemClick(object sender, ItemClickEventArgs e)
        {
            int currentPXPosition = bdsPX.Position;
            int currentCTPXPosition = bdsCTPX.Position;

            try
            {
                gcPX.DataSource = null;
                gcCTPX_PX.DataSource = null;
                txtHOTENNV.DataSource = null;
                repoCBTenVT.DataSource = null;

                qLVTDataSet.DSNV.Clear();
                qLVTDataSet.PHIEUXUAT.Clear();
                qLVTDataSet.VATTU.Clear();
                qLVTDataSet.CTPX.Clear();

                this.taPX.Fill(this.qLVTDataSet.PHIEUXUAT);
                this.taDSNV.Fill(this.qLVTDataSet.DSNV);
                this.taVT.Fill(this.qLVTDataSet.VATTU);
                this.taCTPX.Fill(this.qLVTDataSet.CTPX);

                bdsDSNV.DataSource = qLVTDataSet.DSNV;
                bdsVT.DataSource = qLVTDataSet.VATTU;
                bdsPX.DataSource = qLVTDataSet.PHIEUXUAT;
                bdsCTPX.DataSource = bdsPX;
                bdsCTPX.DataMember = "FK_CHITIETPHIEUXUAT_MAPX";

                // Tái cấu hình txtHOTENNV
                txtHOTENNV.DataBindings.Clear();
                txtHOTENNV.DataSource = bdsDSNV;
                txtHOTENNV.DisplayMember = "HOTEN";
                txtHOTENNV.ValueMember = "MANV";
                txtHOTENNV.DataBindings.Add(new System.Windows.Forms.Binding("SelectedValue", bdsPX, "MANV", true));
                txtHOTENNV.Enabled = true; // Đặt read-only sau khi làm mới

                repoCBTenVT.DataSource = bdsVT;
                gcPX.DataSource = bdsPX;
                gcCTPX_PX.DataSource = bdsCTPX;

                RefreshLookupVatTu();

                gridView2.RefreshData();
                gridView2.Invalidate();
                gridView2.LayoutChanged();
                gcCTPX_PX.RefreshDataSource();

                originalData = qLVTDataSet.PHIEUXUAT.Copy();
                undoStack.Clear();
                redoStack.Clear();
                dangThemMoi = false;
                dangChinhSua = false;
                dangThemMoiCTPX = false;
                dangChinhSuaCTPX = false;

                if (bdsPX.Count > 0)
                {
                    bdsPX.Position = Math.Max(0, Math.Min(currentPXPosition, bdsPX.Count - 1));
                    if (bdsCTPX.Count > 0)
                    {
                        bdsCTPX.Position = Math.Max(0, Math.Min(currentCTPXPosition, bdsCTPX.Count - 1));
                    }
                }
                else
                {
                    bdsCTPX.Position = -1; // Đảm bảo không lỗi khi bdsPX rỗng
                }

                MessageBox.Show("Làm mới thành công.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi nghiêm trọng khi làm mới: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                // Đặt lại dữ liệu an toàn nếu có lỗi
                txtHOTENNV.DataSource = null;
                txtHOTENNV.Text = "";
            }
            finally
            {
                CapNhatTrangThaiGiaoDien();
                CapNhatMaNVDisplay();
            }
        }

        private void barBtnThoat_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (dangThemMoi || dangChinhSua || dangThemMoiCTPX || dangChinhSuaCTPX)
            {
                DialogResult result = MessageBox.Show(
                    "Dữ liệu bạn đang nhập/sửa chưa được lưu.\nBạn có chắc chắn muốn thoát và hủy bỏ các thay đổi?",
                    "Xác Nhận Thoát", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result != DialogResult.Yes) return;

                try
                {
                    if (bdsPX.Current != null) HuyThaoTacPX();
                    if (bdsCTPX.Current != null) HuyThaoTacCTPX();
                    dangThemMoi = false;
                    dangChinhSua = false;
                    dangThemMoiCTPX = false;
                    dangChinhSuaCTPX = false;
                }
                catch (Exception exCancel) { MessageBox.Show("Có lỗi khi hủy thay đổi, nhưng form vẫn sẽ đóng.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            }
            else
            {
                this.Close();
            }
        }

        private void HuyThaoTacPX()
        {
            try { bdsPX.CancelEdit(); }
            catch { }
            if (dangThemMoi)
            {
                DataRowView drv = bdsPX.Current as DataRowView;
                if (drv != null && drv.Row.RowState == DataRowState.Added)
                {
                    try { bdsPX.RemoveCurrent(); }
                    catch { }
                }
                SetGridPosition(viTri);
            }
            else if (dangChinhSua)
            {
                SetGridPosition(viTri);
            }
            dangThemMoi = false;
            dangChinhSua = false;
            CapNhatTrangThaiGiaoDien();
        }

        private void HuyThaoTacCTPX()
        {
            try { bdsCTPX.CancelEdit(); }
            catch { }
            if (dangThemMoiCTPX)
            {
                DataRowView drv = bdsCTPX.Current as DataRowView;
                if (drv != null && drv.Row.RowState == DataRowState.Added)
                {
                    try { bdsCTPX.RemoveCurrent(); }
                    catch { }
                }
                if (bdsCTPX.Count > 0)
                {
                    if (viTriCTPX >= 0 && viTriCTPX < bdsCTPX.Count) bdsCTPX.Position = viTriCTPX;
                    else bdsCTPX.Position = 0;
                }
            }
            else if (dangChinhSuaCTPX)
            {
                if (bdsCTPX.Count > 0)
                {
                    if (viTriCTPX >= 0 && viTriCTPX < bdsCTPX.Count) bdsCTPX.Position = viTriCTPX;
                    else bdsCTPX.Position = 0;
                }
            }
            dangThemMoiCTPX = false;
            dangChinhSuaCTPX = false;
            CapNhatTrangThaiGiaoDien();
        }

        private bool KiemTraDuLieuDauVao()
        {
            string maPX = txtMAPX.Text.Trim();
            string hotenKH = txtHOTENKH.Text.Trim();

            if (string.IsNullOrEmpty(maPX))
            {
                MessageBox.Show("Mã phiếu xuất không được để trống!", "Thiếu Thông Tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtMAPX.Focus(); return false;
            }
            if (!Regex.IsMatch(maPX, @"^[A-Za-z0-9]+$"))
            {
                MessageBox.Show("Mã phiếu xuất chỉ được chứa chữ cái (A-Z, a-z) và số (0-9).", "Dữ Liệu Không Hợp Lệ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtMAPX.Focus(); return false;
            }
            if (maPX.Length > 8)
            {
                MessageBox.Show("Mã phiếu xuất không được vượt quá 8 ký tự!", "Dữ Liệu Không Hợp Lệ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtMAPX.Focus(); return false;
            }

            if (string.IsNullOrEmpty(hotenKH))
            {
                MessageBox.Show("Họ tên khách hàng không được để trống!", "Thiếu Thông Tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtHOTENKH.Focus(); return false;
            }
            if (!Regex.IsMatch(hotenKH, @"^[\p{L} ]+$"))
            {
                MessageBox.Show("Họ tên khách hàng chỉ được chứa chữ cái và khoảng trắng.", "Dữ Liệu Không Hợp Lệ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtHOTENKH.Focus(); return false;
            }
            if (hotenKH.Length > 100)
            {
                MessageBox.Show("Họ tên khách hàng không được vượt quá 100 ký tự!", "Dữ Liệu Không Hợp Lệ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtHOTENKH.Focus(); return false;
            }

            if (txtNGAY.Value == null || txtNGAY.Value > DateTime.Now)
            {
                MessageBox.Show("Vui lòng chọn ngày hợp lệ (không được là ngày tương lai)!", "Dữ Liệu Không Hợp Lệ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNGAY.Focus(); return false;
            }

            if (txtHOTENNV.SelectedValue == null || txtHOTENNV.SelectedValue is DBNull || string.IsNullOrEmpty(txtHOTENNV.SelectedValue.ToString()))
            {
                MessageBox.Show("Vui lòng chọn nhân viên lập phiếu!", "Thiếu Thông Tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtHOTENNV.Focus(); return false;
            }
            if (!int.TryParse(txtHOTENNV.SelectedValue.ToString(), out _))
            {
                MessageBox.Show("Mã nhân viên không hợp lệ. Vui lòng chọn lại.", "Dữ Liệu Không Hợp Lệ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtHOTENNV.Focus(); return false;
            }

            return true;
        }


        private void bdsPX_PositionChanged(object sender, EventArgs e)
        {
            if (dangThemMoiCTPX || dangChinhSuaCTPX) HuyThaoTacCTPX();
            CapNhatMaNVDisplay();
            CapNhatTrangThaiGiaoDien();
        }

        private void BdsCTPX_CurrentChanged(object sender, EventArgs e)
        {
            if (dangThemMoiCTPX || dangChinhSuaCTPX) { }
            CapNhatTrangThaiGiaoDien();
        }

        private string GetTenVT(string maVT)
        {
            if (string.IsNullOrEmpty(maVT)) return "[Chưa chọn]";
            try
            {
                var view = bdsVT.List as DataView;
                if (view != null)
                {
                    view.RowFilter = $"MAVT = '{maVT.Replace("'", "''")}'";
                    if (view.Count > 0) { string tenVT = view[0]["TENVT"].ToString(); view.RowFilter = ""; return tenVT; }
                    view.RowFilter = "";
                }
                DataRow[] rows = qLVTDataSet.VATTU.Select($"MAVT = '{maVT.Replace("'", "''")}'");
                if (rows.Length > 0) return rows[0]["TENVT"].ToString();
            }
            catch (Exception) { }
            return "[Lỗi tên VT]";
        }

        private void popCTCPX(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            CapNhatTrangThaiGiaoDien();
            popCTPX.ShowPopup(Control.MousePosition);
        }

        private void popBtnThemCTPX_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (dangThemMoi || dangChinhSua || dangThemMoiCTPX || dangChinhSuaCTPX) return;
            if (bdsPX.Count == 0 || bdsPX.Current == null)
            {
                MessageBox.Show("Vui lòng chọn một phiếu xuất trước khi thêm chi tiết.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            viTriCTPX = bdsCTPX.Position;
            dangThemMoiCTPX = true;
            bdsCTPX.AddNew();

            var drvPX = (DataRowView)bdsPX.Current;
            var drvCTPX_New = (DataRowView)bdsCTPX.Current;

            if (drvPX != null && drvCTPX_New != null)
            {
                drvCTPX_New["MAPX"] = drvPX["MAPX"];
                drvCTPX_New["SOLUONG"] = 1;
                drvCTPX_New["DONGIA"] = 0;
            }
            else
            {
                MessageBox.Show("Lỗi: Không thể lấy thông tin phiếu xuất hoặc dòng chi tiết mới.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                HuyThaoTacCTPX(); return;
            }

            RefreshLookupVatTu();
            CapNhatTrangThaiGiaoDien();
            gridView2.FocusedRowHandle = gridView2.RowCount - 1;
            gridView2.FocusedColumn = colMAVT;
            gridView2.ShowEditor();
        }

        private void popBtnSuaCTPX_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (dangThemMoi || dangChinhSua || dangThemMoiCTPX || dangChinhSuaCTPX) return;
            if (bdsCTPX.Count == 0 || bdsCTPX.Current == null)
            {
                MessageBox.Show("Không có chi tiết nào để sửa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            var drv = (DataRowView)bdsCTPX.Current;
            if (drv.IsNew)
            {
                MessageBox.Show("Dòng này đang được thêm mới, vui lòng nhấn Ghi.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            viTriCTPX = bdsCTPX.Position;
            dangChinhSuaCTPX = true;
            CapNhatTrangThaiGiaoDien();
            gcCTPX_PX.Focus();
            gridView2.FocusedColumn = colMAVT;
            gridView2.ShowEditor();
        }

        private void popBtnXoaCTPX_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (dangThemMoi || dangChinhSua || dangThemMoiCTPX || dangChinhSuaCTPX) return;
            if (bdsCTPX.Count == 0 || bdsCTPX.Current == null)
            {
                MessageBox.Show("Không có chi tiết phiếu xuất để xóa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var drv = (DataRowView)bdsCTPX.Current;
            string maPX = drv["MAPX"]?.ToString() ?? "[Lỗi Mã PX]";
            string mavt = drv["MAVT"]?.ToString() ?? "[Lỗi Mã VT]";
            string tenVT = GetTenVT(mavt);

            if (MessageBox.Show($"Xác nhận xóa vật tư '{tenVT}' (Mã: {mavt})\nkhỏi phiếu xuất '{maPX}'?", "Xác Nhận Xóa Chi Tiết", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            {
                return;
            }

            int position = bdsCTPX.Position;
            try
            {
                bdsCTPX.RemoveCurrent();
                taCTPX.Update(this.qLVTDataSet.CTPX);
                qLVTDataSet.CTPX.Clear();
                this.taCTPX.Fill(qLVTDataSet.CTPX);
                qLVTDataSet.VATTU.Clear();
                this.taVT.Fill(qLVTDataSet.VATTU);
                if (position >= 0 && position < bdsCTPX.Count) bdsCTPX.Position = position;
                else if (bdsCTPX.Count > 0) bdsCTPX.Position = 0;
                bdsCTPX.ResetBindings(false);
                gcCTPX_PX.RefreshDataSource();
            }
            catch (Exception ex)
            {
                string errorMsg = ex is SqlException ? $"Lỗi SQL khi xóa chi tiết: {ex.Message}" : $"Lỗi không xác định khi xóa chi tiết: {ex.Message}";
                MessageBox.Show(errorMsg, ex is SqlException ? "Lỗi Cơ Sở Dữ Liệu" : "Lỗi Hệ Thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
                try
                {
                    qLVTDataSet.CTPX.Clear();
                    this.taCTPX.Fill(qLVTDataSet.CTPX);
                    qLVTDataSet.VATTU.Clear();
                    this.taVT.Fill(qLVTDataSet.VATTU);
                    if (position >= 0 && position < bdsCTPX.Count) bdsCTPX.Position = position;
                    else if (bdsCTPX.Count > 0) bdsCTPX.Position = 0;
                    bdsCTPX.ResetBindings(false);
                    gcCTPX_PX.RefreshDataSource();
                }
                catch (Exception exFallback) { Console.WriteLine($"Fallback error: {exFallback.Message}"); }
                CapNhatTrangThaiGiaoDien();
            }
        }

        private void popBtnGhiCTPX_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (!dangThemMoiCTPX && !dangChinhSuaCTPX) return;

            try
            {
                if (gridView2.IsEditing)
                {
                    gridView2.CloseEditor();
                    gridView2.UpdateCurrentRow();
                }
                bdsCTPX.EndEdit();
            }
            catch (Exception exEdit) { MessageBox.Show($"Lỗi khi kết thúc nhập liệu chi tiết: {exEdit.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            var drv = (DataRowView)bdsCTPX.Current;
            if (drv == null) { MessageBox.Show("Không lấy được dữ liệu chi tiết hiện tại.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error); if (dangThemMoiCTPX) HuyThaoTacCTPX(); return; }

            string maPX = drv["MAPX"]?.ToString();
            string mavt = drv["MAVT"]?.ToString();
            string tenVT = GetTenVT(mavt);

            if (string.IsNullOrEmpty(mavt))
            {
                MessageBox.Show("Vui lòng chọn vật tư!", "Thiếu Thông Tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                gridView2.FocusedColumn = colMAVT;
                gridView2.ShowEditor(); return;
            }
            int soLuong;
            if (!int.TryParse(drv["SOLUONG"]?.ToString(), out soLuong) || soLuong <= 0)
            {
                MessageBox.Show("Số lượng phải là số nguyên dương!", "Dữ Liệu Không Hợp Lệ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                gridView2.FocusedColumn = colSOLUONG;
                gridView2.ShowEditor(); return;
            }
            int donGia;
            if (!int.TryParse(drv["DONGIA"]?.ToString(), out donGia) || donGia < 0)
            {
                MessageBox.Show("Đơn giá phải là số không âm!", "Dữ Liệu Không Hợp Lệ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                gridView2.FocusedColumn = colDONGIA;
                gridView2.ShowEditor(); return;
            }

            // Kiểm tra số lượng tồn bằng stored procedure trước khi ghi
            try
            {
                string cauTruyVanKiemTraSL = $"DECLARE @result int; EXEC @result = sp_KiemTraSLVatTu '{mavt.Replace("'", "''")}', {soLuong}; SELECT 'Value' = @result;";
                SqlDataReader reader = Program.ExecSqlDataReader(cauTruyVanKiemTraSL);
                if (reader != null && reader.Read() && int.Parse(reader.GetValue(0).ToString()) == 1)
                {
                    MessageBox.Show($"Số lượng xuất ({soLuong}) vượt quá số lượng tồn của vật tư '{tenVT}'!", "Lỗi Kiểm Tra Số Lượng", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    gridView2.FocusedColumn = colSOLUONG;
                    gridView2.ShowEditor();
                    reader.Close();
                    return;
                }
                reader.Close();
            }
            catch (Exception exCheck)
            {
                MessageBox.Show($"Lỗi khi kiểm tra số lượng tồn: {exCheck.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (dangThemMoiCTPX && !string.IsNullOrEmpty(maPX) && !string.IsNullOrEmpty(mavt))
            {
                var existingRows = qLVTDataSet.CTPX.AsEnumerable()
                    .Where(row => row.RowState != DataRowState.Deleted && row.Field<string>("MAPX") == maPX && row.Field<string>("MAVT") == mavt && row != drv.Row);
                if (existingRows.Any())
                {
                    MessageBox.Show($"Vật tư '{tenVT}' đã tồn tại trong phiếu xuất '{maPX}'.", "Lỗi Trùng Vật Tư", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    gridView2.FocusedColumn = colMAVT;
                    gridView2.ShowEditor(); return;
                }
            }

            int position = bdsCTPX.Position;
            try
            {
                this.taCTPX.Update(this.qLVTDataSet.CTPX); // Trigger sẽ tự động giảm SOLUONGTON
                dangThemMoiCTPX = false;
                dangChinhSuaCTPX = false;
                MessageBox.Show($"Ghi chi tiết vật tư '{tenVT}' thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                qLVTDataSet.CTPX.Clear();
                this.taCTPX.Fill(qLVTDataSet.CTPX);
                qLVTDataSet.VATTU.Clear();
                this.taVT.Fill(qLVTDataSet.VATTU); // Cập nhật lại số lượng tồn từ DB
                if (position >= 0 && position < bdsCTPX.Count) bdsCTPX.Position = position;
                else if (bdsCTPX.Count > 0) bdsCTPX.Position = 0;
                bdsCTPX.ResetBindings(false);
                gcCTPX_PX.RefreshDataSource();
            }
            catch (SqlException sqlEx)
            {
                if (sqlEx.Number == 2627)
                {
                    MessageBox.Show($"Vật tư '{tenVT}' đã có trong phiếu xuất '{maPX}'.", "Lỗi Trùng Vật Tư (CSDL)", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    gridView2.ShowEditor(); return;
                }
                // Nếu trigger rollback do số lượng không đủ, bắt lỗi này
                if (sqlEx.Message.Contains("Số lượng tồn không đủ"))
                {
                    MessageBox.Show($"Số lượng xuất ({soLuong}) vượt quá số lượng tồn của vật tư '{tenVT}'!", "Lỗi Kiểm Tra Số Lượng", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    gridView2.FocusedColumn = colSOLUONG;
                    gridView2.ShowEditor();
                    bdsCTPX.CancelEdit();
                    return;
                }
                MessageBox.Show($"Lỗi SQL khi ghi chi tiết: {sqlEx.Message}", "Lỗi Cơ Sở Dữ Liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
                bdsCTPX.CancelEdit();
            }
            catch (Exception ex) { MessageBox.Show($"Lỗi khi ghi chi tiết: {ex.Message}", "Lỗi Hệ Thống", MessageBoxButtons.OK, MessageBoxIcon.Error); bdsCTPX.CancelEdit(); }
            finally { dangThemMoiCTPX = false; dangChinhSuaCTPX = false; CapNhatTrangThaiGiaoDien(); }
        }

        //phu

        private void gridView2_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            if (dangThemMoiCTPX || dangChinhSuaCTPX) return;
            DataRowView drv = gridView2.GetRow(e.FocusedRowHandle) as DataRowView;
            if (drv != null)
            {
                string mavt = drv["MAVT"]?.ToString();
                if (!string.IsNullOrEmpty(mavt)) gridView2.SetRowCellValue(e.FocusedRowHandle, colMAVT, mavt);
            }
        }

        private void gridView2_CustomRowCellEditForEditing(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            if (e.Column == colMAVT) e.RepositoryItem = repoCBTenVT;
        }

        private void repoCBTenVT_QueryPopUp(object sender, System.ComponentModel.CancelEventArgs e)
        {
            LookUpEdit lookUpEdit = sender as LookUpEdit;
            if (lookUpEdit == null) return;

            DataRowView drvPX = bdsPX.Current as DataRowView;
            if (drvPX == null) { lookUpEdit.Properties.DataSource = this.bdsVT; return; }

            string currentMaPX = drvPX["MAPX"]?.ToString();
            if (string.IsNullOrEmpty(currentMaPX)) return;

            DataRowView drvCTPX = gridView2.GetRow(gridView2.FocusedRowHandle) as DataRowView;
            string currentMAVT = drvCTPX != null ? drvCTPX["MAVT"]?.ToString() : null;

            var usedMaVTs = qLVTDataSet.CTPX.AsEnumerable()
                .Where(row => row.RowState != DataRowState.Deleted && row.Field<string>("MAPX") == currentMaPX)
                .Select(row => row.Field<string>("MAVT"))
                .Where(mavt => !string.IsNullOrEmpty(mavt))
                .Distinct()
                .ToList();

            DataView dvVattu = new DataView(qLVTDataSet.VATTU);
            if (usedMaVTs.Count > 0)
            {
                var escapedMaVTs = usedMaVTs.Select(mavt => $"'{mavt.Replace("'", "''")}'");
                string filter = $"MAVT NOT IN ({string.Join(",", escapedMaVTs)})";
                if (dangThemMoiCTPX) dvVattu.RowFilter = filter;
                else if (dangChinhSuaCTPX && !string.IsNullOrEmpty(currentMAVT) && usedMaVTs.Contains(currentMAVT))
                {
                    var filteredMaVTs = usedMaVTs.Where(m => m != currentMAVT).Select(m => $"'{m.Replace("'", "''")}'");
                    dvVattu.RowFilter = filteredMaVTs.Any() ? $"MAVT NOT IN ({string.Join(",", filteredMaVTs)})" : "";
                }
                else dvVattu.RowFilter = "";
            }
            else dvVattu.RowFilter = "";

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
                gcCTPX_PX.BeginUpdate();
                repoCBTenVT.DataSource = null;
                qLVTDataSet.VATTU.Clear();
                this.taVT.Fill(this.qLVTDataSet.VATTU);
                bdsVT.ResetBindings(false);
                repoCBTenVT.DataSource = bdsVT;
                colMAVT.ColumnEdit = repoCBTenVT;
                gridView2.RefreshData();
                gridView2.Invalidate();
                gridView2.LayoutChanged();
                gcCTPX_PX.RefreshDataSource();
            }
            catch (Exception ex) { MessageBox.Show($"Error refreshing VatTu lookup: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            finally { gcCTPX_PX.EndUpdate(); }
        }

    }
}
