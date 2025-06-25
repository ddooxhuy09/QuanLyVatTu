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
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QLVT
{
    public partial class fmPhieuNhap : Form
    {
        int viTri = 0;           // Vị trí của PN
        int viTriCTPN = 0;       // Vị trí của CTPN
        bool dangThemMoi = false;
        bool dangChinhSua = false;
        bool dangThemMoiCTPN = false;
        bool dangChinhSuaCTPN = false;

        Stack<UndoRedoAction> undoStack = new Stack<UndoRedoAction>();
        Stack<UndoRedoAction> redoStack = new Stack<UndoRedoAction>();

        DataTable originalData = new DataTable();
        public fmPhieuNhap()
        {
            InitializeComponent();
        }

        private void pHIEUNHAPBindingNavigatorSaveItem_Click(object sender, EventArgs e)
        {
            this.Validate();
            this.bdsPN.EndEdit();
            this.tableAdapterManager.UpdateAll(this.qLVTDataSet);

        }

        private void fmPhieuNhap_Load(object sender, EventArgs e)
        {
            
            qLVTDataSet.EnforceConstraints = false;

            this.taPN.Connection.ConnectionString = Program.connstr;
            this.taPN.Fill(this.qLVTDataSet.PHIEUNHAP);
            this.taVT.Connection.ConnectionString = Program.connstr;
            this.taVT.Fill(this.qLVTDataSet.VATTU);
            this.taDSNV.Connection.ConnectionString = Program.connstr;
            this.taDSNV.Fill(this.qLVTDataSet.DSNV);
            this.taDSDDHChuaNhap.Connection.ConnectionString = Program.connstr;
            this.taDSDDHChuaNhap.Fill(this.qLVTDataSet.DSDDHChuaNhap);
            this.taDDH.Connection.ConnectionString = Program.connstr;
            this.taDDH.Fill(this.qLVTDataSet.DDH);
            this.taCTPN.Connection.ConnectionString = Program.connstr;
            this.taCTPN.Fill(this.qLVTDataSet.CTPN);

            originalData = qLVTDataSet.PHIEUNHAP.Copy();

            // Cấu hình txtHOTENNV
            txtHOTENNV.DataBindings.Clear();
            txtHOTENNV.DataSource = bdsDSNV;
            txtHOTENNV.DisplayMember = "HOTEN";
            txtHOTENNV.ValueMember = "MANV";
            txtHOTENNV.DataBindings.Add(new System.Windows.Forms.Binding("SelectedValue", bdsPN, "MANV", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            txtHOTENNV.Enabled = true;

            // Cấu hình lookUpMaDDH
            lookUpMaDDH.DataBindings.Clear();
            lookUpMaDDH.DataBindings.Add(new System.Windows.Forms.Binding("EditValue", bdsPN, "MASODDH", true));

            // Làm mới và đồng bộ lookUpMaDDH
            RefreshLookupMaDDH();

            // Đồng bộ giá trị MASODDH ban đầu
            if (bdsPN.Current != null)
            {
                DataRowView drv = bdsPN.Current as DataRowView;
                lookUpMaDDH.EditValue = drv["MASODDH"]?.ToString();
            }

            CapNhatTrangThaiGiaoDien();
            CapNhatMaNVDisplay();
        }
        private void lookUpMaDDH_EditValueChanged(object sender, EventArgs e)
        {
            if (bdsPN.Current != null && (dangThemMoi || dangChinhSua))
            {
                DataRowView drv = bdsPN.Current as DataRowView;
                string selectedMaDDH = lookUpMaDDH.EditValue?.ToString();
                drv["MASODDH"] = selectedMaDDH; // Cập nhật MASODDH vào bdsPN
            }

            // Cập nhật txtNhaCC dựa trên MASODDH
            if (lookUpMaDDH.EditValue != null)
            {
                string maDDH = lookUpMaDDH.EditValue.ToString();
                try
                {
                    // Truy vấn NHACC từ DDH dựa trên MASODDH
                    string query = $"SELECT NHACC FROM DDH WHERE MASODDH = '{maDDH.Replace("'", "''")}'";
                    using (SqlDataReader reader = Program.ExecSqlDataReader(query))
                    {
                        if (reader != null && reader.Read())
                        {
                            txtNhaCC.Text = reader["NHACC"]?.ToString() ?? "";
                        }
                        else
                        {
                            txtNhaCC.Text = "";
                        }
                    }
                }
                catch (Exception ex)
                {
                    txtNhaCC.Text = "";
                    MessageBox.Show($"Lỗi khi lấy nhà cung cấp: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                txtNhaCC.Text = "";
            }
        }


        private void CapNhatMaNVDisplay()
        {
            if (bdsPN.Current != null)
            {
                DataRowView drv = bdsPN.Current as DataRowView;
                if (drv["MANV"] != DBNull.Value)
                {
                    txtMANV.EditValue = drv["MANV"].ToString();
                    txtHOTENNV.SelectedValue = drv["MANV"].ToString(); // Sync txtHOTENNV
                }
                else
                {
                    txtMANV.EditValue = null;
                    txtHOTENNV.SelectedIndex = -1; // Clear selection
                }
            }
            else
            {
                txtMANV.EditValue = null;
                txtHOTENNV.SelectedIndex = -1; // Clear selection
            }
        }

        private void SetGridPosition(int targetPosition)
        {
            if (bdsPN.Count <= 0) return;
            if (targetPosition < 0) targetPosition = 0;
            if (targetPosition >= bdsPN.Count) targetPosition = bdsPN.Count - 1;
            bdsPN.Position = targetPosition;
            gcPN.Focus();
        }

        private void SetGridFocusByMAPN(string targetMAPN, int fallbackPosition)
        {
            if (bdsPN.Count <= 0 || gridView1.DataRowCount <= 0)
            {
                SetGridPosition(fallbackPosition); return;
            }
            int gridRowHandle = gridView1.LocateByValue("MAPN", targetMAPN);
            if (gridRowHandle != GridControl.InvalidRowHandle)
            {
                gridView1.FocusedRowHandle = gridRowHandle;
                gridView1.MakeRowVisible(gridRowHandle, false);
                int bdsIndex = bdsPN.Find("MAPN", targetMAPN);
                if (bdsIndex >= 0 && bdsPN.Position != bdsIndex)
                {
                    bdsPN.Position = bdsIndex;
                }
                else if (bdsIndex >= 0 && bdsPN.Position == bdsIndex)
                {
                    CapNhatMaNVDisplay();
                }
            }
            else
            {
                SetGridPosition(fallbackPosition);
            }
            gcPN.Focus();
        }

        private bool KiemTraDuLieuDauVao()
        {
            string maPN = txtMaPN.Text.Trim();
            if (string.IsNullOrEmpty(maPN))
            {
                MessageBox.Show("Mã phiếu nhập không được để trống!", "Thiếu Thông Tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtMaPN.Focus(); return false;
            }
            if (maPN.Length > 8)
            {
                MessageBox.Show("Mã phiếu nhập không được vượt quá 8 ký tự!", "Dữ Liệu Không Hợp Lệ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtMaPN.Focus(); return false;
            }
            if (txtNgay.Value == null || txtNgay.Value.Date > DateTime.Now.Date)
            {
                MessageBox.Show("Ngày nhập không hợp lệ!", "Dữ Liệu Không Hợp Lệ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNgay.Focus(); return false;
            }
            if (txtHOTENNV.SelectedValue == null || txtHOTENNV.SelectedValue is DBNull)
            {
                MessageBox.Show("Vui lòng chọn nhân viên!", "Thiếu Thông Tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtHOTENNV.Focus(); return false;
            }
            if (!int.TryParse(txtHOTENNV.SelectedValue.ToString(), out _))
            {
                MessageBox.Show("Mã nhân viên không hợp lệ!", "Dữ Liệu Không Hợp Lệ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtHOTENNV.Focus(); return false;
            }
            if (dangThemMoi && (lookUpMaDDH.EditValue == null || string.IsNullOrEmpty(lookUpMaDDH.EditValue.ToString())))
            {
                MessageBox.Show("Vui lòng chọn mã đơn đặt hàng!", "Thiếu Thông Tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                lookUpMaDDH.Focus(); return false;
            }
            return true;
        }

        private void CapNhatTrangThaiGiaoDien()
        {
            bool dangSuaHoacThemPN = dangThemMoi || dangChinhSua;
            bool dangSuaHoacThemCTPN = dangThemMoiCTPN || dangChinhSuaCTPN;
            bool coPN = bdsPN.Count > 0;
            bool coCTPN = bdsCTPN.Count > 0;

            barBtnThem.Enabled = !dangSuaHoacThemPN && !dangSuaHoacThemCTPN;
            barBtnSua.Enabled = !dangSuaHoacThemPN && !dangSuaHoacThemCTPN && coPN;
            barBtnXoa.Enabled = !dangSuaHoacThemPN && !dangSuaHoacThemCTPN && coPN;
            barBtnGhi.Enabled = dangSuaHoacThemPN;
            barBtnHoanTac.Enabled = !dangSuaHoacThemPN && !dangSuaHoacThemCTPN && undoStack.Count > 0;
            barBtnRedo.Enabled = !dangSuaHoacThemPN && !dangSuaHoacThemCTPN && redoStack.Count > 0;
            barBtnLamMoi.Enabled = !dangSuaHoacThemPN && !dangSuaHoacThemCTPN;
            barBtnThoat.Enabled = true;

            panelPN.Enabled = dangSuaHoacThemPN;
            gcPN.Enabled = !dangSuaHoacThemPN && !dangSuaHoacThemCTPN;
            gcCTPN_PN.Enabled = !dangSuaHoacThemPN;
            lookUpMaDDH.ReadOnly = !dangSuaHoacThemPN;

            colMAPN1.OptionsColumn.ReadOnly = !dangSuaHoacThemCTPN;
            colMAVT.OptionsColumn.ReadOnly = !dangSuaHoacThemCTPN;
            colSOLUONG.OptionsColumn.ReadOnly = !dangSuaHoacThemCTPN;
            colDONGIA.OptionsColumn.ReadOnly = !dangSuaHoacThemCTPN;

            popBtnThem.Enabled = !dangSuaHoacThemPN && !dangSuaHoacThemCTPN && coPN;
            popBtnSua.Enabled = !dangSuaHoacThemCTPN && !dangSuaHoacThemPN && coCTPN;
            popBtnXoa.Enabled = !dangSuaHoacThemCTPN && !dangSuaHoacThemPN && coCTPN;
            popBtnGhi.Enabled = dangSuaHoacThemCTPN;

            txtMaPN.ReadOnly = !dangThemMoi;
            if (dangThemMoi) txtMaPN.Focus();

            // Kích hoạt txtHOTENNV khi thêm/sửa
            txtHOTENNV.Enabled = !dangSuaHoacThemPN;
            if (dangThemMoi)
            {
                txtHOTENNV.SelectedIndex = -1; // Đặt lại để người dùng chọn
            }

            // Đồng bộ giá trị MASODDH
            if (!dangThemMoi && !dangChinhSua && bdsPN.Current != null)
            {
                DataRowView drv = bdsPN.Current as DataRowView;
                lookUpMaDDH.EditValue = drv["MASODDH"]?.ToString();
            }
            else if (dangThemMoi || dangChinhSua)
            {
                RefreshLookupMaDDH();
            }

            capNhatONhap(!dangSuaHoacThemPN);
        }

        private void capNhatONhap(bool readOnly)
        {
            txtNgay.Enabled = !readOnly;
            txtHOTENNV.Enabled = !readOnly;
            //lookUpMaDDH.ReadOnly = readOnly; //coi lại
        }

        private void HuyThaoTacPN()
        {
            try { bdsPN.CancelEdit(); } catch { }
            if (dangThemMoi)
            {
                DataRowView drv = bdsPN.Current as DataRowView;
                if (drv != null && drv.Row.RowState == DataRowState.Added)
                {
                    try { bdsPN.RemoveCurrent(); } catch { }
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

        private void HuyThaoTacCTPN()
        {
            try { bdsCTPN.CancelEdit(); } catch { }
            if (dangThemMoiCTPN)
            {
                DataRowView drv = bdsCTPN.Current as DataRowView;
                if (drv != null && drv.Row.RowState == DataRowState.Added)
                {
                    try { bdsCTPN.RemoveCurrent(); } catch { }
                }
                if (bdsCTPN.Count > 0)
                {
                    if (viTriCTPN >= 0 && viTriCTPN < bdsCTPN.Count) bdsCTPN.Position = viTriCTPN;
                    else bdsCTPN.Position = 0;
                }
            }
            else if (dangChinhSuaCTPN)
            {
                if (bdsCTPN.Count > 0)
                {
                    if (viTriCTPN >= 0 && viTriCTPN < bdsCTPN.Count) bdsCTPN.Position = viTriCTPN;
                    else bdsCTPN.Position = 0;
                }
            }
            dangThemMoiCTPN = false;
            dangChinhSuaCTPN = false;
            CapNhatTrangThaiGiaoDien();
        }
        private void popCTCPN(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            //if (e.MenuType == DevExpress.XtraGrid.Views.Grid.GridMenuType.Row)
            {
                popCTPN.ShowPopup(Control.MousePosition);
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

        //xử lý các nút phiếu nhập

        private void barBtnThem_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (dangChinhSua) return;
            viTri = bdsPN.Position;
            dangThemMoi = true;
            bdsPN.AddNew();
            txtNgay.Value = DateTime.Now;
            txtHOTENNV.SelectedIndex = -1; // Reset selection
            txtMANV.EditValue = null; // Clear MANV
            RefreshLookupMaDDH(); // Làm mới danh sách MASODDH chưa dùng
            lookUpMaDDH.EditValue = null; // Yêu cầu chọn MASODDH
            CapNhatTrangThaiGiaoDien();
            txtMaPN.Focus();
        }

        private void barBtnSua_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (dangThemMoi) return;
            if (bdsPN.Count == 0) return;
            viTri = bdsPN.Position;
            dangChinhSua = true;

            // Lưu và đồng bộ giá trị MASODDH hiện tại
            if (bdsPN.Current != null)
            {
                DataRowView drv = bdsPN.Current as DataRowView;
                string currentMaDDH = drv["MASODDH"]?.ToString();
                lookUpMaDDH.EditValue = currentMaDDH;
            }

            RefreshLookupMaDDH(); // Làm mới danh sách MASODDH chưa dùng
            CapNhatTrangThaiGiaoDien();
        }

        private void barBtnXoa_ItemClick(object sender, ItemClickEventArgs e)
        {
            DataRowView drv = bdsPN.Current as DataRowView;
            if (drv == null) { MessageBox.Show("Vui lòng chọn một phiếu nhập.", "Yêu Cầu Thao Tác", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

            string maPN = drv["MAPN"].ToString();
            bool daPhatSinh = bdsCTPN.List.OfType<DataRowView>().Any(r => r.Row.RowState != DataRowState.Deleted && r["MAPN"].ToString() == maPN);

            if (daPhatSinh)
            {
                MessageBox.Show($"Không thể xóa phiếu nhập '{maPN}'.\nPhiếu này đã có chi tiết (CTPN) liên quan.", "Không Thể Xóa Do Ràng Buộc", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

            if (MessageBox.Show($"Xác nhận xóa phiếu nhập '{maPN}'?", "Xác Nhận Xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

            int positionBeforeDelete = bdsPN.Position;
            string doSql = $"DELETE FROM PHIEUNHAP WHERE MAPN = '{maPN.Replace("'", "''")}'";
            string undoSql = $"INSERT INTO PHIEUNHAP (MAPN, NGAY, MANV, MASODDH) VALUES ('{maPN.Replace("'", "''")}', '{((DateTime)drv["NGAY"]).ToString("yyyy-MM-dd")}', {drv["MANV"]}, '{drv["MASODDH"]}')";

            try
            {
                bdsPN.RemoveCurrent();
                this.taPN.Update(this.qLVTDataSet.PHIEUNHAP);
                originalData = qLVTDataSet.PHIEUNHAP.Copy();

                if (!string.IsNullOrEmpty(doSql) && !string.IsNullOrEmpty(undoSql))
                {
                    undoStack.Push(new UndoRedoAction(doSql, undoSql, positionBeforeDelete, maPN));
                    redoStack.Clear();
                }

                MessageBox.Show($"Đã xóa thành công phiếu nhập '{maPN}'.", "Xóa Hoàn Tất", MessageBoxButtons.OK, MessageBoxIcon.Information);
                CapNhatTrangThaiGiaoDien();
            }
            catch (SqlException sqlEx) { MessageBox.Show($"Lỗi SQL khi xóa: {sqlEx.Message}", "Lỗi Cơ Sở Dữ Liệu", MessageBoxButtons.OK, MessageBoxIcon.Error); this.taPN.Fill(this.qLVTDataSet.PHIEUNHAP); originalData = qLVTDataSet.PHIEUNHAP.Copy(); SetGridPosition(positionBeforeDelete); }
            catch (Exception ex) { MessageBox.Show($"Lỗi không xác định khi xóa: {ex.Message}", "Lỗi Hệ Thống", MessageBoxButtons.OK, MessageBoxIcon.Error); this.taPN.Fill(this.qLVTDataSet.PHIEUNHAP); originalData = qLVTDataSet.PHIEUNHAP.Copy(); SetGridPosition(positionBeforeDelete); }
        }

        private void barBtnHoanTac_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (undoStack.Count == 0) return;
            UndoRedoAction action = undoStack.Peek();
            try
            {
                Program.ExecSqlNonQuery(action.UndoSql);
                undoStack.Pop(); redoStack.Push(action);
                this.taPN.Fill(this.qLVTDataSet.PHIEUNHAP); originalData = qLVTDataSet.PHIEUNHAP.Copy();
                SetGridFocusByMAPN((string)action.AffectedKey, action.OriginalPosition);
                MessageBox.Show("Hoàn tác thành công!");
            }
            catch (Exception) { this.taPN.Fill(this.qLVTDataSet.PHIEUNHAP); originalData = qLVTDataSet.PHIEUNHAP.Copy(); SetGridPosition(0); }
            finally { dangThemMoi = false; dangChinhSua = false; CapNhatTrangThaiGiaoDien(); }
        }

        private void barBtnLamMoi_ItemClick(object sender, ItemClickEventArgs e)
        {
            int currentPNPosition = bdsPN.Position;
            int currentCTPNPosition = bdsCTPN.Position;

            try
            {
                gcPN.DataSource = null;
                gcCTPN_PN.DataSource = null;
                txtHOTENNV.DataSource = null;
                repoCBTenVT.DataSource = null;

                qLVTDataSet.DSNV.Clear();
                qLVTDataSet.PHIEUNHAP.Clear();
                qLVTDataSet.VATTU.Clear();
                qLVTDataSet.CTPN.Clear();
                qLVTDataSet.DSNHACC.Clear();

                this.taPN.Fill(this.qLVTDataSet.PHIEUNHAP);
                this.taDSNV.Fill(this.qLVTDataSet.DSNV);
                this.taVT.Fill(this.qLVTDataSet.VATTU);
                this.taCTPN.Fill(this.qLVTDataSet.CTPN);
                this.taDDH.Fill(this.qLVTDataSet.DDH);

                bdsDSNV.DataSource = qLVTDataSet.DSNV;
                bdsVT.DataSource = qLVTDataSet.VATTU;
                bdsPN.DataSource = qLVTDataSet.PHIEUNHAP;
                bdsCTPN.DataSource = bdsPN;
                bdsCTPN.DataMember = "FK_CHITIETPHIEUNHAP_MAPN";

                // Tái cấu hình txtHOTENNV
                txtHOTENNV.DataBindings.Clear();
                txtHOTENNV.DataSource = bdsDSNV;
                txtHOTENNV.DisplayMember = "HOTEN";
                txtHOTENNV.ValueMember = "MANV";
                txtHOTENNV.DataBindings.Add(new System.Windows.Forms.Binding("SelectedValue", bdsPN, "MANV", true));
                txtHOTENNV.Enabled = true; // Đặt read-only sau khi làm mới

                repoCBTenVT.DataSource = bdsVT;
                gcPN.DataSource = bdsPN;
                gcCTPN_PN.DataSource = bdsCTPN;

                gridView2.RefreshData();
                gcCTPN_PN.RefreshDataSource();

                originalData = qLVTDataSet.PHIEUNHAP.Copy();
                undoStack.Clear();
                redoStack.Clear();
                dangThemMoi = false;
                dangChinhSua = false;
                dangThemMoiCTPN = false;
                dangChinhSuaCTPN = false;

                if (bdsPN.Count > 0)
                {
                    bdsPN.Position = Math.Max(0, Math.Min(currentPNPosition, bdsPN.Count - 1));
                    if (bdsCTPN.Count > 0)
                    {
                        bdsCTPN.Position = Math.Max(0, Math.Min(currentCTPNPosition, bdsCTPN.Count - 1));
                    }
                }

                MessageBox.Show("Làm mới thành công.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex) { MessageBox.Show("Lỗi nghiêm trọng khi làm mới: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            finally { CapNhatTrangThaiGiaoDien(); CapNhatMaNVDisplay(); }
        }

        private void barBtnThoat_ItemClick(object sender, ItemClickEventArgs e)
        {
            // Kiểm tra xem có bất kỳ trạng thái thêm mới hoặc chỉnh sửa nào đang diễn ra không
            if (dangThemMoi || dangChinhSua || dangThemMoiCTPN || dangChinhSuaCTPN)
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
                try
                {
                    // Hủy thay đổi trên BindingSource của Phiếu nhập (nếu có)
                    if (bdsPN.Current != null) // Chỉ gọi nếu có dòng hiện tại
                    {
                        HuyThaoTacPN();
                    }

                    // Hủy thay đổi trên BindingSource của Chi tiết Phiếu nhập (nếu có)
                    if (bdsCTPN.Current != null) // Chỉ gọi nếu có dòng hiện tại
                    {
                        HuyThaoTacCTPN();
                    }

                    // Đặt lại các cờ trạng thái (không bắt buộc khi đóng form, nhưng sạch sẽ)
                    dangThemMoi = false;
                    dangChinhSua = false;
                    dangThemMoiCTPN = false;
                    dangChinhSuaCTPN = false;
                }
                catch (Exception exCancel)
                {
                    MessageBox.Show("Có lỗi khi hủy thay đổi, nhưng form vẫn sẽ đóng.", "Lỗi",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                this.Close();
            }
        }

        private void barBtnRedo_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (redoStack.Count == 0) return;
            UndoRedoAction action = redoStack.Peek();
            try
            {
                Program.ExecSqlNonQuery(action.DoSql);
                redoStack.Pop(); undoStack.Push(action);
                this.taPN.Fill(this.qLVTDataSet.PHIEUNHAP); originalData = qLVTDataSet.PHIEUNHAP.Copy();
                SetGridFocusByMAPN((string)action.AffectedKey, action.OriginalPosition);
                MessageBox.Show("Thực hiện lại thành công!");
            }
            catch (Exception) { this.taPN.Fill(this.qLVTDataSet.PHIEUNHAP); originalData = qLVTDataSet.PHIEUNHAP.Copy(); SetGridPosition(0); }
            finally { dangThemMoi = false; dangChinhSua = false; CapNhatTrangThaiGiaoDien(); }
        }

        private void barBtnGhi_ItemClick(object sender, ItemClickEventArgs e)
        {
            panelPN.Focus();
            try { bdsPN.EndEdit(); }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi kết thúc nhập liệu: " + ex.Message, "Lỗi");
                return;
            }

            DataRowView drv = bdsPN.Current as DataRowView;
            if (drv == null)
            {
                MessageBox.Show("Không lấy được dữ liệu phiếu nhập.", "Lỗi");
                return;
            }
            if (!KiemTraDuLieuDauVao()) return;

            string maPN = drv["MAPN"].ToString().Trim();
            int manv = Convert.ToInt32(drv["MANV"]);
            string maDDH = drv["MASODDH"]?.ToString() ?? "";

            int positionBeforeSave = bdsPN.Position;
            string doSql = "", undoSql = "";

            try
            {
                if (dangThemMoi)
                {
                    string checkQuery = $"DECLARE @result int; EXEC @result = sp_KiemTraMaPhieuNhap '{maPN.Replace("'", "''")}'; SELECT 'Value' = @result;";
                    using (SqlDataReader reader = Program.ExecSqlDataReader(checkQuery))
                    {
                        if (reader != null && reader.Read() && int.Parse(reader.GetValue(0).ToString()) == 1)
                        {
                            MessageBox.Show($"Mã PN '{maPN}' đã tồn tại!", "Lỗi Trùng Mã"); txtMaPN.Focus(); return;
                        }
                    }
                    undoSql = $"DELETE FROM PHIEUNHAP WHERE MAPN = '{maPN.Replace("'", "''")}'";
                }
                else
                {
                    DataRow originalRow = originalData.Select($"MAPN = '{maPN.Replace("'", "''")}'").FirstOrDefault();
                    if (originalRow != null)
                    {
                        DateTime? ngayGoc = originalRow["NGAY"] as DateTime?;
                        string ngayGocSql = ngayGoc.HasValue ? $"'{ngayGoc.Value:yyyy-MM-dd}'" : "NULL";
                        int manvGoc = Convert.ToInt32(originalRow["MANV"] ?? 0);
                        string maDDHGoc = originalRow["MASODDH"]?.ToString()?.Replace("'", "''") ?? "";
                        undoSql = $"UPDATE PHIEUNHAP SET NGAY={ngayGocSql}, MANV={manvGoc}, MASODDH='{maDDHGoc}' WHERE MAPN='{maPN.Replace("'", "''")}'";
                    }
                }

                this.taPN.Update(this.qLVTDataSet.PHIEUNHAP);
                originalData = qLVTDataSet.PHIEUNHAP.Copy();

                // Làm mới dữ liệu và đồng bộ
                qLVTDataSet.PHIEUNHAP.Clear();
                this.taPN.Fill(this.qLVTDataSet.PHIEUNHAP);
                int newPosition = bdsPN.Find("MAPN", maPN);
                if (newPosition >= 0)
                {
                    bdsPN.Position = newPosition;
                }
                else
                {
                    MessageBox.Show($"Không tìm thấy mã PN '{maPN}' sau khi làm mới. Đặt lại vị trí mặc định.", "Cảnh Báo");
                    bdsPN.Position = 0;
                }

                DataRowView newDrv = bdsPN.Current as DataRowView;
                if (newDrv == null)
                {
                    MessageBox.Show("Không thể lấy dữ liệu sau khi làm mới.", "Lỗi");
                    return;
                }

                DateTime? ngayMoi = newDrv["NGAY"] as DateTime?;
                string ngayMoiSql = ngayMoi.HasValue ? $"'{ngayMoi.Value:yyyy-MM-dd}'" : "NULL";
                string maDDHMoi = newDrv["MASODDH"]?.ToString()?.Replace("'", "''") ?? "";

                if (dangThemMoi)
                {
                    doSql = $"INSERT INTO PHIEUNHAP (MAPN, NGAY, MANV, MASODDH) VALUES ('{maPN.Replace("'", "''")}', {ngayMoiSql}, {manv}, '{maDDHMoi}')";
                }
                else
                {
                    doSql = $"UPDATE PHIEUNHAP SET NGAY={ngayMoiSql}, MANV={manv}, MASODDH='{maDDHMoi}' WHERE MAPN='{maPN.Replace("'", "''")}'";
                }

                if (!string.IsNullOrEmpty(doSql) && !string.IsNullOrEmpty(undoSql))
                {
                    undoStack.Push(new UndoRedoAction(doSql, undoSql, positionBeforeSave, maPN));
                    redoStack.Clear();
                }

                dangThemMoi = false; dangChinhSua = false;
                RefreshLookupMaDDH(); // Làm mới danh sách MASODDH
                CapNhatTrangThaiGiaoDien();
                SetGridFocusByMAPN(maPN, positionBeforeSave);
                MessageBox.Show("Ghi dữ liệu thành công!", "Thông báo");
            }
            catch (SqlException sqlEx) { MessageBox.Show($"Lỗi SQL khi ghi: {sqlEx.Message}", "Lỗi SQL"); }
            catch (Exception ex) { MessageBox.Show($"Lỗi không xác định khi ghi: {ex.Message}", "Lỗi"); }
        }
        
        //xử lý popup CTPN
        private void popBtnThemCTPN_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (dangThemMoi || dangChinhSua || dangThemMoiCTPN || dangChinhSuaCTPN) return;
            if (bdsPN.Count == 0 || bdsPN.Current == null)
            {
                MessageBox.Show("Vui lòng chọn một phiếu nhập trước khi thêm chi tiết.", "Thông báo",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var drvPN = (DataRowView)bdsPN.Current;
            string maPN = drvPN["MAPN"].ToString();
            string maDDH = drvPN["MASODDH"]?.ToString();

            if (string.IsNullOrEmpty(maDDH))
            {
                MessageBox.Show("Phiếu nhập chưa chọn mã đơn đặt hàng!", "Thông báo",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Check if all materials from CTDDH are already in CTPN
            DataTable dtCTDDH = GetMaterialsFromCTDDH(maDDH);
            var ctddhMaVTs = dtCTDDH.AsEnumerable().Select(row => row.Field<string>("MAVT")).ToList();
            var usedCTPNs = qLVTDataSet.CTPN.AsEnumerable()
                .Where(row => row.RowState != DataRowState.Deleted && row.Field<string>("MAPN") == maPN)
                .Select(row => row.Field<string>("MAVT"))
                .ToList();

            if (ctddhMaVTs.All(usedCTPNs.Contains))
            {
                MessageBox.Show($"Đã nhập hết vật tư của đơn đặt hàng {maDDH} cho phiếu nhập {maPN}.", "Thông báo",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            viTriCTPN = bdsCTPN.Position;
            dangThemMoiCTPN = true;
            try
            {
                bdsCTPN.AddNew();
                var drvCTPN_New = (DataRowView)bdsCTPN.Current;

                if (drvPN != null && drvCTPN_New != null)
                {
                    drvCTPN_New["MAPN"] = maPN;
                    drvCTPN_New["SOLUONG"] = 1;
                    drvCTPN_New["DONGIA"] = 0;
                }
                else
                {
                    MessageBox.Show("Lỗi: Không thể lấy thông tin phiếu nhập hoặc dòng chi tiết mới.", "Lỗi",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                    HuyThaoTacCTPN();
                    return;
                }

                CapNhatTrangThaiGiaoDien();
                RefreshLookupVatTu();
                gridView2.FocusedRowHandle = gridView2.RowCount - 1;
                gridView2.FocusedColumn = colMAVT;
                gridView2.ShowEditor();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi thêm chi tiết: {ex.Message}", "Lỗi",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                HuyThaoTacCTPN();
            }
        }
        private void popBtnSuaCTPN_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (dangThemMoi || dangChinhSua || dangThemMoiCTPN || dangChinhSuaCTPN) return;
            if (bdsCTPN.Count == 0 || bdsCTPN.Current == null)
            {
                MessageBox.Show("Không có chi tiết nào để sửa.", "Thông báo",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            viTriCTPN = bdsCTPN.Position;
            dangChinhSuaCTPN = true;
            CapNhatTrangThaiGiaoDien();
            RefreshLookupVatTu();
            gcCTPN_PN.Focus();
            gridView2.FocusedColumn = colMAVT;
            gridView2.ShowEditor();
        }
        private void popBtnXoaCTPN_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (dangThemMoi || dangChinhSua || dangThemMoiCTPN || dangChinhSuaCTPN) return;
            if (bdsCTPN.Count == 0 || bdsCTPN.Current == null)
            {
                MessageBox.Show("Không có chi tiết phiếu nhập để xóa.", "Thông báo",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var drv = (DataRowView)bdsCTPN.Current;
            string maPN = drv["MAPN"]?.ToString() ?? "[Lỗi Mã PN]";
            string mavt = drv["MAVT"]?.ToString() ?? "[Lỗi Mã VT]";
            string tenVT = GetTenVT(mavt) ?? "[Chưa xác định tên VT]";

            if (MessageBox.Show($"Xác nhận xóa vật tư '{tenVT}' (Mã: {mavt})\nkhỏi phiếu nhập '{maPN}'?",
                                "Xác Nhận Xóa Chi Tiết",
                                MessageBoxButtons.YesNo,
                                MessageBoxIcon.Question,
                                MessageBoxDefaultButton.Button2) != DialogResult.Yes)
            {
                return;
            }

            int position = bdsCTPN.Position;
            try
            {
                bdsCTPN.RemoveCurrent();
                taCTPN.Update(this.qLVTDataSet.CTPN);
                qLVTDataSet.CTPN.Clear();
                this.taCTPN.Fill(qLVTDataSet.CTPN);
                qLVTDataSet.VATTU.Clear();
                this.taVT.Fill(qLVTDataSet.VATTU);
                if (position >= 0 && position < bdsCTPN.Count) bdsCTPN.Position = position;
                else if (bdsCTPN.Count > 0) bdsCTPN.Position = 0;
                bdsCTPN.ResetBindings(false);
                gcCTPN_PN.RefreshDataSource();
            }
            catch (Exception ex)
            {
                string errorMsg = ex is SqlException ? $"Lỗi SQL khi xóa chi tiết: {ex.Message}" : $"Lỗi không xác định khi xóa chi tiết: {ex.Message}";
                MessageBox.Show(errorMsg, ex is SqlException ? "Lỗi Cơ Sở Dữ Liệu" : "Lỗi Hệ Thống",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                try
                {
                    qLVTDataSet.CTPN.Clear();
                    this.taCTPN.Fill(qLVTDataSet.CTPN);
                    qLVTDataSet.VATTU.Clear();
                    this.taVT.Fill(qLVTDataSet.VATTU);
                    if (position >= 0 && position < bdsCTPN.Count) bdsCTPN.Position = position;
                    else if (bdsCTPN.Count > 0) bdsCTPN.Position = 0;
                    bdsCTPN.ResetBindings(false);
                    gcCTPN_PN.RefreshDataSource();
                }
                catch (Exception exFallback) { Console.WriteLine($"Fallback error: {exFallback.Message}"); }
                CapNhatTrangThaiGiaoDien();
            }
        }
        private void popBtnGhiCTPN_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (!dangThemMoiCTPN && !dangChinhSuaCTPN) return;

            try
            {
                if (gridView2.IsEditing)
                {
                    gridView2.CloseEditor();
                    gridView2.UpdateCurrentRow();
                }
                bdsCTPN.EndEdit();
            }
            catch (Exception exEdit)
            {
                MessageBox.Show($"Lỗi khi kết thúc nhập liệu chi tiết: {exEdit.Message}", "Lỗi",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var drv = (DataRowView)bdsCTPN.Current;
            if (drv == null)
            {
                MessageBox.Show("Không lấy được dữ liệu chi tiết hiện tại.", "Lỗi",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                if (dangThemMoiCTPN) HuyThaoTacCTPN();
                return;
            }

            string maPN = drv["MAPN"]?.ToString();
            string mavt = drv["MAVT"]?.ToString();
            string tenVT = GetTenVT(mavt) ?? "[Chưa xác định tên VT]";

            if (string.IsNullOrEmpty(mavt))
            {
                MessageBox.Show("Vui lòng chọn vật tư!", "Thiếu Thông Tin",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                gridView2.FocusedColumn = colMAVT;
                gridView2.ShowEditor();
                return;
            }
            int soLuong;
            if (!int.TryParse(drv["SOLUONG"]?.ToString(), out soLuong) || soLuong <= 0)
            {
                MessageBox.Show("Số lượng phải là số nguyên dương!", "Dữ Liệu Không Hợp Lệ",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                gridView2.FocusedColumn = colSOLUONG;
                gridView2.ShowEditor();
                return;
            }
            int donGia;
            if (!int.TryParse(drv["DONGIA"]?.ToString(), out donGia) || donGia < 0)
            {
                MessageBox.Show("Đơn giá phải là số không âm!", "Dữ Liệu Không Hợp Lệ",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                gridView2.FocusedColumn = colDONGIA;
                gridView2.ShowEditor();
                return;
            }

            if (dangThemMoiCTPN && !string.IsNullOrEmpty(maPN) && !string.IsNullOrEmpty(mavt))
            {
                var existingRows = qLVTDataSet.CTPN.AsEnumerable()
                    .Where(row => row.RowState != DataRowState.Deleted && row.Field<string>("MAPN") == maPN && row.Field<string>("MAVT") == mavt && row != drv.Row);
                if (existingRows.Any())
                {
                    MessageBox.Show($"Vật tư '{tenVT}' đã tồn tại trong phiếu nhập '{maPN}'.", "Lỗi Trùng Vật Tư",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                    gridView2.FocusedColumn = colMAVT;
                    gridView2.ShowEditor();
                    return;
                }
            }

            int position = bdsCTPN.Position;
            try
            {
                this.taCTPN.Update(this.qLVTDataSet.CTPN);
                dangThemMoiCTPN = false;
                dangChinhSuaCTPN = false;
                MessageBox.Show($"Ghi chi tiết vật tư '{tenVT}' thành công!", "Thông báo",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                qLVTDataSet.CTPN.Clear();
                this.taCTPN.Fill(qLVTDataSet.CTPN);
                qLVTDataSet.VATTU.Clear();
                this.taVT.Fill(qLVTDataSet.VATTU);
                if (position >= 0 && position < bdsCTPN.Count) bdsCTPN.Position = position;
                else if (bdsCTPN.Count > 0) bdsCTPN.Position = 0;
                bdsCTPN.ResetBindings(false);
                gcCTPN_PN.RefreshDataSource();
            }
            catch (SqlException sqlEx)
            {
                if (sqlEx.Number == 2627)
                {
                    MessageBox.Show($"Vật tư '{tenVT}' đã có trong phiếu nhập '{maPN}'.", "Lỗi Trùng Vật Tư (CSDL)",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                    gridView2.ShowEditor();
                    return;
                }
                MessageBox.Show($"Lỗi SQL khi ghi chi tiết: {sqlEx.Message}", "Lỗi Cơ Sở Dữ Liệu",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                bdsCTPN.CancelEdit();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi ghi chi tiết: {ex.Message}", "Lỗi Hệ Thống",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                bdsCTPN.CancelEdit();
            }
            finally
            {
                dangThemMoiCTPN = false;
                dangChinhSuaCTPN = false;
                CapNhatTrangThaiGiaoDien();
            }
        }
        private void BdsCTPN_CurrentChanged(object sender, EventArgs e)
        {
            if (dangThemMoiCTPN || dangChinhSuaCTPN) { }
            CapNhatTrangThaiGiaoDien();
        }

        private void gridView2_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            if (dangThemMoiCTPN || dangChinhSuaCTPN) return;
            DataRowView drv = gridView2.GetRow(e.FocusedRowHandle) as DataRowView;
            if (drv != null)
            {
                string mavt = drv["MAVT"]?.ToString();
                if (!string.IsNullOrEmpty(mavt))
                {
                    gridView2.SetRowCellValue(e.FocusedRowHandle, colMAVT, mavt);
                    gridView2.RefreshRow(e.FocusedRowHandle);
                }
            }
        }

        private void gridView2_CustomRowCellEditForEditing(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            if (e.Column == colMAVT) e.RepositoryItem = repoCBTenVT;
        }

        private string GetTenVT(string maVT)
        {
            if (string.IsNullOrEmpty(maVT)) return "[Chưa chọn]";
            try
            {
                var rows = qLVTDataSet.VATTU.AsEnumerable()
                    .Where(row => row.Field<string>("MAVT") == maVT && row.RowState != DataRowState.Deleted)
                    .ToArray();
                if (rows.Length > 0)
                {
                    return rows[0].Field<string>("TENVT") ?? "[Lỗi tên VT]";
                }
                // Nếu không tìm thấy, thử nạp lại dữ liệu VATTU
                qLVTDataSet.VATTU.Clear();
                this.taVT.Fill(qLVTDataSet.VATTU);
                var rowsAfterRefresh = qLVTDataSet.VATTU.AsEnumerable()
                    .Where(row => row.Field<string>("MAVT") == maVT && row.RowState != DataRowState.Deleted)
                    .ToArray();
                return rowsAfterRefresh.Length > 0 ? rowsAfterRefresh[0].Field<string>("TENVT") ?? "[Lỗi tên VT]" : "[Không tìm thấy tên VT]";
            }
            catch (Exception)
            {
                return "[Lỗi khi lấy tên VT]";
            }
        }

        private DataTable GetMaterialsFromCTDDH(string maDDH)
        {
            DataTable dt = new DataTable();
            if (string.IsNullOrEmpty(maDDH))
            {
                return dt; // Return empty DataTable if MASODDH is empty
            }
            try
            {
                using (SqlConnection conn = new SqlConnection(Program.connstr))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("sp_GetVattuFromCTDDH", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@MASODDH", maDDH);
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lấy danh sách vật tư từ CTDDH: {ex.Message}", "Lỗi",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return dt;
        }

        private void RefreshLookupVatTu()
        {
            try
            {
                gcCTPN_PN.BeginUpdate();
                // Không gán lại repoCBTenVT.DataSource để tránh ghi đè
                // Chỉ làm mới dữ liệu VATTU nếu cần cho các mục đích khác
                qLVTDataSet.VATTU.Clear();
                this.taVT.Fill(qLVTDataSet.VATTU);
                bdsVT.ResetBindings(false);
                colMAVT.ColumnEdit = repoCBTenVT; // Đảm bảo LookUpEdit được liên kết
                gridView2.RefreshData();
                gridView2.Invalidate();
                gridView2.LayoutChanged();
                gcCTPN_PN.RefreshDataSource();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi làm mới danh sách vật tư: {ex.Message}", "Lỗi",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                gcCTPN_PN.EndUpdate();
            }
        }

        private void repoCBTenVT_QueryPopUp(object sender, System.ComponentModel.CancelEventArgs e)
        {
            LookUpEdit lookUpEdit = sender as LookUpEdit;
            if (lookUpEdit == null)
            {
                e.Cancel = true;
                return;
            }

            var drvPN = bdsPN.Current as DataRowView;
            if (drvPN == null)
            {
                lookUpEdit.Properties.DataSource = null;
                e.Cancel = true;
                return;
            }

            string currentMaPN = drvPN["MAPN"]?.ToString();
            string maDDH = drvPN["MASODDH"]?.ToString();
            if (string.IsNullOrEmpty(currentMaPN) || string.IsNullOrEmpty(maDDH))
            {
                lookUpEdit.Properties.DataSource = null;
                e.Cancel = true;
                return;
            }

            // Lấy vật tư từ CTDDH
            DataTable dtVattu = GetMaterialsFromCTDDH(maDDH);
            if (dtVattu.Rows.Count == 0)
            {
                lookUpEdit.Properties.DataSource = null;
                e.Cancel = true;
                return;
            }

            DataView dvVattu = new DataView(dtVattu);

            // Lấy danh sách MAVT đã dùng trong CTPN của MAPN hiện tại
            var usedMaVTs = qLVTDataSet.CTPN.AsEnumerable()
                .Where(row => row.RowState != DataRowState.Deleted && row.Field<string>("MAPN") == currentMaPN)
                .Select(row => row.Field<string>("MAVT"))
                .Where(mavt => !string.IsNullOrEmpty(mavt))
                .Distinct()
                .ToList();

            // Lấy MAVT hiện tại (cho chế độ chỉnh sửa)
            var drvCTPN = gridView2.GetRow(gridView2.FocusedRowHandle) as DataRowView;
            string currentMAVT = drvCTPN != null ? drvCTPN["MAVT"]?.ToString() : null;

            // Áp dụng bộ lọc để loại bỏ MAVT đã dùng
            if (usedMaVTs.Count > 0)
            {
                var escapedMaVTs = usedMaVTs.Select(mavt => $"'{mavt.Replace("'", "''")}'");
                string baseFilter = $"MAVT NOT IN ({string.Join(",", escapedMaVTs)})";
                if (dangChinhSuaCTPN && !string.IsNullOrEmpty(currentMAVT) && usedMaVTs.Contains(currentMAVT))
                {
                    // Cho phép MAVT hiện tại xuất hiện khi chỉnh sửa
                    var filteredMaVTs = usedMaVTs.Where(m => m != currentMAVT).Select(m => $"'{m.Replace("'", "''")}'");
                    dvVattu.RowFilter = filteredMaVTs.Any() ? $"MAVT NOT IN ({string.Join(",", filteredMaVTs)})" : "";
                }
                else
                {
                    dvVattu.RowFilter = baseFilter;
                }
            }
            else
            {
                dvVattu.RowFilter = "";
            }

            lookUpEdit.Properties.DataSource = dvVattu;
            lookUpEdit.Properties.PopulateColumns();
            lookUpEdit.Properties.Columns.Clear();
            lookUpEdit.Properties.Columns.Add(new DevExpress.XtraEditors.Controls.LookUpColumnInfo("MAVT", "Mã Vật Tư"));
            lookUpEdit.Properties.Columns.Add(new DevExpress.XtraEditors.Controls.LookUpColumnInfo("TENVT", "Tên Vật Tư"));
            lookUpEdit.Properties.ValueMember = "MAVT";
            lookUpEdit.Properties.DisplayMember = "TENVT";
            lookUpEdit.Properties.ForceInitialize();
        }

        private void BdsPN_PositionChanged(object sender, EventArgs e)
        {
            if (!dangThemMoi && !dangChinhSua && bdsPN.Current != null)
            {
                DataRowView drv = bdsPN.Current as DataRowView;
                lookUpMaDDH.EditValue = drv["MASODDH"]?.ToString();
                CapNhatMaNVDisplay();
            }
            else
            {
                lookUpMaDDH.EditValue = null;
            }
        }

        private void RefreshLookupMaDDH()
        {
            try
            {
                // Làm mới dữ liệu từ bảng DDH (thay vì DSDDHChuaNhap)
                qLVTDataSet.DDH.Clear(); // Giả sử bạn có bảng DDH trong qLVTDataSet
                this.taDDH.Fill(qLVTDataSet.DDH); // Thay taDSDDHChuaNhap bằng taDDH

                // Gán DataSource cho lookUpMaDDH từ toàn bộ DDH
                lookUpMaDDH.Properties.DataSource = qLVTDataSet.DDH;
                lookUpMaDDH.Properties.ValueMember = "MASODDH";
                lookUpMaDDH.Properties.DisplayMember = "MASODDH";
                lookUpMaDDH.Properties.PopulateColumns();
                lookUpMaDDH.Properties.Columns.Clear();
                lookUpMaDDH.Properties.Columns.Add(new DevExpress.XtraEditors.Controls.LookUpColumnInfo("MASODDH", "Mã Đơn Đặt Hàng"));

                // Khi thêm mới hoặc chỉnh sửa, lọc MASODDH chưa được sử dụng
                if (dangThemMoi || dangChinhSua)
                {
                    var usedMaDDHs = qLVTDataSet.PHIEUNHAP.AsEnumerable()
                        .Where(row => row.RowState != DataRowState.Deleted && row["MASODDH"] != DBNull.Value)
                        .Select(row => row.Field<string>("MASODDH"))
                        .ToList();

                    DataView dv = new DataView(qLVTDataSet.DDH);
                    if (usedMaDDHs.Count > 0)
                    {
                        string filter = string.Join(",", usedMaDDHs.Select(m => $"'{m.Replace("'", "''")}'"));
                        dv.RowFilter = $"MASODDH NOT IN ({filter})";
                    }
                    lookUpMaDDH.Properties.DataSource = dv;

                    // Khi thêm mới, đặt EditValue về null
                    if (dangThemMoi)
                    {
                        lookUpMaDDH.EditValue = null;
                    }
                    // Khi chỉnh sửa, giữ giá trị MASODDH hiện tại
                    else if (dangChinhSua && bdsPN.Current != null)
                    {
                        DataRowView drv = bdsPN.Current as DataRowView;
                        lookUpMaDDH.EditValue = drv["MASODDH"]?.ToString();
                    }
                }
                else
                {
                    // Khi xem, đồng bộ với MASODDH của phiếu nhập hiện tại
                    if (bdsPN.Current != null)
                    {
                        DataRowView drv = bdsPN.Current as DataRowView;
                        lookUpMaDDH.EditValue = drv["MASODDH"]?.ToString();
                    }
                    else
                    {
                        lookUpMaDDH.EditValue = null; // Không có phiếu nhập nào được chọn
                    }
                }

                lookUpMaDDH.Properties.ForceInitialize();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi làm mới danh sách mã đơn đặt hàng: {ex.Message}", "Lỗi",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }
}
