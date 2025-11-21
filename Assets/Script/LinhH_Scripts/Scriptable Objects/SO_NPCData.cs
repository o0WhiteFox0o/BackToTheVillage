// 
// Member   : Linh
// Date     : 30/10/2025
// 

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;


/// <summary>
/// Scriptable object chứa dữ liệu của NPC.
/// </summary>
[CreateAssetMenu(fileName = "SO_NewNPC", menuName = "Scriptable Object/NPC/NPC Data")]
public class SO_NPCData : ScriptableObject
{
    public string npcId;
    public LocalizedString npcName;
    public LocalizedString npcDescription;
    public Sprite portrait;
    public bool canDating;
    public List<ItemScriptableObject> likeItem_List;
    public List<ItemScriptableObject> hateItem_List;
}
