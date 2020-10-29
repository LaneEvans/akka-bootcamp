using System;
﻿using Akka.Actor;

namespace WinTail
{
    public class Program
    {
        private static ActorSystem _myActorSystem;

        public static void Main(string[] args)
        {
            // make an actor system 
            _myActorSystem = ActorSystem.Create("MyActorSystem");
            
            // make our first actors!
            var consoleWriterActor = _myActorSystem.ActorOf(Props.Create(() => new ConsoleWriterActor()));
            var consoleReaderActor = _myActorSystem.ActorOf(Props.Create(() => new ConsoleReaderActor(consoleWriterActor)));

            // tell console reader to begin
            consoleReaderActor.Tell(ConsoleReaderActor.StartCommand);

            // blocks the main thread from exiting until the actor system is shut down
            _myActorSystem.WhenTerminated.Wait();
        }

        private static void PrintInstructions()
        {
            Console.WriteLine("Write whatever you want into the console!");
            Console.Write("Some lines will appear as");
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.Write(" red ");
            Console.ResetColor();
            Console.Write(" and others will appear as");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(" green! ");
            Console.ResetColor();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Type 'exit' to quit this application at any time.\n");
        }
    }
}
