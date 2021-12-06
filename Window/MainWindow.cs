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
        PictureBox imageArea;
        ComboBox filtersSelect;
        Panel parametersPanel;
        List<TrackBar> parametersControls;
        Button apply;
        Button originalBtn;
        MenuStrip menuStrip;
        ToolStripMenuItem fileMenu;
        ToolStripMenuItem openItem;
        ToolStripMenuItem saveItem;

        public MainWindow()
        {
            menuStrip = new MenuStrip();
            fileMenu = new ToolStripMenuItem();
            openItem = new ToolStripMenuItem();
            saveItem = new ToolStripMenuItem();
            menuStrip.Items.Add(fileMenu);
            fileMenu.DropDownItems.AddRange(new ToolStripItem[] {openItem, saveItem});
            fileMenu.Text = "Файл";
            openItem.Text = "Открыть";
            saveItem.Text = "Сохранить";
            saveItem.Click += SaveImage;
            openItem.Click += LoadPhoto;
            Controls.Add(menuStrip);            

            imageArea = new PictureBox();
            Controls.Add(imageArea);

            filtersSelect = new ComboBox();
            filtersSelect.DropDownStyle = ComboBoxStyle.DropDownList;
            filtersSelect.SelectedIndexChanged += FilterChanged;
            Controls.Add(filtersSelect);

            originalBtn = new Button();
            originalBtn.Text = "Исходное изображение";
            originalBtn.Click += ReturnOriginal;
            Controls.Add(originalBtn);

            apply = new Button();
            apply.Text = "Применить";
            apply.Enabled = false;
            apply.Click += Process;
            Controls.Add(apply);

            Text = "Image Editor";
            FormBorderStyle = FormBorderStyle.FixedDialog;

            LoadBitmap((Bitmap)Image.FromFile("raccoons.jpg"));
        }

        public void LoadBitmap(Bitmap bmp)
        {
            originalBmp = bmp;
            menuStrip.Top = 0;
            menuStrip.Left = 0;
            imageArea.Image = originalBmp;
            imageArea.Left = 0;
            imageArea.Top = menuStrip.Bottom;
            imageArea.ClientSize = new Size(800, 600);
            imageArea.SizeMode = PictureBoxSizeMode.Zoom;

            filtersSelect.Left = imageArea.Right + 10;
            filtersSelect.Top = menuStrip.Bottom + 10;
            filtersSelect.Width = 200;
            filtersSelect.Height = 20;
                       
            ClientSize = new Size(filtersSelect.Right + 20, imageArea.Bottom);
            apply.Left = ClientSize.Width - 120;
            apply.Top = ClientSize.Height - 50;
            apply.Width = 100;
            apply.Height = 40;

            originalBtn.Left = ClientSize.Width - 230;
            originalBtn.Top = ClientSize.Height - 50;
            originalBtn.Width = 100;
            originalBtn.Height = 40;

            FilterChanged(null, EventArgs.Empty);
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

        void ReturnOriginal(object sender, EventArgs e) {
            imageArea.Image = originalBmp;
        }

        

        void FilterChanged(object sender, EventArgs e)
        {
            var filter = (IFilter)filtersSelect.SelectedItem;
            if (filter == null) return;
            if (parametersPanel != null) Controls.Remove(parametersPanel);
            parametersControls = new List<TrackBar>();
            parametersPanel = new Panel();
            parametersPanel.Left = filtersSelect.Left;
            parametersPanel.Top = filtersSelect.Bottom + 10;
            parametersPanel.Width = filtersSelect.Width;
            parametersPanel.Height = ClientSize.Height - parametersPanel.Top;

            int y = 0;

            foreach (var param in filter.GetParameters())
            {
                var label = new Label();
                label.Left = 0;
                label.Top = y;
                label.Width = parametersPanel.Width;
                label.Height = 40;

                var box = new TrackBar();
                box.Left = 0;
                box.Top = label.Bottom;
                box.Width = parametersPanel.Width;
                box.Height = 20;
                box.Value = (int)param.DefaultValue;
                box.TickFrequency = 5;
                box.Maximum = (int)param.MaxValue;
                box.Minimum = (int)param.MinValue;
                box.LargeChange = 3;

                label.Text = String.Format("{0}\nТекущее значение: {1}", param.Name, box.Value);
                box.Scroll += (a, b) => { label.Text = String.Format("{0}\nТекущее значение: {1}", param.Name, box.Value); };
                parametersPanel.Controls.Add(label);
                parametersPanel.Controls.Add(box);
                y += label.Height + 65;
                parametersControls.Add(box);
            }
            Controls.Add(parametersPanel);
        }

        void LoadPhoto(object sender, EventArgs empty)
        {
            OpenFileDialog OPF = new OpenFileDialog();
            OPF.Filter = "Image Files |*.jpg;*.jpeg;*.png;*.gif";
            OPF.Title = "Выберите изображение";
            if (OPF.ShowDialog() == DialogResult.OK)
                LoadBitmap((Bitmap)Image.FromFile(OPF.FileName));
        }

        private void SaveImage(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "JPeg Image|*.jpg|PNG Image|*.png|Gif Image|*.gif";
            saveFileDialog1.Title = "Сохранить изображение как";
            saveFileDialog1.ShowDialog();

            if (saveFileDialog1.FileName != "")
            {
                FileStream fs =
                    (FileStream)saveFileDialog1.OpenFile();
                switch (saveFileDialog1.FilterIndex)
                {
                    case 1:
                        imageArea.Image.Save(fs,
                          System.Drawing.Imaging.ImageFormat.Jpeg);
                        break;

                    case 2:
                        imageArea.Image.Save(fs,
                          System.Drawing.Imaging.ImageFormat.Png);
                        break;

                    case 3:
                        imageArea.Image.Save(fs,
                          System.Drawing.Imaging.ImageFormat.Gif);
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
            var ph = Convertors.Bitmap2Photo((Bitmap)imageArea.Image);
            result = filter.Process(ph, data);
            var resultBmp = Convertors.Photo2Bitmap(result);
            if (resultBmp.Width > originalBmp.Width || resultBmp.Height > originalBmp.Height)
            {
                float k = Math.Min((float)originalBmp.Width / resultBmp.Width, (float)originalBmp.Height / resultBmp.Height);
                var newBmp = new Bitmap((int)(resultBmp.Width * k), (int)(resultBmp.Height * k));
                using (var g = Graphics.FromImage(newBmp))
                {
                    g.DrawImage(resultBmp, new Rectangle(0, 0, newBmp.Width, newBmp.Height), new Rectangle(0, 0, resultBmp.Width, resultBmp.Height), GraphicsUnit.Pixel);
                }
                resultBmp = newBmp;
            }
            imageArea.Image = resultBmp;
        }

    }
}
