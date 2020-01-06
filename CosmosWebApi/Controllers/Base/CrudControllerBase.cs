using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AutoMapper;
using bleak.Validation;
using CosmosWebApi.DataObjects;
using CosmosWebApi.DataServices;
using CosmosWebApi.DataTransferObjects;
using Microsoft.AspNetCore.Mvc;

namespace CosmosWebApi.Controllers
{

    public abstract class CrudControllerBase<TService, TDtoType, TDatabaseType>
        : ControllerBase
        where TService : IDataService<TDatabaseType>
        where TDatabaseType : IDataEntity
        where TDtoType : IDtoEntity
    {
        public readonly TService _service;
        public IMapper _mapper;
        public CrudControllerBase(
            TService service,
            IMapper mapper
            )
        {
            _service = service;
            _mapper = mapper;
        }

        public string EntityName
        {
            get
            {
                return typeof(TDatabaseType).Name;
            }
        }

        [HttpGet]
        public virtual ActionResult<List<TDatabaseType>> Get()
        {
            return _service.Get();
        }

        [HttpGet("{id:length(24)}")]
        public virtual ActionResult<TDatabaseType> Get(string id)
        {
            var obj = _service.Get(id);

            if (obj == null)
            {
                return NotFound();
            }

            return obj;
        }

        [HttpPost]
        public virtual ActionResult<TDtoType> Create(TDtoType input)
        {
            // map dto to entity
            var databaseInput = ConvertToDatabaseType(input);

            // Validate input
            try
            {
                databaseInput.Validate();
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            // Commit to database
            _service.Create(databaseInput);
            input.Id = databaseInput.Id;
            return input;
        }

        public virtual TDatabaseType ConvertToDatabaseType(TDtoType dtoObject)
        {
            return _mapper.Map<TDatabaseType>(dtoObject);
        }

        public virtual void ValidateModel(object input)
        {
            input.Validate();
        }

        [HttpPut("{id:length(24)}")]
        public virtual IActionResult Update(string id, TDtoType input)
        {
            // map dto to entity
            var databaseInput = ConvertToDatabaseType(input);

            // Validate input
            try
            {
                databaseInput.Validate();
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            // Make sure the object exists in the database
            var obj = _service.Get(id);

            if (obj == null)
            {
                return NotFound();
            }

            // Commit to database
            _service.Update(id, databaseInput);

            return NoContent();
        }

        [HttpDelete("{id:length(24)}")]
        public virtual IActionResult Delete(string id)
        {
            var obj = _service.Get(id);

            if (obj == null)
            {
                return NotFound();
            }

            _service.Remove(obj.Id);

            return NoContent();
        }
    }
}