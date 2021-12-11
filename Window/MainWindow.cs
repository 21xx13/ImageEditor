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
        //Panel paintPanel;
        //Label brushLabel;
        //TrackBar brushSize;
        //Button changeColorBtn;
        //List<Color> colorList;
        //bool isMouse = false;
        UndoRedoHistory<Photo> photoHistory;
        //class ArrayPoints
        //{
        //    private int index = 0;
        //    private Point[] points;
        //    public ArrayPoints(int size)
        //    {
        //        points = new Point[size];
        //    }

        //    public void SetPoint(int x, int y)
        //    {
        //        if (index >= points.Length)
        //        {
        //            index = 0;
        //        }
        //        points[index] = new Point(x, y);
        //        index++;
        //    }

        //    public void ResetPoints()
        //    {
        //        index = 0;
        //    }

        //    public int GetCountPoints()
        //    {
        //        return index;
        //    }

        //    public Point[] GetPoints()
        //    {
        //        return points;
        //    }
        //}
        //private ArrayPoints arrayPoints = new ArrayPoints(2);
        //Graphics graphics;
        //Pen pen = new Pen(Color.Black, 3f);
        //Bitmap map;

        public MainWindow()
        {
            InitializeComponent();

            menuStrip.Top = 0;
            menuStrip.Left = 0;
            LoadBitmap((Bitmap)Image.FromFile("raccoons.jpg"));
            //paintPanel.Top = menuStrip.Bottom + 10;
            //paintPanel.Left = imageArea.Right + 10;
            //paintPanel.Size = new Size(200, 160);
            filtersSelect.Left = imageArea.Right + 10;
            filtersSelect.Top = menuStrip.Bottom + 10;
            filtersSelect.Width = 200;
            filtersSelect.Height = 20;
            //brushSize.Width = paintPanel.Width;

            ClientSize = new Size(filtersSelect.Right + 20, imageArea.Bottom);
            //map = new Bitmap(imageArea.Image);
            apply.Left = ClientSize.Width - 120;
            apply.Top = ClientSize.Height - 50;
            apply.Width = 100;
            apply.Height = 40;
           // SetSize();
           // imageArea.MouseDown += (sender, e) => { isMouse = true; photoHistory.Do(Convertors.BitmapToPhoto((Bitmap)imageArea.Image)); CheckStatusBtn(); };
            //imageArea.MouseUp += (sender, e) => { isMouse = false; arrayPoints.ResetPoints(); };
            //imageArea.MouseMove += MouseMovePaint;
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
            //graphics.Clear(Color.Transparent);
            //graphics = Graphics.FromImage((Bitmap)imageArea.Image);
            imageArea.Image = originalBmp;
            
        }

        void Undo(object sender, EventArgs e)
        {
            imageArea.Image = Convertors.PhotoToBitmap(photoHistory.Undo());
            CheckStatusBtn();
        }

        void Redo(object sender, EventArgs e)
        {
            imageArea.Image = Convertors.PhotoToBitmap(photoHistory.Redo());
            CheckStatusBtn();
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
                    TickFrequency = 5,
                    Maximum = (int)param.MaxValue,
                    Minimum = (int)param.MinValue,
                    Value = (int)param.DefaultValue
                };

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
            CheckStatusBtn();
            var resultBmp = Convertors.PhotoToBitmap(result);
            
            imageArea.Image = resultBmp;
        }

        public void LoadBitmap(Bitmap bmp)
        {
            originalBmp = bmp;
            imageArea.Image = originalBmp;
            photoHistory.Do(Convertors.BitmapToPhoto(originalBmp));
            imageArea.Left = 0;
            imageArea.Top = menuStrip.Bottom;
            imageArea.ClientSize = new Size(800, 600);
            imageArea.SizeMode = PictureBoxSizeMode.Zoom;
            CheckStatusBtn();
            FilterChanged(null, EventArgs.Empty);
        }

        //void CreateColorBtn()
        //{
        //    var x = 0;
        //    int y = 0;
        //    int size = 27;
        //    int lenLine = 6;
        //    for (int i = 0; i < colorList.Count; i++)
        //    {
        //        x = i % lenLine * (size + 5);
        //        y = i / lenLine * (size + 5);
        //        var btn = new Button();
        //        paintPanel.Controls.Add(btn);
        //        btn.BackColor = colorList[i];
        //        btn.FlatStyle = FlatStyle.Flat;
        //        btn.Location = new Point(x, y);
        //        btn.Size = new Size(size, size);
        //        btn.UseVisualStyleBackColor = false;
        //        btn.Click += СhangeColor;
        //    }
        //}

        ToolStripMenuItem CreateToolStripItem(string text, EventHandler e)
        {
            var item = new ToolStripMenuItem { Text = text };
            item.Click += e;
            return item;
        }

        private void InitializeComponent()
        {
            //colorList = new List<Color>() {Color.White, Color.Gray, Color.Black, Color.Red, Color.Orange, Color.Yellow,
            //    Color.Lime, Color.Green, Color.Turquoise, Color.Blue, Color.Fuchsia};
            menuStrip = new MenuStrip();
            photoHistory = new UndoRedoHistory<Photo>();
            CreateMainMenu();

            //paintPanel = new Panel();
            //brushLabel = new Label { Top = 90, Width = 200 };
            //brushSize = new TrackBar { Top = brushLabel.Bottom + 5, Maximum = 30, Minimum = 0, TickFrequency = 2, Value = 1 };
            //brushLabel.Text = String.Format("Толщина кисти: {0}", brushSize.Value);
            //brushSize.Scroll += (s, e) => { brushLabel.Text = String.Format("Толщина кисти: {0}", brushSize.Value); };
            //CreateColorBtn();
            //changeColorBtn = new Button
            //{
            //    BackColor = Color.White,
            //    Location = new Point(160, 32),
            //    Size = new Size(27, 27),
            //    UseVisualStyleBackColor = false
            //};

            imageArea = new PictureBox();

            filtersSelect = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList };
            filtersSelect.SelectedIndexChanged += FilterChanged;
            apply = new Button { Text = "Применить", Enabled = false };
            apply.Click += Process;

            Controls.Add(menuStrip);
            Controls.Add(imageArea);
            //Controls.Add(paintPanel);
            Controls.Add(apply);
            Controls.Add(filtersSelect);
            //paintPanel.Controls.Add(brushLabel);
            //paintPanel.Controls.Add(brushSize);
            //paintPanel.Controls.Add(changeColorBtn);
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

        //private void СhangeColor(object sender, EventArgs e)
        //{

        //}

        //private void MouseMovePaint(object sender, MouseEventArgs e)
        //{
            
        //    if (!isMouse) return;
        //    arrayPoints.SetPoint(e.X, e.Y);
        //    if (arrayPoints.GetCountPoints() >= 2) {
              
        //        graphics.DrawLines(pen, arrayPoints.GetPoints());
                
        //        imageArea.Image = map;
                
        //        arrayPoints.SetPoint(e.X, e.Y);
        //    }                
        //}

        //void SetSize() {

        //    graphics = Graphics.FromImage((Bitmap)map);
        //    pen.StartCap = System.Drawing.Drawing2D.LineCap.Round;
        //    pen.EndCap = System.Drawing.Drawing2D.LineCap.Round;
        //}
    }
}
