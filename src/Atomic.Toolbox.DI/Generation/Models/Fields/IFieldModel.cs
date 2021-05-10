namespace Atomic.Toolbox.DI.Generation.Models.Fields
{
    public interface IFieldModel
    {
        string GetPropertyString();
        string GetPrivateFieldString();
        string GetMethodString();
        string GetConstructorString();
    }
}