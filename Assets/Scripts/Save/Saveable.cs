public abstract class Saveable
{
    //Will be the path of the child 
    public string path { get; private set; }

    protected Saveable(string path)
    {
        this.path = path;
    }

}
