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
            undoBuffer = new List<State>();
            canUndo = false;
            canRedo = false;
        }

        private const int ACTION_MEMORY_CAPACITY = 30;

        private bool canUndo;
        public bool CanUndo { get { return canUndo; } }
        private bool canRedo;
        public bool CanRedo { get { return canRedo; } }

        private List<State> undoBuffer;
        private int bufferPointer;

        public void ClearMemento(State initialState)
        {
            canUndo = false;
            canRedo = false;
            undoBuffer.Clear();
            undoBuffer.Add(initialState);
            bufferPointer = 1;
        }

        public void DoAction(State currentState)
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
            undoBuffer.Add(currentState);
            bufferPointer++;
            updateCanUndoRedo();
        }

        public State UndoAction(State currentState)
        {
            if (bufferPointer == undoBuffer.Count)
            {
                undoBuffer.Add(currentState);
            }
            if (canUndo)
            {
                bufferPointer--;
                updateCanUndoRedo();
                return undoBuffer.ElementAt<State>(bufferPointer);
            }
            return null;
        }

        public State RedoAction()
        {
            if (canRedo)
            {
                bufferPointer++;
                updateCanUndoRedo();
                return undoBuffer.ElementAt<State>(bufferPointer);
            }
            return null;
        }

        private void updateCanUndoRedo(){
            canUndo = bufferPointer > 1;
            canRedo = bufferPointer < undoBuffer.Count - 1;
        }
    }
}
