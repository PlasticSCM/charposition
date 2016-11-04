using System.Drawing;
using System.Windows.Forms;

namespace charposition
{
    public partial class Form1 : Form
    {
        public Form1(string title, string text)
        {
            InitializeComponent();

            this.Text = title;

            Bitmap bmp = RenderFile.Draw(text);

            pictureBox1.Width = bmp.Width;
            pictureBox1.Height = bmp.Height;
            pictureBox1.Left = 0;
            pictureBox1.Top = 0;
            pictureBox1.Image = bmp;
        }
    }
}
