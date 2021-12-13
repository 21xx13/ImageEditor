using System;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.IO;

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
        MenuStrip menuStrip;
        ToolStripMenuItem fileMenu;
        ToolStripMenuItem openItem;
        ToolStripMenuItem editMenu;
        ToolStripMenuItem saveItem;
        ToolStripMenuItem undoItem;
        ToolStripMenuItem redoItem;
        ToolStripMenuItem originalItem;
        UndoRedoHistory<Photo> photoHistory;
        Label imageSize;
        public MainWindow()
        {
            InitializeComponent();

            menuStrip.Top = 0;
            menuStrip.Left = 0;
            LoadBitmap((Bitmap)Image.FromFile("raccoons.jpg"));
            filtersSelect.Left = imageArea.Right + 10;
            filtersSelect.Top = menuStrip.Bottom + 10;
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
            undoItem.Enabled = !photoHistory.IsEmptyUndo;
            redoItem.Enabled = !photoHistory.IsEmptyRedo;
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

        void LoadPhoto(object sender, EventArgs empty)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image Files |*.jpg;*.jpeg;*.png;*.gif",
                Title = "Выберите изображение"
            };
            if (openFileDialog.ShowDialog() == DialogResult.OK)
                LoadBitmap((Bitmap)Image.FromFile(openFileDialog.FileName));
        }

        private void SaveImage(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "JPeg Image|*.jpg|PNG Image|*.png|Gif Image|*.gif",
                Title = "Сохранить изображение как"
            };
            saveFileDialog.ShowDialog();

            if (saveFileDialog.FileName != "")
            {
                FileStream fs = (FileStream)saveFileDialog.OpenFile();
                switch (saveFileDialog.FilterIndex)
                {
                    case 1:
                        imageArea.Image.Save(fs, System.Drawing.Imaging.ImageFormat.Jpeg);
                        break;
                    case 2:
                        imageArea.Image.Save(fs, System.Drawing.Imaging.ImageFormat.Png);
                        break;
                    case 3:
                        imageArea.Image.Save(fs, System.Drawing.Imaging.ImageFormat.Gif);
                        break;
                }
                fs.Close();
            }
        }


        void Process(object sender, EventArgs empty)
        {
            var data = parametersControls.Select(z => (double)z.Value).ToArray();
            var filter = (IFilter)filtersSelect.SelectedItem;
            Photo result = null;
            var ph = Convertors.BitmapToPhoto((Bitmap)imageArea.Image);
            result = filter.Process(ph, data);
            photoHistory.Do(result);           
            ChangeImage(Convertors.PhotoToBitmap(result));
        }

        void ChangeImage(Bitmap newImage) {
            
            CheckStatusBtn();
            imageSize.Text = String.Format("Размер картинки:\n{0}x{1}", newImage.Width, newImage.Height);
            imageArea.Image = newImage;
        }

        public void LoadBitmap(Bitmap bmp)
        {
            originalBmp = bmp;            
            photoHistory.Do(Convertors.BitmapToPhoto(originalBmp));
            imageArea.Left = 0;
            imageArea.Top = menuStrip.Bottom;
            imageArea.ClientSize = new Size(800, 600);
            imageArea.SizeMode = PictureBoxSizeMode.Zoom;
            ChangeImage(originalBmp);
            FilterChanged(null, EventArgs.Empty);
        }
  

        ToolStripMenuItem CreateToolStripItem(string text, EventHandler e)
        {
            var item = new ToolStripMenuItem { Text = text };
            item.Click += e;
            return item;
        }

        private void InitializeComponent()
        {          
            menuStrip = new MenuStrip();
            photoHistory = new UndoRedoHistory<Photo>();
            CreateMainMenu();
            imageSize = new Label();
            imageArea = new PictureBox();

            filtersSelect = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList };
            filtersSelect.SelectedIndexChanged += FilterChanged;
            apply = new Button { Text = "Применить", Enabled = false };
            apply.Click += Process;

            Controls.Add(menuStrip);
            Controls.Add(imageArea);          
            Controls.Add(apply);
            Controls.Add(filtersSelect);
            Controls.Add(imageSize);
            Text = "Image Editor";
        }

        private void CreateMainMenu()
        {
            fileMenu = CreateToolStripItem("Файл", null);
            editMenu = CreateToolStripItem("Редактирование", null);
            openItem = CreateToolStripItem("Открыть", LoadPhoto);
            saveItem = CreateToolStripItem("Сохранить", SaveImage);
            undoItem = CreateToolStripItem("Шаг назад", Undo);
            redoItem = CreateToolStripItem("Шаг вперед", Redo);
            originalItem = CreateToolStripItem("Исходное изображение", ReturnOriginal);
            menuStrip.Items.AddRange(new ToolStripItem[] { fileMenu, editMenu });
            fileMenu.DropDownItems.AddRange(new ToolStripItem[] { openItem, saveItem });
            editMenu.DropDownItems.AddRange(new ToolStripItem[] { undoItem, redoItem, originalItem });
            undoItem.ShortcutKeys = Keys.Control | Keys.Z;
            redoItem.ShortcutKeys = Keys.Control | Keys.Y;
        }
    }
}
