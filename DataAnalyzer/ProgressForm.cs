using System;
using System.Windows.Forms;

namespace YourNamespace
{
    public partial class ProgressForm : Form
    {
        public ProgressForm(int maxValue)
        {
            InitializeComponent();
            progressBar.Minimum = 0;
            progressBar.Maximum = maxValue;
        }

        public void UpdateProgress(int value)
        {
            progressBar.Value = value;
        }
    }
}
