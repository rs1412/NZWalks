using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NZWalks.API.CustomActionFilter;
using NZWalks.API.Model.Domain;
using NZWalks.API.Model.DTO;
using NZWalks.API.Repositories;

namespace NZWalks.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WalksController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IWalkRepository walkRepository;

        public WalksController(IMapper mapper, IWalkRepository walkRepository) 
        {
            this.mapper = mapper;
            this.walkRepository = walkRepository;
        }

        //Create Walk
        //Post: /api/walks
        [HttpPost]
        [ValidateModel]
        public async Task<IActionResult> Create([FromBody] AddWalkRequestDto addWalkRequestDto)
        {
            //if (ModelState.IsValid)
            //{
                //Map DTO to Domain Model
                var walkDomainModel = mapper.Map<Walk>(addWalkRequestDto);

                await walkRepository.CreateAsync(walkDomainModel);
                return Ok(mapper.Map<WalkDto>(walkDomainModel));
            //}
            //else
            //{ 
            //    return BadRequest(ModelState);
            //}
        }

        //Implementing Filtering on GetAll() method
        //api/walks?filterOn=name&filterQuery=Task

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? filetrOn,string? filterQuery, [FromQuery] string? sortBy, [FromQuery] bool? isAscending)
        { 
            var walkDomainModal = await walkRepository.GetAllAsync(filetrOn, filterQuery, sortBy, isAscending ?? true);

            //Map Domain Model to DTO
            return Ok(mapper.Map<List<WalkDto>>(walkDomainModal));
        }

        [HttpGet]
        [Route("{id:Guid}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        { 
            var walkDomainModel = await walkRepository.GetByIdAsync(id);

            if (walkDomainModel == null)
            { 
                return NotFound();
            }

            //Map Domain Model To the DTO
            return Ok(mapper.Map<WalkDto>(walkDomainModel));
        }

        //Update
        [HttpPut]
        [Route("{id:Guid}")]
        [ValidateModel]
        public async Task<IActionResult> Update([FromRoute] Guid id, UpdateWalkRequestDto updateWalkRequestDto)
        {
            //if (ModelState.IsValid)
            //{
                //Map DTO to Domain Model
                var walkDomainModal = mapper.Map<Walk>(updateWalkRequestDto);

                if (walkDomainModal == null)
                {
                    return NotFound();
                }

                //Call The Repository
                walkDomainModal = await walkRepository.UpdateAsync(id, walkDomainModal);

                //Map Domain Model to DTO
                return Ok(mapper.Map<WalkDto>(walkDomainModal));
            //}
            //else
            //{ 
            //    return BadRequest(ModelState);
            //}
        }

        //Delete
        [HttpDelete]
        [Route("{id:Guid}")]
        public async Task<IActionResult> Delete(Guid id)
        { 
            var deleteWalkDomainModal = await walkRepository.DeleteAsync(id);

            if(deleteWalkDomainModal == null)
            { 
                return NotFound();
            }

            //Map Domain Model to DTO
            return Ok(mapper.Map<WalkDto>(deleteWalkDomainModal));
        }

    }
}
