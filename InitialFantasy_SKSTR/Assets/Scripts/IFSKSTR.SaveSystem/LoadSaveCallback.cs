namespace IFSKSTR.SaveSystem
{
    public class LoadSaveCallback
    {
        public delegate void Load();
        public delegate void Save();
        private readonly Load _loaded;
        private readonly Save _saved;
    
        public LoadSaveCallback(in Load onLoad, in Save onSave)
        {
            _loaded = onLoad;
            _saved = onSave;
        }

        public void Saved()
        {
            _saved();
        }
        
        public void Loaded()
        {
            _loaded();
        }

    }
}