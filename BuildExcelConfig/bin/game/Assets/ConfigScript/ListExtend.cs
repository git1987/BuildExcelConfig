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
                UnityEngine.Debug.LogErrorFormat("[{0}] don't as [{1}]:{2}", str[i], default(T).GetType(), e.Message);
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
                    if (strs[i] == string.Empty)
                    {
                        strs[i] = "None";
                    }
                    t = (T)System.Enum.Parse(typeof(T), strs[i]);
                }
                else
                {
                    if (strs[i] == string.Empty)
                    {
                        if (typeof(T) == typeof(string))
                        {
                        }
                        else if (typeof(T) == typeof(bool))
                        {
                            strs[i] = "false";
                        }
                        else
                        {
                            strs[i] = "0";
                        }
                    }
                    t = (T)Convert.ChangeType(strs[i], typeof(T));
                }
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogErrorFormat("[{0}] don't as [{1}]:{2}", str, default(T).GetType(), e.Message);
                return new List<T>();
            }
            list.Add(t);
        }
        return list;
    }
}