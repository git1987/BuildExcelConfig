using System;
using System.Collections.Generic;
public static class ListExtend
{
    static public List<T> ToList<T>(this object arrayList) where T : struct
    {
        string str = arrayList as string;
        if (str == null) return new List<T>();
        if (str == string.Empty) return new List<T>();
        string[] strs = str.Split(',');
        List<T> list = new List<T>();
        for (int i = 0; i < strs.Length; i++)
        {
            T t = default(T);
            try
            {
                t = (T)Convert.ChangeType(strs[i], typeof(T));
            }
            catch
            {
                UnityEngine.Debug.Log(str[i] + "don't as :" + default(T).GetType());
                return new List<T>();
            }
            list.Add(t);
        }
        return list;
    }
}