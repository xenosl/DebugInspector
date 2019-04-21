using System;
using System.Reflection;
using System.Reflection.Emit;

namespace ShuHai.Editor
{
    #region Delegates For Generated Code

    public delegate TValue ThisValueGetter<TInstance, TValue>(ref TInstance obj);

    public delegate void ThisValueSetter<TInstance, TValue>(ref TInstance obj, TValue value);

    public delegate void ThisCall<TInstance>(ref TInstance obj);

    public delegate void ThisCall<TInstance, TArg>(ref TInstance obj, TArg arg);

    public delegate void ThisCall<TInstance, TArg1, TArg2>(ref TInstance obj, TArg1 arg1, TArg2 arg2);

    public delegate void ThisCall<TInstance, TArg1, TArg2, TArg3>(
        ref TInstance obj, TArg1 arg1, TArg2 arg2, TArg3 arg3);

    public delegate void ThisCall<TInstance, TArg1, TArg2, TArg3, TArg4>(
        ref TInstance obj, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4);

    public delegate void ThisCall<TInstance, TArg1, TArg2, TArg3, TArg4, TArg5>(
        ref TInstance obj, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5);

    public delegate void ThisCall<TInstance, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6>(
        ref TInstance obj, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6);

    public delegate TReturn ReturnedThisCall<TInstance, TReturn>(ref TInstance obj);

    public delegate TReturn ReturnedThisCall<TInstance, TArg, TReturn>(ref TInstance obj, TArg arg);

    public delegate TReturn ReturnedThisCall<TInstance, TArg1, TArg2, TReturn>(
        ref TInstance obj, TArg1 arg1, TArg2 arg2);

    public delegate TReturn ReturnedThisCall<TInstance, TArg1, TArg2, TArg3, TReturn>(
        ref TInstance obj, TArg1 arg1, TArg2 arg2, TArg3 arg3);

    public delegate TReturn ReturnedThisCall<TInstance, TArg1, TArg2, TArg3, TArg4, TReturn>(
        ref TInstance obj, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4);

    public delegate TReturn ReturnedThisCall<TInstance, TArg1, TArg2, TArg3, TArg4, TArg5, TReturn>(
        ref TInstance obj, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5);

    public delegate TReturn ReturnedThisCall<TInstance, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TReturn>(
        ref TInstance obj, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6);

    #endregion

    public static class CommonMethodsEmitter
    {
        #region New

        /// <summary>
        ///     Create a delegate which create a instance of specified type.
        /// </summary>
        /// <param name="type"> Type of the instance to create. </param>
        /// <returns> A delegate which create a instance of <paramref name="type" />. </returns>
        public static Func<object> CreateNew(Type type)
        {
            Ensure.Argument.NotNull(type, "type");
            Ensure.Argument.Satisfy(!type.IsAbstract, "type", "Unable to create instance of abstract type.");
            Ensure.Argument.Satisfy(!type.ContainsGenericParameters, "type",
                "Unable to create instance of type which contains generic parameter.");

            var method = CreateNew(type, typeof(object));
            return (Func<object>)method.CreateDelegate(typeof(Func<object>));
        }

        public static Func<T> CreateNew<T>()
        {
            var type = typeof(T);
            var method = CreateNew(type, type);
            return (Func<T>)method.CreateDelegate(typeof(Func<T>));
        }

        public static Func<TReturn> CreateNew<TObject, TReturn>()
            where TReturn : TObject
        {
            var method = CreateNew(typeof(TObject), typeof(TReturn));
            return (Func<TReturn>)method.CreateDelegate(typeof(Func<TReturn>));
        }

