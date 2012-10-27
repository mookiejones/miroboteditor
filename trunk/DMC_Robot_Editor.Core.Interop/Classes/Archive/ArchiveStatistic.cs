namespace DMC_Robot_Editor.Classes.Archive
{
    public class ArchiveStatistic
    {
        private int OriginalLength { get; set; }
        private int CompressedLength { get; set; }
        private int Files { get;  set; }
        private int Folders { get;  set; }

        public int Ratio
        {
            get
            {
                if (OriginalLength > 0)
                {
                    return 100 - (int)((double)CompressedLength * 100.0 / (double)OriginalLength);
                }
                return 0;
            }
        }

        public ArchiveStatistic(int originalLength, int compressedLength)
        {
            OriginalLength = originalLength;
            CompressedLength = compressedLength;
            Files = 1;
        }
        public ArchiveStatistic(int originalLength, int compressedLength, int files, int folders)
        {
            OriginalLength = originalLength;
            CompressedLength = compressedLength;
            Folders = folders;
            Files = files;
        }
    }
}
