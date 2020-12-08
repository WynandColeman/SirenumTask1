using System;
using System.Collections.Generic;
using System.Linq;


namespace SirenumTask1
{
    class Program
    {
        static void Main(string[] args)
        {
            //Generate Data            
            List<shift> shifts = new List<shift>();
            shifts.Add(new shift { start = DateTime.Parse("2020-06-23 09:00"), end = DateTime.Parse("2020-06-23 17:00"), workerId = "John" });
            shifts.Add(new shift { start = DateTime.Parse("2020-06-24 06:00"), end = DateTime.Parse("2020-06-24 14:00"), workerId = "John" });
            shifts.Add(new shift { start = DateTime.Parse("2020-06-23 11:30"), end = DateTime.Parse("2020-06-23 18:00"), workerId = "Emma" });            

            List<payrate> payrates = new List<payrate>();
            payrates.Add(new payrate { hourlyRate = decimal.Parse("15.00"), name = "Morning", timeOfDayStart = DateTime.Parse("05:00"), timeOfDayEnd = DateTime.Parse("10:00") });
            payrates.Add(new payrate { hourlyRate = decimal.Parse("18.00"), name = "Evening", timeOfDayStart = DateTime.Parse("16:30"), timeOfDayEnd = DateTime.Parse("20:00") });
            payrates.Add(new payrate { hourlyRate = decimal.Parse("20.00"), name = "Night", timeOfDayStart = DateTime.Parse("20:00"), timeOfDayEnd = DateTime.Parse("23:00") });
            payrates.Add(new payrate { });

            List<worker> workers = new List<worker>();
            workers.Add(new worker { workerId = "John" });
            workers.Add(new worker { workerId = "Emma" });

            //Create Lists to be used for calculating results and to display
            List<result> resultList = new List<result>();

            List<breakdown> breakdownList = new List<breakdown>();

            //For each shift call method to breakdown shifts into results used for calcs
            foreach (var shift in shifts)
            {
                createResultList(shift);
            }
            
            //For each result call method to calculate breakdown result
            foreach (var result in resultList)
            {

                resultBreakDown(result);
                
            }

            //Display Breakdown results
            foreach (var worker in workers)
            {
                var payRatesforWorker = from breakdown in breakdownList
                                        where breakdown.Worker.Equals(worker.workerId)
                                        select breakdown;                

                    foreach (var rate in payrates)
                    {
                        
                            var rateFilter = from breakdown in payRatesforWorker
                                             where breakdown.RateName.Equals(rate.name) && breakdown.Worker.Equals(worker.workerId)
                                             select breakdown;

                            var hours = new TimeSpan(rateFilter.Sum(item => item.TotalWork.Ticks));
                            var sumOfMinutes = rateFilter.Sum(item => item.TotalWork.TotalMinutes);

                    if (hours != TimeSpan.Parse("00:00:00"))
                    {
                        Console.WriteLine(worker.workerId + ", " + rate.name + ", " + hours + ", £ " + ((sumOfMinutes / 60) * Convert.ToDouble(rate.hourlyRate)));
                    }

                }
            }

            // Method to Create resultlist by breaking the shift down into matching payrates
            void createResultList(shift shift)
            {
                TimeSpan shiftStartHourMinSec = TimeSpan.Parse(shift.start.ToString("HH:mm:ss")); 
                TimeSpan shiftEndHourMinSec = TimeSpan.Parse(shift.end.ToString("HH:mm:ss")); 

                TimeSpan hoursWorked;
                decimal totalPay;
                
                TimeSpan startShift = TimeSpan.Parse(shift.start.ToString("HH:mm:ss")); ;
                TimeSpan endShift = TimeSpan.Parse(shift.end.ToString("HH:mm:ss"));
                var hoursWorkedCountdown = endShift - startShift;
                var lastUsedBeforeDefaultCount = 0;

                while (hoursWorkedCountdown > TimeSpan.Parse("00:00:00"))
                {

                    for (int i = lastUsedBeforeDefaultCount; i < payrates.Count; i++)
                    {
                        payrate rate = payrates[i];

                        TimeSpan rateStartHourMinSec = TimeSpan.Parse(payrates[lastUsedBeforeDefaultCount].timeOfDayStart.ToString("HH:mm:ss"));
                        TimeSpan rateEndHourMinSec = TimeSpan.Parse(payrates[lastUsedBeforeDefaultCount].timeOfDayEnd.ToString("HH:mm:ss"));
                        TimeSpan nextShift = TimeSpan.Parse(payrates[lastUsedBeforeDefaultCount + 1].timeOfDayStart.ToString("HH:mm:ss")); 

                        if (endShift < rateEndHourMinSec && endShift != rateEndHourMinSec)
                        {
                            rateEndHourMinSec = endShift;
                        }


                        if (startShift >= rateStartHourMinSec && startShift < rateEndHourMinSec && rateEndHourMinSec <= nextShift)
                        {
                            hoursWorked = rateEndHourMinSec - startShift;

                            resultList.Add(new result { WorkerId = shift.workerId, RateName = rate.name, HoursWorked = hoursWorked });
                            
                            hoursWorkedCountdown -= hoursWorked; 
                            startShift = TimeSpan.Parse(payrates[lastUsedBeforeDefaultCount].timeOfDayEnd.ToString("HH:mm:ss"));

                            lastUsedBeforeDefaultCount = i;
                            startShift = rateEndHourMinSec;

                        }
                        else if (rate.name == "Default" && hoursWorkedCountdown > TimeSpan.Parse("00:00:00"))
                        {
                            if (endShift < rateEndHourMinSec && hoursWorkedCountdown > (endShift - rateEndHourMinSec))
                            {

                            }
                            else
                            {
                                rateEndHourMinSec = nextShift;

                            }




                            if (endShift < rateEndHourMinSec)
                            {
                                rateEndHourMinSec = endShift;
                            }                            
                            
                            hoursWorked = rateEndHourMinSec - startShift;
                            totalPay = Convert.ToDecimal(hoursWorked.TotalHours) * rate.hourlyRate;

                            resultList.Add(new result { WorkerId = shift.workerId, RateName = rate.name, HoursWorked = hoursWorked});


                            hoursWorkedCountdown -= hoursWorked;
                            startShift = rateEndHourMinSec;
                            lastUsedBeforeDefaultCount += 1;

                        }
                        if (hoursWorkedCountdown == TimeSpan.Parse("00:00:00"))
                        {
                            break;
                        }
                    }
                    

                }
            }
            void resultBreakDown(result result)
            {
                TimeSpan totalWork = TimeSpan.Parse("00:00:00");
                foreach (var rate in payrates)
                {                    
                    if (rate.name == result.RateName)
                    {
                        totalWork += result.HoursWorked;
                    }                  

                }
                breakdownList.Add(new breakdown { Worker = result.WorkerId, RateName = result.RateName, TotalWork = totalWork});
            }
        }   
    }
}
