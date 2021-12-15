using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyPhotoshop
{
    public class UndoRedoHistory<T>
    {
        private bool inUndoRedo = false;
        readonly private Stack<T> undoStack = new Stack<T>();
        readonly private Stack<T> redoStack = new Stack<T>();
        public bool IsEmptyUndo { get => undoStack.Count == 1; }
        public bool IsEmptyRedo { get => redoStack.Count == 0; }

        public T Undo()
        {
            inUndoRedo = true;
            T top = undoStack.Pop();
            redoStack.Push(top);
            inUndoRedo = false;
            return undoStack.Peek();
        }

        public T Redo()
        {
            inUndoRedo = true;
            T top = redoStack.Pop();
            undoStack.Push(top);
            inUndoRedo = false;
            return top;
        }

        public void Do(T m)
        {
            if (inUndoRedo)
                throw new InvalidOperationException(
                    "Выполняется действие отмены/возврата.");
            redoStack.Clear();
            undoStack.Push(m);
        }
    }
}
