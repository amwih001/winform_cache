using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace wincache
{
    public class Cache
    {
        /// <summary>
        /// 存储键值对应数据列表
        /// </summary>
        private static Hashtable list_data;
        /// <summary>
        /// 存储键值对应过期时间列表
        /// </summary>
        private static Hashtable key_data;

        public static void Register()
        {

            list_data = Hashtable.Synchronized(new Hashtable());
            key_data = Hashtable.Synchronized(new Hashtable());

        }

        /// <summary>
        /// 自增/自减一个值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="step">增量, 可以是一个负数</param>
        /// <param name="life"></param>
        /// <returns></returns>
        public static int Incr(string key, int step, int life = 0)
        {
            int value = Cache.GetInt(key);
            value += step;
            Cache.Set(key, value, life);
            return value;
        }
        /// <summary>
        /// 设置一个整数缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="life"></param>
        public static void Set(string key, int value, int life = 0)
        {
            object okey = (object)key;
            object ovalue = (object)value;
            Cache._Set(okey, ovalue, life);
        }
        /// <summary>
        /// 获取一个整数缓存,如果不存在返回0
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static int GetInt(string key)
        {
            object okey = (object)key;
            object obj = Cache._Get(okey);
            if (obj == null)
            {
                return 0;
            }
            int val = 0;
            int.TryParse(obj.ToString(), out val);
            return val;
        }
        /// <summary>
        /// 设置一个字符串缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="life"></param>
        public static void Set(string key, string value, int life = 0)
        {
            object okey = (object)key;
            object ovalue = (object)value;
            Cache._Set(okey, ovalue, life);
        }
        /// <summary>
        /// 获取一个字符串缓存,如果不存在返回空字符
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetString(string key)
        {
            object okey = (object)key;
            object obj = Cache._Get(okey);
            return obj == null ? string.Empty : obj.ToString();
        }
        /// <summary>
        /// 设置一个object对象缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="life"></param>
        public static void Set(string key, object value, int life = 0)
        {
            object okey = (object)key;
            Cache._Set(okey, value, life);
        }
        /// <summary>
        /// 获取一个object对象缓存,如果不存在返回null
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static object GetObject(string key)
        {
            object okey = (object)key;
            return Cache._Get(okey);
        }
        /// <summary>
        /// 检查一个缓存是否存在或是否有效
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool Exists(string key)
        {
            object okey = (object)key;
            object obj = Cache._Get(okey);
            return obj != null;
        }

        private static void _Set(object key, object value, int life)
        {
            try
            {
                life += Cache.TimeStamp();
                object olife = (object)life;
                if (list_data.ContainsKey(key))
                {
                    list_data[key] = value;
                }
                else
                {
                    list_data.Add(key, value);
                }
                if (key_data.ContainsKey(key))
                {
                    key_data[key] = olife;
                }
                else
                {
                    key_data.Add(key, olife);
                }
            }
            catch (Exception)
            { }
        }

        private static object _Get(object key)
        {
            object obj = null;
            try
            {
                if (key_data.ContainsKey(key))
                {
                    object olife = key_data[key];
                    if (olife == null)
                    {
                        Cache.Remove(key);
                        return obj;
                    }
                    int life = 0;
                    int.TryParse(olife.ToString(), out life);
                    if (life < TimeStamp())
                    {
                        Cache.Remove(key);
                        return obj;
                    }
                }
                else
                {
                    return obj;
                }
                if (list_data.ContainsKey(key))
                {
                    obj = list_data[key];
                }
            }
            catch (Exception)
            { }
            return obj;
        }

        public static void Remove(object key)
        {
            try
            {
                list_data.Remove(key);
            }
            catch (Exception)
            { }
            try
            {
                key_data.Remove(key);
            }
            catch (Exception)
            { }
        }

        public static void Remove(string key)
        {
            Cache.Remove((object)key);
        }

        private static int TimeStamp()
        {
            return Convert.ToInt32((DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000);
        }
    }
}
