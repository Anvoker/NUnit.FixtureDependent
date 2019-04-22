// ***********************************************************************
// Copyright (c) 2019 Andrei Ionescu
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// ***********************************************************************

using System;
using System.Reflection;

namespace NUnit.FixtureDependent.Internal
{
    public static class ReflectionHelper
    {
        public static Type GetMemberUnderlyingType(this MemberInfo member)
        {
            switch (member.MemberType)
            {
                case MemberTypes.Event:
                    return ((EventInfo)member).EventHandlerType;
                case MemberTypes.Field:
                    return ((FieldInfo)member).FieldType;
                case MemberTypes.Method:
                    return ((MethodInfo)member).ReturnType;
                case MemberTypes.Property:
                    return ((PropertyInfo)member).PropertyType;
                default:
                    throw new ArgumentException("Input MemberInfo must be " +
                        "if type EventInfo, FieldInfo, MethodInfo, or " +
                        "PropertyInfo");
            }
        }

        public static Type GetCollectionElementType(Type collection)
        {
            if (collection == null || collection == typeof(string))
            {
                return null;
            }

            if (collection.IsArray)
            {
                return collection.GetElementType();
            }

            Type[] genericArguments;
            if (collection.IsGenericType
                && (genericArguments = collection.GetGenericArguments()).Length > 0)
            {
                return genericArguments[0];
            }

            Type[] interfaces = collection.GetInterfaces();

            if (interfaces?.Length > 0)
            {
                foreach (Type i in interfaces)
                {
                    Type ienum = GetCollectionElementType(i);
                    if (ienum != null)
                    {
                        return ienum;
                    }
                }
            }

            if (collection.BaseType != null && collection.BaseType != typeof(object))
            {
                return GetCollectionElementType(collection.BaseType);
            }

            return null;
        }

        public static object GetValue(this MemberInfo member, object instance)
        {
            var field = member as FieldInfo;
            if (field != null)
            {
                return field.IsStatic
                    ? field.GetValue(null)
                    : field.GetValue(instance);
            }

            var property = member as PropertyInfo;
            if (property != null)
            {
                return property.GetMethod.IsStatic
                    ? property.GetValue(null, null)
                    : property.GetMethod.Invoke(instance, null);
            }

            var method = member as MethodInfo;
            if (method != null)
            {
                return method.IsStatic
                    ? method.Invoke(null, null)
                    : method.Invoke(instance, null);
            }

            throw new ArgumentException(
                $"Argument {nameof(member)} has to be {nameof(FieldInfo)}," +
                $"{nameof(PropertyInfo)}, or {nameof(MethodInfo)}.");
        }
    }
}
