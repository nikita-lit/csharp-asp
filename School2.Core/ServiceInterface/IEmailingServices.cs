using School2.Core.DTO;

namespace School2.Core.ServiceInterface;

public interface IEmailingServices
{
    void SendEmail( EmailDTO dto );
    void SendEmailToken( EmailTokenDTO dto, string token );
}