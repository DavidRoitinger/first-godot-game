using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

namespace FirstGodotGame;

public partial class SceneManager: Node
{
    private int _currentLevelIndex = 0;
    private readonly List<string> _levels =
    [
        "main_scene",
        "main_scene",
        "main_scene",
        "test",
        "test",
        "test",
        "main_scene",
        "test",
    ];
    
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
        _currentLevelIndex = Math.Min(_currentLevelIndex + count, _levels.Count - 1);
        await LoadScene(_levels[_currentLevelIndex]);
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
        _currentLevelIndex = Math.Max(_currentLevelIndex - count, 0);
        await LoadScene(_levels[_currentLevelIndex]);
    }
}