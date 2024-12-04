using Microsoft.AspNetCore.Mvc;
using System.Drawing;
using System.Drawing.Imaging;

namespace CrossSolver.API.Controllers {
    [ApiController]
    [Route("[controller]")]
    public class CrossSolverController : ControllerBase {

        private readonly ILogger<CrossSolverController> _logger;

        public CrossSolverController(ILogger<CrossSolverController> logger) {
            _logger = logger;
        }

        [HttpPost(Name = "PostImage")]
        public IActionResult PostImage(IFormFile image) {
            if (image == null || image.Length == 0) {
                return BadRequest("No image uploaded");
            }

            try {
                using (var stream = image.OpenReadStream())
                using (var bitmap = new Bitmap(stream)) {
                    using (var ms = new MemoryStream()) {
                        bitmap.Save(ms, ImageFormat.Png);
                        return File(ms.ToArray(), "image/png");
                    }
                }
            }
            catch {
                return StatusCode(500, "Error processing image");
            }
        }

        [HttpGet(Name = "Get")]
        public IActionResult Get() {
            return Ok("Hallo Auss");
        }
    }
}
