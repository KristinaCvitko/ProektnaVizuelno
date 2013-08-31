using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using CrackingEggs.Properties;

namespace CrackingEggs
{
    class Game
    {
        /// <summary>
        /// tabela so jajca 
        /// </summary>
        public Egg[,] Table;
        /// <summary>
        /// pozicija na momentalnoto jajce za zamena
        /// </summary>
        private Point currEgg;
        /// <summary>
        /// momentalniot pravec na dvizenje na jajceto
        /// </summary>
        private Egg.Direction currDir;
        /// <summary>
        /// Golemina na tabela so jajca
        /// </summary>
        public Size TableSize { get; set; }
        /// <summary>
        /// Brzina so koja se dvizat jajcata
        /// </summary>
        private int speed;
       /// <summary>
       /// rastojanie na tabelata od ramkite na prozorecot
       /// </summary>
        public int Hgap { get; set; }
        public int Vgap { get; set; }
        /// <summary>
        /// Golemina na edno pole (poleto e kvadrat)
        /// </summary>
        public int FieldDimension { get; set; }
        /// <summary>
        /// broj na nepodvizni blokovi 
        /// </summary>
        private int numBlocks;
        /// <summary>
        /// Metar za merenje na momentalnite poeni (desniot del na ekranot)
        /// </summary>
        public LevelMeter levelMeter;
        bool skip;
        /// <summary>
        /// Lista na elementi koi treba da se izbrisat
        /// </summary>
        List<Size> empties;

        Random random;
        /// <summary>
        /// Polinja od sliki za jajca i jajca bombi
        /// </summary>
        Image[] eggs;
        Image[] bombEggs;
        /// <summary>
        /// Broj na razlicni jajca(po boja)
        /// </summary>
        int colorNum;

        public Game(Size TableSize, int FieldDimension, int Vgap, int Hgap, int LevelMeterCount, int colorNum)
        {
            initEggs();
            numBlocks = TableSize.Width;
            this.TableSize = TableSize;
            this.FieldDimension = FieldDimension;
            this.Vgap = Vgap;
            this.Hgap = Hgap;
            skip = false;
            levelMeter = new LevelMeter(new Size(FieldDimension, FieldDimension * TableSize.Height), new Point(FieldDimension * TableSize.Width + 10, Vgap), LevelMeterCount);
            currEgg = Point.Empty;
            this.colorNum = colorNum;
            random = new Random();
            Table = new Egg[TableSize.Height, TableSize.Width];
            changeSpeed();
        }
        /// <summary>
        /// Inicijalizacija na polinja so Image t.e. sliki za jajcata
        /// </summary>
        private void initEggs()
        {
            eggs = new Image[7];
            eggs[0] = Resources.green;
            eggs[1] = Resources.blue;
            eggs[2] = Resources.red;
            eggs[3] = Resources.tirkiz;
            eggs[4] = Resources.violet;
            eggs[5] = Resources.yellow;
            eggs[6] = Resources.block;

            bombEggs = new Image[7];
            bombEggs[0] = Resources.green_bomb;
            bombEggs[1] = Resources.blue_bomb;
            bombEggs[2] = Resources.red_bomb;
            bombEggs[3] = Resources.tirkiz_bomb;
            bombEggs[4] = Resources.violet_bomb;
            bombEggs[5] = Resources.yellow_bomb;
            bombEggs[6] = Resources.block;
        }

