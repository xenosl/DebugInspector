using System;

namespace ShuHai.Xml
{
    [AttributeUsage(AttributeTargets.Method)]
    public abstract class XConvertCallbackAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Method)]
    public class OnConvertingToElementAttribute : XConvertCallbackAttribute { }

    [AttributeUsage(AttributeTargets.Method)]
    public class OnConvertedToElementAttribute : XConvertCallbackAttribute { }

    [AttributeUsage(AttributeTargets.Method)]
    public class OnConvertingToObjectAttribute : XConvertCallbackAttribute { }

    [AttributeUsage(AttributeTargets.Method)]
    public class OnConvertedToObjectAttribute : XConvertCallbackAttribute { }
}