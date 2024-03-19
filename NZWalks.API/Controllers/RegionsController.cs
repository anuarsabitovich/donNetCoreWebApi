using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NZWalks.API.Data;
using NZWalks.API.Models.Domain;
using NZWalks.API.Models.DTO;

namespace NZWalks.API.Controllers
{
    // http://localhost:1234/api/regions
    [Route("api/[controller]")]
    [ApiController]
    public class RegionsController : ControllerBase
    {
        private readonly NZWalksDbContext dbContext;

        public RegionsController(NZWalksDbContext dbContext)
        {
            this.dbContext = dbContext;
        }


        // GET ALL REGIONS 
        // GET: http://localhost:1234/api/regions
        [HttpGet]
        public IActionResult GetAll()
        {
            // Get Data From Database - Domain Models

            var regionsDomain = dbContext.Regions.ToList();

            // Map domain Models to DTOs
            var regionsDto = new List<RegionDto>();
            foreach(var regionDomain in regionsDomain)
            {
                regionsDto.Add(new RegionDto()
                {
                    Id= regionDomain.Id,
                    Code = regionDomain.Code,
                    Name = regionDomain.Name,
                    RegionImageUrl = regionDomain.RegionImageUrl, 

                });
            }

            // Return DTOs 

            return Ok(regionsDto);
        }

        // GET SINGLE REGION (Get region by id)
        // GET:  http://localhost:1234/api/regions/{id}
        [HttpGet]
        [Route("{id:Guid}")]
        public IActionResult GetById([FromRoute]  Guid id)
        {
            //var region = dbContext.Regions.Find(id);
            
            // Get Region Domain Model From Database

            var regionDomain = dbContext.Regions.FirstOrDefault(x => x.Id == id);

            if (regionDomain == null)
            {
                return NotFound();
            }

            // Map/Convert Region Domain Model to Region DTO
            var regionDto = new RegionDto()
            {
                Id = regionDomain.Id,
                Code = regionDomain.Code,
                Name = regionDomain.Name,
                RegionImageUrl = regionDomain.RegionImageUrl,
            };

            // Return DTO back to client
            return Ok(regionDto);
        }

        // POST To Create New Region
        // POST: http://localhost:1234/api/regions
        [HttpPost]
        public IActionResult Create([FromBody]  AddRegionRequestDto addRegionRequestDto)
        {
            // Map or convert DTO to Domain Model

            var regionDomainModel = new Region { 
                Code = addRegionRequestDto.Code, 
                Name = addRegionRequestDto.Name,
                RegionImageUrl = addRegionRequestDto.RegionImageUrl  
                };

            // Use domain model to create region
            dbContext.Add(regionDomainModel);
            dbContext.SaveChanges();

            // Map Domain model back to DTO
            var regionsDto = new RegionDto
            {
                Id = regionDomainModel.Id,
                Code = regionDomainModel.Code,
                Name = regionDomainModel.Name,
                RegionImageUrl = regionDomainModel.RegionImageUrl,
            };

            return CreatedAtAction(nameof(GetById), new {id = regionDomainModel.Id}, regionsDto  );
        }

    }
} 
