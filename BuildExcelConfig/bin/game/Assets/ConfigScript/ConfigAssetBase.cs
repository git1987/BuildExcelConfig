using UnityEngine;

/// <summary>
/// ���û���
/// </summary>
public abstract class ConfigAssetBase : ScriptableObject
{
    public class ConfigAsset
    {
        public int ID;
    }
    public abstract string GetConfigName();
    public abstract void ReadFromData(object obj);
}
