using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace charposition
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            Bitmap bmp = this.Draw(
@"class Socket
{
   void Connect(string server)
   {
      SocketLibrary.Connect(mSocket, server);
   }

   void Disconnect()
   {
      SocketLibrary.Disconnect(mSocket);
   }
}");

            pictureBox1.Width = bmp.Width;
            pictureBox1.Height = bmp.Height;
            pictureBox1.Left = 0;
            pictureBox1.Top = 0;
            pictureBox1.Image = bmp;
        }

        Size GetTextSizeInLinesAndCols(string text)
        {
            int lines = 1;
            int maxCols = 0;

            int lineWidth = 0;

            for (int i = 0; i < text.Length; ++i)
            {
                ++lineWidth;

                if (text[i] == '\n')
                {
                    ++lines;

                    if (maxCols < lineWidth)
                        maxCols = lineWidth;

                    lineWidth = 0;
                }
            }

            return new Size(maxCols, lines);
        }

        void RenderLineAndColumnNumbers(
            Graphics g,
            int lines,
            int columns,
            int x_space,
            int x_initialMargin,
            int y_space,
            int y_initialMargin)
        {
            Font numberFont = new Font("Consolas", 10.0f);

            var brush = new SolidBrush(Color.Gray);

            int x = x_initialMargin;
            int y = 10;

            for (int i = 0; i < columns; ++i)
            {
                Size numberTextSize = TextRenderer.MeasureText(i.ToString(), numberFont);

                g.DrawString(i.ToString(), numberFont, brush,
                    new PointF(
                        x + x_space / 2 - (numberTextSize.Width / 2),
                        y)
                );

                x += x_space;
            }

            x = 5;
            y = y_initialMargin;

            for (int i = 0; i < lines; ++i)
            {
                Size numberTextSize = TextRenderer.MeasureText(i.ToString(), numberFont);

                g.DrawString(i.ToString(), numberFont, brush,
                    new PointF(
                        x,
                        y + y_space / 2 - (numberTextSize.Height / 2))
                );

                y += y_space;
            }
        }

        Bitmap Draw(string text)
        {
            Size maxTextSize = GetTextSizeInLinesAndCols(text);

            Font codeFont = new Font("Consolas", 20.0f);
            Size codeTextSize = TextRenderer.MeasureText("Z", codeFont);

            int x_space = codeTextSize.Width + 2;
            int y_space = codeTextSize.Height + 2;

            int x_initialMargin = x_space;
            int y_initialMargin = y_space;

            var bitmap = new Bitmap(
                maxTextSize.Width * x_space + x_initialMargin,
                maxTextSize.Height * y_space + y_initialMargin,
                PixelFormat.Format32bppArgb);

            var g = Graphics.FromImage(bitmap);

            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.AntiAlias;

            RenderLineAndColumnNumbers(
                g, maxTextSize.Height, maxTextSize.Width,
                x_space, x_initialMargin, y_space, y_initialMargin);

            Font numberFont = new Font("Consolas", 6.0f);
            Font eolFont = new Font("Consolas", 15.0f);

            var eolBrush = new SolidBrush(Color.Gray);

            var brush = new SolidBrush(Color.Navy);
            var pen = new Pen(brush);

            var linePen = new Pen(new SolidBrush(Color.Gray));

            Size numberTextSize = TextRenderer.MeasureText("Z", numberFont);

            int x = x_initialMargin;
            int y = y_initialMargin;

            for (int pos = 0; pos < text.Length; ++pos)
            {
                string charToDraw = text[pos].ToString();

                Font currentFont = codeFont;
                Brush currentBrush = brush;

                if (text[pos] == '\r')
                {
                    charToDraw = "\\r";
                    currentFont = eolFont;
                    currentBrush = eolBrush;
                }

                if (text[pos] == '\n')
                {
                    charToDraw = "\\n";
                    currentFont = eolFont;
                    currentBrush = eolBrush;
                }

                g.DrawString(charToDraw, currentFont, currentBrush, new PointF(x, y));

                g.DrawRectangle(linePen, new Rectangle(x + 1, y + 1, codeTextSize.Width - 1, codeTextSize.Height - 1));

                string posString = pos.ToString();

                g.DrawString(posString, numberFont, eolBrush,
                    new PointF(
                        x + x_space - TextRenderer.MeasureText(posString, numberFont).Width,
                        y + codeTextSize.Height-numberTextSize.Height));

                x += x_space;

                if (text[pos] == '\n')
                {
                    y += y_space;
                    x = x_initialMargin;
                }
            }

            return bitmap;
        }
    }
}
