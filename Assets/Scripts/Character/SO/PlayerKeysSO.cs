using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

[CreateAssetMenu(fileName = "PlayerKeysSO", menuName = "Megaman/PlayerKeysSO", order = 1)]
#endif
public class PlayerKeysSO : ScriptableObject {
    public KeyCode jumpKey, fire, dash;
    public string otherPlayerLayer;
    public string horizontalAxis;

#if UNITY_EDITOR
    [MenuItem("Megaman/Create/Player Keys")]
    public static void CreateMyAsset()
    {
        PlayerKeysSO asset = CreateInstance<PlayerKeysSO>();

        AssetDatabase.CreateAsset(asset, "Assets/NewPlayerKeysSO.asset");
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();

        Selection.activeObject = asset;
    }
#endif
}
