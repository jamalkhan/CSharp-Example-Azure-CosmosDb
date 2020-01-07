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
        public virtual ActionResult<IList<TDatabaseType>> Get(int? page = 0, int? pageSize = 10)
        {
            var data = _service.Get(page: page.Value, pageSize: pageSize.Value);
            var dtos = ConvertToDtoType(data.Results);
            var retval = new PaginatedResults<TDtoType>();
            retval.Results = dtos;
            retval.Returned.Count = data.Returned.Count;
            retval.Returned.FirstRecord = data.Returned.FirstRecord;
            retval.Returned.LastRecord = data.Returned.LastRecord;
            retval.Returned.Page = data.Returned.Page;
            retval.Totals.Records = data.Totals.Records;
            retval.Totals.Pages = data.Totals.Pages;
            return Ok(retval);
        }

        private IList<TDtoType> ConvertToDtoType(IEnumerable<TDatabaseType> data)
        {
            var retval = new List<TDtoType>();
            foreach (var d in data)
            {
                retval.Add(ConvertToDtoType(d));
            }
            return retval;
        }
        public virtual TDtoType ConvertToDtoType(TDatabaseType input)
        {
            return _mapper.Map<TDtoType>(input);
        }

        public virtual TDatabaseType ConvertToDatabaseType(TDtoType input)
        {
            return _mapper.Map<TDatabaseType>(input);
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