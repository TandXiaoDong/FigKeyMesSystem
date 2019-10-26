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
using WindowsFormTelerik.GridViewExportData;
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
            this.tool_package_exportFilter.Items.Add(GridViewExport.ExportFormat.EXCEL);
            this.tool_package_exportFilter.Items.Add(GridViewExport.ExportFormat.HTML);
            this.tool_package_exportFilter.Items.Add(GridViewExport.ExportFormat.PDF);
            this.tool_package_exportFilter.Items.Add(GridViewExport.ExportFormat.CSV);
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

        private void Tool_package_export_Click(object sender, EventArgs e)
        {
            GridViewExport.ExportFormat exportFormat = GridViewExport.ExportFormat.EXCEL;
            Enum.TryParse(tool_package_exportFilter.Text, out exportFormat);
            GridViewExport.ExportGridViewData(exportFormat, radGridViewPackage);
        }
    }
}
