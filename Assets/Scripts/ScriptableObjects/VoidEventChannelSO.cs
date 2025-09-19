using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Events/Void Event Channel")]
public class VoidEventChannelSO : ScriptableObject
{
    public event Action OnEvent;
    public void Raise() => OnEvent?.Invoke();
}
