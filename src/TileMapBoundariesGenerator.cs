using Godot;

namespace FirstGodotGame;

public partial class TileMapBoundariesGenerator : TileMapLayer
{

	public override void _Ready()
	{
		var filledTiles = GetUsedCells();

		foreach (var filledTile in filledTiles)
		{
			var surroundingTiles = GetSurroundingCells(filledTile);
			foreach (var surroundingTile in surroundingTiles)
			{
				if (GetCellSourceId(surroundingTile) == -1)
				{
					SetCell(surroundingTile, 3, new Vector2I(3, 1));
					
				}
			}
		}

		
	}
	
}