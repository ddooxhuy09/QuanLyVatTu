using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraReports.UI;
using QLVT;

namespace QLVT
{
    public partial class XfrmPhieuNvLapTrongNamTheoLoai : Form
    {
        public XfrmPhieuNvLapTrongNamTheoLoai()
        {
            InitializeComponent();
        }

        private void XfrmPhieuNvLapTrongNamTheoLoai_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'qLVTDataSet.DSNVKhongMaNV' table. You can move, or remove it, as needed.
            this.taDSNVKhongMaNV.Fill(this.qLVTDataSet.DSNVKhongMaNV);

        }

        private void btnXemTruoc_Click(object sender, EventArgs e)
        {
            //this.btnXemTruoc.Enabled = false;
            Xrpt_PhieuNvLapTrongNamTheoLoai form = new Xrpt_PhieuNvLapTrongNamTheoLoai();
            form.ShowPreview();
        }
    }
}
