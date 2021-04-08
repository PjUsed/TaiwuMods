using System;
using System.Linq;
using System.Reflection;

namespace UseStorageBook
{
    public static class ReflectionMethods
    {
		private const BindingFlags _flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

		public static T2 Invoke<T1, T2>(T1 instance, string method, params object[] args)
		{
			return (T2)(typeof(T1).GetMethod(method, _flags)?.Invoke(instance, args));
		}

		public static void Invoke<T1>(T1 instance, string method, params object[] args)
		{
			typeof(T1).GetMethod(method, _flags)?.Invoke(instance, args);
		}

		public static object Invoke<T>(T instance, string method, Type[] argTypes, params object[] args)
		{
			argTypes = argTypes ?? new Type[0];
			var methodInfo = typeof(T).GetMethods(_flags).SingleOrDefault(m => m.Name == method && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(argTypes));

            return methodInfo is null ? throw new AmbiguousMatchException("cannot find method to invoke") : methodInfo.Invoke(instance, args);
        }

        public static T2 GetValue<T1, T2>(T1 instance, string field)
		{
			return (T2)(typeof(T1).GetField(field, _flags)?.GetValue(instance));
		}

		public static object GetValue<T>(T instance, string field)
		{
			return typeof(T).GetField(field, _flags)?.GetValue(instance);
		}

		public static void SetValue<T>(T instance, string field, object value)
		{
			typeof(T).GetField(field, _flags)?.SetValue(instance, value);
		}
	}
}
