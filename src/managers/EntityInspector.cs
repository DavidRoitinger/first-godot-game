using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

namespace FirstGodotGame;

public partial class EntityInspector : CanvasLayer
{
    public static EntityInspector Instance { get; private set; }

    public override void _Ready()
    {
        Instance = this;
        _animationPlayer = GetChildren().OfType<AnimationPlayer>().First();
    }
    
    private AnimationPlayer _animationPlayer;
    
    
    private EntityStats _entityStats;
    private CanvasItem _statContainer;
    private CanvasItem _effectContainer;
    private CanvasItem _upgradeContainer;
    private TextureRect _entityTexture;

    public bool IsInspectorOpen;
    private bool _isAnimationFinished;
    private TaskCompletionSource _exitInspector =  new ();

    public async Task OpenEntityInspector(Node entity)
    {
        LoadProperties(entity);
        PopulateStatContainer(_entityStats);
        PopulateUpgradeContainer(_entityStats);
        PopulateEntityTexture(entity);
        PopulateEffectContainer(_entityStats);

        await OpenInspector();
        
        await _exitInspector.Task;
        
        await CloseInspector();
    }

    private async Task OpenInspector()
    {
        Visible = true;
        SoundManager.Instance.PlaySfx(SoundManager.Inspect);
        _animationPlayer.Play("Open");
        await ToSignal(_animationPlayer, "animation_finished");
        _isAnimationFinished = true;
    }

    private async Task CloseInspector()
    {
        SoundManager.Instance.PlaySfx(SoundManager.Inspect);
        _animationPlayer.PlayBackwards("Open");
        await ToSignal(_animationPlayer, "animation_finished");
        Visible = false;
        ClearContainer(_statContainer);
        ClearContainer(_effectContainer);
        ClearContainer(_upgradeContainer);
        _exitInspector = new TaskCompletionSource();
        IsInspectorOpen = false;
        _isAnimationFinished = false;
    }

    private void PopulateEffectContainer(EntityStats entityStats)
    {
        foreach (var effect in entityStats.Effects)
        {
            var texture = new TextureRect()
            {
                CustomMinimumSize = new Vector2(100, 100),
                Texture = ResourceLoader.Load<Texture2D>($"res://assets/sprites/effectIcons/{effect.IconPath}"),
                TooltipText = $"{effect.Name}: {effect.Description}",
            };
            
            _effectContainer.AddChild(texture);
        }
    }

    private void PopulateEntityTexture(Node entity)
    {
        var entitySprite = entity.GetChildren().OfType<Sprite2D>().First();

        _entityTexture.Texture = entitySprite.Texture;
        _entityTexture.Material = entitySprite.Material;
    }

    private void PopulateStatContainer(EntityStats entityStats)
    {

        var stats = new Dictionary<string, string>{
            { "Name", entityStats.EntityName },
            { "Heath", $"{entityStats.Health}/{entityStats.MaxHealth}" },
            { "Amor", $"{entityStats.Armor}" },
            { "Speed", $"{entityStats.Speed}" },
        };
        
        var lableSettings = new LabelSettings()
        {
            FontSize = 48,
            OutlineColor = new Color(0, 0, 0),
            OutlineSize = 20,
        };
        
        foreach (var keyValuePair in stats)
        {
            var label = new Label()
            {
                Text = $"{keyValuePair.Key}: {keyValuePair.Value}"
            };
            label.LabelSettings = lableSettings;
            
            _statContainer.AddChild(label);
        }
        
    }
    
    private void PopulateUpgradeContainer(EntityStats entityStats)
    {
        
        foreach (int upgradeId in entityStats.UpgradeIds)
        {
            var upgrade = UpgradePool.Instance.GetUpgradeById(upgradeId);

            var container = new CenterContainer();

            Color qualityColor = new Color(0,0,0);

            switch (upgrade.UpgradeQuality)
            {
                case Upgrade.Quality.Common:
                    qualityColor = new Color(1, 1, 1);
                    break;
                case Upgrade.Quality.Strange: 
                    qualityColor = new Color(1, 0, 0);
                    break;
                case Upgrade.Quality.Bizarre: 
                    qualityColor = new Color(1, 0, 1);
                    break;
            }

            var background = new ColorRect()
            {
                CustomMinimumSize = new Vector2(200, 200),
                Color = qualityColor,
            };
                
            
            var texture = new TextureRect()
            {
                CustomMinimumSize = new Vector2(170, 170),
                Texture = ResourceLoader.Load<Texture2D>($"res://assets/sprites/upgradeIcons/{upgrade.IconPath}"),
                TooltipText = $"{upgrade.Name}: {upgrade.Description}" ,
                
            };
            
            container.AddChild(background);
            container.AddChild(texture);

            _upgradeContainer.AddChild(container);
        }
        
    }


    private void LoadProperties(Node entity)
    {
        IsInspectorOpen = true;
        _entityStats = entity.GetChildren().OfType<EntityStats>().First();
        _statContainer = GetTree().GetFirstNodeInGroup("StatContainer") as CanvasItem;
        _effectContainer = GetTree().GetFirstNodeInGroup("EffectContainer") as CanvasItem;
        _upgradeContainer = GetTree().GetFirstNodeInGroup("UpgradeContainer") as CanvasItem;
        _entityTexture = GetTree().GetFirstNodeInGroup("EntitySprite") as TextureRect;
    }

    private void ClearContainer(Node container)
    {
        foreach (Node child in container.GetChildren())
        {
            child.QueueFree();
        }
    }
    
    public override void _Input(InputEvent @event)
    {
        if ((@event.IsActionPressed("Cancel") || @event.IsActionPressed("Right_Click")) && _isAnimationFinished && !_exitInspector.Task.IsCompleted)
        {
            _exitInspector.SetResult();
        }
    }
}