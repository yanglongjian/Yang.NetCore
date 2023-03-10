using Microsoft.Extensions.DependencyModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Yang.Core
{
    /// <summary>
    /// 程序集管理器
    /// </summary>
    public static class AssemblyManager
    {
        private static readonly string[] Filters = { "dotnet-", "Microsoft.", "mscorlib", "netstandard", "System", "Windows" };
        private static Assembly[] _allAssemblies;
        private static Type[] _allTypes;

        static AssemblyManager()
        {
            AssemblyFilterFunc = name =>
            {
                return name.Name != null && !Filters.Any(m => name.Name.StartsWith(m));
            };
        }

        /// <summary>
        /// 设置 程序集过滤器
        /// </summary>
        public static Func<AssemblyName, bool> AssemblyFilterFunc { private get; set; }

        /// <summary>
        /// 获取 所有程序集
        /// </summary>
        public static Assembly[] AllAssemblies
        {
            get
            {
                if (_allAssemblies == null)
                {
                    Init();
                }

                return _allAssemblies;
            }
        }

        /// <summary>
        /// 获取 所有类型
        /// </summary>
        public static Type[] AllTypes
        {
            get
            {
                if (_allTypes == null)
                {
                    Init();
                }

                return _allTypes;
            }
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public static void Init()
        {
            if (AssemblyFilterFunc == null)
            {
                throw new ArgumentException("AssemblyManager.AssemblyFilterFunc 不能为空");
            }

            _allAssemblies = DependencyContext.Default.GetDefaultAssemblyNames()
                .Where(AssemblyFilterFunc).Select(Assembly.Load).ToArray();
            _allTypes = _allAssemblies.SelectMany(m => m.GetTypes()).ToArray();
        }

        /// <summary>
        /// 查找指定条件的类型
        /// </summary>
        public static Type[] FindTypes(Func<Type, bool> predicate)
        {
            return AllTypes.Where(predicate).ToArray();
        }

        /// <summary>
        /// 查找指定基类的实现类型
        /// </summary>
        public static Type[] FindTypesByBase<TBaseType>()
        {
            Type baseType = typeof(TBaseType);
            return FindTypesByBase(baseType);
        }

        /// <summary>
        /// 查找指定基类的实现类型
        /// </summary>
        public static Type[] FindTypesByBase(Type baseType)
        {
            return AllTypes.Where(type => type.IsDeriveClassFrom(baseType)).Distinct().ToArray();
        }

        /// <summary>
        /// 判断当前类型是否可由指定类型派生
        /// </summary>
        public static bool IsDeriveClassFrom(this Type type, Type baseType, bool canAbstract = false)
        {
            return type.IsClass && (canAbstract || !type.IsAbstract) && type.IsBaseOn(baseType);
        }

        /// <summary>
        /// 返回当前类型是否是指定基类的派生类
        /// </summary>
        /// <param name="type">当前类型</param>
        /// <param name="baseType">要判断的基类型</param>
        /// <returns></returns>
        public static bool IsBaseOn(this Type type, Type baseType)
        {
            if (baseType.IsGenericTypeDefinition)
            {
                return baseType.IsGenericAssignableFrom(type);
            }
            return baseType.IsAssignableFrom(type);
        }

        /// <summary>
        /// 判断当前泛型类型是否可由指定类型的实例填充
        /// </summary>
        /// <param name="genericType">泛型类型</param>
        /// <param name="type">指定类型</param>
        /// <returns></returns>
        public static bool IsGenericAssignableFrom(this Type genericType, Type type)
        {
            if (!genericType.IsGenericType)
            {
                throw new ArgumentException("该功能只支持泛型类型的调用，非泛型类型可使用 IsAssignableFrom 方法。");
            }

            List<Type> allOthers = new() { type };
            if (genericType.IsInterface)
            {
                allOthers.AddRange(type.GetInterfaces());
            }

            foreach (var other in allOthers)
            {
                Type cur = other;
                while (cur != null)
                {
                    if (cur.IsGenericType)
                    {
                        cur = cur.GetGenericTypeDefinition();
                    }
                    if (cur.IsSubclassOf(genericType) || cur == genericType)
                    {
                        return true;
                    }
                    cur = cur.BaseType;
                }
            }
            return false;
        }


        /// <summary>
        /// 查找指定Attribute特性的实现类型
        /// </summary>
        public static Type[] FindTypesByAttribute<TAttribute>(bool inherit = true)
        {
            Type attributeType = typeof(TAttribute);
            return FindTypesByAttribute(attributeType, inherit);
        }

        /// <summary>
        /// 查找指定Attribute特性的实现类型
        /// </summary>
        public static Type[] FindTypesByAttribute(Type attributeType, bool inherit = true)
        {
            return AllTypes.Where(type => type.IsDefined(attributeType, inherit)).Distinct().ToArray();
        }
    }
}



