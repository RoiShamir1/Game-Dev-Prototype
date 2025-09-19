using UnityEngine;

[CreateAssetMenu(menuName = "Data/Int Variable")]
public class IntVariableSO : ScriptableObject
{
    public int value;
    public IntEventChannelSO OnChanged;

    public void Set(int v) { value = v; OnChanged?.Raise(value); }
    public void Add(int delta) { value += delta; OnChanged?.Raise(value); }
}
