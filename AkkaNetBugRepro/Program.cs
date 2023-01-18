using Akka.Actor;
using Akka.Configuration;
using AkkaNetBugRepro;

var config = ConfigurationFactory.ParseString(@"
	akka.log-dead-letters = false
	akka.persistence {
		journal {
	        plugin = ""akka.persistence.journal.sql-server""

			sql-server {
				class = ""Akka.Persistence.SqlServer.Journal.SqlServerJournal, Akka.Persistence.SqlServer""
				connection-string = ""Server=.\\SQL2017;Database=akka-bug-repro;user id=akka-bug-repro;password='password';MultipleActiveResultSets=true;Connect Timeout=30""
				connection-timeout = 4m
				schema-name = dbo
				table-name = EventJournal
				metadata-table-name = Metadata
				auto-initialize = on
				circuit-breaker.call-timeout = 5m
			}
		}
	}");

var system = ActorSystem.Create("system", config);
var persistentActor = system.ActorOf<MyPersistentActor>("persistentActor");
system.ActorOf(Props.Create(() => new MySenderActor(persistentActor)), "senderActor");

Console.ReadLine();