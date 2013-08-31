using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using CrackingEggs.Properties;

namespace CrackingEggs
{
    class LevelMeter
    {
        /// <summary>
        /// Golemina na objektot
        /// </summary>
        public Size size { get; set; }
        /// <summary>
        /// Lokacija kade se naodja
        /// </summary>
        public Point Location { get; set; }
        /// <summary>
        /// Vkupniot broj na poeni
        /// </summary>
        public int Count { get; set; }
        /// <summary>
        /// Momentalniot broj na poeni
        /// </summary>
        public int currentlevel { get; set; }

        public LevelMeter(Size size, Point location, int Count)
        {
            this.size = size;
            this.Location = location;
            this.Count = Count;
            currentlevel = 0;

        }
        /// <summary>
        /// Metoda za iscrtuvanje
        /// </summary>
        /// <param name="g">Graficki objekt na formata</param>
        public void draw(Graphics g)
        {
            Rectangle r=new Rectangle(Location, size);
            g.DrawImage(Resources.meter, r);
            r.Height =size.Height- size.Height * currentlevel / Count;
            g.FillRectangle(new SolidBrush(Color.White), r);
        }
        /// <summary>
        /// Metoda za kraj na igrata
        /// </summary>
        /// <returns>true dokolku e kraj</returns>
        public bool gameover()
        {
            return Count <= currentlevel;
        }
    }
}
