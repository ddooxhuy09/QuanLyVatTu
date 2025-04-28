using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QLVT
{
    public partial class fmVatTu : Form
    {
        int viTri = 0; //vị trí trên bảng

        bool dangThemMoi = false; //khi thêm sẽ true
        public fmVatTu()
        {
            InitializeComponent();
        }
        

        private void fmVatTu_Load(object sender, EventArgs e)
        {
            //không kiểm tra khóa ngoại nữa
            qLVTDataSet.EnforceConstraints = false;

            this.taVT.Connection.ConnectionString = Program.connstr;
            this.taVT.Fill(this.qLVTDataSet.VATTU);

            this.taCTDDH.Connection.ConnectionString = Program.connstr;
            this.taCTDDH.Fill(this.qLVTDataSet.CTDDH);

            this.taCTPN.Connection.ConnectionString = Program.connstr;
            this.taCTPN.Fill(this.qLVTDataSet.CTPN);

            this.taCTPX.Connection.ConnectionString = Program.connstr;
            this.taCTPX.Fill(this.qLVTDataSet.CTPX);

            

        }

        private void vATTUBindingNavigatorSaveItem_Click(object sender, EventArgs e)
        {
            this.Validate();
            this.bdsVT.EndEdit();
            this.tableAdapterManager.UpdateAll(this.qLVTDataSet);

        }

        private void barBtnThem_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            viTri = bdsVT.Position;
            this.panelVT.Enabled = true;
            dangThemMoi = true;

            //thêm dòng vào bảng
            bdsVT.AddNew();
            intSLT.Value = 1;

            //tắt các nút
            //this.txtMAVT.Enabled = true;
            this.barBtnThem.Enabled = false;
            this.barBtnSua.Enabled = false;
            this.barBtnXoa.Enabled = false;
            this.barBtnHoanTac.Enabled = false;
            this.barBtnLamMoi.Enabled = false;

            this.gcVT.Enabled = false;

        }

        private void barBtnLamMoi_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                this.taVT.Fill(this.qLVTDataSet.VATTU);
                this.gcVT.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi Làm mới" + ex.Message, "Thông báo", MessageBoxButtons.OK);
                return;
            }
        }

        private void barBtnXoa_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (bdsVT.Count == 0)
            {
                barBtnXoa.Enabled = false;
            }

            if (bdsCTDDH.Count > 0)
            {
                MessageBox.Show("Không thể xóa vật tư này vì đã lập đơn đặt hàng", "Thông báo", MessageBoxButtons.OK);
                return;
            }

            if (bdsCTPN.Count > 0)
            {
                MessageBox.Show("Không thể xóa vật tư này vì đã lập phiếu nhập", "Thông báo", MessageBoxButtons.OK);
                return;
            }

            if (bdsCTPX.Count > 0)
            {
                MessageBox.Show("Không thể xóa vật tư này vì đã lập phiếu xuất", "Thông báo", MessageBoxButtons.OK);
                return;
            }
        }
    }
}
