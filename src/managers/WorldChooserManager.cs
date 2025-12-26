using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

namespace FirstGodotGame;

public partial class WorldChooserManager : CanvasLayer
{
        
    public static WorldChooserManager Instance { get; private set; }

    public override void _Ready()
    {
        Instance = this;
        _animationPlayer = GetChildren().OfType<AnimationPlayer>().First();
    }
    
    private AnimationPlayer _animationPlayer;
    
    
    [Export] public Godot.Collections.Dictionary<int, Theme> graphThemes;
    
    
    private CanvasItem _worldContainer;
    private GraphEdit _graphEdit;
    
    
    private TaskCompletionSource _worldSelected =  new ();

    public async Task OpenWorldChooser()
    {
        LoadProperties();
        PopulateWorldContainer();
        PopulateGraphEdit();
        
        Visible = true;
        
        _animationPlayer.Play("Open");
        await ToSignal(_animationPlayer, "animation_finished");
        
        await _worldSelected.Task;
        
        //Close World Chooser...
        _animationPlayer.PlayBackwards("Open");
        await ToSignal(_animationPlayer, "animation_finished");
        Visible = false;
        ClearContainer(_worldContainer);
        _worldSelected = new TaskCompletionSource();
    }

    private void PopulateWorldContainer()
    {
        var nextWorlds = WorldMap.Instance.GetActiveWorld()
            .NextWorld
            .Select(id => WorldMap.Instance.GetWorldById(id));

        Dictionary<int, StringName> graphConnectionPoints = [];
        

        foreach (var world in WorldMap.Instance.WorldList)
        {

            float x = 200 * (world.Id / 10);
            float y = 20 + (100 * (world.Id % 10));

            var pos = new Vector2(x, y);
            
            var graphNode = new GraphNode()
            {
                PositionOffset = pos,
                Title = world.Name,
                Selectable = false,
                Resizable = false,
                Draggable = false,
                
            };
            var label = new Label()
            {
                Text = world.Description
            };
            graphNode.AddChild(label);

            if (WorldMap.Instance.ActiveWorldId == world.Id) graphNode.Theme = graphThemes[0];
            

            graphNode.SetSlot(0, true, 0, Color.Color8(1, 1, 1), 
                true, 0, Color.Color8(1, 1, 1));
            
            _graphEdit.AddChild(graphNode);
            
            graphConnectionPoints.Add(world.Id, graphNode.Name);
        }
        

        foreach (var world in WorldMap.Instance.WorldList)
        {
            foreach (var nextWorldId in world.NextWorld)
            {
                if(nextWorldId == -1) continue;
                _graphEdit.ConnectNode(graphConnectionPoints[world.Id], 0, graphConnectionPoints[nextWorldId], 0);
            }
        }
    }
    
    private void PopulateGraphEdit()
    {
        var nextWorlds = WorldMap.Instance.GetActiveWorld()
            .NextWorld
            .Select(id => WorldMap.Instance.GetWorldById(id));


        foreach (var nextWorld in nextWorlds)
        {
            var button = new Button()
            {
                Text = nextWorld.Name,
                CustomMinimumSize = new Vector2(250, 0),
                TooltipText = nextWorld.Description,
            };
            button.Pressed += () => WorldButton(nextWorld);

            //button.Theme = UpgradeThemes[upgrade.UpgradeQuality];
            
            _worldContainer.AddChild(button); 
            
        }
    }

    private void LoadProperties()
    {
        _worldContainer = GetTree().GetFirstNodeInGroup("WorldContainer") as CanvasItem;
        _graphEdit = GetChildren().OfType<GraphEdit>().First();
    }

    private void ClearContainer(Node container)
    {
        foreach (Node child in container.GetChildren())
        {
            child.QueueFree();
        }
    }

    
    private void WorldButton(World nextWorld)
    {
        WorldMap.Instance.ActiveWorldId = nextWorld.Id;
        WorldMap.Instance.LevelIndex = 0;
        
        _worldSelected.SetResult();
    }


    public void _on_graph_edit_connection_request(StringName from_node, int from_port, StringName to_node, int to_port)
    {
        GD.Print($"{from_node};{from_port};{to_node};{to_port}");
    }
}