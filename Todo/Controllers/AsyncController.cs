using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Todo.Services;

namespace Todo.Controllers
{
    [Route("api/{controller}")]
    public class AsyncController : Controller
    {
        private AsyncService _asyncService;

        public AsyncController(AsyncService asyncService) 
        {
            _asyncService = asyncService;
        }

        [HttpGet]
        public async Task<double> getAsyncTest() 
        {
            Task<double> result = _asyncService.caculateAll();
            return await result;
        }
    }
}
