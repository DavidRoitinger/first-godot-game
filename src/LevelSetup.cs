using System.Linq;
using Godot;
using Godot.Collections;

namespace FirstGodotGame;

public partial class LevelSetup : Node
{
    [Export] public int EnemyCount { get; set; }
    [Export] public float LevelDifficulty { get; set; }
    [Export] public Array<PackedScene> Enemies { get; set; }

    private TileMapLayer _groundLayer;
    public TileMapLayer GroundLayer
    {
        get
        {
            _groundLayer ??= GetTree()
                .GetNodesInGroup("Tilemap")
                .First(node => node.Name == "GroundLayer") as TileMapLayer;
            return _groundLayer;
        }
    }

    public override void _Ready()
    {
        SetupLevel();
    }

    public void SetupLevel()
    {
        var node = GetNode("../Entities");
        for (int i = 0; i < EnemyCount; i++)
        {
            var instantce = Enemies[0].Instantiate();

            instantce.GetChildren()
                .OfType<EntityStats>()
                .First()
                .GridPosition = GroundLayer.GetUsedCells().PickRandom();
            
            node.AddChild(instantce);
        }
    }
}