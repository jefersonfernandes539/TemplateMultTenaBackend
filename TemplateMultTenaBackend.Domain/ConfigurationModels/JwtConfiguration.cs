using System.ComponentModel.DataAnnotations;
using TemplateMultTenaBackend.Application.Attributes;

namespace TemplateMultTenaBackend.Domain.ConfigurationModels
{
    public class JwtConfiguration
    {
        public string Section { get; set; } = "JwtSettings";

        [Required, NotEmpty]
        public string ValidIssuer { get; set; } = string.Empty;

        [Required, NotEmpty]
        public string ValidAudience { get; set; } = string.Empty;

        [Required, NotEmpty]
        public string Expires { get; set; } = string.Empty;

        [Required, NotEmpty]
        public string Secret { get; set; } = string.Empty;
    }
}
