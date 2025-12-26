using System;
using Godot;

namespace FirstGodotGame;

public partial class SoundManager : Node
{
    [Export] public AudioStreamPlayer MusicPlayer;
    [Export] public AudioStreamPlayer SfxPlayer;
    public static SoundManager Instance { get; private set; }

    public override void _Ready()
    {
        Instance = this;
    }

    public void PlayMusic(string musicPath)
    {
        var audioStream = ResourceLoader.Load<AudioStream>($"res://assets/audio/music/{musicPath}");

        if (MusicPlayer.Stream == audioStream) return;
        MusicPlayer.Stream = audioStream;
        MusicPlayer.Play();
    }    
    public void PlayMusic(string[] musicPaths)
    {
        var rand = new Random();
        var audioStream = ResourceLoader.Load<AudioStream>($"res://assets/audio/music/{
            musicPaths[rand.Next(musicPaths.Length)]
        }");
        
        if (MusicPlayer.Stream == audioStream) return;
        MusicPlayer.Stream = audioStream;
        MusicPlayer.Play();
    }
    
    public void PlaySfx(string sfxPath)
    {
        var audioStream = ResourceLoader.Load<AudioStream>($"res://assets/audio/sfx/{sfxPath}");
        
        // 0.0 -> 1.0   /5
        // 0.0 -> 0.2   +0.9
        // 0.9 -> 1.1   ???
        var rand = new Random();
        SfxPlayer.PitchScale = rand.NextSingle() / 5 + 0.9f;
        SfxPlayer.Stream = audioStream;
        SfxPlayer.Play();
    }
    public void PlaySfx(string[] sfxPaths)
    {
        var rand = new Random();
        var audioStream = ResourceLoader.Load<AudioStream>($"res://assets/audio/sfx/{
            sfxPaths[rand.Next(sfxPaths.Length)]
        }");
        
        // 0.0 -> 1.0   /5
        // 0.0 -> 0.2   +0.9
        // 0.9 -> 1.1   ???
        SfxPlayer.PitchScale = rand.NextSingle() / 5 + 0.9f;
        SfxPlayer.Stream = audioStream;
        SfxPlayer.Play();
    }

    public static readonly string[] Shot = [
        "corru/booms/shot1.ogg",
        "corru/booms/shot2.ogg",
        "corru/booms/shot3.ogg",
        "corru/booms/shot4.ogg",
        "corru/booms/shot5.ogg",
        "corru/booms/shot6.ogg",
        "corru/chomp.ogg",
        "corru/hit.ogg",
        "corru/stab.ogg",
        "corru/crit.ogg",
    ];

    public static readonly string Miss = "corru/miss.ogg";
    
    public static readonly string[] Buff = [
        "corru/mend.ogg",
    ];
    
    public static readonly string[] Move = [
        "corru/click1.ogg",
        "corru/click2.ogg",
    ];
    
}