using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Marvin.Cache.Headers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyWebAPI.Helpers;
using MyWebAPI.IRepository;
using MyWebAPI.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MyWebAPI.Controllers
{
    public class CountryController : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CountryController> _logger;
        private readonly IMapper _mapper;

        public CountryController(IUnitOfWork unitOfWork, ILogger<CountryController> logger, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        // [ResponseCache(Duration = 60)]
        // [ResponseCache(CacheProfileName = "120SecondsDuration")]
        // Overriding global cache rule
        [HttpCacheExpiration(CacheLocation = CacheLocation.Public, MaxAge = 120)]
        [HttpCacheValidation(MustRevalidate = false)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCountries([FromQuery] RequestParams requestParams)
        {
            try
            {
                var countries = await _unitOfWork.Countries.GetPagedList(requestParams);
                var result = _mapper.Map<IList<CountryDTO>>(countries);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{Constants.SomethingWrong} {nameof(GetCountries)}");
                return StatusCode(500, Constants.Error500);
            }
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCountry(int id)
        {
            try
            {
                var country = await _unitOfWork.Countries.Get(x => x.Id == id, 
                    includes: q => q.Include(x => x.Hotels));
                var result = _mapper.Map<CountryDTO>(country);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{Constants.SomethingWrong} {nameof(GetCountry)}");
                return StatusCode(500, Constants.Error500);
            }
        }
    }
}
