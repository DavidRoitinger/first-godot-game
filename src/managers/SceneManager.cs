using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

namespace FirstGodotGame;

public partial class SceneManager: Node
{

    // private readonly List<string> _levels =
    // [
    //     "worlds/1_a/1_a_1",
    //     "worlds/1_a/1_a_2",
    //     "worlds/1_a/1_a_3",
    //     "worlds/1_a/1_a_4",
    //     "test"
    // ];
    
    // private Dictionary<string, int> _levels = new Dictionary<string, int>({
    //     { "1_a", 2 }
    // });
    
    private AnimationPlayer _transitionAnimationPlayer;
    
    public static SceneManager Instance { get; private set; }

    public override void _Ready()
    {
        Instance = this;
        _transitionAnimationPlayer = GetChildren().OfType<AnimationPlayer>().First();
    }
    
    public async Task LoadScene(string sceneName)
    {
        _transitionAnimationPlayer.Play("Fade");
        
        SaveLoadManager.Save(GetTree());
        
        await ToSignal(_transitionAnimationPlayer, "animation_finished");
        var scene = ResourceLoader.Load<PackedScene>($"res://scenes/{sceneName}.tscn");
        GetTree().ChangeSceneToPacked(scene);
        _transitionAnimationPlayer.PlayBackwards("Fade");
        await ToSignal(_transitionAnimationPlayer, "animation_finished");
    }
    
    public async Task LoadNextLevel(int count = 1)
    {
        var worldMap = WorldMap.Instance;
        if ((worldMap.LevelIndex + count) >= worldMap.GetActiveWorld().LevelCount)
        {
            //Todo: World selection comes here...

            await WorldChooserManager.Instance.OpenWorldChooser();
        }
        else
        {
            worldMap.LevelIndex += count;
        }

        string levelPath = $"worlds/{worldMap.ActiveWorldId}/{worldMap.ActiveWorldId}_{worldMap.LevelIndex}";
        
        GD.Print(levelPath);
        
        await LoadScene(levelPath);
    }
    public async Task ReloadCurrentLevel()
    {
        _transitionAnimationPlayer.Play("Fade");
        await ToSignal(_transitionAnimationPlayer, "animation_finished");
        GetTree().ReloadCurrentScene();
        _transitionAnimationPlayer.PlayBackwards("Fade");
        await ToSignal(_transitionAnimationPlayer, "animation_finished");
    }
    public async Task LoadPreviousLevel(int count = 1)
    {
        var worldMap = WorldMap.Instance;
        worldMap.LevelIndex = Math.Max(worldMap.LevelIndex - count, 0);
        
        string levelPath = $"worlds/{worldMap.ActiveWorldId}/{worldMap.ActiveWorldId}_{worldMap.LevelIndex}";
        
        GD.Print(levelPath);
        
        await LoadScene(levelPath);
    }
}