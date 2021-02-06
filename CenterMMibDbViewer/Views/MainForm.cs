using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CenterMMibDbViewer.Domain;
using CenterMMibDbViewer.Utils;
using DevExpress.Export;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraPrinting;

namespace CenterMMibDbViewer.Views
{
    public partial class MainForm : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        private readonly MainService _mainService = MainService.Instance;

        private readonly List<GridColumn> _columns = new List<GridColumn>();

        public MainForm()
        {
            InitializeComponent();

            _columns.AddRange(gvInPacketData.Columns);
            CodeEditorHelper.instance.Initialize(scintilla1, this);
            ViewInPacketData(null);

            //richEditControl1.ReplaceService<ISyntaxHighlightService>(new HTMLSyntaxHighlightService(richEditControl1));
            //using (IRichEditDocumentServer server = richEditControl1.CreateDocumentServer())
            //{
            //    server.Text = "some HTML text";
            //    richEditControl1.Text = server.HtmlText;
            //}
            //richEditControl1.Document.DefaultCharacterProperties.FontName = "Consolas";
            //richEditControl1.Document.DefaultCharacterProperties.FontSize = 9;

            RefreshData();
        }


        private void gridView1_FocusedRowChanged(object sender,
            DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            ProcessSelectedInPacket();
            //var paramValue = new NpgsqlParameter<long>("@inp_suid1", inPacket.suid);

            // SELECT lo_get(inp.payload) FROM in_packet inp WHERE inp.suid = 44;

           //syntaxEditor1.Document.Update();


            //using (var cmd = new NpgsqlCommand("SELECT lo_get(inp.payload) FROM in_packet inp WHERE inp.suid = @inp_suid", MainService.instance.MibDbContext.Database.Connection as NpgsqlConnection))
            //{
            //    cmd.Parameters.Add(new NpgsqlParameter<long>("@inp_suid", inPacket.suid));
            //    MainService.instance.MibDbContext.Database.Connection.Open();


            //    using (var reader = cmd.ExecuteReader())
            //    {
            //        while (reader.Read())
            //        {
            //            var ss = reader[0];
            //        }
            //    }
            //}
        }

        private void ProcessSelectedInPacket()
        {
            var inPacket = GetSelectedInPacket();

            LoadPayloadData(inPacket);
            ProcessInPacketData(inPacket);

            scintilla1.ReadOnly = false;
            scintilla1.Text = inPacket?.payloadData;
            scintilla1.ReadOnly = true;

            ViewInPacketData(inPacket?.parsedData);
        }

        private void LoadPayloadData(in_packet inp)
        {
            if (inp == null)
                return;

            if (string.IsNullOrEmpty(inp.payloadData))
            {
                var inpData = _mainService.Database.SqlQuery<byte[]>(
                    "SELECT lo_get(inp.payload) FROM in_packet inp WHERE inp.suid = " + inp.suid);

                if (inpData.Any())
                    inp.payloadData = Encoding.UTF8.GetString(inpData.ElementAt(0));
            }
        }

        private in_packet GetSelectedInPacket() => (in_packet) gvInPackets?.GetRow(gvInPackets.FocusedRowHandle);

        private void cmdUpdate_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            RefreshData();
        }

        private void RefreshData()
        {
            //_mainService.MibDbContext.in_packet.Load();
            using (var context = _mainService.GetNewDbContext())
            {
                var data = context.Set<in_packet>();
                data.Load();
                bsInPackets.DataSource = data.Local.ToBindingList();
                ProcessSelectedInPacket();
            }
        }

        private void cmdCopy_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Clipboard.SetText(scintilla1.Text);
        }

        private void cmdProcess_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ViewInPacketData(GetTableFromRawData(File.ReadAllText(@"..\..\Data\data.xml")));
        }

        private void ProcessInPacketData(in_packet inp)
        {
            if (inp != null)
            {
                if (inp.parsedData == null)
                {
                    LoadPayloadData(inp);
                    inp.parsedData = GetTableFromRawData(inp.payloadData);
                }
            }
        }

        private Table GetTableFromRawData(string rawData) => new XmlDataProcessor().Process(rawData);


        private void ViewInPacketData(Table inPacketData)
        {
            gvInPacketData.Columns.Clear();
            bsInPacketData.DataSource = null;

            if (inPacketData == null)
                return;

            for (int i = 0; i < inPacketData.header.Count; ++i)
            {
                var colVal = _columns.Find(column => column.Name == "colvalue" + (i + 1));

                if (colVal == null)
                    continue;

                var headerCol = inPacketData.header[i];

                colVal.Caption = headerCol.name + (!string.IsNullOrEmpty(headerCol.description)
                    ? " (" + headerCol.description + ")"
                    : null);

                gvInPacketData.Columns.Add(colVal);
                colVal.VisibleIndex = i;
            }

            if (inPacketData.header.Count != 0)
                bsInPacketData.DataSource = inPacketData.rows;

            gridInPacketData.Tag = inPacketData;
        }

        private void cmdExport_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ExportGridViewData(gvInPacketData, "informationalObject");
        }

        private void ExportGridViewData(GridView gridView, string reportName)
        {
            //gridView.OptionsPrint.PrintAllNodes = true;

            if (gridView.Columns.Count == 0)
                return;

            gridView.OptionsPrint.ShowPrintExportProgress = true;
            gridView.BestFitColumns();

            var reportPath = Path.Combine(Environment.CurrentDirectory, reportName + ".xlsx");

            gridView.OptionsView.ColumnAutoWidth = false;
            gridView.BestFitColumns();

            var exportParams =
                new XlsxExportOptionsEx
                {
                    AllowSortingAndFiltering = DevExpress.Utils.DefaultBoolean.True,
                    ExportType = ExportType.WYSIWYG,
                    ShowGridLines = true,
                };

            gridView.ExportToXlsx(reportPath, exportParams);
            gridView.OptionsView.ColumnAutoWidth = true;

            Process.Start(reportPath);
        }
    }
}