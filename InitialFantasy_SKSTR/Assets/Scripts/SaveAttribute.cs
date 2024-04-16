using System;

public class Conduit<T>{
public readonly System.Action<T> Set;
public readonly System.Func<T> Get;
 
    public Conduit(System.Action<T> set, System.Func<T> get)
    {
        this.Set = set;
        this.Get = get;
    }
}