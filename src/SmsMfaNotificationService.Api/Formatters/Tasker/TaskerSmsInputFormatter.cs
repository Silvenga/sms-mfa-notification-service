using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace SmsMfaNotificationService.Api.Formatters.Tasker
{
    public class TaskerSmsInputFormatter : InputFormatter
    {
        private const string ContentType = "application/vnd+tasker+sms";

        public TaskerSmsInputFormatter()
        {
            SupportedMediaTypes.Add(ContentType);
        }

        public override async Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context)
        {
            var request = context.HttpContext.Request;
            using var reader = new StreamReader(request.Body);
            var content = await reader.ReadToEndAsync();

            var result = TaskerSmsInputParser.Parse(content);

            return await InputFormatterResult.SuccessAsync(result);
        }

        public override bool CanRead(InputFormatterContext context)
        {
            var contentType = context.HttpContext.Request.ContentType;
            return contentType.StartsWith(ContentType)
                   && context.ModelType == typeof(TaskerSmsReceived);
        }
    }
}
