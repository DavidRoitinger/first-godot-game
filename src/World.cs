using System.Collections.Generic;

namespace FirstGodotGame;

public class World
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int LevelCount { get; set; }
    public List<int> NextWorld { get; set; }
}