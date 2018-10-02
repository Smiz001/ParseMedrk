using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ParseMedrk
{
  public partial class СharacteristicForm : Form
  {
    private Element element;
    public СharacteristicForm(Element elem)
    {
      InitializeComponent();
      this.element = elem;

      var bindList = new BindingList<Characteristic>(elem.Characteristics);
      dgvCharacteristic.DataSource = bindList;
    }

    private void СharacteristicForm_Load(object sender, EventArgs e)
    {
      dgvCharacteristic.AutoGenerateColumns = false;

      DataGridViewColumn colName = new DataGridViewTextBoxColumn();
      colName.DataPropertyName = "Name";
      colName.HeaderText = "Наименование";
      colName.Name = "Name";
      colName.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
      dgvCharacteristic.Columns.Add(colName);

      DataGridViewColumn colValue = new DataGridViewTextBoxColumn();
      colValue.DataPropertyName = "Value";
      colValue.HeaderText = "Значение";
      colValue.Name = "Value";
      colValue.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
      dgvCharacteristic.Columns.Add(colValue);
    }
  }
}
