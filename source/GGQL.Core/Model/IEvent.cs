namespace GGQL.Core
{
    public interface IEvent
    {
        EventKey GetKey();
        string GetJsonData();
    }
}