using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace KeyManagement.Controllers;

[ApiController]
[Route("[controller]")]
public class CheckController(LiteContext context) : ControllerBase
{
    [HttpPost]
    public Task<ActionResult> CheckKey([FromBody] KeyDto? keyDto)
    {
        try
        {
            if (keyDto is null) return Task.FromResult<ActionResult>(BadRequest());
            var keys = context.Keys.FindAll().ToList();
            var findKey = keys.FirstOrDefault(k => k.Key.ToString() == keyDto.Key);
            if (findKey is null) return Task.FromResult<ActionResult>(NotFound());

            findKey.RequestsCount++;
            context.Keys.Update(findKey);
            
            return Task.FromResult<ActionResult>(Ok());
        }
        catch (Exception e)
        {
            Log.ForContext<CheckController>().Error(e, "Ошибка проверки ключа: {key}", keyDto?.Key);
            return Task.FromResult<ActionResult>(StatusCode(500));
        }
    }
}

public class KeyDto
{
    public required string Key { get; set; }
}