﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        public async Task<IActionResult>  GetAll()
        {
            // Get Data From Database - Domain Models

            var regionsDomain = await dbContext.Regions.ToListAsync();

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
        public async Task<IActionResult>  GetById([FromRoute]  Guid id)
        {
            //var region = dbContext.Regions.Find(id);
            
            // Get Region Domain Model From Database

            var regionDomain = await dbContext.Regions.FirstOrDefaultAsync(x => x.Id == id);

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
        public async Task<IActionResult>  Create([FromBody]  AddRegionRequestDto addRegionRequestDto)
        {
            // Map or convert DTO to Domain Model

            var regionDomainModel = new Region { 
                Code = addRegionRequestDto.Code, 
                Name = addRegionRequestDto.Name,
                RegionImageUrl = addRegionRequestDto.RegionImageUrl  
                };

            // Use domain model to create region
            await dbContext.AddAsync(regionDomainModel);
            await dbContext.SaveChangesAsync();

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

        // Update region
        // PUT : http://localhost:1234/api/regions/{id}
        [HttpPut]
        [Route("{id:Guid}")]
        public async Task<IActionResult>  Update([FromRoute] Guid id, [FromBody] UpdateRegionRequestDto updateRegionRequestDto)
        {
            // check if region exists 
            var regionDomainModel = await dbContext.Regions.FirstOrDefaultAsync(x => x.Id == id);

            if (regionDomainModel == null)
            {
                return NotFound();
            }

            // Map DTO to Domain model
            regionDomainModel.Code = updateRegionRequestDto.Code;  
            regionDomainModel.Name = updateRegionRequestDto.Name;
            regionDomainModel.RegionImageUrl = updateRegionRequestDto.RegionImageUrl;

            await dbContext.SaveChangesAsync();

            // Convert Domain Model to DTO
            var regionDto = new RegionDto
            {
                Id = regionDomainModel.Id,
                Code = regionDomainModel.Code,
                Name = regionDomainModel.Name,
                RegionImageUrl = regionDomainModel.RegionImageUrl,
            };

            return Ok(regionDto);
        }

        // Delete Region
        // Delete: http://localhost:1234/api/regions/{id}

        [HttpDelete]
        [Route("{id:Guid}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var regionDomainModel = await dbContext.Regions.FirstOrDefaultAsync(x => x.Id == id);

            if (regionDomainModel == null)
            {
                return NotFound();
            }

            // Delete region
            dbContext.Regions.Remove(regionDomainModel);
            await dbContext.SaveChangesAsync();

            // return deleted Region back
            // map Domain Model to DTO
            var regionDto = new RegionDto()
            {
                Id = regionDomainModel.Id,
                Code = regionDomainModel.Code,
                Name = regionDomainModel.Name,
                RegionImageUrl = regionDomainModel.RegionImageUrl,
            };
            return Ok(regionDto);
        }

    }
} 
