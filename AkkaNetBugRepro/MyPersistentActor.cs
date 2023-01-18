using Akka.Actor;
using Akka.Persistence;

namespace AkkaNetBugRepro;

public record Ping;
public record Pong;
public class MyPersistentActor : ReceivePersistentActor
{

    public MyPersistentActor()
    {
        Recover<Ping>(p => true);
        
        Command<Ping>(p =>
        {
            Persist("", _ => {});
        });
    }

    protected override bool AroundReceive(Receive r, object message)
    {
        var result = base.AroundReceive(r, message);

        Console.WriteLine($"Message received {message} from {Sender.Path}");

        if (message is WriteMessagesFailed wmf)
        {
            Console.WriteLine($"WriteMessagesFailed received: {wmf.Cause}");
            Context.Stop(Self);
        }

        if (!IsRecovering && message is Ping)
        {
            // This defer async causes the bug
            DeferAsync( "", _ => Sender.Tell(new Pong()));
        }
        
        return result;
    }

    public override string PersistenceId => "MyId";
}