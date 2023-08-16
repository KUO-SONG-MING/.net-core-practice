using System;
using System.Threading.Tasks;

namespace Todo.Services
{
    public class AsyncService
    {
        public async Task<double> caculateAll() 
        {
            Task<double> result1 = caculate1();
            Task<double> result2 = caculate2();
            Task<double> result3 = caculate3();

            var result =await result1 + await result2 + await result3;
            return result;

        }

        public async Task<double> caculate1() 
        {
            DateTime start = DateTime.Now;
            await Task.Delay(1000);
            DateTime end = DateTime.Now;
            TimeSpan result = start - end;
            return result.TotalSeconds;
        }
        public async Task<double> caculate2()
        {
            DateTime start = DateTime.Now;
            await Task.Delay(2000);
            DateTime end = DateTime.Now;
            TimeSpan result = start - end;
            return result.TotalSeconds;
        }
        public async Task<double> caculate3()
        {
            DateTime start = DateTime.Now;
            await Task.Delay(3000);
            DateTime end = DateTime.Now;
            TimeSpan result = start - end;
            return result.TotalSeconds;
        }
    }
}