        private static DynamicMethod CreateNew(Type objType, Type retType)
        {
            Ensure.Argument.Is(objType, "objType", retType);

            var method = new DynamicMethod("new_" + objType.FullName, retType, null, objType);

            var gen = method.GetILGenerator();
            if (objType.IsValueType)
            {
                var objTypeInfo = gen.DeclareLocal(objType);
                gen.Emit(OpCodes.Ldloca, objTypeInfo.LocalIndex);
                gen.Emit(OpCodes.Initobj, objType);
                gen.Emit(OpCodes.Ldloc, objTypeInfo.LocalIndex);
                bool boxRetValue = objType.IsValueType && !retType.IsValueType;
                if (boxRetValue)
                    gen.Emit(OpCodes.Box, objType);
            }
            else
            {
                var constructor = objType.GetDefaultConstructor();
                if (constructor == null)
                {
                    throw new InvalidProgramException(string.Format(
                        "Default constructor for '{0}' not found.", objType));
                }

                gen.Emit(OpCodes.Newobj, constructor);
            }
            gen.Emit(OpCodes.Ret);

            return method;
        }

        #endregion New

        #region Type Cast

        public static Converter<TIn, TOut> CreateCast<TIn, TOut>()
        {
            Type inType = typeof(TIn), outType = typeof(TOut);
            var name = string.Format("cast_{0}_to_{1}", inType.Name, outType.Name);
            var method = new DynamicMethod(name, outType, new[] { inType });

            var gen = method.GetILGenerator();
            gen.Emit(OpCodes.Ldarg_0);
            gen.EmitTypeCast(inType, outType);
            gen.Emit(OpCodes.Ret);

            return (Converter<TIn, TOut>)method.CreateDelegate(typeof(Converter<TIn, TOut>));
        }

        #endregion Type Cast

        #region Value Getter/Setter

        #region Static

        #region Field Getter/Setter

        public static Func<TValue> CreateStaticFieldGetter<TClass, TValue>(string fieldName)
        {
            return CreateStaticFieldGetter<TValue>(typeof(TClass).GetField(fieldName, true));
        }

        public static Func<TValue> CreateStaticFieldGetter<TValue>(FieldInfo field)
        {
            StaticFieldArgCheck<TValue>(field, "field");

            var method = CreateStaticGetterMethod<TValue>(field);

            var gen = method.GetILGenerator();
            if (!field.IsLiteral) // Non-const value
            {
                gen.Emit(OpCodes.Ldsfld, field);
            }
            else // const value
            {
                var type = field.FieldType;
                var typeCode = Type.GetTypeCode(type);
                var value = field.GetValue(null);
                switch (typeCode)
                {
                    case TypeCode.Boolean:
                        gen.Emit((bool)value ? OpCodes.Ldc_I4_1 : OpCodes.Ldc_I4_0);
                        break;
                    case TypeCode.Char:
                        var charValue = (char)value;
                        var charIsAscii = charValue <= 0xFF;
                        if (charIsAscii)
                            gen.Emit(OpCodes.Ldc_I4_S, (byte)charValue);
                        else
                            gen.Emit(OpCodes.Ldc_I4, (int)charValue);
                        break;
                    case TypeCode.SByte:
                        gen.Emit(OpCodes.Ldc_I4_S, (byte)(sbyte)value);
                        break;
                    case TypeCode.Byte:
                        gen.Emit(OpCodes.Ldc_I4, (int)(byte)value);
                        break;
                    case TypeCode.Int16:
                        gen.Emit(OpCodes.Ldc_I4, (int)(short)value);
                        break;
                    case TypeCode.UInt16:
                        gen.Emit(OpCodes.Ldc_I4, (int)(ushort)value);
                        break;
                    case TypeCode.Int32:
                        gen.Emit(OpCodes.Ldc_I4, (int)value);
                        break;
                    case TypeCode.UInt32:
                        gen.Emit(OpCodes.Ldc_I4, unchecked((int)(uint)value));
                        break;
                    case TypeCode.Int64:
                        gen.Emit(OpCodes.Ldc_I8, (long)value);
                        break;
                    case TypeCode.UInt64:
                        gen.Emit(OpCodes.Ldc_I8, unchecked((long)(ulong)value));
                        break;
                    case TypeCode.Single:
                        gen.Emit(OpCodes.Ldc_R4, (float)value);
                        break;
                    case TypeCode.Double:
                        gen.Emit(OpCodes.Ldc_R8, (double)value);
                        break;
                    case TypeCode.String:
                        gen.Emit(OpCodes.Ldstr, (string)value);
                        break;
                    case TypeCode.Decimal:
                        gen.Emit(OpCodes.Ldsfld, field);
                        break;
                    default:
                        throw new InvalidProgramException(string.Format(
                            @"Illegal constant value of type ""{0}"".", type));
                }
            }
            gen.Emit(OpCodes.Ret);

            return CreateStaticGetterDelegate<TValue>(method);
        }

