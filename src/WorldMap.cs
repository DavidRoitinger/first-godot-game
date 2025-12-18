using System.Collections.Generic;
using System.Linq;

namespace FirstGodotGame;

public class WorldMap
{
    private static WorldMap _instance;
    public static WorldMap Instance
    {
        get
        {
            _instance ??= new WorldMap();
            return _instance;
        }
    }
    

    public int ActiveWorldId = 10;
    
    public int LevelIndex;

    public World GetWorldById(int id)
    {
        return WorldList
            .First(x => x.Id == id);
    }
    
    public World GetActiveWorld()
    {
        return WorldList
            .First(x => x.Id == ActiveWorldId);
    }

    public List<World> WorldList = [
        new World() {
            Id = 10,
            Name = "First World",
            Description = "Hello Test",
            LevelCount = 4,
            NextWorld = [20, 21],
        },
        new World() {
            Id = 20,
            Name = "Second Easy World",
            Description = "Hello Test",
            LevelCount = 4,
            NextWorld = [30],
        },
        new World() {
            Id = 21,
            Name = "Second Hard World",
            Description = "Hello Test",
            LevelCount = 4,
            NextWorld = [30],
        },
        new World() {
            Id = 30,
            Name = "Third World",
            Description = "Hello Test",
            LevelCount = 4,
            NextWorld = [-1],
        },
    ];

}