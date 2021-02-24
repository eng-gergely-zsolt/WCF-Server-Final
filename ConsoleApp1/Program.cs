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
                // Példa: 03:15:31 PM
                //string actualTimeTT = DateTime.Now.ToString("hh':'mm':'ss tt");

                // Példa: 15:16:48, 24 órás formátum.
                string actualTime = DateTime.Now.ToString("HH':'mm':'ss");

                var busTraceDatas = (from line in BusDBEntities.line
                            join busTrace in BusDBEntities.bus_trace
                            on line.id equals busTrace.line_id
                            where busTrace.exactTime == actualTime
                            select new
                            {
                                line_id = line.id,
                                busTrace.course_id,
                                busTrace.latitude,
                                busTrace.longitude,
                                busTrace.direction
                            }).ToList();

                //for (int i = 0; i < busTraceDatas.Count(); ++i)
                //{
                //    Console.WriteLine(busTraceDatas[i]);
                //}
                //Console.WriteLine(actualTime);
                //Console.WriteLine("--------------------");


                //A kapott koordinátákat beírjuk az adatbázisba, hogy onnan a szerver ki tudja olvasni.
                for (int i = 0; i < busTraceDatas.Count(); ++i)
                {
                    course_data data = new course_data
                    {
                        line_id = busTraceDatas[i].line_id,
                        course_id = busTraceDatas[i].course_id,
                        latitude = busTraceDatas[i].latitude,
                        longitude = busTraceDatas[i].longitude,
                        direction = busTraceDatas[i].direction,
                        measurement_timestamp = System.DateTime.Now
                    };

                    BusDBEntities.course_data.Add(data);
                    BusDBEntities.SaveChanges();
                }


                //List<CourseDataType> BusInfirmationList = new List<CourseDataType>();
                //// Lekéri a bus_data táblából az összes rekordot, ahol a mesurement_timesatmp időpont benne van a 6 másodperces intervallumban.
                //// Az adatokat course_id szerint csoportosítja.
                //var groupedByCourseId = (from courseData in BusDBEntities.course_data
                //                         where System.Data.Entity.DbFunctions.DiffSeconds(courseData.measurement_timestamp, System.DateTime.Now) < 6
                //                         group courseData by courseData.course_id into groups
                //                         select new { courseId = groups.Key, restData = groups.ToList() });

                //BusInfirmationList.Clear();
                //foreach (var i in groupedByCourseId)
                //{
                //    Console.WriteLine(i.courseId);
                //    Console.WriteLine("--------------------");
                //    // Lekérdezi az aktuális csoportból a legfrisebb időpontot.
                //    var minTimestamp = i.restData.Min(x => x.measurement_timestamp);

                //    // Lekérdezi azt a rekordot a csoportból, amely tartalmazza az előbb lekérdezett időpontot.
                //    var busInformation = i.restData.First(x => x.measurement_timestamp == minTimestamp);

                //    BusInfirmationList.Add(new CourseDataType(
                //        busInformation.course_id, busInformation.line_id,
                //        busInformation.direction, busInformation.latitude, busInformation.longitude,
                //        busInformation.measurement_timestamp));
                //}


                //for (int i = 0; i < BusInfirmationList.Count(); ++i)
                //{
                //    Console.WriteLine(BusInfirmationList[i]);
                //}

                

                Thread.Sleep(100);
            }
        }
    }
}
