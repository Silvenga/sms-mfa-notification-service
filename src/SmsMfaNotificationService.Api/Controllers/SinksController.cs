using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SmsMfaNotificationService.Api.Actions.Commands;
using SmsMfaNotificationService.Api.Formatters.Tasker;

namespace SmsMfaNotificationService.Api.Controllers
{
    [Route("api/v1/sinks"), ApiController]
    public class SinksController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<SinksController> _logger;

        public SinksController(IMediator mediator, ILogger<SinksController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpPost("sms-received/tasker")]
        public async Task<IActionResult> SmsReceivedTasker([FromHeader(Name = "X-Client-Id"), Required]
                                                           string clientId,
                                                           [FromBody] TaskerSmsReceived payload,
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
