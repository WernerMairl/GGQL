namespace GGQL.Core
{

    

    public class Alert
    {
        //Trigger defines when and how often the conditions should be checked
        //basic Trigger is a scheduler, later we can also have inbound webhooks or listening on other TriggerSource

        //Notifictions are sendet out only in case of some condition

        //Condition mens : go over the entire eventstream and check every event!
        //create the required notofications


        public Trigger Trigger { get; private set; }


        public IEvent[] GetEvents()
        {
            //events durchgehen und filtern
            return null;
        }



    }
}