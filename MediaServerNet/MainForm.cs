using System;
using System.Windows.Forms;
using MediaServerNet.Models;

namespace MediaServerNet
{
    public partial class MainForm : Form
    {
        #region Image source

        private FileSource _source;

        public FileSource Source
        {
            get { return _source; }
            private set
            {
                _source = value;
                Text = _source.FromPath;
                CurrentPhotoIndex = 0;
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

        #region Image processing

        private int _currentPhotoIndex;

        public int CurrentPhotoIndex
        {
            get { return _currentPhotoIndex; }
            set
            {
                _currentPhotoIndex = value;
                if (_currentPhotoIndex < 0) _currentPhotoIndex = 0;
                
                CurrentPhoto = (_currentPhotoIndex < Source.PictureCount)
                    ? Source[_currentPhotoIndex]
                    : null;

                if (CurrentPhoto == null)
                {
                    picture.Image = null;
                    if (Source.PictureCount == 0)
                    {
                        pics.Text = @"No photos found";
                    }
                    else
                    {
                        pics.Text = string.Format(@"Photo {0} / {1} · load error", _currentPhotoIndex + 1, Source.PictureCount);
                    }
                }
                else
                {
                    picture.Image = CurrentPhoto.Small;
                    pics.Text = string.Format(@"Photo {0} / {1} · {2:F2}MB · {3}", 
                        _currentPhotoIndex + 1, 
                        Source.PictureCount,
                        CurrentPhoto.Size / (1024.0 * 1024.0),
                        CurrentPhoto.Hash);
                }

                EnableButtons();
            }
        }

        public Photo CurrentPhoto { get; private set; }

        #endregion

        #region Album

        private Album _currentAlbum;

        public Album CurrentAlbum
        {
            get { return _currentAlbum; }
            private set
            {
                if (_currentAlbum != null)                
                    _currentAlbum.Disconnect();

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
                progress.Maximum = (int)(a.TotalBytesExpected/10240);
                progress.Value = (int)(a.TotalBytesSofar/10240);
                transfer.Text = string.Format(
                    "{0:F2} / {1:F2} MB · {2}",
                    a.TotalBytesSofar/(1024.0*1024.0),
                    a.TotalBytesExpected/(1024.0*1024.0),
                    a.Status);
            });
        }

        #endregion

        private void EnableButtons()
        {
            var enabled = CurrentPhoto != null && CurrentAlbum != null;
            add.Enabled = enabled;
            hide.Enabled = enabled;
            skip.Enabled = enabled;
            left.Enabled = enabled;
            right.Enabled = enabled;
            save.Enabled = (CurrentAlbum != null);
        }

        public MainForm()
        {
            InitializeComponent();
            Source = new FileSource(@"C:\");
        }

        private void add_Click(object sender, EventArgs e)
        {
            var photo = CurrentPhoto;
            if (photo != null)
                CurrentAlbum.Add(photo);            

            ++CurrentPhotoIndex;            
        }

        private void hide_Click(object sender, EventArgs e)
        {
            var photo = CurrentPhoto;
            if (photo != null)
                CurrentAlbum.Add(photo, true);            

            ++CurrentPhotoIndex;
        }

        private void skip_Click(object sender, EventArgs e)
        {
            ++CurrentPhotoIndex;
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
            if (CurrentPhoto != null)
            {
                CurrentPhoto.RotateLeft();
                picture.Image = CurrentPhoto.Small;
            }
        }

        private void right_Click(object sender, EventArgs e)
        {
            if (CurrentPhoto != null)
            {
                CurrentPhoto.RotateRight();
                picture.Image = CurrentPhoto.Small;
            }
        }

        private void save_Click(object sender, EventArgs e)
        {
            if (CurrentAlbum != null)
                CurrentAlbum.SaveToDisk();
        }
    }
}
