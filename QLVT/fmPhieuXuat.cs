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
    public partial class fmPhieuXuat : Form
    {
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
            // TODO: This line of code loads data into the 'qLVTDataSet.VATTU' table. You can move, or remove it, as needed.
            this.taVT.Fill(this.qLVTDataSet.VATTU);
            //không kiểm tra khóa ngoại nữa
            qLVTDataSet.EnforceConstraints = false;

            this.taDSNV.Connection.ConnectionString = Program.connstr;
            this.taDSNV.Fill(this.qLVTDataSet.DSNV);

            this.taPX.Connection.ConnectionString = Program.connstr;
            this.taPX.Fill(this.qLVTDataSet.PHIEUXUAT);

            this.taCTPX.Connection.ConnectionString = Program.connstr;
            this.taCTPX.Fill(this.qLVTDataSet.CTPX);

            this.taDSPTPX.Connection.ConnectionString = Program.connstr;
            this.taDSPTPX.Fill(this.qLVTDataSet.DSCTPX);

            

        }

        private void popCTCPX(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            //if (e.MenuType == DevExpress.XtraGrid.Views.Grid.GridMenuType.Row)
            {
                popCTPX.ShowPopup(Control.MousePosition);
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
    }
}