        /// <summary>
        /// Proverka na tabela za ponistuvanje na nekoi jajca
        /// </summary>
        /// <returns>broj na ponisteni jajca</returns>
        public int checkTable()
        {
            //objasneta vo dokumentacijata
            int count = 0;
            List<int> col = new List<int>();
            List<int> row = new List<int>();
            for (int i = 0; i < TableSize.Height; i++)
            {
                Egg lastegg = null;
                int br = 1;
                bool check = false;
                for (int j = 0; j < TableSize.Width; j++)
                {
                    if (lastegg != null && Table[i, j].Equals(lastegg))
                    {
                        br++;
                        if (j == TableSize.Width - 1) check = true;
                    }
                    else
                    {
                        if (br > 2)
                        {
                            count += br;
                            while (br != 0)
                            {
                                row.Add(i);
                                col.Add(j - br);
                                br--;
                            }
                        }
                        br = 1;
                        lastegg = Table[i, j];
                    }
                }

                if (br > 2 && check)
                {
                    count += br;
                    while (br != 0)
                    {
                        row.Add(i);
                        col.Add(TableSize.Width - br);
                        br--;
                    }
                }
            }

            for (int j = 0; j < TableSize.Width; j++)
            {
                Egg lastegg = null;
                int br = 1;
                bool check = false;
                for (int i = 0; i < TableSize.Height; i++)
                {
                    if (lastegg != null && Table[i, j].Equals(lastegg))
                    {
                        br++;
                        if (i == TableSize.Height - 1) check = true;
                    }
                    else
                    {
                        if (br > 2)
                        {
                            count += br;
                            while (br != 0)
                            {
                                bool predict = false;
                                for (int k = 0; k < row.Count; k++)
                                {
                                    if (row.ElementAt(k) == i - br && col.ElementAt(k) == j)
                                    {
                                        predict = true;
                                    }
                                }

                                if (predict)
                                {
                                    newEgg(i - br, j, true);
                                    row.Remove(i - br);
                                    col.Remove(j);
                                }
                                else
                                {
                                    row.Add(i - br);
                                    col.Add(j);
                                }
                                br--;
                            }
                        }
                        br = 1;
                        lastegg = Table[i, j];
                    }
                }
                if (br > 2 && check)
                {
                    count += br;
                    while (br != 0)
                    {
                        bool predict = false;
                        for (int i = 0; i < row.Count; i++)
                        {
                            if (row.ElementAt(i) == TableSize.Height - br && col.ElementAt(i) == j)
                            {
                                predict = true;
                            }
                        }

                        if (predict)
                        {
                            newEgg(TableSize.Height - br, j, true);
                            row.Remove(TableSize.Height - br);
                            col.Remove(j);
                        }
                        else
                        {
                            row.Add(TableSize.Height - br);
                            col.Add(j);
                        }
                        br--;
                    }
                }
            }
            for (int i = 0; i < row.Count; i++)
            {
                int r = row.ElementAt(i);
                int c = col.ElementAt(i);

                if (Table[r, c] != null && Table[r, c].Bomb)
                {
                    if (r > 0)
                    {
                        Table[r - 1, c] = null;
                        count++;
                        if (c > 0)
                        {
                            Table[r - 1, c - 1] = null;
                            Table[r, c - 1] = null;
                            count += 2;
                        }
                        if (c + 1 < TableSize.Width)
                        {
                            Table[r - 1, c + 1] = null;
                            Table[r, c + 1] = null;
                            count += 2;
                        }
                    }

                    if (r + 1 < TableSize.Height)
                    {
                        Table[r + 1, c] = null;
                        count++;
                        if (c > 0)
                        {
                            Table[r + 1, c - 1] = null;
                            count++;
                        }
                        if (c + 1 < TableSize.Width)
                        {
                            Table[r + 1, c + 1] = null;
                            count++;
                        }
                    }
                }
                Table[row.ElementAt(i), col.ElementAt(i)] = null;
            }
            return count;
        }

        private void newEgg(int i, int j, bool bomb)
        {
            Point p = new Point(j * FieldDimension + 5, (i) * FieldDimension + Vgap);
            int rnd = 0;
            if (numBlocks > 0) rnd = random.Next(colorNum);
            else rnd = random.Next(colorNum - 1);
            if (rnd == 6) numBlocks--;
            Table[i, j] = new Egg(getColor(rnd, true), FieldDimension, p, rnd, true,speed);
            Table[i, j].currPosition = p;
        }

        public void changeSpeed()
        {
            //Brzinata se presmetuva otprilika sekoja figura(jajce) da go prejde poleto za 15 edinici vreme(15 X 10ms za 150 mili sekundi)
            speed = FieldDimension / 15;

            foreach (Egg egg in Table)
            {
                if (egg != null) egg.speed = speed;
            }
        }

