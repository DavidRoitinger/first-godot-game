using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;


namespace FirstGodotGame;

public partial class ShopManager : CanvasLayer
{
    
    public static ShopManager Instance { get; private set; }

    public override void _Ready()
    {
        Instance = this;
        _animationPlayer = GetChildren().OfType<AnimationPlayer>().First();
    }
    
    private AnimationPlayer _animationPlayer;

    [Export] public Godot.Collections.Dictionary<Upgrade.Quality, Theme> UpgradeThemes;

    private Upgrade _selectedUpgrade;
    private CanvasItem _itemContainer;
    private CanvasItem _playerContainer;
    private CanvasItem _rerollContainer;
    
    private List<EntityStats> _playerStats;
    private TaskCompletionSource _upgradeSelected =  new ();

    public async Task OpenShop()
    {
        LoadProperties();

        PopulatePlayerContainer();
        PopulateItemContainer();
        PopulateRerollContainer();
        Visible = true;
        
        _animationPlayer.Play("Open");
        await ToSignal(_animationPlayer, "animation_finished");
        

        await _upgradeSelected.Task;
        
        //Close Shop...
        _animationPlayer.PlayBackwards("Open");
        await ToSignal(_animationPlayer, "animation_finished");
        Visible = false;
        ClearContainer(_itemContainer);
        ClearContainer(_playerContainer);
        ClearContainer(_rerollContainer);
        _upgradeSelected = new TaskCompletionSource();
    }

    private void PopulateRerollContainer()
    {
        var rerollButton = new Button()
        {
            Text = "Reroll",
        };
        rerollButton.Pressed += RerollButton;
        _rerollContainer.AddChild(rerollButton);
    }

    private void PopulateItemContainer()
    {
        var upgrades = GetRandomUpgrades(3);


        foreach (var upgrade in upgrades)
        {
            var button = new Button()
            {
                Text = upgrade.Name,
                CustomMinimumSize = new Vector2(250, 0),
                TooltipText = upgrade.Description,
            };
            button.Pressed += () => UpgradeButton(upgrade);

            button.Theme = UpgradeThemes[upgrade.UpgradeQuality];
            
            _itemContainer.AddChild(button); 
            
        }
    }

    private void LoadProperties()
    {
        _playerStats = GetTree().GetNodesInGroup("Player")
            .Select(node => node.GetChildren().OfType<EntityStats>().First())
            .ToList();

        _itemContainer = GetTree().GetFirstNodeInGroup("ItemContainer") as CanvasItem;
        _playerContainer = GetTree().GetFirstNodeInGroup("PlayerContainer") as CanvasItem;
        _rerollContainer = GetTree().GetFirstNodeInGroup("RerollContainer") as CanvasItem;
    }

    private void ClearContainer(Node container)
    {
        foreach (Node child in container.GetChildren())
        {
            child.QueueFree();
        }
    }

    private List<Upgrade> GetRandomUpgrades(int count)
    {
        var rand = new Random();

        List<Upgrade> shopUpgrades = [];

        for (int i = 0; i < count; i++)
        {
            float num = rand.NextSingle();
            if (num <= 0.65)
            {
                //common
                shopUpgrades.Add(GetRandomUnpickedUpgrade(shopUpgrades, UpgradePool.Instance.CommonUpgradePool));
            }else if (num <= 0.85)
            {
                //strange
                shopUpgrades.Add(GetRandomUnpickedUpgrade(shopUpgrades, UpgradePool.Instance.StrangeUpgradePool));
            }else
            {
                //bizarre
                shopUpgrades.Add(GetRandomUnpickedUpgrade(shopUpgrades, UpgradePool.Instance.BizarreUpgradePool));
            }
        }
        
        //var upgrades = rand.GetItems(new ReadOnlySpan<Upgrade>(UpgradePool.Instance.CommonUpgradePool.ToArray()), 3);
        return shopUpgrades;
    }

    private Upgrade GetRandomUnpickedUpgrade(List<Upgrade> pickedUpgrades, List<Upgrade> upgradePool)
    {
        Random rand = new Random();
        var unpickedUpgrades = upgradePool
            .Where(x => !pickedUpgrades.Contains(x))
            .ToList();
                
        return unpickedUpgrades[rand.Next(unpickedUpgrades.Count)];
    }

    private void PopulatePlayerContainer()
    {
        foreach (var stats in _playerStats)
        {
            var button = new Button()
            {
                Text = stats.EntityName,
                CustomMinimumSize = new Vector2(150, 0),
            };
            button.Pressed += () => PlayerButton(stats);
            
            _playerContainer.AddChild(button); 
        }
    }

    private void UpgradeButton(Upgrade upgrade)
    {
        _playerContainer.Visible = true;

        _selectedUpgrade = upgrade;
        
    }
    private void PlayerButton(EntityStats player)
    {
        _playerContainer.Visible = false;

        _selectedUpgrade.ApplyUpgrade(player);
        
        _upgradeSelected.SetResult();
    }
    private void RerollButton()
    {
        _playerContainer.Visible = false;

        ClearContainer(_itemContainer);
        PopulateItemContainer();
        
    }
}