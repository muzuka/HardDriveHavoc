
using UnityEngine.UI;

public class Game 
{
    public Image icon { get; set; }
    public string name { get; set; }
    public int space { get; set; }

    public Game(string name, int space)
    {
        this.name = name;
        this.space = space;
    }
}