        /// <summary>
        /// Se pravi resize na site polinja(jajca) i metarot za broj na poeni
        /// </summary>
        /// <param name="size">golemina na pole</param>
        public void resize(int size)
        {
            FieldDimension = size;

            levelMeter.Location = new Point(FieldDimension * TableSize.Width + 10, Vgap);
            levelMeter.size = new Size(size, size * TableSize.Height);

            for (int i = 0; i < TableSize.Height; i++)
            {
                for (int j = 0; j < TableSize.Width; j++)
                {
                    if (Table[i, j] != null)
                    {
                        Table[i, j].size = FieldDimension;
                        Table[i, j].position = new Point(j * FieldDimension + Hgap, i * FieldDimension + Vgap);
                        Table[i, j].currPosition = Table[i, j].position;
                    }

                }
            }
        }
        /// <summary>
        /// Iscruvanje na jajcata i metarot za poeni
        /// </summary>
        /// <param name="g">Graficki objekt od formata</param>
        public void draw(Graphics g)
        {


            g.FillRectangle(new SolidBrush(Color.White), Hgap, Vgap, TableSize.Width * FieldDimension, TableSize.Height * FieldDimension);
            levelMeter.draw(g);

            for (int i = 0; i < TableSize.Height; i++)
            {
                for (int j = 0; j < TableSize.Width; j++)
                {
                    if (Table[i, j] != null) Table[i, j].draw(g);

                }
            }



        }
        /// <summary>
        /// Gi menuva mestata na soodvetnite figuri
        /// </summary>
        /// <param name="direction"> pravec na menuvanje</param>
        /// <returns>true dokolku e vozmozna zamena, inaku false</returns>
        public bool swap(Egg.Direction direction)
        {
            currDir = direction;
            //menuvanje mesta so blok ne e dozvoleno
            if (Table[currEgg.X, currEgg.Y].isBrick()) return false;
            //vo zavisnost od pravecot se menuva pozicijata i mesto vo tabelata
            //currDir se menuva vo obratna nasoka bidejki se zameneti poziciite
            switch (direction)
            {
                case Egg.Direction.Up:
                    if (currEgg.X - 1 >= 0)
                    {
                        if (Table[currEgg.X - 1, currEgg.Y].isBrick()) return false;
                        Egg tmp = Table[currEgg.X, currEgg.Y];
                        Table[currEgg.X, currEgg.Y] = Table[currEgg.X - 1, currEgg.Y];
                        Table[currEgg.X - 1, currEgg.Y] = tmp;

                        int y = Table[currEgg.X, currEgg.Y].position.Y;
                        Table[currEgg.X, currEgg.Y].position = new Point(Table[currEgg.X, currEgg.Y].position.X, Table[currEgg.X - 1, currEgg.Y].position.Y);
                        Table[currEgg.X - 1, currEgg.Y].position = new Point(Table[currEgg.X, currEgg.Y].position.X, y);

                        currDir = Egg.Direction.Down;
                        return true;

                    }
                    break;
                case Egg.Direction.Right:
                    if (currEgg.Y + 1 < TableSize.Width)
                    {
                        if (Table[currEgg.X, currEgg.Y+1].isBrick()) return false;
                        Egg tmp = Table[currEgg.X, currEgg.Y];
                        Table[currEgg.X, currEgg.Y] = Table[currEgg.X, currEgg.Y + 1];
                        Table[currEgg.X, currEgg.Y + 1] = tmp;

                        int x = Table[currEgg.X, currEgg.Y].position.X;
                        Table[currEgg.X, currEgg.Y].position = new Point(Table[currEgg.X, currEgg.Y + 1].position.X, Table[currEgg.X, currEgg.Y].position.Y);
                        Table[currEgg.X, currEgg.Y + 1].position = new Point(x, Table[currEgg.X, currEgg.Y].position.Y);

                        currDir = Egg.Direction.Left;
                        return true;
                    }
                    break;
                case Egg.Direction.Left:
                    if (currEgg.Y - 1 >= 0)
                    {
                        if (Table[currEgg.X, currEgg.Y-1].isBrick()) return false;
                        Egg tmp = Table[currEgg.X, currEgg.Y];
                        Table[currEgg.X, currEgg.Y] = Table[currEgg.X, currEgg.Y - 1];
                        Table[currEgg.X, currEgg.Y - 1] = tmp;

                        int x = Table[currEgg.X, currEgg.Y].position.X;
                        Table[currEgg.X, currEgg.Y].position = new Point(Table[currEgg.X, currEgg.Y - 1].position.X, Table[currEgg.X, currEgg.Y].position.Y);
                        Table[currEgg.X, currEgg.Y - 1].position = new Point(x, Table[currEgg.X, currEgg.Y].position.Y);

                        currDir = Egg.Direction.Right;
                        return true;
                    }
                    break;
                case Egg.Direction.Down:
                    if (currEgg.X + 1 < TableSize.Height)
                    {
                        if (Table[currEgg.X+1, currEgg.Y].isBrick()) return false;
                        Egg tmp = Table[currEgg.X, currEgg.Y];
                        Table[currEgg.X, currEgg.Y] = Table[currEgg.X + 1, currEgg.Y];
                        Table[currEgg.X + 1, currEgg.Y] = tmp;

                        int y = Table[currEgg.X, currEgg.Y].position.Y;
                        Table[currEgg.X, currEgg.Y].position = new Point(Table[currEgg.X, currEgg.Y].position.X, Table[currEgg.X + 1, currEgg.Y].position.Y);
                        Table[currEgg.X + 1, currEgg.Y].position = new Point(Table[currEgg.X, currEgg.Y].position.X, y);

                        currDir = Egg.Direction.Up;
                        return true;
                    }
                    break;
            }
            return false;
        }
        /// <summary>
        /// dokolku ne bil vcaliden potezot figurite treba da si se vratat na mesto
        /// </summary>
        /// <returns>sekogas true</returns>
        public bool reSwap()
        {
            //dokolku ima nevalkiden poteg se vrakaat figurite
            switch (currDir)
            {
                case Egg.Direction.Up:
                    return swap(Egg.Direction.Down);
                case Egg.Direction.Right:
                    return swap(Egg.Direction.Left);
                case Egg.Direction.Left:
                    return swap(Egg.Direction.Right);
                case Egg.Direction.Down:
                    return swap(Egg.Direction.Up);
            }
            return true;
        }
        /// <summary>
        /// se zacuvuva momentalnoto jajce t.e. pole na koe e kliknato
        /// </summary>
        /// <param name="p">tocka kade e kliknato</param>
        /// <returns>true dokolku e kliknato vo nekoe pole</returns>
        public bool hit(Point p)
        {
            
            int x = (p.X - Hgap) / FieldDimension;
            int y = (p.Y - Vgap) / FieldDimension;
            if (x < TableSize.Width && y < TableSize.Height)
            {
                currEgg = new Point(y, x);
                return true;
            }

            return false;
        }
        /// <summary>
        /// funkcija koja ja vraka soodvetnata slika (boja na jajceto)
        /// </summary>
        /// <param name="i">random broj od 0 do 6</param>
        /// <param name="bomb">true ako e bomba inaku false</param>
        /// <returns>Image na soodvetnoto jajce</returns>
        Image getColor(int i, bool bomb)
        {
            if (bomb)
            {
                return bombEggs[i];
            }
            return eggs[i];
        }
        /// <summary>
        /// funkcija so koja se spustaat jajcata odozgora i se inicijaliziraat novi
        /// </summary>
        /// <returns>true se dodeka se spustaat jajcata, na krajot false</returns>
        public bool drop()
        {
            //vo nekoja iteracija se preskoknuva prviot del
            if (!skip)
            {
                empties = new List<Size>();
                //se baraat prazni polinja
                for (int i = 0; i < TableSize.Width; i++)
                {
                    for (int j = 0; j < TableSize.Height; j++)
                    {
                        if (Table[j, i] == null)
                        {
                            empties.Add(new Size(i, j));
                            break;
                        }
                    }
                }
                //dokolku nema prazni polinja ne padjaat figuri
                if (empties.Count == 0)
                {
                    return false;
                }
                //pogornite figuri gi zazimaat mestata na praznite i se inicijaliziraat novi
                for (int i = 0; i < empties.Count; i++)
                {
                    int col = empties.ElementAt(i).Width;
                    int row = empties.ElementAt(i).Height;

                    while (row != 0)
                    {
                        Table[row, col] = Table[row - 1, col];
                        int y = Table[row, col].position.Y + FieldDimension;
                        int x = Table[row, col].position.X;
                        Table[row, col].position = new Point(x, y);
                        row--;
                    }
                    int rnd = 0;
                    if (numBlocks > 0)
                    {
                        rnd = random.Next(colorNum);
                    }
                    else
                    {
                        rnd = random.Next(colorNum-1);
                    }
                    if (rnd == 6) numBlocks--;
                    Table[0, col] = new Egg(getColor(rnd, false), FieldDimension, new Point(col * FieldDimension + 5, Vgap), rnd, false,speed);

                }
                //vo slednata iteracija ke se preskokne ovoj del
                skip = true;
            }

            for (int i = 0; i < empties.Count; i++)
            {
                int col = empties.ElementAt(i).Width;
                int row = empties.ElementAt(i).Height;

                while (row >= 0)
                {
                    //koga ke prestanat da padjaat vo slednata iteracija pak se proveruva za prazni polinja
                    if (!Table[row, col].move(Egg.Direction.Down))
                    {
                        skip = false;
                    }
                    row--;
                }
            }
            return true;
        }
        /// <summary>
        /// smenetite figuri vo swap() sega se pomestuvaat soodvetno
        /// </summary>
        /// <returns>true dodeka trae pomestuvanjeto</returns>
        public bool move()
        {
            //se pomestuvaat soodvetnite jajca
            switch (currDir)
            {
                case Egg.Direction.Up:
                    Table[currEgg.X + 1, currEgg.Y].move(Egg.Direction.Down);
                    break;
                case Egg.Direction.Right:
                    Table[currEgg.X, currEgg.Y - 1].move(Egg.Direction.Left);
                    break;
                case Egg.Direction.Left:
                    Table[currEgg.X, currEgg.Y + 1].move(Egg.Direction.Right);
                    break;
                case Egg.Direction.Down:
                    Table[currEgg.X - 1, currEgg.Y].move(Egg.Direction.Up);
                    break;
            }
            if (!Table[currEgg.X, currEgg.Y].move(currDir))
            {
                return false;
            }
            return true;
        }
        /// <summary>
        /// Kraj na igrata(t.e. levelot)
        /// </summary>
        /// <returns>true dokolku e kraj na levelot</returns>
        public bool gameOver()
        {
            return levelMeter.gameover();
        }
    }
}
