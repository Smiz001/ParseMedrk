using System;
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
    private void btChooseSaveFile_Click(object sender, EventArgs e)
    {
      using (var sw = new StreamWriter(@"D:\Medr.csv"))
      {
        sw.WriteLine("Категория;Подкатегория;Id;Наименование;Цена;Описание;Ссылка на картинку");
      }
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
        ParseAll(collections);

      MessageBox.Show("Completed");
    }

    private void ParseAll(IHtmlCollection<IElement> collections)
    {        
      var random = new Random();
   
      foreach (var element in collections)
      {
        var href = element.GetElementsByClassName("hidden-xs")[0].GetAttribute("href").Replace("/shop/","");
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

        if(countPageElements.Length > 2)
        {
          countPage = countPageElements[0].GetElementsByTagName("LI").Length - 1;
        }

        for (int i = 1; i <= countPage; i++)
        {
          Thread.Sleep(random.Next(1000,3000));
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

      var image = document.GetElementsByClassName("lb-image");
      if(image.Length>0)
      {
        elem.UrlImage = image[0].GetAttribute("src");
      }

      WriteInFileElement(elem);
    }

    private void WriteInFileElement(Element elem)
    {
      using (var sw = new StreamWriter(new FileStream(@"D:\Medr.csv", FileMode.Open), Encoding.UTF8))
      {
        sw.BaseStream.Position = sw.BaseStream.Length;
        sw.WriteLine($@"'{elem.NameCategory}';'{elem.NameSubCategory}';'{elem.Id}';'{elem.NameElement}';'{elem.Price}';'{elem.Description.Replace("'","")}';'{elem.UrlImage}'");
      }
    }
  }
}
