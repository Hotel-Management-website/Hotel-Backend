using HotelBooking.API.DTOs.Booking;
using HotelBookingAPI.Services.Interfaces;
using MailKit.Net.Smtp;
using MimeKit;

namespace HotelBooking.API.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;

    public EmailService(IConfiguration config)
    {
        _config = config;
    }

    public async Task SendBookingConfirmationAsync(string toEmail, ConfirmationDto dto)
    {
        var email = new MimeMessage();
        email.From.Add(new MailboxAddress(
            _config["EmailSettings:FromName"],
            _config["EmailSettings:FromAddress"]));

        email.To.Add(MailboxAddress.Parse(toEmail));
        email.Subject = "Booking Confirmation";

        email.Body = new TextPart("html")
        {
            Text = $"<h2>Booking Confirmed</h2>" +
                   $"<p>Reservation: {dto.ReservationNumber}</p>" +
                   $"<p>Hotel: {dto.HotelName}</p>" +
                   $"<p>Room: {dto.RoomNumber}</p>" +
                   $"<p>Check-in: {dto.CheckIn}</p>" +
                   $"<p>Check-out: {dto.CheckOut}</p>" +
                   $"<p>Total: {dto.TotalPrice}</p>"
        };

        using var smtp = new SmtpClient();
        await smtp.ConnectAsync(_config["EmailSettings:Host"], int.Parse(_config["EmailSettings:Port"]), false);
        await smtp.AuthenticateAsync(_config["EmailSettings:Username"], _config["EmailSettings:Password"]);
        await smtp.SendAsync(email);
        await smtp.DisconnectAsync(true);
    }

    public async Task SendCancellationEmailAsync(string toEmail, string reservationNumber)
    {
        var email = new MimeMessage();
        email.From.Add(new MailboxAddress(
            _config["EmailSettings:FromName"],
            _config["EmailSettings:FromAddress"]));

        email.To.Add(MailboxAddress.Parse(toEmail));
        email.Subject = "Booking Cancelled";

        email.Body = new TextPart("plain")
        {
            Text = $"Your booking {reservationNumber} has been cancelled."
        };

        using var smtp = new SmtpClient();
        await smtp.ConnectAsync(_config["EmailSettings:Host"], int.Parse(_config["EmailSettings:Port"]), false);
        await smtp.AuthenticateAsync(_config["EmailSettings:Username"], _config["EmailSettings:Password"]);
        await smtp.SendAsync(email);
        await smtp.DisconnectAsync(true);
    }
}