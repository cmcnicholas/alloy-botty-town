﻿using System;
using System.Collections.Generic;
using System.IO;
//using System.Reflection;

namespace GeoAPI
{
    /// <summary>
    /// Static class that provides access to a  <see cref="IGeometryServices"/> class.
    /// </summary>
    public static class GeometryServiceProvider
    {
        private static IGeometryServices _instance;
        private static readonly object LockObject = new object();

        /// <summary>
        /// Gets or sets the <see cref="IGeometryServices"/> instance.
        /// </summary>
        public static IGeometryServices Instance
        {
            get
            {
                lock (LockObject)
                {
                    return _instance ?? (_instance = ReflectInstance());
                }
            }

            set
            {
                if (value == null)
                    throw new ArgumentNullException("value", "You must not assign null to Instance!");

                lock (LockObject)
                {
                    _instance = value;
                }
            }
        }

        //private static IEnumerable<Type> GetLoadableTypes(Assembly assembly)
        //{
        //    if (assembly == null) throw new ArgumentNullException("assembly");
        //    try
        //    {
        //        return assembly.GetExportedTypes();
        //    }
        //    catch (TypeLoadException)
        //    {
        //        return new Type[0];
        //    }
        //    catch (FileNotFoundException)
        //    {
        //        return new Type[0];
        //    }
        //    //catch (ReflectionTypeLoadException ex)
        //    //{
        //    //    var types = ex.Types;
        //    //    IList<Type> list = new List<Type>(types.Length);
        //    //    foreach (var t in types)
        //    //        if (t != null && t.IsPublic)
        //    //            list.Add(t);
        //    //    return list;
        //    //}
        //}

        private static IGeometryServices ReflectInstance()
        {
#if !SILVERLIGHT && !UNITY_WEBPLAYER
            //var a = AppDomain.CurrentDomain.GetAssemblies();
            //foreach (var assembly in a)
            //{
            //    // Take a look at issue 114: http://code.google.com/p/nettopologysuite/issues/detail?id=114
            //    //if (assembly is System.Reflection.Emit.AssemblyBuilder) continue;
            //    //if (assembly.GetType().FullName == "System.Reflection.Emit.InternalAssemblyBuilder") continue;
            //    if (assembly.GlobalAssemblyCache && assembly.CodeBase == Assembly.GetExecutingAssembly().CodeBase) continue;

            //    foreach (var t in GetLoadableTypes(assembly))
            //    {
            //        if (t.IsInterface) continue;
            //        if (t.IsAbstract) continue;
            //        if (t.IsNotPublic) continue;
            //        if (!typeof(IGeometryServices).IsAssignableFrom(t)) continue;

            //        var constuctors = t.GetConstructors();
            //        foreach (var constructorInfo in constuctors)
            //        {
            //            if (constructorInfo.IsPublic && constructorInfo.GetParameters().Length == 0)
            //                return (IGeometryServices)Activator.CreateInstance(t);
            //        }
            //    }
            //}
#endif
            throw new InvalidOperationException("Cannot use GeometryServiceProvider without an assigned IGeometryServices class");
        }
    }
}