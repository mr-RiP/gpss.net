﻿using GPSS;
using GPSS.Enums;
using GPSS.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Example
{
    class Program
    {
        static void Main(string[] args)
        {
            var model = new Model()
                .Storage("Queue", 10)
                .Storage("Tables", 10)
                .Generate(5, 1)
                .Gate(
                    (sna => GateCondition.StorageNotFull),
                    (sna => "Tables"),
                    (sna => "Overload"))
                .Gate(
                    (sna => GateCondition.StorageNotFull),
                    (sna => "Queue"),
                    (sna => "Overload"))
                .Enter(sna => "Queue")
                .Seize(sna => "CashDesk")
                .Leave(sna => "Queue")
                .Advance(3, 2)
                .Release(sna => "CashDesk")
                .Enter(sna => "Tables")
                .Advance(sna => sna.RandomNumberGenerator().Normal(20, 4))
                .Leave(sna => "Tables")
                .Terminate(1)
                .Label("Overload")
                .Terminate(0);

            var simulation = model.CreateSimulation();
            var report = simulation.Start(1000);

            Console.WriteLine("EndTime: " + report.EndTime);
            Console.ReadLine();
        }
    }
}
