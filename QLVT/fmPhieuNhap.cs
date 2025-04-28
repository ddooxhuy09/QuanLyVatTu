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
    public partial class fmPhieuNhap : Form
    {
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
            //không kiểm tra khóa ngoại nữa
            qLVTDataSet.EnforceConstraints = false;

            this.taVT.Connection.ConnectionString = Program.connstr;
            this.taVT.Fill(this.qLVTDataSet.VATTU);

            this.taPN.Connection.ConnectionString = Program.connstr;
            this.taPN.Fill(this.qLVTDataSet.PHIEUNHAP);

            this.taDSNV.Connection.ConnectionString = Program.connstr;
            this.taDSNV.Fill(this.qLVTDataSet.DSNV);

            this.taCTPN.Connection.ConnectionString = Program.connstr;
            this.taCTPN.Fill(this.qLVTDataSet.CTPN);

            this.taDSCTPN.Connection.ConnectionString = Program.connstr;
            this.taDSCTPN.Fill(this.qLVTDataSet.DSCTPN);

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

        private void gcCTPN_PN_Click(object sender, EventArgs e)
        {

        }
    }
}
