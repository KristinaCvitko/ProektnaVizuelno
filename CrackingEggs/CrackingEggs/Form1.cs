using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Media;
using CrackingEggs.Properties;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace CrackingEggs
{
    public partial class Form1 : Form
    {
        Game game;
        /// <summary>
        /// Golemina na tabela vo polinja
        /// </summary>
        public Size TableSize { get; set; }
        /// <summary>
        /// Vkupno poeni potrebni za eden level
        /// </summary>
        public int MeterCount { get; set; }

        bool mouseDown, clicking, swaped;

        public Point firstClick { get; set; }

        int Time, elapsed, colorNum, totalP;

        int Level;
        public Form1()
        {
            InitializeComponent();
            TableSize = new Size(4, 3);
            MeterCount = 20;
            totalP = 0;
            Time = 20;
            colorNum = 2;
            Level = 0;
            newGame();

        }
        /// <summary>
        /// Inicijalizacija na nova igra kade se zgolemuva tabelata
        /// i potrebni poeni za pominuvanje na level
        /// </summary>
        void newGame()
        {
            Time += 5;
            elapsed = 0;

            Level++;
            lbLevel.Text = Level.ToString();
            if (colorNum < 7) colorNum++;

            mouseDown = false;
            clicking = false;
            swaped = false;
            firstClick = Point.Empty;
            if (TableSize.Width <= TableSize.Height)
            {
                TableSize = new Size(TableSize.Width + 1, TableSize.Height);
            }
            else
            {
                TableSize = new Size(TableSize.Width, TableSize.Height + 1);
            }

            int height = (this.Height - 2 * menuStrip1.Height - 5) / TableSize.Height;
            int width = (this.Width - 20) / (TableSize.Width + 1);
            int fsize;
            if (height < width)
                fsize = height;
            else fsize = width;
            MeterCount += 5;
            game = new Game(TableSize, fsize, menuStrip1.Height + 5, 5, MeterCount, colorNum);
            timerDrop.Start();
            timer.Start();
        }

        /// <summary>
        /// Timer tick za potpolnuvanje na prazi mesta so figuri
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timerDrop_Tick(object sender, EventArgs e)
        {
            if (!game.drop())
            {
                //dokolku ne padjaat se stopira tajmerot
                timerDrop.Stop();
                //se proveruva tabelata i se zimaat poenite
                int points = game.checkTable();
                if (points > 0)
                {
                    using (var player = new SoundPlayer(Resources.crack))
                    {
                        player.Play();
                    }
                    //dokolku ima poeni se apdejtiraat
                    game.levelMeter.currentlevel += points;
                    totalP += points;
                    Lbpoints.Text = game.levelMeter.currentlevel.ToString();
                    totalPoints.Text = totalP.ToString();
                    timerDrop.Start();
                    clicking = false;
                }
                else
                {
                    if (game.gameOver())
                    {
                        //dokolku nema poeni i e zavrsena igrata
                        //se apdejtiraat poenite i se prikazuva forma za slednoto nivo
                        game.levelMeter.currentlevel += (Time - elapsed) * 2;
                        totalP += (Time - elapsed) * 2;
                        Lbpoints.Text = game.levelMeter.currentlevel.ToString();
                        totalPoints.Text = totalP.ToString();
                        Invalidate();
                        timer.Stop();
                        timerDrop.Stop();
                        timerSwap.Stop();
                        meniForm(true);
                    }
                    clicking = true;
                }
            }
            Invalidate();
        }

        /// <summary>
        /// Funkcija za povikuvanje na meni formata po zavrsetok na igra
        /// </summary>
        /// <param name="nextLevel">
        /// dali da se ovozmozi kopceto za pominuvanje vo sledniot level
        /// </param>
        private void meniForm(bool nextLevel)
        {

            Invalidate();
            updateHighScore();
            Meni f = new Meni(nextLevel);
            f.ShowDialog(this);
            if (f.nextLevel)
            {
                newGame();
            }
            else if (f.Reset)
            {
                reset();
            }
            else if (f.NewGame)
            {
                TableSize = new Size(4, 3);
                MeterCount = 20;
                totalP = 0;
                Time = 20;
                colorNum = 2;
                Level = 0;
                newGame();
            }
            Lbpoints.Text = 0.ToString();
            totalPoints.Text = totalP.ToString();
        }

        /// <summary>
        /// Funkcija za restartiranje na level
        /// </summary>
        private void reset()
        {
            totalP -= game.levelMeter.currentlevel;
            elapsed = 0;
            mouseDown = false;
            clicking = false;
            swaped = false;
            firstClick = Point.Empty;

            int height = (this.Height - 2 * menuStrip1.Height - 5) / TableSize.Height;
            int width = (this.Width - 20) / (TableSize.Width + 1);
            int fsize;
            if (height < width)
                fsize = height;
            else fsize = width;
            game = new Game(TableSize, fsize, menuStrip1.Height + 5, 5, MeterCount, colorNum);
            timerDrop.Start();
            timer.Start();
        }

        /// <summary>
        /// So resize end se menuvaat dimenziite na polinjata
        /// i brzinata so koja se spustaat figurite
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_ResizeEnd(object sender, EventArgs e)
        {
            int height = (this.Height - 2 * menuStrip1.Height - 5) / TableSize.Height;
            int width = (this.Width - 20) / (TableSize.Width + 1);
            if (height < width)
                game.resize(height);
            else game.resize(width);
            game.changeSpeed();
            Invalidate();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            game.draw(e.Graphics);

        }


        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            //validacija na kliknato i postavuvanje na prv klik
            if (clicking && game.hit(e.Location))
            {
                clicking = false;
                mouseDown = true;
                firstClick = e.Location;
            }
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {

            if (mouseDown)
            {
                if (calculateDirection(e.Location))
                {
                    clicking = false;
                    timerSwap.Start();
                }
                else
                {
                    clicking = true;
                }
            }

        }

        /// <summary>
        /// Presmetuva vo koj pravec treba da se dvizi figurata (gore, dole, levo, desno)
        /// </summary>
        /// <param name="e">
        /// Momentalnata tocka kade se naodja kursorot koj se sporeduva so kliknata tocka</param>
        /// <returns>Vraka true dokolku e najden pravecot</returns>
        private bool calculateDirection(Point e)
        {

            int dX = firstClick.X - e.X;
            int dY = firstClick.Y - e.Y;

            int dist = (int)Math.Sqrt(dX * dX + dY * dY);

            if (dist > game.FieldDimension)
            {
                //rastojanieto treba da e pogolemo od poleto
                firstClick = Point.Empty;
                mouseDown = false;
                //se presmetuva nasokata dokolku nemoze da si gi smenat mestata se vraka false
                if (dX >= 0 && dY >= 0)
                {
                    if (dX > dY)
                    {
                        if (!game.swap(Egg.Direction.Left)) return false;
                    }
                    else
                        if (!game.swap(Egg.Direction.Up)) return false;
                }
                else if (dX < 0 && dY >= 0)
                {
                    if (-dX > dY)
                    {
                        if (!game.swap(Egg.Direction.Right)) return false;
                    }
                    else
                        if (!game.swap(Egg.Direction.Up)) return false;
                }
                else if (dX >= 0 && dY < 0)
                {
                    if (dX > -dY)
                    {

                        if (!game.swap(Egg.Direction.Left)) return false;
                    }
                    else
                        if (!game.swap(Egg.Direction.Down)) return false;
                }
                else
                {
                    if (-dX > -dY)
                    {
                        if (!game.swap(Egg.Direction.Right)) return false;
                    }
                    else
                        if (!game.swap(Egg.Direction.Down)) return false;
                }
                return true;
            }


            return false;
        }

        /// <summary>
        /// Go zacuvuva rekordot dokolku ima vo C:\CrackingEggs.HighScore so serializacija
        /// </summary>
        private void updateHighScore()
        {

            if (totalP > getHscore())
            {
                //dokolku ne e kreirana dadotekata
                using (System.IO.File.Create(@"C:\CrackingEggs.HighScore")) ;

                using (FileStream fileStream = new FileStream(@"C:\CrackingEggs.HighScore", FileMode.Create))
                {
                    IFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(fileStream, totalP);
                }

            }
        }
        /// <summary>
        /// Go zima rekordot od C:\CrackingEggs.HighScore 
        /// </summary>
        /// <returns>Rekord(High score)</returns>
        private int getHscore()
        {
            try
            {
                using (FileStream fileStream = new FileStream(@"C:\CrackingEggs.HighScore", FileMode.Open))
                {
                    IFormatter formater = new BinaryFormatter();
                    return (int)formater.Deserialize(fileStream);
                }
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
        /// <summary>
        /// Timer tick kade se vrsi zamena na figurite soodvetno na krajot se proveruva tabelata
        /// i i se zapocnuva so timerDrop...
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timerSwap_Tick(object sender, EventArgs e)
        {

            if (!game.move())
            {
                //dokolku ne e se pomeraat se stopira tajmerot i se proveruva tabelata
                timerSwap.Stop();
                int points = game.checkTable();
                if (points > 0)
                {
                    using (var player = new SoundPlayer(Resources.crack))
                    {
                        player.Play();
                    }
                    game.levelMeter.currentlevel += points;
                    totalP += points;
                    Lbpoints.Text = game.levelMeter.currentlevel.ToString();
                    totalPoints.Text = totalP.ToString();
                    timerDrop.Start();
                    clicking = false;
                }
                else
                {
                    if (game.gameOver())
                    {
                        game.levelMeter.currentlevel += (Time - elapsed) * 2;
                        totalP += (Time - elapsed) * 2;
                        totalPoints.Text = totalP.ToString();
                        Lbpoints.Text = game.levelMeter.currentlevel.ToString();
                        Invalidate();
                        timer.Stop();
                        timerDrop.Stop();
                        timerSwap.Stop();
                        meniForm(true);

                    }
                    else
                    {
                        //dokolku e izigran nedozvolen poteg jajcata se vrakaat na mesto
                        if (!swaped)
                        {
                            game.reSwap();
                            timerSwap.Start();
                            swaped = true;
                        }
                        else
                        {
                            swaped = false;
                        }
                    }
                    clicking = true;
                }
            }
            Invalidate();
        }

        /// <summary>
        /// Timer tick za pominato vreme i proverka dali pominalo
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer_Tick(object sender, EventArgs e)
        {
            //vremenskiot tek
            elapsed++;
            if (Time - elapsed >= 0) time.Text = string.Format("{0} : {1}", (Time - elapsed) / 60, (Time - elapsed) % 60);
            if (elapsed == Time)
            {
                if (!game.gameOver())
                {
                    timerDrop.Stop();
                    timerSwap.Stop();
                    meniForm(false);
                }
            }
        }

        private void restarLevelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            reset();
        }

        private void newGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            updateHighScore();
            TableSize = new Size(4, 3);
            MeterCount = 20;
            totalP = 0;
            Time = 20;
            colorNum = 2;
            newGame();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            updateHighScore();
        }

        private void highScoreToolStripMenuItem_Click(object sender, EventArgs e)
        {
            updateHighScore();
            MessageBox.Show(getHscore().ToString(), "High Score");
        }

    }
}
