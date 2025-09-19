using UnityEngine;

public class ScoreSystem : MonoBehaviour
{
    public IntVariableSO score;
    public VoidEventChannelSO OnEnemyDied;
    public int pointsPerKill = 100;

    void OnEnable() { if (OnEnemyDied) OnEnemyDied.OnEvent += HandleKill; }
    void OnDisable() { if (OnEnemyDied) OnEnemyDied.OnEvent -= HandleKill; }

    void HandleKill() { if (score) score.Add(pointsPerKill); }
}
