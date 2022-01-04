using System;
using System.Windows.Forms;

namespace MyPhotoshop
{
    
    public static class MenuStripCreater
    {
        public static MenuStrip MenuStrip { get; private set; }
        public static ToolStripMenuItem FileMenu { get; private set; }
        public static ToolStripMenuItem OpenItem { get; private set; }
        public static ToolStripMenuItem EditMenu { get; private set; }
        public static ToolStripMenuItem SaveItem { get; private set; }
        public static ToolStripMenuItem UndoItem { get; private set; }
        public static ToolStripMenuItem RedoItem { get; private set; }
        public static ToolStripMenuItem OriginalItem { get; private set; }
        public static ToolStripMenuItem CropItem { get; private set; }

        private static ToolStripMenuItem CreateToolStripItem(string text, EventHandler e)
        {
            var item = new ToolStripMenuItem { Text = text };
            item.Click += e;
            return item;
        }

        public static void CreateMainMenu(EventHandler save, EventHandler load, EventHandler undo, EventHandler redo,
            EventHandler original, EventHandler crop)
        {
            MenuStrip = new MenuStrip();
            FileMenu = CreateToolStripItem("Файл", null);
            EditMenu = CreateToolStripItem("Редактирование", null);
            OpenItem = CreateToolStripItem("Открыть", load);
            SaveItem = CreateToolStripItem("Сохранить", save);
            UndoItem = CreateToolStripItem("Шаг назад", undo);
            RedoItem = CreateToolStripItem("Шаг вперед", redo);
            OriginalItem = CreateToolStripItem("Исходное изображение", original);
            CropItem = CreateToolStripItem("Кадрировать", crop);
            MenuStrip.Items.AddRange(new ToolStripItem[] { FileMenu, EditMenu });
            FileMenu.DropDownItems.AddRange(new ToolStripItem[] { OpenItem, SaveItem });
            EditMenu.DropDownItems.AddRange(new ToolStripItem[] { UndoItem, RedoItem, OriginalItem, CropItem });
            UndoItem.ShortcutKeys = Keys.Control | Keys.Z;
            RedoItem.ShortcutKeys = Keys.Control | Keys.Y;
        }
    }
}
