using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using AngleSharp.Dom;
using AngleSharp.Parser.Html;
using ParseMedrk.Export;

namespace ParseMedrk
{
  public partial class MainForm : Form
  {
    public MainForm()
    {
      InitializeComponent();
    }

    private string mainLink = @"http://www.medrk.ru/shop/";
    private string mainFilePath;
    private static object locker = new object();
    private BindingList<Element> listElements = new BindingList<Element>();
    private delegate void AddToListDelegate(BindingList<Element> list, Element elem);

    private void AddList(BindingList<Element> list, Element item)
    {
      if (InvokeRequired)
      {
        Invoke(new AddToListDelegate(AddList), new object[] { list, item });
        return;
      }
      list.Add(item);
    }

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
        //ParseAll(collections);
        for (int i = 0; i < collections.Length; i++)
        {
          Thread thread = new Thread(ParseAll);
          thread.Start(collections[i]);

        }
        Cursor.Current = Cursors.Default;
      }
      dgvMainInfo.DataSource = listElements;
    }

    private void ParseAll(object element)
    {
      var elem = element as IElement;
      var random = new Random();
      var href = elem.GetElementsByClassName("hidden-xs")[0].GetAttribute("href").Replace("/shop/", "");
      var nameCategory = elem.GetElementsByClassName("img-responsive")[0].GetAttribute("alt");
      var ul = elem.GetElementsByClassName("catalog-parts hidden-xs");

      if (ul.Length > 0)
      {
        // thread = new Thread()
        ParseCategory($"{mainLink}{href}", nameCategory);
      }
      else
      {
        ParseSubCategory($"{mainLink}{href}&kol_page=25", nameCategory, "");
      }
      MessageBox.Show($"Загрузка выполнена - {element}");
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
      Thread.Sleep(random.Next(1000,2000));
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
          countPage = countPageElements.Length - 1;
        }

        for (int i = 1; i <= countPage; i++)
        {
          Thread.Sleep(random.Next(1000, 1500));
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
              elem.Price = regex.Match(elem.Price).Value;
              elem.Price = elem.Price.Replace("₽", "").Replace(".", ",");
            }
            else if (priceGreen.Length > 0)
            {
              elem.Price = priceGreen[0].TextContent.Replace(" ", "");
              var regex = new Regex(@"(\d+\.\d+₽)");
              elem.Price = regex.Match(elem.Price).Value;
              elem.Price = elem.Price.Replace("₽","").Replace(".", ",");
            }
            else if (priceRed.Length > 0)
            {
              elem.Price = "1";
            }
            else
              elem.Price = "1";

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
      if(about.Children.Count() == 1)
      {
        if(about.Children[0].TagName == "DIV")
        {
          var chldsDiv = about.Children[0].Children;
          foreach (var ch in chldsDiv)
          {
            if (ch.TagName == "P" || ch.TagName == "SPAN")
              elem.Description += ch.TextContent;
          }
          if(string.IsNullOrWhiteSpace(elem.Description))
          {
            elem.Description = about.Children[0].TextContent;
          }
        }
        else if (about.Children[0].TagName == "P")
        {
          elem.Description += about.Children[0].TextContent;
        }
      }
      else if (about.Children.Count() == 0)
        elem.Description = about.TextContent;
      else
      {
        var childs = about.Children;
        foreach (var p in childs)
        {
          if (p.TagName == "P" || p.TagName == "SPAN")
            elem.Description += p.TextContent;
        }
      }

      if (string.IsNullOrWhiteSpace(elem.Description))
      {
        var childs = about.Children;
        foreach (var ch in childs)
        {
          if (ch.TagName == "DIV")
          {
            ParseDivDescription(elem, ch);
          }
        }
      }

      if (string.IsNullOrWhiteSpace(elem.Description))
      {
        var childs = about.Children;
        foreach (var ch in childs)
        {
          if (ch.TagName == "P")
          {
            elem.Description += ch.TextContent;
          }
        }
      }

      var image = document.GetElementsByClassName("col-md-3 col-sm-3 text-center hidden-xs");
      for (int i = 0; i < image.Length; i++)
      {
        var a = image[i].GetElementsByTagName("A");
        if (a.Length >0 )
        {
          if(i == image.Length-1)
            elem.UrlImage += a[0].GetAttribute("href");
          else
            elem.UrlImage += a[0].GetAttribute("href")+";";
        }
      }

      var table = document.GetElementsByTagName("TABLE");

      for (int i = 0; i < table.Length; i++)
      {
        if (elem.Characteristics.Count == 0)
        {
          var chldsTable = table[i].GetElementsByTagName("TABLE");
          if (chldsTable.Length > 0)
          {
            for (int j = 0; j < chldsTable.Length; j++)
            {
              if (chldsTable[j].TextContent.Contains("Наименование") || chldsTable[j].TextContent.Contains("Название"))
              {
                ParseTable(elem, table[i]);
                break;
              }
            }
          }
          else if (table[i].TextContent.Contains("Наименование") || table[i].TextContent.Contains("Название"))
          {
            ParseTable(elem, table[i]);
          }
        }
        else
          break;
      
      }
      //WriteInFileElement(elem);
      elem.Description = elem.Description.Replace("  ", "").Replace("\n", "");

      Monitor.Enter(locker);
      AddList(listElements, elem);
      //listElements.Add(elem);
      Monitor.Exit(locker);
    }

    private void ParseDivDescription(Element element, IElement divElement)
    {
      var divCh = divElement.Children;
      foreach (var ch in divCh)
      {
        if (ch.TagName == "P" || ch.TagName == "SPAN" || ch.TagName == "UL")
          element.Description += ch.TextContent;
      }
    }

    private void WriteInFileElement(Element elem, StreamWriter sw)
    {
      sw.BaseStream.Position = sw.BaseStream.Length;
      sw.WriteLine($@"'{elem.NameCategory}';'{elem.NameSubCategory}';'{elem.Id}';'{elem.NameElement}';'{elem.Price}';'{elem.Description.Replace("'", "")}';'{elem.InfoFromTable.Replace("'", "")}';'{elem.UrlImage}'");
    }

    private void ParseTable(Element elem, IElement htmlElement)
    {
      var trs = htmlElement.GetElementsByTagName("TR");

      for (int i = 1; i < trs.Length; i++)
      {
        var tds = trs[i].GetElementsByTagName("td");
        if (tds.Length >= 2)
        {
          if (tds[0].TextContent != "Наименование" && !tds[0].TextContent.Contains("Таблица 1"))
          {
            elem.Characteristics.Add(new Characteristic { Name = tds[0].TextContent, Value = tds[1].TextContent });
          }
        }
      }
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
      colSubCat.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
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

      DataGridViewColumn colUrl = new DataGridViewTextBoxColumn();
      colUrl.DataPropertyName = "UrlImage";
      colUrl.HeaderText = "Адрес изображения";
      colUrl.Name = "UrlImage";
      colUrl.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
      dgvMainInfo.Columns.Add(colUrl);
      
      sfdExport.Filter = "xlsx files(*.xlsx)|*.xlsx";
    }

    private void btExport_Click(object sender, EventArgs e)
    {
      if (listElements.Count > 0)
      {
        sfdExport.Title = "Сохранение данных";
        if (sfdExport.ShowDialog() == DialogResult.OK)
        {
          mainFilePath = sfdExport.FileName;
          var export = new Excel(listElements.ToList(), mainFilePath);
          export.ExecuteExport();
          //using (var sw = new StreamWriter(mainFilePath))
          //{
          //  sw.WriteLine("Категория;Подкатегория;Id;Наименование;Цена;Описание;Информация с таблиц;Ссылка на картинку");

          //  foreach (var element in listElements)
          //  {
          //    WriteInFileElement(element, sw);
          //  }
          //}
          MessageBox.Show("Выгрузка выполнена");
        }
      }
    }

    private void dgvMainInfo_CellClick(object sender, DataGridViewCellEventArgs e)
    {
      //if (dgvMainInfo.SelectedColumns.Count == 0)
      //{
      //  if (dgvMainInfo.SelectedRows.Count == 0)
      //  {
      //    if (dgvMainInfo.SelectedCells.Count == 1)
      //    {
      //      var cell = dgvMainInfo.SelectedCells[0];
      //      if (dgvMainInfo.Columns[cell.ColumnIndex].Name == "UrlImage")
      //      {
      //        var url = dgvMainInfo.SelectedCells[0].Value.ToString();
      //        Process.Start(url);
      //      }
      //    }
      //  }
      //}
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

    private void показатьХарактеристикуToolStripMenuItem_Click(object sender, EventArgs e)
    {

    }
  }
}
