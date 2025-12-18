using System.Collections.Generic;
using Godot;
using Godot.Collections;

namespace FirstGodotGame;

public partial class Level
{
    public string Name { get; set; }
    public float Difficulty { get; set; }
    public List<PackedScene> Enemies { get; set; }
    public int SafetyRange { get; set; }
}