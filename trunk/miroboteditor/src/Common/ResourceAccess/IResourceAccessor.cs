namespace miRobotEditor.Common.ResourceAccess
{
    public interface IResourceAccessor
    {
        object GetObject(object requester, string key);
        string GetString(object requester, string key, params object[] args);
    }
}
