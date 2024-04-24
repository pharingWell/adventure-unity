namespace IFSKSTR.SaveSystem
{
    public interface ISavable
    {
        public void OnLoad();
        public void OnSave();
    }
}