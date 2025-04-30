using Gamecraft.Other;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Chat", menuName = "Dialogue/Chat")]
public class Chat : ScriptableObject
{
    public string Name;
    public List<Talk> Talks;
}
