using System.Drawing;

namespace Figures
{
    public class Ellipse : Figure
    {
        public Ellipse(string name, int id, int x, int y, int r1, int r2) : base(id, x, y)
        {
            this.name = name;
            w = r1 * 2;
            h = r2 * 2;
        }

        public override void Draw()
        {
            Graphics g = Graphics.FromImage(Init.bitmap);
            Init.pen.Color = this.color;
            g.DrawEllipse(Init.pen, x, y, w, h);
            Init.pictureBox.Image = Init.bitmap;
        }
    }
}
