//using System;
//using System.Collections.Generic;
//using System.Drawing;
//using System.Drawing.Drawing2D;
//using System.Drawing.Imaging;
//using System.Drawing.Text;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows.Forms;

//namespace charposition
//{
//    static class RenderFile
//    {
//        static internal Bitmap Draw(string text)
//        {
//            Size maxTextSize = GetTextSizeInLinesAndCols(text);

//            Font codeFont = new Font("Consolas", 14.0f);
//            Size codeTextSize = TextRenderer.MeasureText("Z", codeFont);

//            int xSpace = codeTextSize.Width + 2;
//            int ySpace = codeTextSize.Height + 10;

//            int xInitialMargin = xSpace;
//            int yInitialMargin = ySpace;

//            var bitmap = new Bitmap(
//                maxTextSize.Width * xSpace + xInitialMargin +1 ,
//                maxTextSize.Height * ySpace + yInitialMargin + 1,
//                PixelFormat.Format32bppArgb);

//            var g = Graphics.FromImage(bitmap);

//            g.SmoothingMode = SmoothingMode.AntiAlias;
//            g.TextRenderingHint = TextRenderingHint.AntiAlias;

//            RenderNumbers numbers = new RenderNumbers();
//            numbers.RenderColumns(g, maxTextSize.Width, xSpace, xInitialMargin);
//            numbers.RenderLines(g, maxTextSize.Height, ySpace, yInitialMargin);

//            Font numberFont = new Font("Consolas", 7.0f);
//            Font eolFont = new Font("Consolas", 11.0f);

//            var eolBrush = new SolidBrush(Color.Gray);

//            var brush = new SolidBrush(Color.Navy);
//            var pen = new Pen(brush);

//            var linePen = new Pen(new SolidBrush(Color.Gray));

//            Size numberTextSize = TextRenderer.MeasureText("Z", numberFont);

//            int x = xInitialMargin;
//            int y = yInitialMargin;

//            for (int pos = 0; pos < text.Length; ++pos)
//            {
//                string charToDraw = text[pos].ToString();

//                Font currentFont = codeFont;
//                Brush currentBrush = brush;

//                if (text[pos] == '\r')
//                {
//                    charToDraw = "\\r";
//                    currentFont = eolFont;
//                    currentBrush = eolBrush;
//                }

//                if (text[pos] == '\n')
//                {
//                    charToDraw = "\\n";
//                    currentFont = eolFont;
//                    currentBrush = eolBrush;
//                }

//                g.DrawString(charToDraw, currentFont, currentBrush, new PointF(x, y));

//                g.DrawRectangle(linePen, new Rectangle(
//                    x, y,
//                    xSpace,
//                    ySpace));

//                string posString = pos.ToString();

//                g.DrawString(posString, numberFont, eolBrush,
//                    new PointF(
//                        x + xSpace - TextRenderer.MeasureText(posString, numberFont).Width,
//                        y + ySpace - numberTextSize.Height));

//                x += xSpace;

//                if (text[pos] == '\n')
//                {
//                    y += ySpace;
//                    x = xInitialMargin;
//                }
//            }

//            return bitmap;
//        }

//        static Size GetTextSizeInLinesAndCols(string text)
//        {
//            int lines = 1;
//            int maxCols = 0;

//            int lineWidth = 0;

//            for (int i = 0; i < text.Length; ++i)
//            {
//                ++lineWidth;

//                if (text[i] == '\n')
//                {
//                    ++lines;

//                    if (maxCols < lineWidth)
//                        maxCols = lineWidth;

//                    lineWidth = 0;
//                }
//            }

//            return new Size(maxCols, lines);
//        }

//        class RenderNumbers
//        {
//            internal void RenderColumns(
//                Graphics g,
//                int columns,
//                int xSpace,
//                int xInitialMargin)
//            {
//                int x = xInitialMargin;
//                int y = 10;

//                for (int i = 1; i <= columns; ++i)
//                {
//                    Size numberTextSize = TextRenderer.MeasureText(i.ToString(), mNumberFont);

//                    g.DrawString(i.ToString(), mNumberFont, mGrayBrush,
//                        new PointF(
//                            x + xSpace / 2 - (numberTextSize.Width / 2),
//                            y)
//                    );

//                    x += xSpace;
//                }
//            }

//            internal void RenderLines(
//                Graphics g,
//                int lines,
//                int ySpace,
//                int yInitialMargin)
//            {
//                int x = 5;
//                int y = yInitialMargin;

//                for (int i = 1; i <= lines; ++i)
//                {
//                    Size numberTextSize = TextRenderer.MeasureText(i.ToString(), mNumberFont);

//                    g.DrawString(i.ToString(), mNumberFont, mGrayBrush,
//                        new PointF(
//                            x,
//                            y + ySpace / 2 - (numberTextSize.Height / 2))
//                    );

//                    y += ySpace;
//                }
//            }

//            Font mNumberFont = new Font("Consolas", 9.0f);
//            Brush mGrayBrush = new SolidBrush(Color.Gray);
//        }
//    }
//}
