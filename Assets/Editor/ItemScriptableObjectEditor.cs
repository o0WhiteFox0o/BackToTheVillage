using UnityEditor;
using UnityEngine;

//[CustomEditor(typeof(ItemScriptableObject))]
public class ItemScriptableObjectEditor : Editor
{
    public override void OnInspectorGUI()
    {
        ItemScriptableObject item = (ItemScriptableObject)target;

        EditorGUILayout.LabelField("Overview", EditorStyles.boldLabel);
        item.id = EditorGUILayout.TextField("ID", item.id);
        item.icon = (Sprite)EditorGUILayout.ObjectField("Icon", item.icon, typeof(Sprite), false);
        // item.displayName = EditorGUILayout.TextField("Display Name", item.displayName);
        item.stackable = EditorGUILayout.Toggle("Stackable", item.stackable);
        item.canSell = EditorGUILayout.Toggle("Can Sell", item.canSell);
        item.itemType = (ItemType)EditorGUILayout.EnumPopup("Item Type", item.itemType);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Price", EditorStyles.boldLabel);
        item.buyPrice = EditorGUILayout.IntField("Buy Price", item.buyPrice);
        item.sellPrice = EditorGUILayout.IntField("Sell Price", item.sellPrice);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Prefab Settings", EditorStyles.boldLabel);
        item.itemPrefab = (GameObject)EditorGUILayout.ObjectField("Item Prefab", item.itemPrefab, typeof(GameObject), false);

        // ?? Ch? hi?n ph?n "Seed Settings" n?u itemType == Seed
        if (item.itemType == ItemType.Seed)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Seed Settings", EditorStyles.boldLabel);
            item.plantPrefab = (GameObject)EditorGUILayout.ObjectField("Plant Prefab", item.plantPrefab, typeof(GameObject), false);
        }

        if (GUI.changed)
        {
            EditorUtility.SetDirty(item);
        }
    }
}
