using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using AngleSharp.Dom;
using AngleSharp.Parser.Html;

namespace ParseMedrk
{
  public partial class MainForm : Form
  {
    public MainForm()
    {
      InitializeComponent();
    }

    private string mainLink = @"http://www.medrk.ru/shop/";
    private string direcPath = @"D:\ParserInfo\Medr\";
    private string mainFilePath = @"D:\ParserInfo\Medr\Medr.csv";
    private BindingList<Element> listElements = new BindingList<Element>();

    private void btDownloadData_Click(object sender, EventArgs e)
    {
      IHtmlCollection<IElement> collections;
      using (var webClient = new WebClient())
      {
        ServicePointManager.Expect100Continue = true;
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
        ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
        webClient.Encoding = Encoding.UTF8;
        var responce = webClient.DownloadString(mainLink);
        var parser = new HtmlParser();
        var document = parser.Parse(responce);

        collections = document.GetElementsByClassName("category-item");
      }
      if (collections.Length > 0)
      {
        Cursor.Current = Cursors.WaitCursor;
        ParseAll(collections);
        Cursor.Current = Cursors.Default;
      }
      dgvMainInfo.DataSource = listElements;
      MessageBox.Show("Загрузка выполнена");
    }

    private void ParseAll(IHtmlCollection<IElement> collections)
    {
      var random = new Random();

      foreach (var element in collections)
      {
        var href = element.GetElementsByClassName("hidden-xs")[0].GetAttribute("href").Replace("/shop/", "");
        var nameCategory = element.GetElementsByClassName("img-responsive")[0].GetAttribute("alt");
        var ul = element.GetElementsByClassName("catalog-parts hidden-xs");
        if (ul.Length > 0)
        {
          ParseCategory($"{mainLink}{href}", nameCategory);
        }
        else
          ParseSubCategory($"{mainLink}{href}&kol_page=25", nameCategory, "");
      }
    }

    private void ParseCategory(string urlCategory, string nameCategory)
    {
      using (var webClient = new WebClient())
      {
        ServicePointManager.Expect100Continue = true;
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
        ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
        webClient.Encoding = Encoding.UTF8;
        var responce = webClient.DownloadString(urlCategory);

        var parser = new HtmlParser();
        var document = parser.Parse(responce);
        var subCat = document.GetElementsByClassName("category-item");
        foreach (var sub in subCat)
        {
          var href = sub.GetElementsByClassName("hidden-xs")[0].GetAttribute("href").Replace("/shop/", "");
          ParseSubCategory($"{mainLink}{href}&kol_page=25", nameCategory, sub.TextContent);
        }
      }
    }

    private void ParseSubCategory(string urlSubCategory, string nameCat, string nameSubCat)
    {
      var nameSubCatLocl = nameSubCat.Replace("  ", "").Replace("\n", "");
      var random = new Random();
      using (var webClient = new WebClient())
      {
        ServicePointManager.Expect100Continue = true;
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
        ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
        webClient.Encoding = Encoding.UTF8;
        var responce = webClient.DownloadString(urlSubCategory);

        var parser = new HtmlParser();
        var document = parser.Parse(responce);

        int countPage = 1;
        var countPageElements = document.GetElementsByClassName("col-md-9 col-sm-9 page-amount")[0].GetElementsByTagName("LI");

        if (countPageElements.Length > 2)
        {
          countPage = countPageElements[0].GetElementsByTagName("LI").Length - 1;
        }

        for (int i = 1; i <= countPage; i++)
        {
          Thread.Sleep(random.Next(1000, 3000));
          urlSubCategory += $@"&page={i}/";
          responce = webClient.DownloadString(urlSubCategory);
          document = parser.Parse(responce);

          var collectionItems = document.GetElementsByClassName("row catalog-product hidden-xs");
          foreach (var element in collectionItems)
          {
            var elem = new Element { NameCategory = nameCat, NameSubCategory = nameSubCatLocl };
            var s16 = element.GetElementsByClassName("s16");
            var priceGray = element.GetElementsByClassName("price-gray");
            var priceGreen = element.GetElementsByClassName("price-green");
            var priceRed = element.GetElementsByClassName("price-red");
            string href = string.Empty;

            if (s16.Length > 0)
            {
              elem.NameElement = s16[0].TextContent;
              href = s16[0].GetAttribute("href").Replace("/shop/", "");
            }

            if (priceGray.Length > 0)
            {
              elem.Price = priceGray[0].TextContent.Replace(" ", "");
              var regex = new Regex(@"(\d+\.\d+₽)");
              elem.Price = regex.Match(elem.Price).Value;
            }
            else if (priceGreen.Length > 0)
            {
              elem.Price = priceGreen[0].TextContent.Replace(" ", "");
              var regex = new Regex(@"(\d+\.\d+₽)");
              elem.Price = regex.Match(elem.Price).Value;
            }
            else if (priceRed.Length > 0)
            {
              elem.Price = "Не поставляется";
            }
            else
              elem.Price = "Цена по запросу";

            if (!string.IsNullOrEmpty(href))
            {
              var regex = new Regex(@"(\d+$)");
              elem.Id = long.Parse(regex.Match(href).Value);
              ParseDescription(elem, mainLink + href, webClient);
            }
          }
        }
      }
    }

