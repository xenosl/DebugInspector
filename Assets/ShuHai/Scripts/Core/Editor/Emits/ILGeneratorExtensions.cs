using System;
using System.Reflection.Emit;

namespace ShuHai.Editor
{
    public static class ILGeneratorExtensions
    {
        /// <summary>
        ///     Emit type cast code.
        /// </summary>
        /// <returns>
        ///     <see langword="true" /> if the casting code is emitted; or <see langword="false" /> if no code is emitted.
        /// </returns>
        /// <remarks>
        ///     The stack transitional behavior, in sequential order, is:
        ///     1. The object to cast is pushed onto the stack.
        ///     2. The object is cast to wanted type and popped from the stack.
        ///     3. The casted object is pushed onto the stack.
        /// </remarks>
        public static bool EmitTypeCast(this ILGenerator self, Type inType, Type outType)
        {
            Ensure.Argument.NotNull(self, "gen");
            Ensure.Argument.NotNull(inType, "inType");
            Ensure.Argument.NotNull(outType, "outType");

            if (inType == outType)
                return false;

            bool isInValue = inType.IsValueType, isOutValue = outType.IsValueType;
            if (isInValue && !isOutValue)
                self.Emit(OpCodes.Box, inType);
            else if (!isInValue && isOutValue)
                self.Emit(OpCodes.Unbox_Any, outType);
            else if (!isInValue) // && !isOutValue
                self.Emit(OpCodes.Castclass, outType);
            else // isInValue && isOutValue
                return EmitNumberCast(self, outType);

            return true;
        }

        /// <summary>
        ///     Emit number cast code.
        /// </summary>
        /// <returns>
        ///     <see langword="true" /> if the casting code is emitted; or <see langword="false" /> if no code is emitted.
        /// </returns>
        /// <remarks>
        ///     The stack transitional behavior, in sequential order, is:
        ///     1. The object to cast is pushed onto the stack.
        ///     2. The object is converted to wanted primitive number type and popped from the stack.
        ///     3. The converted number is pushed onto the stack.
        /// </remarks>
        public static bool EmitNumberCast(this ILGenerator self, Type outType)
        {
            Ensure.Argument.NotNull(self, "gen");
            Ensure.Argument.NotNull(outType, "outType");

            switch (Type.GetTypeCode(outType))
            {
                case TypeCode.SByte:
                    self.Emit(OpCodes.Conv_I1);
                    return true;
                case TypeCode.Byte:
                    self.Emit(OpCodes.Conv_U1);
                    return true;
                case TypeCode.Int16:
                    self.Emit(OpCodes.Conv_I2);
                    return true;
                case TypeCode.UInt16:
                    self.Emit(OpCodes.Conv_U2);
                    return true;
                case TypeCode.Int32:
                    self.Emit(OpCodes.Conv_I4);
                    return true;
                case TypeCode.UInt32:
                    self.Emit(OpCodes.Conv_U4);
                    return true;
                case TypeCode.Int64:
                    self.Emit(OpCodes.Conv_I8);
                    return true;
                case TypeCode.UInt64:
                    self.Emit(OpCodes.Conv_U8);
                    return true;
                case TypeCode.Single:
                    self.Emit(OpCodes.Conv_R4);
                    return true;
                case TypeCode.Double:
                    self.Emit(OpCodes.Conv_R8);
                    return true;
                default:
                    return false;
            }
        }
    }
}