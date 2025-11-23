using Godot;
using System;

public partial class Test : TileMapLayer
{
	[Export] public TileMapLayer trees;

	public override bool _UseTileDataRuntimeUpdate(Vector2I coords)
	{
		Godot.Collections.Array<Vector2I> cells = trees.GetUsedCellsById(0);

		foreach (var cell in cells)
		{
			if (cell == coords)
				return true;
		}
		return false;
	}

	public override void _TileDataRuntimeUpdate(Vector2I coords, TileData tileData)
	{
		Godot.Collections.Array<Vector2I> cells = trees.GetUsedCellsById(0);

		foreach (var cell in cells)
		{
			if (cell == coords)
				tileData.SetNavigationPolygon(0, null);
		}
	}
}