    private void ParseDescription(Element elem, string url, WebClient webClient)
    {
      var responce = webClient.DownloadString(url);
      var parser = new HtmlParser();
      var document = parser.Parse(responce);

      var about = document.GetElementById("about");
      var childs = about.Children;
      foreach (var p in childs)
      {
        if (p.TagName == "P")
          elem.Description += p.TextContent;
      }

      var image = document.GetElementsByClassName("img-responsive img-rounded item-big-img");
      if (image.Length > 0)
      {
        elem.UrlImage = image[0].GetAttribute("src");
        //webClient.DownloadFile(elem.UrlImage, $"{direcPath}{elem.NameElement}.jpg");
      }

      var table = document.GetElementsByTagName("TABLE");
      if (table.Length > 0)
        ParseTable(elem, table[table.Length - 1]);

      //WriteInFileElement(elem);
      listElements.Add(elem);
    }

    private void WriteInFileElement(Element elem, StreamWriter sw)
    {

      sw.BaseStream.Position = sw.BaseStream.Length;
      sw.WriteLine($@"'{elem.NameCategory}';'{elem.NameSubCategory}';'{elem.Id}';'{elem.NameElement}';'{elem.Price}';'{elem.Description.Replace("'", "")}';'{elem.InfoFromTable.Replace("'", "")}';'{elem.UrlImage}'");

    }

    private void ParseTable(Element elem, IElement htmlElement)
    {
      var trs = htmlElement.GetElementsByTagName("TR");
      var sb = new StringBuilder();

      for (int i = 1; i < trs.Length; i++)
      {
        var tds = trs[i].GetElementsByTagName("td");
        for (int j = 0; j < tds.Length; j++)
        {
          sb.Append($"{tds[j].TextContent},");
        }
        sb.AppendLine();
      }
      elem.InfoFromTable = sb.ToString();
    }

    private void MainForm_Load(object sender, EventArgs e)
    {
      dgvMainInfo.AutoGenerateColumns = false;

      DataGridViewColumn colCat = new DataGridViewTextBoxColumn();
      colCat.DataPropertyName = "NameCategory";
      colCat.HeaderText = "Категория";
      colCat.Name = "NameCategory";
      colCat.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
      dgvMainInfo.Columns.Add(colCat);

      DataGridViewColumn colSubCat = new DataGridViewTextBoxColumn();
      colSubCat.DataPropertyName = "NameSubCategory";
      colSubCat.HeaderText = "Полкатегория";
      colSubCat.Name = "NameSubCategory";
      colCat.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
      dgvMainInfo.Columns.Add(colSubCat);

      DataGridViewColumn colElem = new DataGridViewTextBoxColumn();
      colElem.DataPropertyName = "NameElement";
      colElem.HeaderText = "Наименование";
      colElem.Name = "NameElement";
      colElem.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
      dgvMainInfo.Columns.Add(colElem);

      DataGridViewColumn colPrice = new DataGridViewTextBoxColumn();
      colPrice.DataPropertyName = "Price";
      colPrice.HeaderText = "Цена";
      colPrice.Name = "Price";
      colPrice.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
      dgvMainInfo.Columns.Add(colPrice);

      DataGridViewColumn colId = new DataGridViewTextBoxColumn();
      colId.DataPropertyName = "Id";
      colId.HeaderText = "Id";
      colId.Name = "Id";
      colId.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
      dgvMainInfo.Columns.Add(colId);

      DataGridViewColumn colUrl = new DataGridViewLinkColumn();
      colUrl.DataPropertyName = "UrlImage";
      colUrl.HeaderText = "Адрес изображения";
      colUrl.Name = "UrlImage";
      colUrl.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
      dgvMainInfo.Columns.Add(colUrl);
      
      sfdExport.Filter = "CSV files(*.csv)|*.csv";
    }

    private void btExport_Click(object sender, EventArgs e)
    {
      if (listElements.Count > 0)
      {
        sfdExport.Title = "Сохранение данных";
        if (sfdExport.ShowDialog() == DialogResult.OK)
        {
          mainFilePath = sfdExport.FileName;
          using (var sw = new StreamWriter(mainFilePath))
          {
            sw.WriteLine("Категория;Подкатегория;Id;Наименование;Цена;Описание;Информация с таблиц;Ссылка на картинку");

            foreach (var element in listElements)
            {
              WriteInFileElement(element, sw);
            }
          }

          MessageBox.Show("Выгрузка выполнена");
        }
      }
    }

    private void dgvMainInfo_CellClick(object sender, DataGridViewCellEventArgs e)
    {
      if (dgvMainInfo.SelectedColumns.Count == 0)
      {
        if (dgvMainInfo.SelectedRows.Count == 0)
        {
          if (dgvMainInfo.SelectedCells.Count == 1)
          {
            var cell = dgvMainInfo.SelectedCells[0];
            if (dgvMainInfo.Columns[cell.ColumnIndex].Name == "UrlImage")
            {
              var url = dgvMainInfo.SelectedCells[0].Value.ToString();
              Process.Start(url);
            }
          }
        }
      }
    }

    private void показатьОписаниеToolStripMenuItem_Click(object sender, EventArgs e)
    {
      if (dgvMainInfo.SelectedColumns.Count == 0)
      {
        if (dgvMainInfo.SelectedRows.Count == 0)
        {
          if (dgvMainInfo.SelectedCells.Count == 1)
          {
            var cell = dgvMainInfo.SelectedCells[0];
            var elem = dgvMainInfo.Rows[cell.RowIndex].DataBoundItem as Element;
            using (var df = new DescriptionForm(elem))
            {
              df.ShowDialog();
            }
            //Process.Start(url);
          }
        }
      }
    }

    private void dgvMainInfo_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
    {
      if (e.Button == MouseButtons.Right)
      {
        // Add this
        dgvMainInfo.CurrentCell = dgvMainInfo.Rows[e.RowIndex].Cells[e.ColumnIndex];
        // Can leave these here - doesn't hurt
        dgvMainInfo.Rows[e.RowIndex].Selected = true;
        dgvMainInfo.Focus();
      }

    }
  }
}
