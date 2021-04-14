using Godot;
using System;
using System.Reflection;
using System.Collections.Generic;
using Godot.Collections;
using System.Linq;

public static class MonoCustomResourceIO
{
    public static T Load<T>(string resourcePath) where T : Resource, new()
    {
        // Godot supports C# resouce loading. Yay!
        return ResourceLoader.Load<T>(resourcePath);
    }

    public static void Save(string path, Resource resource)
    {
        if (ResourceLoader.Exists(path))
            SaveExisting(path, resource);
        else
            SaveNew(path, resource);
    }

    public static void SaveNew(string path, Resource resource)
    {
        var prototypePath = FindPrototypePathRecursive(resource.GetType());
        if (prototypePath == null)
            throw new Exception($"Could not find prototype for resource: {resource.GetType()}.");
        var prototype = ResourceLoader.Load(prototypePath);
        ResourceSaver.Save(path, prototype);
        var newInstance = ResourceLoader.Load(path);
        SaveNewHelper(newInstance, resource, resource.GetType());
    }

    private static void SaveNewHelper(Resource newInstance, Resource resourceBluePrint, Type resouceType)
    {
        var props = GetCustomResourceProperties(resourceBluePrint);
        foreach (var prop in props)
        {
            var propInfo = resourceBluePrint.GetType().GetProperty(prop);
            if (propInfo.PropertyType.Assembly == Assembly.GetExecutingAssembly())
            {
                if (propInfo.PropertyType.IsArray && propInfo.PropertyType.GetElementType().IsSubclassOf(typeof(Resource)))
                {
                    throw new Exception("Resource cannot contain a Resource array! Resource arrays are not implemented in saving.");
                } else if (propInfo.PropertyType.IsSubclassOf(typeof(Resource)))
                {
                    SaveNewHelper((Resource) newInstance.Get(prop), (Resource) propInfo.GetValue(resourceBluePrint), propInfo.PropertyType);
                } else
                {
                    throw new Exception("Resource cannot contain a non-Resource class!");
                }
            }
            else newInstance.Set(prop, propInfo.GetValue(resourceBluePrint));
        }
    }

    public static void SaveExisting(string path, Resource resource)
    {
        if (!ResourceLoader.Exists(path))
            throw new Exception($"Resource at {path} does not exist!");
        
        var diskResource = ResourceLoader.Load(path);
        var props = GetCustomResourceProperties(diskResource);
        foreach (var prop in props)
        {
            var propInfo = resource.GetType().GetProperty(prop);
            if (propInfo.PropertyType.Assembly == Assembly.GetExecutingAssembly())
            {
                if (propInfo.PropertyType.IsArray && propInfo.PropertyType.GetElementType().IsSubclassOf(typeof(Resource)))
                {
                    throw new Exception("Resource cannot contain a Resource array! Resource arrays are not implemented in saving.");
                } else if (propInfo.PropertyType.IsSubclassOf(typeof(Resource)))
                {
                    var saveMethod = typeof(MonoCustomResourceIO).GetTypeInfo().GetDeclaredMethod("SaveExisting");
                    saveMethod.Invoke(null, new object[] { ((Resource) propInfo.GetValue(diskResource)).ResourcePath, propInfo.GetValue(resource) });
                } else
                {
                    throw new Exception("Resource cannot contain a non-Resource class!");
                }
            }
            else diskResource.Set(prop, propInfo.GetValue(resource));
        }
        ResourceSaver.Save(diskResource.ResourcePath, diskResource);
    }

    private static List<string> GetCustomResourceProperties(Resource resource)
    {
        var props = new List<string>();
        var listening = false;
        foreach (Dictionary property in resource.GetPropertyList())
        {
            if (property["name"].ToString() == "Script Variables") 
                listening = true;
            else if (listening && !property["name"].ToString().EndsWith(">k__BackingField"))
            {
                var propInfo = resource.GetType().GetProperty(property["name"].ToString());
                
                if (propInfo == null)
                    continue;

                bool isExported = Attribute.IsDefined(propInfo, typeof(ExportAttribute));
                if (isExported) 
                    props.Add(property["name"].ToString());
            }
        }
        return props;
    }

    private static string FindPrototypePathRecursive(Type type)
    {
        foreach (string directory in MonoCustomResourceRegistry.Settings.ResourcePrototypeDirectories)
        {
            string fileFound = FindPrototypePathRecursiveHelper(type, directory);
            if (fileFound != null)
                return fileFound;
        }
        return null;
    }

    private static string FindPrototypePathRecursiveHelper(Type type, string directory)
    {
        Directory dir = new Directory();

        if (dir.Open(directory) == Error.Ok)
        {
            dir.ListDirBegin();

            while (true)
            {
                var fileOrDirName = dir.GetNext();
                
                // Skips hidden files like .
                if (fileOrDirName == "")
                    break;
                else if (fileOrDirName.BeginsWith("."))
                    continue;
                else if (dir.CurrentIsDir())
                {
                    string foundFilePath = FindPrototypePathRecursiveHelper(type, dir.GetCurrentDir() + "/" + fileOrDirName);
                    if (foundFilePath != null)
                    {
                        dir.ListDirEnd();
                        return foundFilePath;
                    }
                }
                else if (
                    fileOrDirName == $"{type.Name}_ResourcePrototype.tres" || 
                    fileOrDirName == $"{type.Name}_ResourcePrototype.res")
                    return dir.GetCurrentDir() + "/" + fileOrDirName;
            }
        }
        return null;
    }
}