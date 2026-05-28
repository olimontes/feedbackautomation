using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/whatsapp")]
public class WhatsAppController : ControllerBase
{
    private readonly EvolutionService _evolution;

    public WhatsAppController(EvolutionService evolution)
    {
        _evolution = evolution;
    }

    [HttpPost("send")]
    public async Task<IActionResult> Send()
    {
        await _evolution.SendMessage(
            "5538998953383",
            "Mensagem enviada pelo sistema .NET 🚀"
        );

        return Ok(new
        {
            success = true
        });
    }
}