using Microsoft.AspNetCore.Mvc;

namespace School.xUnitTests;

public abstract class TestBase : IDisposable
{
    private List<Controller> _controllers = [];

    public T Controller<T>() where T : Controller, new()
    {
        var controller = new T();
        _controllers.Add(controller);
        return controller;
    }
    
    void IDisposable.Dispose()
    {
        foreach (var controller in _controllers)
            controller.Dispose();
    }
}