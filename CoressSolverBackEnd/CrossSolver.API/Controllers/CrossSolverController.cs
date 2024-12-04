using Microsoft.AspNetCore.Mvc;
using OpenCvSharp;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace CrossSolver.API.Controllers {
    [ApiController]
    [Route("[controller]")]
    public class CrossSolverController : ControllerBase {

        private readonly ILogger<CrossSolverController> _logger;

        public CrossSolverController(ILogger<CrossSolverController> logger) {
            _logger = logger;
        }

        [HttpPost("PostImageMarkLines")]
        public IActionResult PostImageMarkLines(IFormFile image) {
            if (image == null || image.Length == 0) {
                return BadRequest("No image uploaded");
            }

            try {
                using (var stream = image.OpenReadStream()) {
                    // Lade das Bild in OpenCV-Matrix
                    var cvImage = Mat.FromStream(stream, ImreadModes.Color);

                    // Konvertiere in Graustufen
                    var gray = new Mat();
                    Cv2.CvtColor(cvImage, gray, ColorConversionCodes.BGR2GRAY);

                    // Rauschunterdrückung (Gaussian Blur)
                    var blurred = new Mat();
                    Cv2.GaussianBlur(gray, blurred, new OpenCvSharp.Size(5, 5), 0);

                    // Kanten mit Canny erkennen
                    var edges = new Mat();
                    Cv2.Canny(blurred, edges, 50, 150);

                    // Finde Konturen
                    Cv2.FindContours(edges, out var contours, out _, RetrievalModes.External, ContourApproximationModes.ApproxSimple);

                    // Suche nach einem Quadrat (4 Ecken)
                    foreach (var contour in contours) {
                        var perimeter = Cv2.ArcLength(contour, true);
                        var approx = Cv2.ApproxPolyDP(contour, 0.02 * perimeter, true);

                        if (approx.Length == 4) // Quadrat gefunden
                        {
                            // Zeichne die Kontur rot auf das Bild
                            Cv2.Polylines(cvImage, new[] { approx }, true, new Scalar(0, 255, 0), 3);
                        }
                    }

                    // Rückgabe des Bildes mit markierten Quadraten
                    return File(cvImage.ToBytes(".png"), "image/png");
                }
            }
            catch {
                return StatusCode(500, "Error processing image");
            }
        }

        [HttpPost("PostImage")]
        public IActionResult PostImage(IFormFile image) {
            if (image == null || image.Length == 0) {
                return BadRequest("No image uploaded");
            }

            try {
                using (var stream = image.OpenReadStream()) {
                    // Lade das Bild in OpenCV-Matrix
                    var cvImage = Mat.FromStream(stream, ImreadModes.Color);

                    // Konvertiere in Graustufen
                    var gray = new Mat();
                    Cv2.CvtColor(cvImage, gray, ColorConversionCodes.BGR2GRAY);

                    // Rauschunterdrückung (Gaussian Blur)
                    var blurred = new Mat();
                    Cv2.GaussianBlur(gray, blurred, new OpenCvSharp.Size(5, 5), 0);

                    // Kanten mit Canny erkennen
                    var edges = new Mat();
                    Cv2.Canny(blurred, edges, 50, 150);

                    // Finde Konturen
                    Cv2.FindContours(edges, out var contours, out _, RetrievalModes.External, ContourApproximationModes.ApproxSimple);

                    // Suche nach einem Quadrat (4 Ecken)
                    foreach (var contour in contours) {
                        var perimeter = Cv2.ArcLength(contour, true);
                        var approx = Cv2.ApproxPolyDP(contour, 0.02 * perimeter, true);

                        if (approx.Length == 4) // Quadrat gefunden
                        {
                            // Sortiere die Punkte im Uhrzeigersinn
                            var sortedPoints = SortPointsClockwise(approx);

                            // Zielpunkte für die Perspektivkorrektur
                            var width = 500; // Zielbreite
                            var height = 500; // Zielhöhe
                            var destPoints = new[]
                            {
                        new Point2f(0, 0),
                        new Point2f(width - 1, 0),
                        new Point2f(width - 1, height - 1),
                        new Point2f(0, height - 1)
                    };

                            // Berechne die Perspektivtransformation
                            var matrix = Cv2.GetPerspectiveTransform(sortedPoints, destPoints);

                            // Wende die Transformation an
                            var alignedImage = new Mat();
                            Cv2.WarpPerspective(cvImage, alignedImage, matrix, new OpenCvSharp.Size(width, height));

                            return File(alignedImage.ToBytes(".png"), "image/png");
                        }
                    }

                    return BadRequest("No square detected");
                }
            }
            catch {
                return StatusCode(500, "Error processing image");
            }
        }

        private Point2f[] SortPointsClockwise(OpenCvSharp.Point[] points) {
            // Konvertiere von OpenCvSharp.Point zu OpenCvSharp.Point2f
            var points2f = points.Select(p => new Point2f(p.X, p.Y)).ToArray();

            // Berechne den Mittelpunkt
            var center = new Point2f(points2f.Average(p => p.X), points2f.Average(p => p.Y));

            // Sortiere die Punkte basierend auf ihrem Winkel relativ zum Mittelpunkt
            return points2f
                .OrderBy(p => Math.Atan2(p.Y - center.Y, p.X - center.X))
                .ToArray();
        }

        [HttpGet("Get")]
        public IActionResult Get() {
            return Ok("Hallo Auss");
        }
    }
}
