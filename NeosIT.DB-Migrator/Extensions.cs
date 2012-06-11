using System;
using System.Reflection;

namespace NeosIT.DB_Migrator
{
    public static class Extensions
    {
        public static bool HasMethod(this object obj, string methodName)
        {
            Type type = obj.GetType();
            return null != type.GetMethod(methodName);
        }

        public static MethodInfo GetMethod(this object obj, string methodName)
        {
            MethodInfo method = obj.GetType().GetMethod(methodName);
            return method;
            //return method.MakeGenericMethod();
        }
    }
}