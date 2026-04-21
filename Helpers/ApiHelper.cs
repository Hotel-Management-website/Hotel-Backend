using Microsoft.AspNetCore.Mvc;

namespace HotelBookingAPI.Helpers
{
    public static class ApiHelper
    {
        public static IActionResult Success<T>(T data, string message = "Success") =>
            new OkObjectResult(ApiResponse<T>.Ok(data, message));

        public static IActionResult Created<T>(T data, string message = "Created successfully") =>
            new ObjectResult(ApiResponse<T>.Ok(data, message)) { StatusCode = 201 };

        public static IActionResult BadRequest(string message) =>
            new BadRequestObjectResult(ApiResponse<object>.Fail(message));

        public static IActionResult NotFound(string message = "Resource not found") =>
            new NotFoundObjectResult(ApiResponse<object>.Fail(message));

        public static IActionResult Unauthorized(string message = "Unauthorized") =>
            new UnauthorizedObjectResult(ApiResponse<object>.Fail(message));
    }
}
