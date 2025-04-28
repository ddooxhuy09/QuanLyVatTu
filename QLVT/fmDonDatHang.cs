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
    public partial class fmDonDatHang : Form
    {
        public fmDonDatHang()
        {
            InitializeComponent();
        }

        private void dDHBindingNavigatorSaveItem_Click(object sender, EventArgs e)
        {
            this.Validate();
            this.bdsDDH.EndEdit();
            this.tableAdapterManager.UpdateAll(this.qLVTDataSet);

        }

        private void fmDonDatHang_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'qLVTDataSet.DSNV' table. You can move, or remove it, as needed.
            this.taDSNV.Fill(this.qLVTDataSet.DSNV);
            // TODO: This line of code loads data into the 'qLVTDataSet.DSCTDDH' table. You can move, or remove it, as needed.
            this.taDSCTDDH.Fill(this.qLVTDataSet.DSCTDDH);
            //không kiểm tra khóa ngoại nữa
            qLVTDataSet.EnforceConstraints = false;

            this.taDDH.Connection.ConnectionString = Program.connstr;
            this.taDDH.Fill(this.qLVTDataSet.DDH);

            this.taCTDDH.Connection.ConnectionString = Program.connstr;
            this.taCTDDH.Fill(this.qLVTDataSet.CTDDH);
            this.taPN.Connection.ConnectionString = Program.connstr;
            this.taPN.Fill(this.qLVTDataSet.PHIEUNHAP);
        }



        private void txtHOTENNV_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                txtMANV.Text = txtHOTENNV.SelectedValue.ToString();
            }
            catch (Exception) { }
        }
    }
}
