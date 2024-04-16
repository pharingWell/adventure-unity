using GBD.SaveSystem;
using UnityEngine; //https://github.com/GabrielBigardi/Generic-Save-System/blob/main/DOCUMENTATION.md

public static class GameSecrets
{
    public const string SAVE_KEY = "6c1996cf353b4593cb393055e1c0c27c";
}

public class SaveStateSingleton
{
    private static SaveState _instance;

    public static SaveState GetInstance()
    {
        if (_instance == null) {_instance = new SaveState();}
        return _instance;
    }
}

public class SaveState
{
    [SerializeField] public Unit player;
    public SaveState()
    {
       // player.Save(this);
    }
}

public class Conduit<T>
{
    public readonly System.Action<T> Set;
    public readonly System.Func<T> Get;
 
    public Conduit(System.Action<T> set, System.Func<T> get)
    {
        this.Set = set;
        this.Get = get;
    }
}

public class SaveManager
{
    
}