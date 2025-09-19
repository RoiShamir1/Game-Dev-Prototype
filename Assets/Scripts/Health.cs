using UnityEngine;

public class Health : MonoBehaviour
{
    public int maxHp = 3;
    int hp;

    void Awake() { hp = maxHp; }

    public bool ApplyDamage(int dmg)
    {
        hp = Mathf.Max(0, hp - Mathf.Max(0, dmg));
        return hp == 0;
    }
}
