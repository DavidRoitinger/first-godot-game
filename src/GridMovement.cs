using System.Linq;
using Godot;

namespace FirstGodotGame;

public partial class GridMovement : Node2D
{
	private EntityStats _entityStats;

	private Vector2I _lastHighlightPosition = Vector2I.Zero;
	private Vector2I _lastHighlightAtlasCoords = new Vector2I(-1, -1);

	private TileMapLayer _groundLayer;
	private TileMapLayer _highlightLayer;
	
	public override void _Ready()
	{
		_entityStats = GetChildren().OfType<EntityStats>().Single();
		_groundLayer = GetTree().GetNodesInGroup("Tilemap").First(node => node.Name == "GroundLayer") as TileMapLayer;
		_highlightLayer = GetTree().GetNodesInGroup("Tilemap").First(node => node.Name == "HighlightLayer") as TileMapLayer;
	}
	
	public override void _Process(double delta)
	{
		var currentMousePosition = _groundLayer.LocalToMap(_groundLayer.ToLocal(GetGlobalMousePosition()));
		
		if (_lastHighlightPosition == currentMousePosition) return;
		
		_highlightLayer.SetCell(
			_lastHighlightPosition,
			1,
			_lastHighlightAtlasCoords);
		
		_lastHighlightPosition = currentMousePosition;
		_lastHighlightAtlasCoords = _highlightLayer.GetCellAtlasCoords(currentMousePosition);
		
		_highlightLayer.SetCell(
			currentMousePosition,
			1,
			new Vector2I(0, 0));

		
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseButton mouseEvent && @event.IsPressed() && mouseEvent.ButtonIndex == MouseButton.Left)
		{
			_entityStats.GridPosition = _groundLayer.LocalToMap(_groundLayer.ToLocal(GetGlobalMousePosition()));
			
			Position = _groundLayer.MapToLocal(_entityStats.GridPosition);

		}
	}
}
