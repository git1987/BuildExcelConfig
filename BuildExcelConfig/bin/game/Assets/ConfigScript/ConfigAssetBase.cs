using UnityEngine;

/// <summary>
/// 配置基类
/// </summary>
public abstract class ConfigAssetBase : ScriptableObject
{
    public class ConfigAsset
    {
        public int ID;
    }
    public abstract string GetConfigName();
    public abstract void ReadFromData(object obj);
    public abstract void ReadList();
}
