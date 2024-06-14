using Azure.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Praktek_API.Data; 
using Praktek_API.Models;
using System.Data;
using System.Net;
using System.Numerics;

namespace Praktek_API.Controllers 
{
    [Route("api/[controller]")]
    [ApiController]
    public class KaryawanController : ControllerBase
    {
        private readonly DataContext _context;

        public KaryawanController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetData()
        {
            return Ok(await _context.Karyawan.ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> StoreData(RequestData request)
        {
            var data = new Karyawan()
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Divisi = request.Divisi,
            };

            await _context.AddAsync(data);
            await _context.SaveChangesAsync();

            return Ok(data);
        }

        [HttpGet]
        [Route("{id:guid}")]
        public async Task<IActionResult> GetDetail([FromRoute] Guid id)
        {
            var data = await _context.Karyawan.FindAsync(id);

            if (data == null)
            {
                return NotFound();
            }

            return Ok(data);
        }

        [HttpPut]
        [Route("{id:guid}")]
        public async Task<IActionResult> Update([FromRoute] Guid id, RequestData request)
        {
            var data = await _context.Karyawan.FindAsync(id);

            if (data != null)
            {
                data.Name = request.Name;
                data.Divisi = request.Divisi;

                await _context.SaveChangesAsync();
                return Ok(data);
            }

            return NotFound();
        }

        [HttpDelete]
        [Route("{id:guid}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var data = await _context.Karyawan.FindAsync(id);

            if (data != null)
            {
                _context.Remove(data);
                await _context.SaveChangesAsync();
                return Ok(data);
            }

            return NotFound();
        }

        [HttpGet("ApiBaru/getKaryawan")]
        [ProducesResponseType(typeof(List<Karyawan>), (int)HttpStatusCode.OK)]
        public JsonResult GetKaryawan()
        {
            var sql = "SELECT * FROM Karyawan";

            var results = _context.Karyawan.FromSqlRaw(sql).ToList();
            return new JsonResult(results);
        }

        [Route("Store/DataKaryawanNew")]
        [HttpPost]
        [ProducesResponseType(typeof(List<Karyawan>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> StoreData2([FromBody] RequestData request)
        {
            Guid Id = Guid.NewGuid();
            if (!PostKaryawan(Id, request.Name, request.Divisi))
            {
                ModelState.AddModelError("", "Something went wrong while saving");
                return StatusCode(500, ModelState);
            }
            return Ok();
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [NonAction]
        public bool PostKaryawan(Guid Id, string Name, string Divisi)
        {
            try
            {
                var sql = "INSERT INTO Karyawan (Id, Name, Divisi) VALUES (@id, @name, @divisi)";
                var parameters = new[] {
                    new SqlParameter("@id", SqlDbType.UniqueIdentifier) { Value = Id },
                    new SqlParameter("@name", SqlDbType.NVarChar) { Value = Name },
                    new SqlParameter("@divisi", SqlDbType.NVarChar) { Value = Divisi },
                };

                _context.Database.ExecuteSqlRaw(sql, parameters);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
