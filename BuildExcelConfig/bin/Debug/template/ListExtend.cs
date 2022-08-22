using System;
using System.Collections.Generic;
using LitJson;
public static class ListExtend
{
    public static List<T> ToList<T>(this object arrayList)
    {
        string str = arrayList as string;
        if (str == null) return new List<T>();
        if (str == string.Empty) return new List<T>();
        string[] strs = str.Split('|');
        List<T> list = new List<T>();
        for (int i = 0; i < strs.Length; i++)
        {
            T t = default(T);
            try
            {
                t = (T)Convert.ChangeType(strs[i], typeof(T));
            }
            catch (Exception e)
            {
                UnityEngine.Debug.Log(str[i] + "don't as :" + default(T).GetType());
                return new List<T>();
            }
            list.Add(t);
        }
        return list;
    }
    public static List<T> ToList<T>(this JsonData jsonData)
    {
        List<T> list = new List<T>();
        string str = jsonData.ToString();
        if (str == "" || str == "0")
        {
            return list;
        }
        string[] strs = str.Split('|');
        for (int i = 0; i < strs.Length; i++)
        {
            T t = default(T);
            try
            {
                if (typeof(T).IsEnum)
                {
                    t = (T)System.Enum.Parse(typeof(T), strs[i]);
                }
                else
                {
                    t = (T)Convert.ChangeType(strs[i], typeof(T));
                }
            }
            catch (Exception e)
            {
                UnityEngine.Debug.Log(jsonData.ToString()[i] + "don't as :" + default(T).GetType());
                return new List<T>();
            }
            list.Add(t);
        }
        return list;
    }
}