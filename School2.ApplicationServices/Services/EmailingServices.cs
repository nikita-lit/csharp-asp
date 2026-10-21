using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using School2.Core.DTO;
using School2.Core.ServiceInterface;

namespace School2.ApplicationServices.Services;

public class EmailingServices( IConfiguration config ) : IEmailingServices
{
    public void SendEmail( EmailDTO dto )
    {
        var email = new MimeMessage();
        email.From.Add( MailboxAddress.Parse( config.GetSection( "EmailUserName" ).Value ) );
        email.To.Add( MailboxAddress.Parse( dto.To ) );
        email.Subject = dto.Subject;

        var builder = new BodyBuilder
        {
            HtmlBody = dto.Body
        };

        //failide lisamine
        foreach ( var file in dto.Attachment )
            if ( file.Length > 0 && file.Length < 10485760 ) //10MB
                using ( var ms = new MemoryStream() )
                {
                    file.CopyTo( ms );
                    ms.Position = 0;
                    //var fileBytes = ms.ToArray();
                    //builder.Attachments.Add(file.FileName, fileBytes, ContentType.Parse(file.ContentType));
                    builder.Attachments.Add( file.FileName, ms.ToArray() );
                }

        email.Body = builder.ToMessageBody();

        using var smtp = new SmtpClient();

        smtp.Connect( config.GetSection( "EmailHost" ).Value, 587, SecureSocketOptions.StartTls );
        smtp.Authenticate( config.GetSection( "EmailUserName" ).Value, config.GetSection( "EmailPassword" ).Value );
        smtp.Send( email );
        smtp.Disconnect( true );
    }

    public void SendEmailToken( EmailTokenDTO dto, string token )
    {
        dto.Token = token;
        var email = new MimeMessage();

        email.From.Add( MailboxAddress.Parse( config.GetSection( "EmailUserName" ).Value ) );
        email.To.Add( MailboxAddress.Parse( dto.To ) );
        email.Subject = dto.Subject;
        var builder = new BodyBuilder
        {
            HtmlBody = dto.Body
        };

        email.Body = builder.ToMessageBody();
        using var smtp = new SmtpClient();

        smtp.Connect( config.GetSection( "EmailHost" ).Value, 587, SecureSocketOptions.StartTls );
        smtp.Authenticate( config.GetSection( "EmailUserName" ).Value, config.GetSection( "EmailPassword" ).Value );
        smtp.Send( email );
        smtp.Disconnect( true );
    }
}