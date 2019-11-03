namespace XTCode {
    public class ChunkData
    {
        private int x, y, z;
 
        public ChunkData(int _x, int _y, int _z)
        {
            this.x = _x;
            this.y = _y;
            this.z = _z;
        }
 
        public int X
        {
            get { return this.x; }
            set { this.x = value; }
        }
 
        public int Y
        {
            get { return this.y; }
            set { this.y = value; }
        }
 
        public int Z
        {
            get { return this.z; }
        }
 
        public override string ToString()
        {
            return string.Format("Chunk ( {0},{1},{2} )", this.x, this.y, this.z);
        }
    }
}