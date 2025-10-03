using System;
using Microsoft.AspNetCore.Mvc;
using Zeus.Academia.Api.Controllers.V1;
using Zeus.Academia.Api.Models.Common;

public class TestController : FacultyController
{
    public void TestWhichBaseIsUsed()
    {
        var result = CreateSuccessResponse("test");
        Console.WriteLine($"CreateSuccessResponse returns: {result.GetType().Name}");

        if (result is ApiResponse)
        {
            Console.WriteLine("Using Controllers/Base/BaseApiController");
        }
        else if (result is IActionResult)
        {
            Console.WriteLine("Using Controllers/BaseApiController");
        }
    }
}