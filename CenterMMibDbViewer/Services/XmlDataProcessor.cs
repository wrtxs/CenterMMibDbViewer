using System;
using System.Globalization;
using System.Xml;


namespace CenterMMibDbViewer
{
    class XmlDataProcessor
    {
        private Table _tableData;

        public Table Process(string xmlData)
        {
            _tableData = new Table();

            if (!string.IsNullOrEmpty(xmlData))
            {
                var doc = new XmlDocument();

                try
                {
                    doc.LoadXml(xmlData);

                    var tableElements = doc.GetElementsByTagName("table");

                    if (tableElements.Count > 0)
                        ProcessTableElement(tableElements[0] as XmlElement);
                }
                catch (Exception ex)
                {
                    RegisterErrorInfo(ex.GetType().Name + ": " + ex.Message);
                }
            }

            return _tableData;
        }

        private void RegisterErrorInfo(string errorInfo)
        {
            _tableData.header.Clear();
            _tableData.rows.Clear();

            _tableData.header.Add(new Column
                {name = "Error", description = "Описание ошибки", dataType = ColumnDataType.c});

            _tableData.rows.Add(new Row {values = {errorInfo}});
        }

        private void ProcessTableElement(XmlElement node)
        {
            foreach (XmlNode childNode in node)
            {
                switch (childNode.Name.ToLower(CultureInfo.InvariantCulture))
                {
                    case "header":
                        ProcessHeader(childNode as XmlElement);
                        break;
                    case "row":
                        ProcessRow(childNode as XmlElement);
                        break;
                }
            }
        }

        private void ProcessHeader(XmlElement headerNode)
        {
            foreach (XmlElement colNode in headerNode.GetElementsByTagName("column"))
            {
                var column = new Column
                {
                    name = colNode.InnerText,
                    dataType = GetColumnDataType(colNode.Attributes["type"].Value),
                    description = colNode.Attributes["description"].Value
                };

                _tableData.header.Add(column);
            }
        }

        private static ColumnDataType GetColumnDataType(string strType)
        {
            switch (strType.ToLower())
            {
                case "n": return ColumnDataType.n;
                case "c": return ColumnDataType.c;
                default: return ColumnDataType.Unknown;
            }
        }

        private void ProcessRow(XmlElement rowNode)
        {
            var row = new Row();

            foreach (XmlElement rowValueNode in rowNode.ChildNodes)
            {
                row.values.Add(rowValueNode.InnerText);
            }

            _tableData.rows.Add(row);
        }
    }
}
