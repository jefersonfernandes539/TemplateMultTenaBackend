using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Newtonsoft.Json;

namespace TemplateMultTenaBackend.Domain.RequestFeatures.Extensions
{
    [ApiController]
    public abstract class MainController : Controller
    {
        protected ActionResult CustomResponse<T>(ServiceResult<T> request)
        {
            if (request.IsSuccess)
            {
                if (Request.Method == "GET")
                {
                    if (request.Data is Unit or null)
                        return Ok();

                    var data = request.Data;

                    Type type = data.GetType();
                    if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(PagedResponse<>))
                    {
                        var items = type.GetProperty("Items")!.GetValue(data);
                        var metaData = type.GetProperty("MetaData")!.GetValue(data);

                        Response.Headers.Append("X-Pagination", JsonConvert.SerializeObject(metaData));
                        return Ok(items);
                    }

                    return Ok(data);
                }
                else if (Request.Method == "POST")
                    return Created(string.Empty, request.Data);
                else if (Request.Method == "PUT" || Request.Method == "DELETE")
                    return request.Data is Unit or null ? NoContent() : Ok(request.Data);
            }

            return StatusCode(request.StatusCode, request.ErrorMessages);
        }
    }

    public static class WebExtension
    {
        public static Guid TenantId(this MainController mainController)
        {
            var tenantId = mainController.HttpContext?.User?.FindFirst("Tenant.Id")?.Value;
            return Guid.TryParse(tenantId, out var tenantGuid) ? tenantGuid : Guid.Empty;
        }

        public static Guid UserId(this MainController mainController)
        {
            var userId = mainController.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;
            return Guid.TryParse(userId, out var currentUserIdGuid) ? currentUserIdGuid : Guid.Empty;
        }

        public static Guid RoleId(this MainController mainController)
        {
            var roleId = mainController.HttpContext?.User?.FindFirst("TenantUser.RoleId")?.Value;
            return Guid.TryParse(roleId, out var currentRoleIdGuid) ? currentRoleIdGuid : Guid.Empty;
        }
    }
}
