using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CreateAssetMenu(fileName = "BaseAtributesSO", menuName = "Megaman/BaseAtributesSO", order = 1)]
#endif
public class BaseAtributesSO : ScriptableObject
{
    [SerializeField]
    private int healthPoints;
    [SerializeField]
    private int healthPointsMax;
    public int HealthPointsMax
    {
        get
        {
            return healthPointsMax;
        }
    }

    public delegate void DamageDelegate(int healthPoint);
    public DamageDelegate OnLifeChange;

    public void Damage(int damage)
    {
        healthPoints -= damage;
        healthPoints = Mathf.Clamp(healthPoints, 0, healthPointsMax);

        DispatchOnLifeChange();
    }

    public void Heal(int points)
    {
        healthPoints += points;
        healthPoints = Mathf.Clamp(healthPoints, 0, healthPointsMax);

        DispatchOnLifeChange();
    }

    public void Reset()
    {
        healthPoints = healthPointsMax;
    }

    public void OnEnable()
    {
        Reset();
    }

    private void DispatchOnLifeChange()
    {
        if (OnLifeChange != null)
        {
            OnLifeChange(healthPoints);
        }
    }

#if UNITY_EDITOR
    [MenuItem("Megaman/Create/Base Atributes")]
    public static void CreateMyAsset()
    {
        BaseAtributesSO asset = CreateInstance<BaseAtributesSO>();

        AssetDatabase.CreateAsset(asset, "Assets/NewBaseAtributesSO.asset");
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();

        Selection.activeObject = asset;
    }
#endif

    public void Die()
    {
        this.healthPoints = 0;
        this.DispatchOnLifeChange();
    }
}
