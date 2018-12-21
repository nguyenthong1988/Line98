using System;
using System.Collections.Generic;

public class ServiceLocator
{
    // DO NOT USE SINGLETON CLASS SINCE IT WILL JUST CONFUSE YOU, ServiceLocator is usually always a static class

    public class CannotHaveTwoInstancesException : Exception
    {
        public CannotHaveTwoInstancesException() : base("There can be only one instance of a the Services class. It is a singleton...") { }
    }

    public class ServiceAlreadyRegisteredException : Exception
    {
        public ServiceAlreadyRegisteredException(System.Type T) : base("A service of that name (" + T.ToString() + ")  has already been registered!") { }
    }

    public class ServiceNotFoundException : Exception
    {
        public ServiceNotFoundException(System.Type T) : base("Service (" + T.ToString() + ") not found.\nAlways Register() in Awake(). Never Find() in Awake(). Check Script Execution Order.") { }
    }

    private static ServiceLocator mInstance;
    private Dictionary<Type, object> mServices = new Dictionary<Type, object>();

    public ServiceLocator()
    {
        if (mInstance != null)
        {
            throw new CannotHaveTwoInstancesException();
        }
    }

    protected static ServiceLocator Instance
    {
        get
        {
            if (mInstance == null)
            {
                mInstance = new ServiceLocator();
            }

            return mInstance;
        }
    }

    public static void Register<T>(T service, bool overwrite = false) where T : class
    {
        if (!overwrite && Instance.mServices.ContainsKey(typeof(T)))
        {
            throw new ServiceAlreadyRegisteredException(typeof(T));
        }
        Instance.mServices[typeof(T)] = service;
    }

    public static T Get<T>() where T : class
    {
        T ret = null;
        try
        {
            ret = Instance.mServices[typeof(T)] as T;
        }
        catch (KeyNotFoundException)
        {
            throw new ServiceNotFoundException(typeof(T));
        }
        return ret;
    }

    public static void Unregister<T>()
    {
        if (Instance.mServices.ContainsKey(typeof(T)))
        {
            Instance.mServices.Remove(typeof(T));
        }
    }

    public static void Clear()
    {
        Instance.mServices.Clear();
    }

    public static void Debug()
    {
        string output = "Debug Services List:\n";

        foreach (var s in Instance.mServices)
        {
            output += "* " + s.Key + " = " + s.Value.ToString() + "\n";
        }
        output += "Total: " + Instance.mServices.Count.ToString() + " services registered.";

        UnityEngine.Debug.Log(output);
    }
}