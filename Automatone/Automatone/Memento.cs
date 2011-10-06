using System.Collections.Generic;
using System.Linq;

namespace Automatone
{
    public class Memento
    {
        private static Memento instance;
        public static Memento Instance
        {
            get
            {
                if (instance == null)
                    instance = new Memento();
                return instance;
            }
        }
        private Memento()
        {
            undoBuffer = new List<CellState[,]>();
            canUndo = false;
            canRedo = false;
        }

        private const int ACTION_MEMORY_CAPACITY = 30;

        private bool canUndo;
        public bool CanUndo { get { return canUndo; } }
        private bool canRedo;
        public bool CanRedo { get { return canRedo; } }

        private List<CellState[,]> undoBuffer;
        private int bufferPointer;

        public void ClearMemento(CellState[,] initialState)
        {
            canUndo = false;
            canRedo = false;
            undoBuffer.Clear();
            undoBuffer.Add((CellState[,])initialState.Clone());
            bufferPointer = 1;
        }

        public void DoAction(CellState[,] currentState)
        {
            while (undoBuffer.Count >= ACTION_MEMORY_CAPACITY)
            {
                undoBuffer.RemoveAt(0);
                bufferPointer--;
            }
            while (undoBuffer.Count > bufferPointer)
            {
                undoBuffer.RemoveAt(bufferPointer);
            }
            undoBuffer.Add((CellState[,])currentState.Clone());
            bufferPointer++;
            updateCanUndoRedo();
        }

        public CellState[,] UndoAction(CellState[,] currentState)
        {
            if (bufferPointer == undoBuffer.Count)
            {
                undoBuffer.Add((CellState[,])currentState.Clone());
            }
            if (canUndo)
            {
                bufferPointer--;
                updateCanUndoRedo();
                return (CellState[,])undoBuffer.ElementAt<CellState[,]>(bufferPointer).Clone();
            }
            return null;
        }

        public CellState[,] RedoAction()
        {
            if (canRedo)
            {
                bufferPointer++;
                updateCanUndoRedo();
                return (CellState[,])undoBuffer.ElementAt<CellState[,]>(bufferPointer).Clone();
            }
            return null;
        }

        private void updateCanUndoRedo(){
            canUndo = bufferPointer > 1;
            canRedo = bufferPointer < undoBuffer.Count - 1;
        }
    }
}
