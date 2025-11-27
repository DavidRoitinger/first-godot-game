using System;
using System.Collections.Generic;
using Godot;

namespace FirstGodotGame;

public partial class SceneManager: Node
{
    private int _currentLevelIndex = 0;
    private readonly List<string> _levels =
    [
        "main_scene",
        "test",
        "main_scene",
        "test",
        "main_scene",
        "test",
        "main_scene",
        "test",
    ];
    
    public void LoadScene(string sceneName)
    {
        var scene = ResourceLoader.Load<PackedScene>($"res://scenes/{sceneName}.tscn");
        GetTree().ChangeSceneToPacked(scene);
    }
    public static SceneManager Instance { get; private set; }

    public override void _Ready()
    {
        Instance = this;
    }
    public void LoadNextLevel(int count = 1)
    {
        _currentLevelIndex = Math.Min(_currentLevelIndex + count, _levels.Count - 1);
        LoadScene(_levels[_currentLevelIndex]);
    }
    public void ReloadCurrentLevel()
    {
        GetTree().ReloadCurrentScene();
    }
    public void LoadPreviousLevel(int count = 1)
    {
        _currentLevelIndex = Math.Max(_currentLevelIndex - count, 0);
        LoadScene(_levels[_currentLevelIndex]);
    }
}