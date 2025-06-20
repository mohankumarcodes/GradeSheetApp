using GradeSheetApp.Models;
using GradeSheetApp.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GradeSheetApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GradeSheetController : ControllerBase
    {
        private readonly IGradeCalculator _calculator;
        private readonly ILogger<GradeSheetController> _logger;

        public GradeSheetController(IGradeCalculator calculator,
                                    ILogger<GradeSheetController> logger)
        {
            _calculator = calculator;
            _logger = logger;
        }

        /// <summary>
        /// Calculates overall percentages & letter grades.
        /// Upload the CSV via multipart/form‑data with field name "file".
        /// First column = StudentId, remaining columns = numeric marks.
        /// </summary>
        [HttpPost("calculate")]
        public async Task<ActionResult<GradeSheetResponse>> Calculate(
            IFormFile file,
            [FromQuery(Name = "exclude-lowest")] bool excludeLowest = false,
            CancellationToken ct = default)
        {
            if (file == null || file.Length == 0)
                return BadRequest("File is required.");

            try
            {
                await using var stream = file.OpenReadStream();
                var result = await _calculator.CalculateAsync(stream, excludeLowest, ct);
                return Ok(result);
            }
            catch (InvalidDataException ex)
            {
                _logger.LogWarning(ex, "Bad CSV");
                return BadRequest(ex.Message);
            }
        }
    }
}
