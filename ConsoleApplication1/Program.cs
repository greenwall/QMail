using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//using Microsoft.Office.Core;
//using Microsoft.Office.Interop.Excel;
using Excel = Microsoft.Office.Interop.Excel; 
using Microsoft.Office.Core;
using Microsoft.Office.Interop.Excel;

using System.Data;
using System.Data.OleDb;


namespace excelreader
{


    class Program
    {
        static void Main(string[] args)
        {
//            try
//            {
                Program p = new Program();
                p.ReadExcelFile(@"C:\Users\Torben\Downloads\test-kalender.xlsx");
                //p.ReadExcelFile("C:\\Users\\Torben\\Downloads\\test.xlsx");
                //                p.ReadUsingJet("C:\\Users\\Torben\\Downloads\\test.xlsx");
                
            /*
                        } 
                            catch (Exception e) 
                            {
                                Console.WriteLine(e.Message);
                            }*/
            Console.ReadLine();
        }

        public void ReadUsingJet(string excelFileName)
        {
            OleDbConnection con = new OleDbConnection(@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source="+excelFileName+";Extended Properties=Excel 8.0");
            OleDbDataAdapter da = new OleDbDataAdapter("select * from MyTable", con);
            System.Data.DataTable dt = new System.Data.DataTable();
            da.Fill(dt);
        }

        private Object cellValue(Worksheet sheet, int row, int col)
        {
            Range range = sheet.Cells[row, col];

            if (range != null)
            {
                return range.Value;
            }
            return null;
        }

        private String cellValueAsString(Worksheet sheet, int row, int col, String def = "")
        {
            var value = cellValue(sheet, row, col);
            if (value!=null)
            {
                return value.ToString();
            }
            return def;
        }

        private IEnumerable<String> activitiesAt(Worksheet sheet, DateTime date)
        {
            DateTime start = new DateTime(2012, 1, 1);
            int yearDiff = date.Year - start.Year;
            int monthDiff = date.Month - start.Month;
            int dayDiff = date.Day - start.Day;

            int row = 10 + dayDiff*10;
            int col = 24 + (monthDiff + yearDiff * 12) * 8;

            List<String> activities = new List<String>();
            for (int r = row; r < row + 10; r++)
            {
                String activity = cellValueAsString(sheet, r, col);
                if (!String.IsNullOrWhiteSpace(activity))
                {
                    activities.Add(activity);
                }
            }
            return activities;
        }

        public void ReadExcelFile(string excelFileName)
        {
            Application app = new Application();
            Workbook workBook = app.Workbooks.Open(excelFileName, 0, true, 5, "", "", true, XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
            Worksheet workSheet = workBook.Worksheets.get_Item(1);

            //012345678901234567890123456789
            //abcdefghijklmnopqrstuvwxyz

            int row = 9;
            int col = 24;
            String value = cellValueAsString(workSheet, row, col, "?");

            Console.WriteLine("{0},{1} = {2}", row, col, value);

            DateTime date = new DateTime(2012, 1, 2);
            Console.WriteLine("{0} = {1}", date, String.Join<String>(";",activitiesAt(workSheet, date)));

            date = new DateTime(2012, 3, 5);
            Console.WriteLine("{0} = {1}", date, activitiesAt(workSheet, date));


            for (int r = 8; r < 11; r++)
            {
                for (int c = 22; c < 25; c++)
                {
                    Range range = workSheet.Cells[r, c];
                    string type = "?";
                    value = "?";
                    if (range != null && range.Value != null)
                    {
                        value = range.Value.ToString();
                        if (range.Value.GetType() != null)
                        {
                            type = range.Value.GetType().ToString();
                        }
                    }
//                    range.Value.GetType().ToString();
                    
//                    value = range==null || range.Value==null ? "null" : range.Value.ToString();

                     
                    Console.WriteLine("{0},{1} = {2} {3}", r,c,value, type);

                }
            }
/*
                Workbook theWorkbook = Workbooks.Open(
                   excelFileName, 0, true, 5,
                    "", "", true, XlPlatform.xlWindows, "\t", false, false,
                    0, true);
                Sheets sheets = theWorkbook.Worksheets;
                Worksheet worksheet = (Worksheet)sheets.get_Item(1);
                for (int i = 1; i <= 10; i++)
                {
                    Range range = worksheet.get_Range("A" + i.ToString(), "J" + i.ToString());
                    Array myvalues = (System.Array)range.Cells.Value;
                    //string[] strArray = ConvertToStringArray(myvalues);
                }
  */         

        }
    }
}
