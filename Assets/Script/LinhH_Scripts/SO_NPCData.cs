// 
// Member   : Linh
// Date     : 30/10/2025
// 

using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Scriptable object chứa dữ liệu của NPC.
/// </summary>
[CreateAssetMenu(fileName = "New NPC", menuName = "NPC/NPC Data")]
public class SO_NPCData : ScriptableObject
{
    public string npcId;
    public string npcName;
    public Sprite portrait;
    public bool canDating;
    public List<ItemScriptableObject> likeItem_List;
    public List<ItemScriptableObject> hateItem_List;
}
