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
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;

namespace QLVT
{
    public partial class FrmSaoLuu : Form
    {
        private BindingSource bdsBackup = new BindingSource();
        private const string DEFAULT_DEVICE_NAME = "DEVICE_QLVT";
        private const string DEFAULT_DATABASE_NAME = "QLVT";
        private const string DEFAULT_BACKUP_PATH = @"D:\Backup\QLVT_Device.bak";
        private const string DEFAULT_BACKUP_LOG_PATH = @"D:\Backup\QLVT_Device.trn";
        private bool hasBackupDevice = false;
        private bool isCheckEdit1Checked = false;
        private int File = -1;
        private DateTime? selectedRestoreTime = null;
        public FrmSaoLuu()
        {
            InitializeComponent();
            this.Load += new System.EventHandler(this.FrmSaoLuu_Load);

            dateEdit1.Enabled = false;
            timeSpanEdit2.Enabled = false;
            barCheckItem1.Checked = false;
        }

        private void LoadBackupData()
        {
            string query = $"EXEC SP_LayThongTinSaoLuu @database_name = '{DEFAULT_DATABASE_NAME}'";
            try
            {
                DataTable dt = Program.ExecSqlDataTable(query);
                if (!dt.Columns.Contains("DienGiai"))
                {
                    dt.Columns.Add("DienGiai", typeof(string));
                }
                bdsBackup.DataSource = dt;
                gridControl1.DataSource = bdsBackup;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu: {ex.Message}", "Debug", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private (DateTime? BackupTime, int? Position) GetMaxBackupTime()
        {
            string query = $"EXEC SP_LayThongTinSaoLuu @database_name = '{DEFAULT_DATABASE_NAME}'";
            try
            {
                DataTable dt = Program.ExecSqlDataTable(query);
                if (dt.Rows.Count > 0)
                {
                    DataRow firstRow = dt.Rows[0];
                    DateTime? backupTime = firstRow["backup_start_date"] != DBNull.Value
                        ? firstRow.Field<DateTime>("backup_start_date")
                        : (DateTime?)null;
                    int? position = firstRow["position"] != DBNull.Value
                        ? firstRow.Field<int>("position")
                        : (int?)null;
                    return (backupTime, position);
                }
                return (null, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lấy thông tin sao lưu: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return (null, null);
            }
        }

        private void CheckBackupDevice()
        {
            try
            {
                if (Program.conn == null || Program.conn.State != ConnectionState.Open)
                {
                    int result = Program.KetNoi();
                    if (result == 0)
                    {
                        MessageBox.Show("Không thể kết nối đến cơ sở dữ liệu!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }

                string query = "SELECT 1 FROM sys.backup_devices WHERE name = @logicalName";
                using (SqlCommand command = new SqlCommand(query, Program.conn))
                {
                    command.Parameters.AddWithValue("@logicalName", DEFAULT_DEVICE_NAME);
                    object result = command.ExecuteScalar();
                    hasBackupDevice = result != null;
                }

                barButtonItem1.Enabled = hasBackupDevice;
                barButtonItem2.Enabled = hasBackupDevice;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi kiểm tra backup device: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FrmSaoLuu_Load(object sender, EventArgs e)
        {
            CheckBackupDevice();
            LoadBackupData();

            dateEdit1.Enabled = isCheckEdit1Checked;
            timeSpanEdit2.Enabled = isCheckEdit1Checked && dateEdit1.EditValue != null;

            // Thiết lập MinValue cho dateEdit1
            if (isCheckEdit1Checked)
            {
                var (maxBackupTime, _) = GetMaxBackupTime();
                if (maxBackupTime != null)
                {
                    dateEdit1.Properties.MinValue = maxBackupTime.Value.Date;
                }
                else
                {
                    dateEdit1.Enabled = false;
                    timeSpanEdit2.Enabled = false;
                    MessageBox.Show("Không thể lấy thông tin sao lưu mới nhất. Vui lòng kiểm tra lại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void checkEdit1_CheckedChanged(object sender, EventArgs e)
        {
            if (sender is CheckEdit checkEdit)
            {
                isCheckEdit1Checked = checkEdit.Checked;
            }
        }

        private void labelControl1_Click(object sender, EventArgs e)
        {
        }
        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (!hasBackupDevice)
                {
                    MessageBox.Show("Vui lòng tạo backup device trước khi sao lưu!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (Program.conn == null || Program.conn.State != ConnectionState.Open)
                {
                    int result = Program.KetNoi();
                    if (result == 0)
                    {
                        MessageBox.Show("Không thể kết nối đến cơ sở dữ liệu!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }

                string backupCommand = $"BACKUP DATABASE {DEFAULT_DATABASE_NAME} TO {DEFAULT_DEVICE_NAME}";
                if (isCheckEdit1Checked)
                {
                    backupCommand += " WITH INIT";
                }
                backupCommand += ";";

                using (SqlCommand command = new SqlCommand(backupCommand, Program.conn))
                {
                    command.ExecuteNonQuery();
                    MessageBox.Show("Sao lưu cơ sở dữ liệu thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                LoadBackupData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi sao lưu dữ liệu: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void barButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.Close();
        }

        private void gridControl1_Click(object sender, EventArgs e)
        {
            // Lấy hàng được nhấp
            GridView view = gridControl1.MainView as GridView;
            if (view != null && view.FocusedRowHandle >= 0)
            {
                DataRow row = view.GetDataRow(view.FocusedRowHandle);
                if (row != null && row["position"] != DBNull.Value)
                {
                    File = Convert.ToInt32(row["position"]);
                    // Hiển thị giá trị File để kiểm tra (có thể xóa dòng này sau khi kiểm thử)
                    richTextBox1.Text = $"Đã chọn bản sao lưu thứ: {File}";
                }
            }
        }

        private void FrmSaoLuu_Load_1(object sender, EventArgs e)
        {

        }

        private void barButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (Program.conn == null)
                {
                    MessageBox.Show("Đối tượng kết nối (Program.conn) chưa được khởi tạo!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (Program.conn.State != ConnectionState.Open)
                {
                    int result = Program.KetNoi();
                    if (result == 0)
                    {
                        MessageBox.Show("Không thể kết nối đến cơ sở dữ liệu!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }

                string query = $"EXEC SP_TaoBackupDevice @logicalName = @logicalName, @physicalPath = @physicalPath, @deviceExists = @deviceExists OUTPUT";
                using (SqlCommand command = new SqlCommand(query, Program.conn))
                {
                    command.Parameters.AddWithValue("@logicalName", DEFAULT_DEVICE_NAME);
                    command.Parameters.AddWithValue("@physicalPath", DEFAULT_BACKUP_PATH); // Truyền physicalPath
                    SqlParameter deviceExistsParam = new SqlParameter("@deviceExists", SqlDbType.Bit)
                    {
                        Direction = ParameterDirection.Output
                    };
                    command.Parameters.Add(deviceExistsParam);

                    command.ExecuteNonQuery();

                    bool deviceExists = (bool)deviceExistsParam.Value;
                    MessageBox.Show(
                        deviceExists
                            ? $"Backup device '{DEFAULT_DEVICE_NAME}' đã tồn tại và được cập nhật!"
                            : $"Tạo backup device '{DEFAULT_DEVICE_NAME}' thành công!",
                        "Thông báo",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                }

                LoadBackupData();
                hasBackupDevice = true;
                barButtonItem1.Enabled = true;
                barButtonItem2.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tạo backup device: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void barCheckItem1_CheckedChanged(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            isCheckEdit1Checked = barCheckItem1.Checked;
            dateEdit1.Enabled = isCheckEdit1Checked;
            timeSpanEdit2.Enabled = isCheckEdit1Checked && dateEdit1.EditValue != null;

            if (!isCheckEdit1Checked)
            {
                selectedRestoreTime = null;
                dateEdit1.EditValue = null;
                timeSpanEdit2.EditValue = null;
                dateEdit1.Properties.MinValue = DateTime.MinValue;
                timeSpanEdit2.Properties.MinValue = TimeSpan.Zero;
            }
            else
            {
                var (maxBackupTime, _) = GetMaxBackupTime();
                if (maxBackupTime != null)
                {
                    dateEdit1.Properties.MinValue = maxBackupTime.Value.Date;
                }
                else
                {
                    dateEdit1.Enabled = false;
                    timeSpanEdit2.Enabled = false;
                    MessageBox.Show("Không thể lấy thông tin sao lưu mới nhất. Vui lòng kiểm tra lại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {

                // Sử dụng chuỗi kết nối từ Program hoặc cấu hình
                string connectionString = Program.connstr; // Giả sử Program.connstr chứa chuỗi kết nối
                if (string.IsNullOrEmpty(connectionString))
                {
                    MessageBox.Show("Chuỗi kết nối không hợp lệ!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Hiển thị xác nhận khôi phục
                DialogResult confirm = MessageBox.Show("Bạn có chắc chắn muốn khôi phục cơ sở dữ liệu? Dữ liệu hiện tại sẽ bị ghi đè!",
                    "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (confirm != DialogResult.Yes)
                {
                    return;
                }

                // Tạo kết nối mới
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    if (isCheckEdit1Checked)
                    {
                        if (!selectedRestoreTime.HasValue)
                        {
                            MessageBox.Show("Vui lòng chọn thời gian khôi phục!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        var (maxBackupTime, filePosition) = GetMaxBackupTime();
                        if (maxBackupTime == null || filePosition == null)
                        {
                            MessageBox.Show("Không thể lấy thông tin sao lưu mới nhất!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        if (selectedRestoreTime.Value <= maxBackupTime)
                        {
                            MessageBox.Show("Thời gian khôi phục phải lớn hơn thời gian sao lưu lớn nhất!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        // Gợi ý sao lưu trước khi khôi phục
                        DialogResult backupConfirm = MessageBox.Show("Bạn có muốn sao lưu cơ sở dữ liệu hiện tại trước khi khôi phục?",
                            "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (backupConfirm == DialogResult.Yes)
                        {
                            barButtonItem1_ItemClick(sender, null);
                        }

                        // Thực thi các lệnh SQL riêng lẻ cho point-in-time recovery
                        string[] sqlCommands = new[]
                        {
                    $"ALTER DATABASE [{DEFAULT_DATABASE_NAME}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;",
                    $"USE tempdb;",
                    $"BACKUP LOG [{DEFAULT_DATABASE_NAME}] TO DISK = '{DEFAULT_BACKUP_LOG_PATH}' WITH INIT, NORECOVERY;",
                    $"RESTORE DATABASE [{DEFAULT_DATABASE_NAME}] FROM DISK = '{DEFAULT_BACKUP_PATH}' WITH NORECOVERY, FILE = {filePosition};",
                    $"RESTORE LOG [{DEFAULT_DATABASE_NAME}] FROM DISK = '{DEFAULT_BACKUP_LOG_PATH}' WITH STOPAT = '{selectedRestoreTime.Value.ToString("yyyy-MM-dd HH:mm:ss")}', RECOVERY;",
                    $"ALTER DATABASE [{DEFAULT_DATABASE_NAME}] SET MULTI_USER;"
                };

                        foreach (string sql in sqlCommands)
                        {
                            using (SqlCommand command = new SqlCommand(sql, connection))
                            {
                                command.CommandTimeout = 300; // Tăng thời gian chờ
                                command.ExecuteNonQuery();
                            }
                        }
                    }
                    else
                    {
                        // Gợi ý sao lưu trước khi khôi phục
                        DialogResult backupConfirm = MessageBox.Show("Bạn có muốn sao lưu cơ sở dữ liệu hiện tại trước khi khôi phục?",
                            "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (backupConfirm == DialogResult.Yes)
                        {
                            barButtonItem1_ItemClick(sender, null);
                        }

                        if (File == -1)
                        {
                            MessageBox.Show("Vui lòng chọn một bản sao lưu từ danh sách!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        // Kiểm tra vị trí file sao lưu
                        string checkQuery = "SELECT 1 FROM MSDB.dbo.backupset WHERE database_name = @database_name AND position = @file_position";
                        using (SqlCommand checkCommand = new SqlCommand(checkQuery, connection))
                        {
                            checkCommand.Parameters.AddWithValue("@database_name", DEFAULT_DATABASE_NAME);
                            checkCommand.Parameters.AddWithValue("@file_position", File);
                            object result = checkCommand.ExecuteScalar();
                            if (result == null)
                            {
                                MessageBox.Show("Vị trí file sao lưu không hợp lệ!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                        }

                        // Thực thi các lệnh SQL riêng lẻ cho khôi phục thông thường
                        string[] sqlCommands = new[]
                        {
                    $"ALTER DATABASE [{DEFAULT_DATABASE_NAME}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;",
                    $"USE tempdb;",
                    $"RESTORE DATABASE [{DEFAULT_DATABASE_NAME}] FROM {DEFAULT_DEVICE_NAME} WITH FILE = {File}, REPLACE;",
                    $"ALTER DATABASE [{DEFAULT_DATABASE_NAME}] SET MULTI_USER;"
                };

                        foreach (string sql in sqlCommands)
                        {
                            using (SqlCommand command = new SqlCommand(sql, connection))
                            {
                                command.CommandTimeout = 300; // Tăng thời gian chờ
                                command.ExecuteNonQuery();
                            }
                        }
                    }

                    // Chuyển về database QLVT
                    using (SqlCommand useDbCommand = new SqlCommand($"USE [{DEFAULT_DATABASE_NAME}];", connection))
                    {
                        useDbCommand.ExecuteNonQuery();
                    }

                    MessageBox.Show("Khôi phục cơ sở dữ liệu thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadBackupData();
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"Lỗi SQL khi phục hồi dữ liệu: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi phục hồi dữ liệu: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {

        }

        private void dateEdit1_EditValueChanged(object sender, EventArgs e)
        {
            if (!isCheckEdit1Checked) return;

            var (maxBackupTime, _) = GetMaxBackupTime();
            if (maxBackupTime == null)
            {
                MessageBox.Show("Không thể lấy thông tin sao lưu mới nhất!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                dateEdit1.EditValue = null;
                timeSpanEdit2.Enabled = false;
                timeSpanEdit2.EditValue = null;
                selectedRestoreTime = null;
                return;
            }

            if (dateEdit1.EditValue != null)
            {
                DateTime selectedDate = dateEdit1.DateTime;
                string dateString = selectedDate.ToString("yyyy-MM-dd");

                // Thiết lập MinValue cho timeSpanEdit2 nếu ngày trùng với maxBackupTime
                timeSpanEdit2.Enabled = true;
                if (selectedDate.Date == maxBackupTime.Value.Date)
                {
                    timeSpanEdit2.Properties.MinValue = maxBackupTime.Value.TimeOfDay.Add(TimeSpan.FromSeconds(1));
                }
                else
                {
                    timeSpanEdit2.Properties.MinValue = TimeSpan.Zero;
                }

                // Cập nhật selectedRestoreTime
                if (timeSpanEdit2.EditValue != null)
                {
                    TimeSpan selectedTime = timeSpanEdit2.TimeSpan;
                    selectedRestoreTime = DateTime.Parse($"{dateString} {selectedTime.ToString(@"hh\:mm\:ss")}");
                }
                else
                {
                    selectedRestoreTime = DateTime.Parse($"{dateString} 00:00:00");
                }
            }
            else
            {
                timeSpanEdit2.Enabled = false;
                timeSpanEdit2.EditValue = null;
                timeSpanEdit2.Properties.MinValue = TimeSpan.Zero;
                timeSpanEdit2.Properties.MaxValue = new TimeSpan(23, 59, 59);
                selectedRestoreTime = null;
            }
        }

        private void panelControl2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void timeSpanEdit2_EditValueChanged(object sender, EventArgs e)
        {
            if (!isCheckEdit1Checked || dateEdit1.EditValue == null) return;

            var (maxBackupTime, _) = GetMaxBackupTime();
            if (maxBackupTime == null)
            {
                MessageBox.Show("Không thể lấy thông tin sao lưu mới nhất!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                timeSpanEdit2.EditValue = null;
                selectedRestoreTime = null;
                return;
            }

            // Đảm bảo MaxValue luôn được thiết lập
            timeSpanEdit2.Properties.MaxValue = new TimeSpan(23, 59, 59); // Giới hạn tối đa 23:59:59

            if (timeSpanEdit2.EditValue != null)
            {
                TimeSpan selectedTime = timeSpanEdit2.TimeSpan;
                string timeString = selectedTime.ToString(@"hh\:mm\:ss");
                DateTime selectedDate = dateEdit1.DateTime;
                selectedRestoreTime = DateTime.Parse($"{selectedDate.ToString("yyyy-MM-dd")} {timeString}");
            }
            else
            {
                selectedRestoreTime = DateTime.Parse($"{dateEdit1.DateTime.ToString("yyyy-MM-dd")} 00:00:00");
            }
        }
    }
}