using System.Collections.Generic;

namespace CenterMMibDbViewer
{
    public class Table
    {
        public List<Column> header { get; } = new List<Column>();
        public List<Row> rows { get; } = new List<Row>();
    }

    public class Column
    {
        public ColumnDataType dataType { get; set; }
        public string name { get; set; }
        public string description { get; set; }
    }


    public class Row
    {
        public List<string> values { get; } = new List<string>();

        public string value1 => GetValue(0);
        public string value2 => GetValue(1);
        public string value3 => GetValue(2);
        public string value4 => GetValue(3);
        public string value5 => GetValue(4);
        public string value6 => GetValue(5);
        public string value7 => GetValue(6);
        public string value8 => GetValue(7);
        public string value9 => GetValue(8);
        public string value10 => GetValue(9);
        public string value11 => GetValue(10);
        public string value12 => GetValue(11);
        public string value13 => GetValue(12);
        public string value14 => GetValue(13);
        public string value15 => GetValue(14);
        public string value16 => GetValue(15);
        public string value17 => GetValue(16);
        public string value18 => GetValue(17);
        public string value19 => GetValue(18);
        public string value20 => GetValue(19);
        public string value21 => GetValue(20);
        public string value22 => GetValue(21);
        public string value23 => GetValue(22);
        public string value24 => GetValue(23);
        public string value25 => GetValue(24);
        public string value26 => GetValue(25);
        public string value27 => GetValue(26);
        public string value28 => GetValue(27);
        public string value29 => GetValue(28);
        public string value30 => GetValue(29);

        private string GetValue(int index) => index < values.Count ? values[index] : null;
    }

    public enum ColumnDataType
    {
        Unknown,
        n,
        c
    }
}

/*
 *  <table>
        <header>
         <column type="n" description="">tb_obs_id</column>
         <column type="c" description="">tk_nshsp_name</column>
        </header>
        <row>
         <r0>268</r0>
         <r1>Восточный военный округ</r1>
        </row>
        <row>
         <r0>262</r0>
         <r1>Западный Военный округ</r1>
        </row>
        <row>
         <r0>308</r0>
         <r1>Центральный Военный округ</r1>
        </row>
        <row>
         <r0>269</r0>
         <r1>Южный Военный округ</r1>
        </row>
       </table>
 */
