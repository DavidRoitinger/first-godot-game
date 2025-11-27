using System.Collections.Generic;

namespace FirstGodotGame;

public class Attack
{
    public string Name { get; set; }
    public int Damage { get; set; }
    
    public List<List<int>> OriginPattern { get; set; }
    public List<List<int>> AttackPattern  { get; set; }

    public const int NOA = 0;
    public const int AOR = 1;
    public const int AOE = 2;
    
}