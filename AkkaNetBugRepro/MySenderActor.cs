using Akka.Actor;

namespace AkkaNetBugRepro;

public class MySenderActor : UntypedActor
{
    public MySenderActor(IActorRef persistentActor)
    {
        persistentActor.Tell(new Ping());
    }

    protected override void OnReceive(object message)
    {
    }
}