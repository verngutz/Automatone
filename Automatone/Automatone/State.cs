namespace Automatone
{
    public class State
    {
        private CellState[,] songCells;
        public CellState[,] SongCells
        {
            get
            {
                if (songCells != null)
                {
                    songCells = (CellState[,])songCells.Clone();
                }
                return songCells;
            }
        }
        private int topStartCursorIndex;
        public int TopStartCursorIndex { get { return topStartCursorIndex; } }
        private int topEndCursorIndex;
        public int TopEndCursorIndex { get { return topEndCursorIndex; } }
        private int leftStartCursorIndex;
        public int LeftStartCursorIndex { get { return leftStartCursorIndex; } }
        private int leftEndCursorIndex;
        public int LeftEndCursorIndex { get { return leftEndCursorIndex; } }

        public State(CellState[,] songCells, int topStartCursorIndex, int topEndCursorIndex, int leftStartCursorIndex, int leftEndCursorIndex)
        {
            if (songCells != null)
            {
                songCells = (CellState[,])songCells.Clone();
            }
            this.songCells = songCells;
            this.topStartCursorIndex = topStartCursorIndex;
            this.topEndCursorIndex = topEndCursorIndex;
            this.leftStartCursorIndex = leftStartCursorIndex;
            this.leftEndCursorIndex = leftEndCursorIndex;
        }
    }
}
