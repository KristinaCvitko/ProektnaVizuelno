using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace CrackingEggs
{
    class Egg
    {
        /// <summary>
        /// golemina na jajceto
        /// </summary>
        public int size { get; set; }
        /// <summary>
        /// Pozicijata (krajna) na polozbata na jajceto
        /// </summary>
        public Point position { get; set; }
        /// <summary>
        /// medjusebno rastojanie so drugite jajca
        /// </summary>
        public int Gap { get; set; }
        /// <summary>
        /// Momentalnata pozicija na jajceto
        /// </summary>
        public Point currPosition { get; set; }
        /// <summary>
        /// Brzina so koe se dvizi
        /// </summary>
        public int speed { get; set; }
        /// <summary>
        /// Izgledot na jajceto
        /// </summary>
        public readonly Image EggImg;
        /// <summary>
        /// Id za sporedba so drugi (od 0 do 6)
        /// </summary>
        public readonly int Id;
        /// <summary>
        /// Dali e bomba
        /// </summary>
        public bool Bomb { get; set; }
        /// <summary>
        /// Nasoki na dvizenje
        /// </summary>
        public enum Direction
        {
            Up,
            Right,
            Left,
            Down
        };

        public Egg(Image EggImg, int size, Point position,int Id,bool bomb,int speed)
        {
            this.EggImg = EggImg;
            this.size = size;
            this.position = position;
            this.currPosition = new Point(position.X, position.Y - size);
            this.Id = Id;
            Bomb = bomb;
            Gap = 3;
            this.speed = speed;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>true ako e nepodvizen blok</returns>
        public bool isBrick()
        {
            return Id == 6;
        }

        /// <summary>
        /// Metoda za pomestuvanje na jajceto
        /// </summary>
        /// <param name="dir">Nasoka na pomestuvanje</param>
        /// <returns>true ako se dvizi jajceto</returns>
        public bool move(Direction dir)
        {
            //dokolku momentalnata pozicija i pozicijata kade treba da e elementot se sovpadjaat
            //se vraka false za zavrseno pomestuvanje
            if (currPosition.Equals(position))
            {
                return false;
            }
            //vo zavisnost od pravecot se menuvaat soodvetnite koordinati
            if (dir == Direction.Up)
            {
                currPosition = new Point(currPosition.X, currPosition.Y - speed);
            }
            else if (dir == Direction.Down)
            {
                currPosition = new Point(currPosition.X, currPosition.Y + speed);
            }
            else if (dir == Direction.Right)
            {
                currPosition = new Point(currPosition.X+speed, currPosition.Y);
            }
            else if (dir == Direction.Left)
            {
                currPosition = new Point(currPosition.X-speed, currPosition.Y);
            }

            //Se ogranicuvaat figurite vo naredna iteracija da ne prejdat podaleku od potrebno
            if (Math.Abs(currPosition.X - position.X) <= speed && Math.Abs(currPosition.X - position.X)!=0)
            {
                currPosition = position;
            }
            if (Math.Abs(currPosition.Y - position.Y) <= speed && Math.Abs(currPosition.Y - position.Y) != 0)
            {
                currPosition = position;
            }
            return true;
        }
        /// <summary>
        /// Metoda za iscrtuvanje
        /// </summary>
        /// <param name="g">Graficki objekt na formata</param>
        public void draw(Graphics g)
        {
            g.DrawImage(EggImg, currPosition.X + Gap, currPosition.Y + Gap, size - Gap, size - Gap);
        }

        

        // override object.Equals
        public override bool Equals(object obj)
        {
            //se sporeduva spored id
            //dokolku id-to e 6 togas stanuva zbor za blok
            if (Id == 6 || (obj as Egg).Id == 6) return false;
            return Id == (obj as Egg).Id;
        }

    }
}
