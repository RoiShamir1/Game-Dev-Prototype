using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Events/Int Event Channel")]
public class IntEventChannelSO : ScriptableObject
{
    public event Action<int> OnEvent;
    public void Raise(int value) => OnEvent?.Invoke(value);
}
