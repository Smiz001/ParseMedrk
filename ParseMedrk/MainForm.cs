using System;
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
        {
          ParseSubCategory($"{mainLink}{href}&kol_page=100/", nameCategory,"");
        }
      }
    }

    private void ParseCategory(string urlCategory, string nameCategory)
    {

    }

    private void ParseSubCategory(string urlSubCategory, string nameCategory, string nameSubCategory)
    {
      using (var webClient = new WebClient())
      {
        ServicePointManager.Expect100Continue = true;
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
        ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
        webClient.Encoding = Encoding.UTF8;
        var responce = webClient.DownloadString(urlSubCategory);

        var parser = new HtmlParser();
        var document = parser.Parse(responce);

        var collectionItems = document.GetElementsByClassName("row catalog-product hidden-xs");
        foreach (var element in collectionItems)
        {
          var s16 = element.GetElementsByClassName("s16");
          var price = element.GetElementsByClassName("price-gray");
          string nameElment = string.Empty;
          string priceElement = string.Empty;
          if (s16.Length > 0)
            nameElment = s16[0].TextContent;
          if (price.Length > 0)
          {
            priceElement = price[0].TextContent.Replace(" ", "");
            var regex = new Regex(@"(\d+\.\d+₽)");
            priceElement = regex.Match(priceElement).Value;
          }
        }
      }
    }

    private void WriteInFileElement()
    {

    }
  }
}