        public static Action<TValue> CreateStaticFieldSetter<TClass, TValue>(string fieldName)
        {
            return CreateStaticFieldSetter<TValue>(typeof(TClass).GetField(fieldName, true));
        }

        public static Action<TValue> CreateStaticFieldSetter<TValue>(FieldInfo field)
        {
            StaticFieldArgCheck<TValue>(field, "field");
            Ensure.Argument.Satisfy(!field.IsLiteral, "field", "Attempt to create constant field setter method.");
            Ensure.Argument.Satisfy(!field.IsInitOnly, "field", "Attempt to create readonly field setter method.");

            var method = CreateStaticSetterMethod<TValue>(field);

            var gen = method.GetILGenerator();
            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Stsfld, field);
            gen.Emit(OpCodes.Ret);

            return CreateStaticSetterDelegate<TValue>(method);
        }

        private static void StaticFieldArgCheck<TValue>(FieldInfo field, string argName)
        {
            Ensure.Argument.NotNull(field, argName);
            Ensure.Argument.Satisfy(field.IsStatic, argName, "Static field expected.");
            Ensure.Argument.Is<TValue>(field.FieldType, argName + ".FieldType");
        }

        #endregion Field Getter/Setter

        #region Property Getter/Setter

        public static Func<TValue> CreateStaticPropertyGetter<TClass, TValue>(string propertyName)
        {
            return CreateStaticPropertyGetter<TValue>(typeof(TClass).GetProperty(propertyName, true));
        }

        public static Func<TValue> CreateStaticPropertyGetter<TValue>(PropertyInfo property)
        {
            var getMethod = PrepareCreateStaticPropertyAccessor<TValue>(property, "property", true);
            var dynamicMethod = CreateStaticGetterMethod<TValue>(property);

            var gen = dynamicMethod.GetILGenerator();
            gen.Emit(OpCodes.Call, getMethod);
            gen.Emit(OpCodes.Ret);

            return CreateStaticGetterDelegate<TValue>(dynamicMethod);
        }

        public static Action<TValue> CreateStaticPropertySetter<TClass, TValue>(string propertyName)
        {
            return CreateStaticPropertySetter<TValue>(typeof(TClass).GetProperty(propertyName, true));
        }

        public static Action<TValue> CreateStaticPropertySetter<TValue>(PropertyInfo property)
        {
            var setMethod = PrepareCreateStaticPropertyAccessor<TValue>(property, "property", false);
            var dynamicMethod = CreateStaticSetterMethod<TValue>(property);

            var gen = dynamicMethod.GetILGenerator();
            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Call, setMethod);
            gen.Emit(OpCodes.Ret);

