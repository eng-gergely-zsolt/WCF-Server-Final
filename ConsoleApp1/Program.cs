using System;
using System.Linq;
using System.Threading;
using System.ServiceModel;
using System.Collections.Generic;
using System.ServiceModel.Description;
using static System.Console;
using WCFService;


abstract class CounterBase
{
    public abstract void Increment();

    public abstract void Decrement();
}

class Counter : CounterBase
{
    public int Count { get; private set; }

    public override void Increment()
    {
        Count++;
    }

    public override void Decrement()
    {
        Count--;
    }
}


namespace ConsoleApp1
{
    class Program
    {
        static void Main()
        {
            System.ServiceModel.ServiceHost host = new ServiceHost(typeof(WCFService.Service1));


            try
            {
                Thread oThread = new Thread(() => DoWork());


                // Start the thread
                oThread.Start();

                // Start the service.
                host.Open();

                Console.WriteLine("Running on endpoints:");
                foreach (ServiceEndpoint serviceEndpoint in host.Description.Endpoints)
                    Console.WriteLine(serviceEndpoint.Address.ToString());

                Console.WriteLine("The service is ready.");

                // Close the ServiceHost to stop the service.
                Console.WriteLine("Press <Enter> to terminate the service.");
                Console.WriteLine();
                Console.ReadLine();

                host.Close();
                Console.WriteLine("Service Stoped");
                oThread.Join();
                Console.WriteLine("Worker Stoped");
            }
            catch (CommunicationException ce)
            {
                Console.WriteLine("An exception occurred: {0}", ce.Message);
                host.Abort();
            }
        }


        public static void DoWork()
        {
            List<int> buses = new List<int>
            {
                26,
                27,
                44
            };

            ServiceReference1.Service1Client myService = new ServiceReference1.Service1Client();
            bus_dbEntities BusDBEntities = new bus_dbEntities();


            while (true)
            {
                string actualTimeTT = DateTime.Now.ToString("hh':'mm':'ss tt");
                string actualTime = DateTime.Now.ToString("hh':'mm':'ss");

                var fasz = (from bus in BusDBEntities.bus
                            join busTrace in BusDBEntities.bus_trace
                            on bus.line_id equals busTrace.line_id
                            where busTrace.exactTime == actualTime
                            select new
                            {
                                bus.id,
                                bus.line_id,
                                busTrace.latitude,
                                busTrace.longitude,
                                busTrace.exactTime
                            }).ToList();

                //A kapott koordinátákat beírjuk az adatbázisba, hogy onnan a szerver ki tudja olvasni.
                for(int i = 0; i < fasz.Count(); ++i)
                {
                    bus_data data = new bus_data
                    {
                        bus_id = 1,
                        number_plate = "MS09BZB",
                        latitude = fasz[i].latitude,
                        longitude = fasz[i].longitude,
                        speed = 0,
                        measurement_timestamp = System.DateTime.Now
                    };

                    BusDBEntities.bus_data.Add(data);
                    BusDBEntities.SaveChanges();
                }

                Thread.Sleep(100);
            }

        }

    }
}
