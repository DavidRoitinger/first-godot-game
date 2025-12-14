using System.Collections.Generic;

namespace FirstGodotGame;

public class Attack
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Damage { get; set; }
    public int UserKnockback { get; set; }
    public int TargetKnockback { get; set; }
    
    public List<List<int>> OriginPattern { get; set; }
    public List<List<int>> NeutralAttackPattern  { get; set; }
    
    public List<List<int>> UpAttackPattern  { get; set; }
    public List<List<int>> RightAttackPattern  { get; set; }
    public List<List<int>> DownAttackPattern  { get; set; }
    public List<List<int>> LeftAttackPattern  { get; set; }

    public const int NA = 0;
    public const int OR = 1;
    public const int NO = 2;
    
    public const int UO = 3;
    public const int RO = 4;
    public const int DO = 5;
    public const int LO = 6;
    
}