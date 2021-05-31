using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace Common
{
    /// <summary>
    /// excel帮助类，简单的excel导出
    /// </summary>
    public static class ExcelUtil
    {
        #region 导出Dictionary<string, List<object>>类型，按列导出
        /// <summary>
        /// 导出Excel到文件
        /// </summary>
        /// <param name="FilePath">目的路径</param>
        /// <param name="SheetName">页签名称</param>
        /// <param name="Data">数据，key:列名  value:这列的值，会根据数据类型来输出</param>
        /// <param name="FirstColumnIndexName">第一列序号列名称，写此参数，会在第一列加自增序号</param>
        public static void ExportFile(string FilePath, Dictionary<string, ArrayList> Data,
            string SheetName = "Sheet1", string FirstColumnIndexName = "")
        {
            FileStream fs = new FileStream(FilePath, FileMode.Create, FileAccess.ReadWrite);
            var workbook = Export(Data, SheetName, FirstColumnIndexName);
            workbook.Write(fs);
        }
        /// <summary>
        /// 返回excel的内存流
        /// </summary>
        /// <param name="SheetName">页签名称</param>
        /// <param name="Data">数据，key:列名  value:这列的值，会根据数据类型来输出</param>
        /// <param name="FirstColumnIndexName">第一列序号列名称，写此参数，会在第一列加自增序号</param>
        public static MemoryStream ExportStream(Dictionary<string, ArrayList> Data,
            string SheetName = "Sheet1", string FirstColumnIndexName = "")
        {
            MemoryStream ms = new MemoryStream();
            var workbook = Export(Data, SheetName, FirstColumnIndexName);
            workbook.Write(ms,true);
            ms.Position = 0;
            return ms;
        }
        /// <summary>
        /// 导出Excel
        /// </summary>
        /// <param name="SheetName">页签名称</param>
        /// <param name="Data">数据，key:列名  value:这列的值，会根据数据类型来输出</param>
        /// <param name="FirstColumnIndexName">第一列序号列名称，写此参数，会在第一列加自增序号</param>
        public static XSSFWorkbook Export(Dictionary<string, ArrayList> Data, string SheetName = "Sheet1", string FirstColumnIndexName = "")
        {
            int numCurrentCol = 0;//当前分析列
            //集合里，最大总数
            int numMaxDataCount = Data.Values.Max(l => l.Count);
            XSSFWorkbook workbook = CreateWorkbook(SheetName, numMaxDataCount, FirstColumnIndexName, ref numCurrentCol);
            ISheet sheet = workbook.GetSheet(SheetName);
            //填充数据
            foreach (var kv in Data)
            {
                AddColData(sheet, 1, numCurrentCol, kv.Value);
                numCurrentCol++;
            }

            return workbook;
        }
        #endregion

        #region 导出DataTable类型
        /// <summary>
        /// 导出Excel到文件
        /// </summary>
        /// <param name="FilePath">目的路径</param>
        /// <param name="SheetName">页签名称</param>
        /// <param name="Data">数据</param>
        /// <param name="FirstColumnIndexName">第一列序号列名称，写此参数，会在第一列加自增序号</param>
        public static void ExportFile(string FilePath, DataTable Data,
            string SheetName = "Sheet1", string FirstColumnIndexName = "")
        {
            FileStream fs = new FileStream(FilePath, FileMode.Create, FileAccess.ReadWrite);
            var workbook = Export(Data, SheetName, FirstColumnIndexName);
            workbook.Write(fs);
        }
        /// <summary>
        /// 返回excel的内存流
        /// </summary>
        /// <param name="SheetName">页签名称</param>
        /// <param name="Data">数据</param>
        /// <param name="FirstColumnIndexName">第一列序号列名称，写此参数，会在第一列加自增序号</param>
        public static MemoryStream ExportStream(DataTable Data,
            string SheetName = "Sheet1", string FirstColumnIndexName = "")
        {
            MemoryStream ms = new MemoryStream();
            var workbook = Export(Data, SheetName, FirstColumnIndexName);
            workbook.Write(ms,true);
            ms.Position = 0;
            return ms;
        }
        /// <summary>
        /// datatable导出Excel
        /// </summary>
        /// <param name="Data">数据</param>
        /// <param name="SheetName">页签名称</param>
        /// <param name="FirstColumnIndexName">第一列序号列名称，写此参数，会在第一列加自增序号</param>
        public static XSSFWorkbook Export(DataTable Data, string FirstColumnIndexName = "", string SheetName = "Sheet1")
        {
            int numCurrentCol = 0;//当前分析列
            int startRow = 0;
            //创建workbook
            XSSFWorkbook workbook = CreateWorkbook(SheetName, Data.Rows.Count, FirstColumnIndexName, ref numCurrentCol);
            ISheet sheet = workbook.GetSheet(SheetName);

            //填充列名
            ArrayList listColNameRow = new ArrayList();
            for (int i = 0; i < Data.Columns.Count; i++)
                listColNameRow.Add(Data.Columns[i].ColumnName.ToString());
            AddRowData(sheet, startRow, numCurrentCol, listColNameRow);
            startRow++;

            foreach (DataRow dr in Data.Rows)
            {
                ArrayList listValue = new ArrayList();
                for (int i = 0; i < Data.Columns.Count; i++)
                    listValue.Add(dr[i]);

                AddRowData(sheet, startRow, numCurrentCol, listValue);
                startRow++;
            }

            return workbook;
        }
        #endregion

        #region 导出List<T>，一行一行导出
        /// <summary>
        /// 导出Excel到文件
        /// </summary>
        /// <param name="FilePath">目的路径</param>
        /// <param name="SheetName">页签名称</param>
        /// <param name="Data">每个对象作为一行的数据</param>
        /// <param name="FirstColumnIndexName">第一列序号列名称，写此参数，会在第一列加自增序号</param>
        public static void ExportFile<T>(string FilePath, List<T> Data, string FirstColumnIndexName = "", string SheetName = "Sheet1") where T : class, new()
        {
            FileStream fs = new FileStream(FilePath, FileMode.Create, FileAccess.ReadWrite);
            var workbook = Export(Data, SheetName, FirstColumnIndexName);
            workbook.Write(fs);
        }
        /// <summary>
        /// 返回excel的内存流
        /// </summary>
        /// <param name="SheetName">页签名称</param>
        /// <param name="Data">每个对象作为一行的数据</param>
        /// <param name="FirstColumnIndexName">第一列序号列名称，写此参数，会在第一列加自增序号</param>
        public static MemoryStream ExportStream<T>(List<T> Data, string FirstColumnIndexName = "", string SheetName = "Sheet1") where T : class, new()
        {
            MemoryStream ms = new MemoryStream();
            var workbook = Export(Data, SheetName, FirstColumnIndexName);
            workbook.Write(ms,true);
            ms.Position = 0;
            return ms;
        }
        /// <summary>
        /// 类导出到Excel
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Data">每个对象作为一行的数据</param>
        /// <param name="FirstColumnIndexName">在第一列加自增序列</param>
        /// <param name="SheetName">页签名称</param>
        public static XSSFWorkbook Export<T>(List<T> Data, string FirstColumnIndexName = "", string SheetName = "Sheet1") where T : class, new()
        {
            int numCurrentCol = 0;//当前分析列
            int startRow = 0;
            //创建workbook
            XSSFWorkbook workbook = CreateWorkbook(SheetName, Data.Count, FirstColumnIndexName, ref numCurrentCol);
            ISheet sheet = workbook.GetSheet(SheetName);

            Type typeModel = typeof(T);
            var props = typeModel.GetProperties();

            foreach (var d in Data)
            {
                ArrayList listValue = new ArrayList();
                foreach (var item in props)
                {
                    listValue.Add(item.GetValue(d, null));
                }
                AddRowData(sheet, startRow, numCurrentCol, listValue);
                startRow++;
            }

            return workbook;
        }
        #endregion

        #region 导入时调用的私有方法
        /// <summary>
        /// 按列插入 
        /// </summary>
        /// <param name="sheet">页签</param>
        /// <param name="startRow">起始行号</param>
        /// <param name="startCol">起始列号</param>
        /// <param name="colData">填充列的数组</param>
        private static void AddColData(ISheet sheet, int startRow, int startCol, ArrayList colData)
        {
            for (int i = 0; i < colData.Count; i++)
            {
                IRow row = sheet.GetRow(startRow);
                ICell cell = row.CreateCell(startCol);
                object data = colData[i];
                switch (data)
                {
                    case int ii: cell.SetCellValue(ii); break;
                    case double db: cell.SetCellValue(db); break;
                    case float f: cell.SetCellValue(f); break;
                    case decimal dc: cell.SetCellValue((double)dc); break;
                    case DateTime dt: cell.SetCellValue(dt.ToyyyyMMddHHmmss()); break;
                    case bool b: cell.SetCellValue(b); break;
                    default: cell.SetCellValue(data.ToString()); break;
                }
                startRow++;
            }
        }

        /// <summary>
        /// 按行插入
        /// </summary>
        /// <param name="sheet">页签</param>
        /// <param name="startRow">起始行号</param>
        /// <param name="startCol">起始列号</param>
        /// <param name="rowData">填充行的数组</param>
        private static void AddRowData(ISheet sheet, int startRow, int startCol, ArrayList rowData)
        {
            IRow row = sheet.GetRow(startRow);
            for (int i = 0; i < rowData.Count; i++)
            {
                ICell cell = row.CreateCell(startCol);
                object data = rowData[i];
                switch (data)
                {
                    case int ii: cell.SetCellValue(ii); break;
                    case double db: cell.SetCellValue(db); break;
                    case float f: cell.SetCellValue(f); break;
                    case decimal dc: cell.SetCellValue((double)dc); break;
                    case DateTime dt: cell.SetCellValue(dt.ToyyyyMMddHHmmss()); break;
                    case bool b: cell.SetCellValue(b); break;
                    default: cell.SetCellValue(data.ToString()); break;
                }
                startCol++;
            }
        }

        /// <summary>
        /// 创建workbook，如果有序号列名，则会给numCurrentCol+1
        /// </summary>
        /// <param name="SheetName">页签名称</param>
        /// <param name="count">行数</param>
        /// <param name="FirstColumnIndexName">在第一列添加自增序列</param>
        /// <param name="numCurrentCol">当前列号</param>
        /// <returns></returns>
        private static XSSFWorkbook CreateWorkbook(string SheetName, int count, string FirstColumnIndexName, ref int numCurrentCol)
        {
            XSSFWorkbook workbook = new XSSFWorkbook();
            if (SheetName.IsNullOrEmpty())
                SheetName = "Sheet1";

            ISheet sheet = workbook.CreateSheet(SheetName);

            //创建所有行
            for (int i = 0; i < count + 1; i++)
            {
                sheet.CreateRow(i);
            }

            if (FirstColumnIndexName.IsNotNullOrEmpty())
            {
                IRow rowFirst = sheet.GetRow(0);
                //第一列添加序号
                ArrayList index = new ArrayList();

                for (int j = 0; j < count; j++)
                {
                    index.Add(j);
                }
                rowFirst.CreateCell(numCurrentCol).SetCellValue(FirstColumnIndexName);
                AddColData(sheet, 1, numCurrentCol, index);
                numCurrentCol++;
            }
            return workbook;
        }
        #endregion
    }
}
