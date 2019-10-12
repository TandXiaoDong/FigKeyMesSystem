using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls;
using Telerik.WinControls.UI;
using MesManager.Control;
using System.IO;
using Telerik.WinControls.UI.Export;
using CommonUtils.Logger;
using CommonUtils.FileHelper;
using MesManager.TelerikWinform.GridViewCommon.GridViewDataExport;
using System.Threading;
using Telerik.WinControls.Themes;
using MesManager.Common;

namespace MesManager.UI
{
    public partial class ProductPackageDetail : RadForm
    {
        private string outCaseCode;
        private MesService.MesServiceClient serviceClient;
        public ProductPackageDetail(string outcasecode)
        {
            InitializeComponent();
            this.outCaseCode = outcasecode;
            this.StartPosition = FormStartPosition.CenterParent;
        }

        private void ProductPackageDetail_Load(object sender, EventArgs e)
        {
            this.tool_package_exportFilter.Items.Clear();
            this.tool_package_exportFilter.Items.Add(ExportFormat.EXCEL);
            this.tool_package_exportFilter.Items.Add(ExportFormat.HTML);
            this.tool_package_exportFilter.Items.Add(ExportFormat.PDF);
            this.tool_package_exportFilter.Items.Add(ExportFormat.CSV);
            this.tool_package_exportFilter.SelectedIndex = 0;
            serviceClient = new MesService.MesServiceClient();
            DataGridViewCommon.SetRadGridViewProperty(this.radGridViewPackage,false);
            this.radGridViewPackage.ReadOnly = true;
            LoadDataSource(this.outCaseCode);
        }

        private void Btn_selectOfPackage_Click(object sender, EventArgs e)
        {
            var queryFilter = this.tb_package.Text;
            QueryDataSource(queryFilter);
        }

        private void LoadDataSource(string queryFilter)
        {
            var dt = serviceClient.SelectPackageProductOfCaseCode(queryFilter, "1", true).Tables[0];
            this.radGridViewPackage.DataSource = null;
            this.radGridViewPackage.DataSource = dt;
        }

        private void QueryDataSource(string queryFilter)
        {
            var dt = serviceClient.SelectPackageProduct(this.outCaseCode,queryFilter, "1", true).Tables[0];
            this.radGridViewPackage.DataSource = null;
            this.radGridViewPackage.DataSource = dt;
        }
        private enum ExportFormat
        {
            EXCEL,
            HTML,
            PDF,
            CSV
        }

        private void ExportGridViewData(int selectIndex, RadGridView radGridView)
        {
            var filter = "Excel (*.xls)|*.xls";
            if (selectIndex == (int)ExportFormat.EXCEL)
            {
                filter = "Excel (*.xls)|*.xls";
                var path = FileSelect.SaveAs(filter, "C:\\");
                if (path == "")
                    return;
                ExportData.RunExportToExcelML(path, radGridView);
            }
            else if (selectIndex == (int)ExportFormat.HTML)
            {
                filter = "Html File (*.htm)|*.htm";
                var path = FileSelect.SaveAs(filter, "C:\\");
                if (path == "")
                    return;
                ExportData.RunExportToHTML(path, radGridView);
            }
            else if (selectIndex == (int)ExportFormat.PDF)
            {
                filter = "PDF file (*.pdf)|*.pdf";
                var path = FileSelect.SaveAs(filter, "C:\\");
                if (path == "")
                    return;
                ExportData.RunExportToPDF(path, radGridView);
            }
            else if (selectIndex == (int)ExportFormat.CSV)
            {
                filter = "PDF file (*.pdf)|*.csv";
                var path = FileSelect.SaveAs(filter, "C:\\");
                if (path == "")
                    return;
                ExportData.RunExportToCSV(path, radGridView);
            }
        }

        private void Tool_package_export_Click(object sender, EventArgs e)
        {
            ExportGridViewData(this.tool_package_exportFilter.SelectedIndex,this.radGridViewPackage);
        }
    }
}
