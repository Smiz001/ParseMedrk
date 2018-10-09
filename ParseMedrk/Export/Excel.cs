using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Microsoft.Office.Interop.Excel;
using Application = Microsoft.Office.Interop.Excel.Application;

namespace ParseMedrk.Export
{
  public class Excel
  {
    private List<Element> listElements;
    private string filePath;

    public Excel(List<Element> list, string filePath)
    {
      listElements = new List<Element>(list);
      this.filePath = filePath;
    }

    public void ExecuteExport()
    {
      Application excel;
      Workbook workbook;
      Worksheet worksheet;
      try
      {
        excel = new Application();
        excel.Visible = false;
        excel.DisplayAlerts = false;

        workbook = excel.Workbooks.Add(Type.Missing);
        worksheet = (Worksheet) workbook.ActiveSheet;
        worksheet.Name = "Основная информация";
        CreateTitle(worksheet);
        FillInfo(worksheet);

        workbook.SaveAs(filePath); ;
        workbook.Close();
        excel.Quit();
      }
      catch (Exception e)
      {
        MessageBox.Show(e.Message);
      }
      finally
      {
        worksheet = null;
        workbook = null;
        excel = null;
      }
     
    }

    private void CreateTitle(Worksheet sheet)
    {
      sheet.Cells[1, 1] = "Категория";
      sheet.Cells[1, 2] = "Подкатегория";
      sheet.Cells[1, 3] = "Наименование";
      sheet.Cells[1, 4] = "Цена";
      sheet.Cells[1, 5] = "Id";
      sheet.Cells[1, 6] = "Описание";
      sheet.Cells[1, 7] = "Адрес картинки";
      //sheet.Cells[1, 8] = "Табличная информация";
    }

    private void FillInfo(Worksheet sheet)
    {
      int startRow = 2;
      foreach (var elem in listElements)
      {
        sheet.Cells[startRow, 1] = elem.NameCategory;
        sheet.Cells[startRow, 2] = elem.NameSubCategory;
        sheet.Cells[startRow, 3] = elem.NameElement;
        sheet.Cells[startRow, 4] = elem.Price;
        sheet.Cells[startRow, 5] = elem.Id;
        sheet.Cells[startRow, 6] = elem.Description;
        var arrUrl = elem.UrlImage.Split(';');
        for (int i = 0; i < arrUrl.Length; i++)
        {
          sheet.Cells[startRow, 7+i] = arrUrl[i];
        }
        //sheet.Cells[startRow, 7] = elem.UrlImage;
        startRow++;
      }

    }
  }
}
