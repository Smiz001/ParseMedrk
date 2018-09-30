using System.Windows.Forms;

namespace ParseMedrk
{
  public partial class DescriptionForm : Form
  {
    public DescriptionForm(Element elem)
    {
      InitializeComponent();
      this.Text = $"Опиание {elem.NameElement}";
      tbDescription.Text = elem.Description;
    }
  }
}
