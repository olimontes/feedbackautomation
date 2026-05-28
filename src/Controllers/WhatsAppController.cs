using FeedbackAutomation.Data;
using FeedbackAutomation.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/whatsapp")]
public class WhatsAppController : ControllerBase
{
    private readonly EvolutionService _evolution;
    private readonly AppDbContext _context;

    public WhatsAppController(
        EvolutionService evolution,
        AppDbContext context
    )
    {
        _evolution = evolution;
        _context = context;
    }

    [HttpPost("send/{clienteId:int}")]
    public async Task<IActionResult> SendPorCliente(
        int clienteId,
        [FromQuery] string? mensagem
    )
    {
        return await EnviarParaClienteAsync(clienteId, mensagem);
    }

    [HttpPost("send")]
    public async Task<IActionResult> Send(
        [FromBody] EnviarMensagemWhatsAppRequest request
    )
    {
        if (request.ClienteId == null)
        {
            return BadRequest(new
            {
                success = false,
                error = "Informe o ClienteId para buscar o telefone no banco."
            });
        }

        return await EnviarParaClienteAsync(
            request.ClienteId.Value,
            request.Mensagem
        );
    }

    private async Task<IActionResult> EnviarParaClienteAsync(
        int clienteId,
        string? mensagem
    )
    {
        var cliente = await _context.Clientes
            .FirstOrDefaultAsync(c => c.Id == clienteId);

        if (cliente == null)
        {
            return NotFound(new
            {
                success = false,
                error = "Cliente nao encontrado."
            });
        }

        var telefone = TelefoneHelper.NormalizarTelefoneWhatsApp(
            cliente.Telefone
        );

        if (!TelefoneHelper.TelefoneValidoParaWhatsApp(telefone))
        {
            return BadRequest(new
            {
                success = false,
                error = "Cliente sem telefone valido para WhatsApp.",
                clienteId = cliente.Id,
                cliente.NomeRede,
                telefone = cliente.Telefone
            });
        }

        if (cliente.Telefone != telefone)
        {
            cliente.Telefone = telefone;
            await _context.SaveChangesAsync();
        }

        await _evolution.SendMessage(
            telefone,
            mensagem ?? "Mensagem enviada pelo sistema .NET"
        );

        return Ok(new
        {
            success = true,
            clienteId = cliente.Id,
            cliente.NomeRede,
            telefone
        });
    }
}

public class EnviarMensagemWhatsAppRequest
{
    public int? ClienteId { get; set; }

    public string? Mensagem { get; set; }
}
