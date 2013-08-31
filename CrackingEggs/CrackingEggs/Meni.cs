using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CrackingEggs
{
    public partial class Meni : Form
    {
        public bool nextLevel { get; set; }
        public bool Reset { get; set; }
        public bool NewGame { get; set; }

        public Meni(bool nextLevel)
        {
            InitializeComponent();
            next.Enabled = nextLevel;
            CancelButton = reset;
            AcceptButton = next;
        }
       
        private void button1_Click(object sender, EventArgs e)
        {
            nextLevel = true;
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Reset = true;
            this.Close();
        }

        private void newGame_Click(object sender, EventArgs e)
        {
            NewGame = true;
            this.Close();
        }

        private void Meni_FormClosed(object sender, FormClosedEventArgs e)
        {
            //dokolku nisto ne e kliknato da se resetira tekovnoto nivo
            if (!nextLevel && !Reset && !NewGame)
            {
                Reset = true;
            }
        }

        
    }
}
