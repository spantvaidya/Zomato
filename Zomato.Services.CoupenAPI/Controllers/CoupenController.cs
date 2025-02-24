using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Zomato.Services.CoupenAPI.Data;

namespace Zomato.Services.CoupenAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoupenController : ControllerBase
    {
        private readonly AppDbContext _dbcontext;
        public CoupenController(AppDbContext dbContext)
        {
            _dbcontext = dbContext;
        }
        [HttpGet]
        public object GetCoupens()
        {
            return _dbcontext.Coupens.ToList();
        }

        [HttpGet]
        [Route("{id:int}")]
        public object GetCoupen(int id)
        {
            try
            {
                return _dbcontext.Coupens.FirstOrDefault(x => x.CoupenId == id);
            }
            catch (Exception ex)
            {

            }

            return null;
        }
    }
}
