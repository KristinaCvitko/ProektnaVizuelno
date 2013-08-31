namespace CrackingEggs
{
    partial class Meni
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Meni));
            this.next = new System.Windows.Forms.Button();
            this.reset = new System.Windows.Forms.Button();
            this.newGame = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // next
            // 
            this.next.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.next.ForeColor = System.Drawing.Color.CornflowerBlue;
            this.next.Location = new System.Drawing.Point(40, 25);
            this.next.Name = "next";
            this.next.Size = new System.Drawing.Size(213, 71);
            this.next.TabIndex = 0;
            this.next.Text = "N E X T   L E V E L";
            this.next.UseVisualStyleBackColor = true;
            this.next.Click += new System.EventHandler(this.button1_Click);
            // 
            // reset
            // 
            this.reset.FlatAppearance.BorderColor = System.Drawing.Color.Red;
            this.reset.FlatAppearance.BorderSize = 5;
            this.reset.Location = new System.Drawing.Point(83, 111);
            this.reset.Name = "reset";
            this.reset.Size = new System.Drawing.Size(127, 45);
            this.reset.TabIndex = 1;
            this.reset.Text = "R E S T A R T";
            this.reset.UseVisualStyleBackColor = true;
            this.reset.Click += new System.EventHandler(this.button2_Click);
            // 
            // newGame
            // 
            this.newGame.FlatAppearance.BorderColor = System.Drawing.Color.Red;
            this.newGame.FlatAppearance.BorderSize = 5;
            this.newGame.Location = new System.Drawing.Point(83, 171);
            this.newGame.Name = "newGame";
            this.newGame.Size = new System.Drawing.Size(127, 45);
            this.newGame.TabIndex = 2;
            this.newGame.Text = "N E W   GAME";
            this.newGame.UseVisualStyleBackColor = true;
            this.newGame.Click += new System.EventHandler(this.newGame_Click);
            // 
            // Meni
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::CrackingEggs.Properties.Resources.backg;
            this.ClientSize = new System.Drawing.Size(292, 266);
            this.Controls.Add(this.newGame);
            this.Controls.Add(this.reset);
            this.Controls.Add(this.next);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Meni";
            this.Text = "Meni";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Meni_FormClosed);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button next;
        private System.Windows.Forms.Button reset;
        private System.Windows.Forms.Button newGame;
    }
}