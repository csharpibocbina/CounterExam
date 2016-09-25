using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ocbina_israel_Exam.Models;
using System.Web.Script.Serialization;
using System.Threading.Tasks;
using System.Threading;

/*********************************************
 * Developer    : Israel Ocbina
 * Date         : 9/23/2016
 * Description  : This is a counter that count 1...10 but
 *              it cancels >10 hits. This saves record every hits to the SQL local Database.
 *              The Database was created at runtime using 
 *              CODEFIRST approach in EF5. The DB file attached to
 *              the App_Data root folder in the project. Also, AngularJS v1.5.8.
 *              was used for AJAX, REST calls.
 * Note         : README.txt was added in this project.
 * Copyright    : Cyberocbina ™ 2016.
 * 
 * *******************************************/

namespace ocbina_israel_Exam.Controllers
{

    public partial class HomeController : Controller
    {
        private object oLock = new object();
        private SynchronizationContext syncContext = new SynchronizationContext();
        #region --Get Counter Value HTTPGET
        [HttpGet]
        public string GetCurrentValue()
        {
            var serialize = new JavaScriptSerializer();
            var counter = new Counter();
            int initVal = 0;
            using (var counterContext = new CounterDBContext())
            {
                if (counterContext.Counters.Count() == 0)
                {
                    initVal = counterContext.Counters.Count();
                }
                else
                {
                    initVal = counterContext.Counters.OrderBy(o => o.Id).Take(1).SingleOrDefault().CounterValue;
                }
            }
            return serialize.Serialize(initVal.ToString());
        }
        #endregion

        #region --Get Incremental Value HTTPGET
        [HttpGet]
        public async Task<string> IncrementValue(dynamic prevVal)
        {
            return await Task.Run(() =>
            {
                var serialize = new JavaScriptSerializer();
                int latestValue = Convert.ToInt16(serialize.Deserialize<string>(prevVal[0]));
                int newValue = latestValue;
                Counter counter = new Counter();
                var counterContext = new CounterDBContext();
                {
                    if (newValue < 1)
                    {
                        syncContext.Post((o) =>
                        {
                            newValue++;
                            counter.CounterValue = newValue;
                        }, null);
                        counterContext.Entry(counter).State = EntityState.Added;
                        counterContext.SaveChanges();
                    }
                    else if (newValue < 10)
                    {
                        counter = counterContext.Counters.Where(o => o.CounterValue == latestValue).FirstOrDefault<Counter>();
                        newValue++;
                        syncContext.Post((o) =>
                        {
                            counter.CounterValue = newValue;
                        }, null);
                        counterContext.Entry(counter).State = EntityState.Modified;
                        counterContext.SaveChanges();
                        Thread.Sleep(50);
                    }
                }
                return serialize.Serialize(newValue.ToString());
            },CancellationToken.None);
        }
        #endregion
    }
}
