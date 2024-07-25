using ArchiBase.Users;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using MimeKit;

namespace ArchiBase.Utils;

public class EmailConfiguration
{
    public string From { get; set; }
    public string SmtpServer { get; set; }
    public int Port { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
}

public class EmailSender : IEmailSender
{
    private readonly EmailConfiguration emailConfiguration;

    public EmailSender(EmailConfiguration emailConfiguration)
    {
        this.emailConfiguration = emailConfiguration;
    }


    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("no-reply", emailConfiguration.From));
        message.To.Add(new MailboxAddress(email, email));
        message.Subject = subject;
        message.Body = new TextPart("html") { Text = htmlMessage };

        using var client = new SmtpClient();
        try
        {
            client.CheckCertificateRevocation = false;
            await client.ConnectAsync(emailConfiguration.SmtpServer, emailConfiguration.Port);
            client.AuthenticationMechanisms.Remove("XOAUTH2");
            client.Authenticate(emailConfiguration.Username, emailConfiguration.Password);
            client.Send(message);
        }
        catch
        {
            throw;
        }
        finally
        {
            client.Disconnect(true);
            client.Dispose();
        }
    }
}

public class UserEmailSender : IEmailSender<ArchiBaseUser>
{

    private readonly IEmailSender emailSender;

    public UserEmailSender(IEmailSender emailSender)
    {
        this.emailSender = emailSender;
    }

    public async Task SendConfirmationLinkAsync(ArchiBaseUser user, string email, string confirmationLink)
    {
        await emailSender.SendEmailAsync(email, "Address Confirmation",
            $"""
            Hello, <b>{user.UserName}!</b><br>
            To confirm your e-mail address, click the following link:
            <a href="{confirmationLink}">Confirm e-mail</a>
            """);
    }

    public async Task SendPasswordResetCodeAsync(ArchiBaseUser user, string email, string resetCode)
    {
        await emailSender.SendEmailAsync(email, "Password reset code",
            $"""
            Hello, <b>{user.UserName}!</b><br>
            To reset your password, enter the following code:
            <b>{resetCode}</b>
            """);
    }

    public async Task SendPasswordResetLinkAsync(ArchiBaseUser user, string email, string resetLink)
    {
        await emailSender.SendEmailAsync(email, "Reset Password",
            $"""
            Hello, <b>{user.UserName}!</b><br>
            To reset your password, click the following link:
            <a href="{resetLink}">Reset password</a>
            """);
    }
}