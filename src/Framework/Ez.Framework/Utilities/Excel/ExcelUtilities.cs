using Ez.Framework.Utilities.Excel.Attributes;
using Ez.Framework.Utilities.Excel.Enums;
using Ez.Framework.Utilities.Excel.Models;
using Ez.Framework.Utilities.Reflection;
using NPOI.HSSF.UserModel;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Ez.Framework.Utilities.Excel
{
    /// <summary>
    /// Excel utilities
    /// </summary>
    public class ExcelUtilities
    {
        public static HSSFWorkbook CreateWorkBook<T>(IEnumerable<T> data, bool exportAll = true)
        {
            var properties = exportAll
                ? ReflectionUtilities.GetAllPropertiesOfType(typeof(T))
                : ReflectionUtilities.GetPropertiesHaveAttribute(typeof(T), typeof(ExcelExportAttribute));

            var excelProperties = new List<ExcelProperty>();
            foreach (var property in properties)
            {
                var attribute = property.GetCustomAttribute<ExcelExportAttribute>();

                if (attribute != null && attribute.Ignore)
                    break;

                var name = attribute == null ? string.Empty : attribute.Name;
                var priority = attribute == null ? ExportPriority.Medium : attribute.Priority;

                if (string.IsNullOrEmpty(name))
                {
                    name = property.Name.ToLowercaseNamingConvention();
                }

                excelProperties.Add(new ExcelProperty
                {
                    DisplayName = name,
                    Name = property.Name,
                    Priority = priority
                });
            }

            excelProperties = excelProperties.OrderBy(p => p.Priority).ToList();

            //Create new Excel workbook
            var workbook = new HSSFWorkbook();

            //Create new Excel sheet
            var sheet = workbook.CreateSheet();

            //Create a header row
            var headerRow = sheet.CreateRow(0);

            var itemNumber = 0;
            foreach (var property in excelProperties)
            {
                headerRow.CreateCell(itemNumber).SetCellValue(property.DisplayName);
                itemNumber++;
            }

            var rowNumber = 1;
            foreach (T item in data)
            {
                //Create a new row
                var row = sheet.CreateRow(rowNumber);
                itemNumber = 0;
                foreach (var property in excelProperties)
                {
                    try
                    {
                        row.CreateCell(itemNumber).SetCellValue(item.GetPropertyValue(property.Name).ToString());
                    }
                    catch
                    {
                        row.CreateCell(itemNumber).SetCellValue(string.Empty);
                    }
                    itemNumber++;
                }
                rowNumber++;
            }
            return workbook;
        }
    }
}
