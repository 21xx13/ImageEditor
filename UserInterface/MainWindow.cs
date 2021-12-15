using System;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;

namespace MyPhotoshop
{
    public class MainWindow : Form
    {
        Bitmap originalBmp;
        Panel parametersPanel;
        List<TrackBar> parametersControls;
        PictureBox imageArea;
        ComboBox filtersSelect;
        Button apply;
        UndoRedoHistory<Photo> photoHistory;
        Label imageSize;
        public MainWindow()
        {
            InitializeComponent();
            MenuStripCreater.MenuStrip.Top = 0;
            MenuStripCreater.MenuStrip.Left = 0;
            CropController.SetImage(imageArea);
            LoadBitmap((Bitmap)Image.FromFile("raccoons.jpg"));
            filtersSelect.Left = imageArea.Right + 10;
            filtersSelect.Top = MenuStripCreater.MenuStrip.Bottom + 10;
            filtersSelect.Width = 200;
            filtersSelect.Height = 20;

            ClientSize = new Size(filtersSelect.Right + 20, imageArea.Bottom);
            apply.Left = ClientSize.Width - 100;
            apply.Top = ClientSize.Height - 50;
            apply.Width = 80;
            apply.Height = 40;

            imageSize.Left = imageArea.Right + 10;
            imageSize.Top = ClientSize.Height - 45;
        }

        private void CropMouseEnter(object sender, EventArgs e) => Cursor = Cursors.Cross;
        protected override void OnMouseEnter(EventArgs e) => Cursor = Cursors.Default;
        private void CropMouseMove(object sender, MouseEventArgs e) => CropController.CropMouseMove(sender, e);
        private void CropMouseDown(object sender, MouseEventArgs e) => CropController.CropMouseDown(sender, e);

        private void CropToggle(object sender, EventArgs e)
        {
            if (MenuStripCreater.CropItem.Checked)
            {
                imageArea.MouseDown -= (CropMouseDown);
                imageArea.MouseMove -= (CropMouseMove);
                imageArea.MouseEnter -= (CropMouseEnter);

            }
            else
            {
                imageArea.MouseDown += (CropMouseDown);
                imageArea.MouseMove += (CropMouseMove);
                imageArea.MouseEnter += (CropMouseEnter);
            }
            Cursor = Cursors.Default;
            imageArea.Refresh();
            MenuStripCreater.CropItem.Checked = !MenuStripCreater.CropItem.Checked;
        }      
        private void CropProcess(object sender, EventArgs e)
        {
            photoHistory.Do(Convertors.BitmapToPhoto((Bitmap)imageArea.Image));
            Bitmap crpImg = CropController.CropProcess();
            ChangeImage(crpImg);
        }

        public void AddFilter(IFilter filter)
        {
            filtersSelect.Items.Add(filter);
            if (filtersSelect.SelectedIndex == -1)
            {
                filtersSelect.SelectedIndex = 0;
                apply.Enabled = true;
            }
        }

        void ReturnOriginal(object sender, EventArgs e)
        {
            photoHistory.Do(Convertors.BitmapToPhoto(originalBmp));
            ChangeImage(originalBmp);
        }

        void Undo(object sender, EventArgs e)
        {
            ChangeImage(Convertors.PhotoToBitmap(photoHistory.Undo()));
        }

        void Redo(object sender, EventArgs e)
        {
            ChangeImage(Convertors.PhotoToBitmap(photoHistory.Redo()));
        }

        void CheckStatusBtn()
        {
            MenuStripCreater.UndoItem.Enabled = !photoHistory.IsEmptyUndo;
            MenuStripCreater.RedoItem.Enabled = !photoHistory.IsEmptyRedo;
        }

        void FilterChanged(object sender, EventArgs e)
        {
            var filter = (IFilter)filtersSelect.SelectedItem;
            if (filter == null) return;
            if (parametersPanel != null) Controls.Remove(parametersPanel);
            parametersControls = new List<TrackBar>();
            parametersPanel = new Panel
            {
                Left = filtersSelect.Left,
                Top = filtersSelect.Bottom + 10,
                Width = filtersSelect.Width
            };
            parametersPanel.Height = ClientSize.Height - parametersPanel.Top;
            int y = 0;

            foreach (var param in filter.GetParameters())
            {
                var label = new Label
                {
                    Left = 0,
                    Top = y,
                    Width = parametersPanel.Width,
                    Height = 40
                };

                var box = new TrackBar
                {
                    Left = 0,
                    Top = label.Bottom,
                    Width = parametersPanel.Width,
                    Height = 20,
                    Maximum = (int)param.MaxValue,
                    Minimum = (int)param.MinValue,
                    Value = (int)param.DefaultValue
                };
                box.TickFrequency = (box.Maximum - box.Minimum) / 20;

                label.Text = String.Format("{0}\nТекущее значение: {1}", param.Name, box.Value);
                box.Scroll += (s, ev) => { label.Text = String.Format("{0}\nТекущее значение: {1}", param.Name, box.Value); };
                parametersPanel.Controls.Add(label);
                parametersPanel.Controls.Add(box);
                y += label.Height + 65;
                parametersControls.Add(box);
            }
            Controls.Add(parametersPanel);
        }

        private void LoadPhoto(object sender, EventArgs empty) => ImageController.LoadImage(LoadBitmap);
        private void SaveImage(object sender, EventArgs empty) => ImageController.SaveImage(imageArea.Image);

        void Process(object sender, EventArgs empty)
        {
            if (MenuStripCreater.CropItem.Checked)
            {
                CropToggle(sender, empty);
                CropProcess(sender, empty);
                return;
            }
            var data = parametersControls.Select(z => (double)z.Value).ToArray();
            var filter = (IFilter)filtersSelect.SelectedItem;
            Photo result = null;
            var ph = Convertors.BitmapToPhoto((Bitmap)imageArea.Image);
            result = filter.Process(ph, data);
            photoHistory.Do(result);
            ChangeImage(Convertors.PhotoToBitmap(result));
        }

        void ChangeImage(Bitmap newImage)
        {
            CheckStatusBtn();
            imageSize.Text = String.Format("Размер картинки:\n{0}x{1}", newImage.Width, newImage.Height);
            imageArea.Image = newImage;
        }

        public void LoadBitmap(Bitmap bmp)
        {
            originalBmp = bmp;
            photoHistory.Do(Convertors.BitmapToPhoto(originalBmp));
            imageArea.Left = 0;
            imageArea.Top = MenuStripCreater.MenuStrip.Bottom;
            imageArea.ClientSize = new Size(800, 600);
            imageArea.SizeMode = PictureBoxSizeMode.Zoom;
            ChangeImage(originalBmp);
            FilterChanged(null, EventArgs.Empty);
        }

        private void InitializeComponent()
        {
            photoHistory = new UndoRedoHistory<Photo>();
            MenuStripCreater.CreateMainMenu(SaveImage, LoadPhoto, Undo, Redo, ReturnOriginal, CropToggle);
            imageSize = new Label();
            imageArea = new PictureBox();

            filtersSelect = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList };
            filtersSelect.SelectedIndexChanged += FilterChanged;
            apply = new Button { Text = "Применить", Enabled = false };
            apply.Click += Process;

            Controls.Add(MenuStripCreater.MenuStrip);
            Controls.Add(imageArea);
            Controls.Add(apply);
            Controls.Add(filtersSelect);
            Controls.Add(imageSize);
            Text = "Image Editor";
        }
    }
}
