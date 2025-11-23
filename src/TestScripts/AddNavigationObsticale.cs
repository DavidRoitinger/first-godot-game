using System.Linq;
using Godot;

namespace FirstGodotGame;

public partial class AddNavigationObsticale : Node2D
{
	private TileMapLayer GroundLayer;
	private TileMapLayer ObjectLayer;
	
	public override void _Ready()
	{
		// GroundLayer = GetChildren().First(node => node.GetName() == "GroundLayer") as TileMapLayer;
		// ObjectLayer = GetChildren().First(node => node.GetName() == "ObjectLayer") as TileMapLayer;
		//
		// var objects = ObjectLayer.GetUsedCells();
		// foreach (var obj in objects)
		// {
		// 	GroundLayer.GetCellTileData(obj).SetNavigationPolygon(0,null);
		// }
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}
	
	
}