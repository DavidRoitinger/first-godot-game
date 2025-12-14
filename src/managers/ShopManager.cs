using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Godot;

namespace FirstGodotGame;

public partial class ShopManager : CanvasLayer
{
    
    public static ShopManager Instance { get; private set; }

    public override void _Ready()
    {
        Instance = this;
    }

    private Upgrade _selectedUpgrade;
    private CanvasItem _itemContainer;
    private CanvasItem _playerContainer;
    private CanvasItem _rerollContainer;
    
    private List<EntityStats> _playerStats;
    private TaskCompletionSource _upgradeSelected =  new ();

    public async Task OpenShop()
    {
        Visible = true;
        
        LoadProperties();

        PopulatePlayerContainer();
        PopulateItemContainer();
        PopulateRerollContainer();

        await _upgradeSelected.Task;
        
        //Close Shop...
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
        var upgrades = GetRandomUpgrades();


        foreach (var upgrade in upgrades)
        {
            var button = new Button()
            {
                Text = upgrade.Name,
                CustomMinimumSize = new Vector2(250, 0),
                TooltipText = upgrade.Description,
            };
            button.Pressed += () => UpgradeButton(upgrade);

            //Todo:Set Color...
            
            // switch (upgrade.UpgradeQuality)
            // {
            //     case Upgrade.Quality.Common:
            //         
            // }
            
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

    private Upgrade[] GetRandomUpgrades()
    {
        var rand = new Random();
        var upgrades = rand.GetItems(new ReadOnlySpan<Upgrade>(UpgradePool.Instance.CommonUpgradePool.ToArray()), 3);
        return upgrades;
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