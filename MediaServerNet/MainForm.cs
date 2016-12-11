using System;
using System.IO;
using System.Windows.Forms;
using MediaServerNet.Models;

namespace MediaServerNet
{
    public partial class MainForm : Form
    {
        #region Media source

        private FileSource _source;

        public FileSource Source
        {
            get { return _source; }
            private set
            {
                _source = value;
                Text = _source.FromPath;
                CurrentIndex = 0;
            }
        }

        private void changeDir_Click(object sender, EventArgs e)
        {
            var result = browse.ShowDialog(this);
            if (result == DialogResult.OK)
            {
                Source = new FileSource(browse.SelectedPath);
            }
        }

        #endregion

        #region Media processing

        private int _currentIndex;

        public int CurrentIndex
        {
            get { return _currentIndex; }
            set
            {
                var i = _currentIndex = value;
                if (_currentIndex < 0) _currentIndex = 0;
                
                CurrentMedia = i < Source.PictureCount
                    ? Source[i]
                    : null;

                if (CurrentMedia == null)
                {
                    media.Image = null;
                    medias.Text = Source.PictureCount == 0 
                        ? @"No media found" 
                        : $@"Media {i + 1} / {Source.PictureCount} · load error";
                }
                else
                {
                    const double mb = 1024.0*1024.0;
                    media.Image = CurrentMedia.Small;
                    medias.Text =
                        $@"Media {i + 1} / {Source.PictureCount} · {CurrentMedia.Size/mb:F2}MB · {CurrentMedia.Hash} · {Path.GetFileName(CurrentMedia.Filename)}";
                }

                EnableButtons();
            }
        }

        public Media CurrentMedia { get; private set; }

        #endregion

        #region Album

        private Album _currentAlbum;

        public Album CurrentAlbum
        {
            get { return _currentAlbum; }
            private set
            {
                _currentAlbum?.Disconnect();

                _currentAlbum = value;
                _currentAlbum.Progress += CurrentAlbumOnProgress;
                album.Text = @"Album: " + _currentAlbum.Name;                
                EnableButtons();
            }
        }

        private void CurrentAlbumOnProgress(Album a)
        {
            Invoke((MethodInvoker)delegate
            {
                const double mb = 1024.0*1024.0;
                progress.Maximum = (int)(a.TotalBytesExpected/10240);
                progress.Value = (int)(a.TotalBytesSofar/10240);
                transfer.Text =
                    $"{a.TotalBytesSofar/mb:F2} / {a.TotalBytesExpected/mb:F2} MB · {a.Status}";
            });
        }

        #endregion

        private void EnableButtons()
        {
            var enabled = CurrentMedia != null && CurrentAlbum != null;
            add.Enabled = enabled;
            hide.Enabled = enabled;
            skip.Enabled = enabled;
            left.Enabled = enabled;
            right.Enabled = enabled;
            save.Enabled = CurrentAlbum != null;
        }

        public MainForm()
        {
            InitializeComponent();
            Source = new FileSource(@"C:\");
        }

        private void add_Click(object sender, EventArgs e)
        {
            var photo = CurrentMedia;
            if (photo != null)
                CurrentAlbum.Add(photo);            

            ++CurrentIndex;            
        }

        private void hide_Click(object sender, EventArgs e)
        {
            var photo = CurrentMedia;
            if (photo != null)
                CurrentAlbum.Add(photo, true);            

            ++CurrentIndex;
        }

        private void skip_Click(object sender, EventArgs e)
        {
            ++CurrentIndex;
        }

        private void album_Click(object sender, EventArgs e)
        {
            var result = browse.ShowDialog(this);
            if (result == DialogResult.OK)
            {
                CurrentAlbum = new Album(browse.SelectedPath);
            }
        }

        private void left_Click(object sender, EventArgs e)
        {
            if (CurrentMedia != null)
            {
                CurrentMedia.RotateLeft();
                media.Image = CurrentMedia.Small;
            }
        }

        private void right_Click(object sender, EventArgs e)
        {
            if (CurrentMedia != null)
            {
                CurrentMedia.RotateRight();
                media.Image = CurrentMedia.Small;
            }
        }

        private void save_Click(object sender, EventArgs e)
        {
            CurrentAlbum?.SaveToDisk();
        }
    }
}
