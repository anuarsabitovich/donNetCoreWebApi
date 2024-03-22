using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NZWalks.API.CustomActionFilters;
using NZWalks.API.Data;
using NZWalks.API.Models.Domain;
using NZWalks.API.Models.DTO;
using NZWalks.API.Repositories;

namespace NZWalks.API.Controllers
{
    // http://localhost:1234/api/regions
    [Route("api/[controller]")]
    [ApiController]
    public class RegionsController : ControllerBase
    {
        private readonly NZWalksDbContext dbContext;
        private readonly IRegionRepository regionRepository;
        private readonly IMapper mapper;

        public RegionsController(NZWalksDbContext dbContext, IRegionRepository regionRepository, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.regionRepository = regionRepository;
            this.mapper = mapper;
        }


        // GET ALL REGIONS 
        // GET: http://localhost:1234/api/regions
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            // Get Data From Database - Domain Models

            var regionsDomain = await regionRepository.GetAllAsync();

            // Map Domain Models to DTOs

            var regionsDto = mapper.Map<List<RegionDto>>(regionsDomain);

            // Return DTOs 

            return Ok(regionsDto);
        }

        // GET SINGLE REGION (Get region by id)
        // GET:  http://localhost:1234/api/regions/{id}
        [HttpGet]
        [Route("{id:Guid}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            //var region = dbContext.Regions.Find(id);

            // Get Region Domain Model From Database

            var regionDomain = await regionRepository.GetByIdAsync(id);

            if (regionDomain == null)
            {
                return NotFound();
            }

            // Map/Convert Region Domain Model to Region DTO

            var regionDto = mapper.Map<RegionDto>(regionDomain);

            // Return DTO back to client
            return Ok(regionDto);
        }

        // POST To Create New Region
        // POST: http://localhost:1234/api/regions
        [HttpPost]
        [ValidateModel]
        public async Task<IActionResult> Create([FromBody] AddRegionRequestDto addRegionRequestDto)
        {
          
                // Map or convert DTO to Domain Model

                var regionDomainModel = mapper.Map<Region>(addRegionRequestDto);

                // Use domain model to create region
                regionDomainModel = await regionRepository.CreateAsync(regionDomainModel);

                // Map Domain model back to DTO

                var regionsDto = mapper.Map<RegionDto>(regionDomainModel);

                return CreatedAtAction(nameof(GetById), new { id = regionDomainModel.Id }, regionsDto);
      
        }

        // Update region
        // PUT : http://localhost:1234/api/regions/{id}
        [HttpPut]
        [Route("{id:Guid}")]
        [ValidateModel]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateRegionRequestDto updateRegionRequestDto)
        {
           
                // Map dto to domain model

                var regionDomainModel = mapper.Map<Region>(updateRegionRequestDto);

                // check if region exists 
                regionDomainModel = await regionRepository.UpdateAsync(id, regionDomainModel);

                if (regionDomainModel == null)
                {
                    return NotFound();
                }

                return Ok(mapper.Map<RegionDto>(regionDomainModel));
         
        }

        // Delete Region
        // Delete: http://localhost:1234/api/regions/{id}

        [HttpDelete]
        [Route("{id:Guid}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var regionDomainModel = await regionRepository.DeleteAsync(id);

            if (regionDomainModel == null)
            {
                return NotFound();
            }

            return Ok(mapper.Map<RegionDto>(regionDomainModel));
        }

    }
}
