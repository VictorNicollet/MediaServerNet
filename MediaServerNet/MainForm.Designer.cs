namespace MediaServerNet
{
    partial class MainForm
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
            this.picture = new System.Windows.Forms.PictureBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.skip = new System.Windows.Forms.Button();
            this.album = new System.Windows.Forms.Button();
            this.hide = new System.Windows.Forms.Button();
            this.add = new System.Windows.Forms.Button();
            this.changeDir = new System.Windows.Forms.Button();
            this.browse = new System.Windows.Forms.FolderBrowserDialog();
            this.status = new System.Windows.Forms.StatusStrip();
            this.pics = new System.Windows.Forms.ToolStripStatusLabel();
            this.progress = new System.Windows.Forms.ToolStripProgressBar();
            this.transfer = new System.Windows.Forms.ToolStripStatusLabel();
            this.right = new System.Windows.Forms.Button();
            this.left = new System.Windows.Forms.Button();
            this.save = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.picture)).BeginInit();
            this.panel1.SuspendLayout();
            this.status.SuspendLayout();
            this.SuspendLayout();
            // 
            // picture
            // 
            this.picture.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.picture.Location = new System.Drawing.Point(12, 37);
            this.picture.Name = "picture";
            this.picture.Size = new System.Drawing.Size(1156, 522);
            this.picture.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.picture.TabIndex = 0;
            this.picture.TabStop = false;
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.save);
            this.panel1.Controls.Add(this.left);
            this.panel1.Controls.Add(this.right);
            this.panel1.Controls.Add(this.skip);
            this.panel1.Controls.Add(this.album);
            this.panel1.Controls.Add(this.hide);
            this.panel1.Controls.Add(this.add);
            this.panel1.Controls.Add(this.changeDir);
            this.panel1.Location = new System.Drawing.Point(0, 2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1180, 29);
            this.panel1.TabIndex = 1;
            // 
            // skip
            // 
            this.skip.Enabled = false;
            this.skip.Location = new System.Drawing.Point(845, 2);
            this.skip.Name = "skip";
            this.skip.Size = new System.Drawing.Size(72, 23);
            this.skip.TabIndex = 10;
            this.skip.Text = "Skip";
            this.skip.UseVisualStyleBackColor = true;
            this.skip.Click += new System.EventHandler(this.skip_Click);
            // 
            // album
            // 
            this.album.Location = new System.Drawing.Point(107, 3);
            this.album.Name = "album";
            this.album.Size = new System.Drawing.Size(218, 23);
            this.album.TabIndex = 9;
            this.album.Text = "Album: none";
            this.album.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.album.UseVisualStyleBackColor = true;
            this.album.Click += new System.EventHandler(this.album_Click);
            // 
            // hide
            // 
            this.hide.Enabled = false;
            this.hide.Location = new System.Drawing.Point(923, 2);
            this.hide.Name = "hide";
            this.hide.Size = new System.Drawing.Size(72, 23);
            this.hide.TabIndex = 6;
            this.hide.Text = "Add Hidden";
            this.hide.UseVisualStyleBackColor = true;
            this.hide.Click += new System.EventHandler(this.hide_Click);
            // 
            // add
            // 
            this.add.Enabled = false;
            this.add.Location = new System.Drawing.Point(1053, 2);
            this.add.Name = "add";
            this.add.Size = new System.Drawing.Size(72, 23);
            this.add.TabIndex = 5;
            this.add.Text = "Add";
            this.add.UseVisualStyleBackColor = true;
            this.add.Click += new System.EventHandler(this.add_Click);
            // 
            // changeDir
            // 
            this.changeDir.Location = new System.Drawing.Point(3, 3);
            this.changeDir.Name = "changeDir";
            this.changeDir.Size = new System.Drawing.Size(98, 23);
            this.changeDir.TabIndex = 2;
            this.changeDir.Text = "Change directory";
            this.changeDir.UseVisualStyleBackColor = true;
            this.changeDir.Click += new System.EventHandler(this.changeDir_Click);
            // 
            // browse
            // 
            this.browse.RootFolder = System.Environment.SpecialFolder.MyComputer;
            // 
            // status
            // 
            this.status.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.pics,
            this.progress,
            this.transfer});
            this.status.Location = new System.Drawing.Point(0, 549);
            this.status.Name = "status";
            this.status.Size = new System.Drawing.Size(1180, 22);
            this.status.TabIndex = 2;
            this.status.Text = "statusStrip1";
            // 
            // pics
            // 
            this.pics.Name = "pics";
            this.pics.Size = new System.Drawing.Size(98, 17);
            this.pics.Text = "No photos found";
            // 
            // progress
            // 
            this.progress.Name = "progress";
            this.progress.Size = new System.Drawing.Size(300, 16);
            // 
            // transfer
            // 
            this.transfer.Name = "transfer";
            this.transfer.Size = new System.Drawing.Size(51, 17);
            this.transfer.Text = "0 / 0 MB";
            // 
            // right
            // 
            this.right.Location = new System.Drawing.Point(1131, 2);
            this.right.Name = "right";
            this.right.Size = new System.Drawing.Size(46, 23);
            this.right.TabIndex = 11;
            this.right.Text = "90° R";
            this.right.UseVisualStyleBackColor = true;
            this.right.Click += new System.EventHandler(this.right_Click);
            // 
            // left
            // 
            this.left.Location = new System.Drawing.Point(1001, 2);
            this.left.Name = "left";
            this.left.Size = new System.Drawing.Size(46, 23);
            this.left.TabIndex = 12;
            this.left.Text = "90° L";
            this.left.UseVisualStyleBackColor = true;
            this.left.Click += new System.EventHandler(this.left_Click);
            // 
            // save
            // 
            this.save.Location = new System.Drawing.Point(331, 3);
            this.save.Name = "save";
            this.save.Size = new System.Drawing.Size(47, 23);
            this.save.TabIndex = 13;
            this.save.Text = "Save";
            this.save.UseVisualStyleBackColor = true;
            this.save.Click += new System.EventHandler(this.save_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1180, 571);
            this.Controls.Add(this.status);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.picture);
            this.Name = "MainForm";
            this.Text = "MediaServer.Net";
            ((System.ComponentModel.ISupportInitialize)(this.picture)).EndInit();
            this.panel1.ResumeLayout(false);
            this.status.ResumeLayout(false);
            this.status.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox picture;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button changeDir;
        private System.Windows.Forms.Button hide;
        private System.Windows.Forms.Button add;
        private System.Windows.Forms.Button album;
        private System.Windows.Forms.Button skip;
        private System.Windows.Forms.FolderBrowserDialog browse;
        private System.Windows.Forms.StatusStrip status;
        private System.Windows.Forms.ToolStripStatusLabel pics;
        private System.Windows.Forms.ToolStripProgressBar progress;
        private System.Windows.Forms.ToolStripStatusLabel transfer;
        private System.Windows.Forms.Button left;
        private System.Windows.Forms.Button right;
        private System.Windows.Forms.Button save;
    }
}

