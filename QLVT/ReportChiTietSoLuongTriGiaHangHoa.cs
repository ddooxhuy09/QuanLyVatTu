using System;
using DevExpress.XtraReports.UI;

namespace QLVT
{
    public partial class ReportChiTietSoLuongTriGiaHangHoa : DevExpress.XtraReports.UI.XtraReport
    {
        public ReportChiTietSoLuongTriGiaHangHoa()
        {
            InitializeComponent();
        }

        // Thêm thuộc tính public để truy cập các nhãn
        public XRLabel LoaiPhieuLabel => xrLabel3;
        public XRLabel TuNgayLabel => xrLabel5;
        public XRLabel DenNgayLabel => xrLabel6;
        public XRLabel TongTriGiaLabel => xrLabel9;
    }
}