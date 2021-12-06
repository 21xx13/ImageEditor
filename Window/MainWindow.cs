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
        PictureBox original;
        PictureBox processed;
        ComboBox filtersSelect;
        Panel parametersPanel;
        List<TrackBar> parametersControls;
        Button apply;
        Button select;
        Button save;
        Button originalBtn;

        public MainWindow()
        {
            original = new PictureBox();
            Controls.Add(original);

            processed = new PictureBox();
            Controls.Add(processed);

            filtersSelect = new ComboBox();
            filtersSelect.DropDownStyle = ComboBoxStyle.DropDownList;
            filtersSelect.SelectedIndexChanged += FilterChanged;
            Controls.Add(filtersSelect);

            select = new Button();
            select.Text = "Выбрать изображение";
            select.Click += LoadPhoto;
            Controls.Add(select);

            save = new Button();
            save.Text = "Сохранить как";
            save.Click += SaveImage;
            Controls.Add(save);

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

            original.Image = originalBmp;
            original.Left = 0;
            original.Top = 0;
            original.ClientSize = new Size(800, 600);
            original.SizeMode = PictureBoxSizeMode.Zoom;

            //processed.Left = 0;
            //processed.Top = original.Bottom;
            //processed.Size = original.Size;
            //processed.SizeMode = PictureBoxSizeMode.Zoom;

            filtersSelect.Left = original.Right + 10;
            filtersSelect.Top = 20;
            filtersSelect.Width = 200;
            filtersSelect.Height = 20;


            ClientSize = new Size(filtersSelect.Right + 20, original.Bottom);

            apply.Left = ClientSize.Width - 120;
            apply.Top = ClientSize.Height - 50;
            apply.Width = 100;
            apply.Height = 40;

            select.Left = ClientSize.Width - 120;
            select.Top = ClientSize.Height - 100;
            select.Width = 100;
            select.Height = 40;

            save.Left = ClientSize.Width - 120;
            save.Top = ClientSize.Height - 150;
            save.Width = 100;
            save.Height = 40;

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
            original.Image = originalBmp;
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

                //var box = new NumericUpDown();
                //box.Left = 0;
                //box.Top = label.Bottom;
                //box.Width = parametersPanel.Width;
                //box.Height = 20;
                //box.Value = (decimal)param.DefaultValue;
                //box.Increment = (decimal)param.Increment / 3;
                //box.Maximum = (decimal)param.MaxValue;
                //box.Minimum = (decimal)param.MinValue;
                //box.DecimalPlaces = 2;
                parametersPanel.Controls.Add(box);
                y += label.Height + 65;
                parametersControls.Add(box);
            }
            Controls.Add(parametersPanel);
        }

        void ScrollLabel(object sender, EventArgs empty) {
            
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
                        original.Image.Save(fs,
                          System.Drawing.Imaging.ImageFormat.Jpeg);
                        break;

                    case 2:
                        original.Image.Save(fs,
                          System.Drawing.Imaging.ImageFormat.Png);
                        break;

                    case 3:
                        original.Image.Save(fs,
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
            var ph = Convertors.Bitmap2Photo((Bitmap)original.Image);
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
            original.Image = resultBmp;
        }
    }
}
