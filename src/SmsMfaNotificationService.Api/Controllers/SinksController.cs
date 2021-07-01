using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SmsMfaNotificationService.Api.Actions.Commands;

namespace SmsMfaNotificationService.Api.Controllers
{
    [Route("api/v1/sinks"), ApiController]
    public class SinksController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SinksController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("sms-received/tasker")]
        public async Task<IActionResult> SmsReceivedTasker([FromHeader, Required] string clientId,
                                                           [FromBody] string payload,
                                                           CancellationToken cancellationToken)
        {
            var command = new SmsReceived.Command(clientId, payload);
            var result = await _mediator.Send(command, cancellationToken);
            if (result is SmsReceived.SuccessfulResult)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        [HttpPost("ping")]
        public IActionResult Ping()
        {
            return Ok(new
            {
                Result = "pong"
            });
        }
    }
}
