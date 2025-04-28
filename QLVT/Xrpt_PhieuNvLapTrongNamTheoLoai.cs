using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using DevExpress.XtraReports.UI;

namespace QLVT
{
    public partial class Xrpt_PhieuNvLapTrongNamTheoLoai : DevExpress.XtraReports.UI.XtraReport
    {
        //constructor
        public Xrpt_PhieuNvLapTrongNamTheoLoai()
        {
            InitializeComponent();
        }

        public Xrpt_PhieuNvLapTrongNamTheoLoai(int manv, string loai, int nam)
        {
            InitializeComponent();
            this.sqlDataSource1.Connection.ConnectionString = Program.connstr;
            this.sqlDataSource1.Queries[0].Parameters[0].Value = manv;
            this.sqlDataSource1.Queries[0].Parameters[1].Value = loai;
            this.sqlDataSource1.Queries[0].Parameters[2].Value = nam;
            this.sqlDataSource1.Fill();
        }


    }
}
