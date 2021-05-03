namespace Atomic.Injector.Generators.Interfaces
{
    public interface IFieldModel
    {
        string GetPropertyString();
        string GetPrivateFieldString();
        string GetMethodString();
        string GetConstructorString();
    }
}