            return CreateStaticSetterDelegate<TValue>(dynamicMethod);
        }

        private static MethodInfo PrepareCreateStaticPropertyAccessor<TValue>(
            PropertyInfo property, string argName, bool isGetter)
        {
            Ensure.Argument.NotNull(property, argName);
            Ensure.Argument.Is<TValue>(property.PropertyType, argName + ".PropertyType");

            var method = isGetter ? property.GetGetMethod(true) : property.GetSetMethod(true);
            Ensure.Argument.NotNull(method, argName);
            Ensure.Argument.Satisfy(method.IsStatic, argName, "Static property expected.");
            return method;
        }

        #endregion Property Getter/Setter

        #region Utilities

        private static Func<TValue> CreateStaticGetterDelegate<TValue>(DynamicMethod method)
        {
            return (Func<TValue>)method.CreateDelegate(typeof(Func<TValue>));
        }

        private static DynamicMethod CreateStaticGetterMethod<TValue>(MemberInfo member)
        {
            var owner = member.DeclaringType;
            var name = owner.FullName + ".get_" + member.Name;
            return new DynamicMethod(name, typeof(TValue), null, owner);
        }

        private static Action<TValue> CreateStaticSetterDelegate<TValue>(DynamicMethod method)
        {
            return (Action<TValue>)method.CreateDelegate(typeof(Action<TValue>));
        }

        private static DynamicMethod CreateStaticSetterMethod<TValue>(MemberInfo member)
        {
            var owner = member.DeclaringType;
            var name = owner.FullName + ".set_" + member.Name;
            return new DynamicMethod(name, null, new[] { typeof(TValue) }, owner);
        }

        #endregion Utilities

        #endregion Static

        #region Instance

        #region Field Getter/Setter

        public static ThisValueGetter<TInstance, TValue> CreateInstanceFieldGetter<TInstance, TValue>(string fieldName)
        {
            return CreateInstanceFieldGetter<TInstance, TValue>(typeof(TInstance).GetField(fieldName, false));
        }

        public static ThisValueGetter<TInstance, TValue> CreateInstanceFieldGetter<TInstance, TValue>(FieldInfo field)
        {
            InstanceFieldArgCheck<TInstance, TValue>(field, "field");

            var method = CreateInstanceGetterMethod<TInstance, TValue>(field);

            var gen = method.GetILGenerator();
            gen.Emit(OpCodes.Ldarg_0);
            if (!typeof(TInstance).IsValueType)
                gen.Emit(OpCodes.Ldind_Ref);
            gen.Emit(OpCodes.Ldfld, field);
            gen.Emit(OpCodes.Ret);

            return CreateInstanceGetterDelegate<TInstance, TValue>(method);
        }

        public static ThisValueSetter<TInstance, TValue> CreateInstanceFieldSetter<TInstance, TValue>(string fieldName)
        {
            return CreateInstanceFieldSetter<TInstance, TValue>(typeof(TInstance).GetField(fieldName, false));
        }

        public static ThisValueSetter<TInstance, TValue> CreateInstanceFieldSetter<TInstance, TValue>(FieldInfo field)
        {
            InstanceFieldArgCheck<TInstance, TValue>(field, "field");

            var method = CreateInstanceSetterMethod<TInstance, TValue>(field);

            var gen = method.GetILGenerator();
            gen.Emit(OpCodes.Ldarg_0);
            if (!typeof(TInstance).IsValueType)
                gen.Emit(OpCodes.Ldind_Ref);
            gen.Emit(OpCodes.Ldarg_1);
            gen.Emit(OpCodes.Stfld, field);
            gen.Emit(OpCodes.Ret);

            return CreateInstanceSetterDelegate<TInstance, TValue>(method);
        }

        #endregion Field Getter/Setter

        #region Property Getter/Setter

        public static ThisValueGetter<TInstance, TValue>
            CreateInstancePropertyGetter<TInstance, TValue>(string propertyName)
        {
            return CreateInstancePropertyGetter<TInstance, TValue>(typeof(TInstance).GetProperty(propertyName, false));
        }

        public static ThisValueGetter<TInstance, TValue>
            CreateInstancePropertyGetter<TInstance, TValue>(PropertyInfo property)
        {
            InstancePropertyArgCheck<TInstance, TValue>(property, "property");
            Ensure.Argument.Satisfy(property.CanRead, "property", "Readable property expected.");

            var getMethod = property.GetGetMethod(true);
            Ensure.Argument.Satisfy(!getMethod.IsStatic, "property", "Non-static property expected.");

            var method = CreateInstanceGetterMethod<TInstance, TValue>(property);

            var gen = method.GetILGenerator();
            gen.Emit(OpCodes.Ldarg_0);
            if (!typeof(TInstance).IsValueType)
                gen.Emit(OpCodes.Ldind_Ref);
            gen.Emit(OpCodes.Callvirt, getMethod);
            gen.Emit(OpCodes.Ret);

            return CreateInstanceGetterDelegate<TInstance, TValue>(method);
        }

        public static ThisValueSetter<TInstance, TValue>
            CreateInstancePropertySetter<TInstance, TValue>(string propertyName)
        {
            return CreateInstancePropertySetter<TInstance, TValue>(typeof(TInstance).GetProperty(propertyName, false));
        }

        public static ThisValueSetter<TInstance, TValue>
            CreateInstancePropertySetter<TInstance, TValue>(PropertyInfo property)
        {
            InstancePropertyArgCheck<TInstance, TValue>(property, "property");
            Ensure.Argument.Satisfy(property.CanWrite, "property", "Writable property expected.");

            var setMethod = property.GetSetMethod(true);
            Ensure.Argument.Satisfy(!setMethod.IsStatic, "property", "Non-static property expected.");

            var method = CreateInstanceSetterMethod<TInstance, TValue>(property);

            var gen = method.GetILGenerator();
            gen.Emit(OpCodes.Ldarg_0);
            if (!typeof(TInstance).IsValueType)
                gen.Emit(OpCodes.Ldind_Ref);
            gen.Emit(OpCodes.Ldarg_1);
            gen.Emit(OpCodes.Callvirt, setMethod);
            gen.Emit(OpCodes.Ret);

            return CreateInstanceSetterDelegate<TInstance, TValue>(method);
        }

        #endregion Instance Property Getter/Setter

        #region Utilities

        private static void InstanceFieldArgCheck<TOwner, TValue>(FieldInfo field, string argName)
        {
            InstanceMemberArgCheck<TOwner>(field, argName);
            Ensure.Argument.Satisfy(!field.IsStatic, argName, "Instance field expected.");
            Ensure.Argument.Is<TValue>(field.FieldType, argName + ".FieldType");
        }

        private static void InstancePropertyArgCheck<TOwner, TValue>(PropertyInfo property, string argName)
        {
            InstanceMemberArgCheck<TOwner>(property, argName);
            Ensure.Argument.Is<TValue>(property.PropertyType, argName + ".PropertyType");
        }

        private static void InstanceMemberArgCheck<TOwner>(MemberInfo member, string argName)
        {
            Ensure.Argument.NotNull(member, argName);
            Ensure.Argument.Is<TOwner>(member.ReflectedType, argName + ".ReflectedType");
        }

        private static DynamicMethod CreateInstanceGetterMethod<TInstance, TValue>(MemberInfo member)
        {
            var owner = member.DeclaringType;
            var name = owner.FullName + ".get_" + member.Name;
            return new DynamicMethod(name, typeof(TValue), new[] { typeof(TInstance).MakeByRefType() }, owner);
        }

        private static ThisValueGetter<TInstance, TValue>
            CreateInstanceGetterDelegate<TInstance, TValue>(DynamicMethod method)
        {
            return (ThisValueGetter<TInstance, TValue>)method
                .CreateDelegate(typeof(ThisValueGetter<TInstance, TValue>));
        }

        private static DynamicMethod CreateInstanceSetterMethod<TInstance, TValue>(MemberInfo member)
        {
            var owner = member.DeclaringType;
            var name = owner.FullName + ".set_" + member.Name;
            return new DynamicMethod(name, null, new[] { typeof(TInstance).MakeByRefType(), typeof(TValue) }, owner);
        }

        private static ThisValueSetter<TInstance, TValue>
            CreateInstanceSetterDelegate<TInstance, TValue>(DynamicMethod method)
        {
            return (ThisValueSetter<TInstance, TValue>)method
                .CreateDelegate(typeof(ThisValueSetter<TInstance, TValue>));
        }

        #endregion Utilities

        #endregion Instance

        #endregion Value Getter/Setter
    }
}