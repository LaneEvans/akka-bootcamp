using System;
using Akka.Actor;

namespace WinTail
{
    public class TailCoordinatorActor : UntypedActor
    {
        protected override void OnReceive(object message)
        {
            if (message is StartTail startTail)
            {
                // here we are creating our first parent/child relationship!
                // the TailActor instance created here is a child
                // of this instance of TailCoordinatorActor
                Context.ActorOf(Props.Create(() => new TailActor(startTail.ReporterActor, startTail.FilePath)));
            }
        }

        protected override SupervisorStrategy SupervisorStrategy()
        {
            return new OneForOneStrategy(
                10, // maxNumberOfRetries
                TimeSpan.FromSeconds(30), // withinTimeRange
                x => // localOnlyDecider
                {
                    switch (x)
                    {
                        //Maybe we consider ArithmeticException to not be application critical
                        //so we just ignore the error and keep going.
                        //Error that we cannot recover from, stop the failing actor
                        case ArithmeticException _:
                            return Directive.Resume;
                        //In all other cases, just restart the failing actor
                        case NotSupportedException _:
                            return Directive.Stop;
                        default:
                            return Directive.Restart;
                    }
                });
        }

        #region Message types

        /// <summary>
        ///     Start tailing the file at user-specified path.
        /// </summary>
        public class StartTail
        {
            public StartTail(string filePath, IActorRef reporterActor)
            {
                FilePath = filePath;
                ReporterActor = reporterActor;
            }

            public string FilePath { get; }

            public IActorRef ReporterActor { get; }
        }

        /// <summary>
        ///     Stop tailing the file at user-specified path.
        /// </summary>
        public class StopTail
        {
            public StopTail(string filePath)
            {
                FilePath = filePath;
            }

            public string FilePath { get; }
        }

        #endregion
    }
}