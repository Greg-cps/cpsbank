using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace cpsbank.Pages.Clients
{
    public class CreateModel : PageModel
    {
        private readonly string _connectionString;
        private readonly ILogger<CreateModel> _logger;

        [BindProperty]
        public ClientInfo NewClient { get; set; } = new ClientInfo();

        public CreateModel(IConfiguration configuration, ILogger<CreateModel> logger)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _logger = logger;
        }

        public void OnGet()
        {
            // This method is intentionally left blank for GET requests.
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    string sql = @"
                        INSERT INTO Clients (FirstName, LastName, Email, PhoneNumber, DateOfBirth, AccountNumber, AccountBalance)
                        VALUES (@FirstName, @LastName, @Email, @PhoneNumber, @DateOfBirth, @AccountNumber, @AccountBalance)";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@FirstName", NewClient.FirstName);
                        command.Parameters.AddWithValue("@LastName", NewClient.LastName);
                        command.Parameters.AddWithValue("@Email", NewClient.Email);
                        command.Parameters.AddWithValue("@PhoneNumber", NewClient.PhoneNumber);
                        command.Parameters.AddWithValue("@DateOfBirth", NewClient.DateOfBirth);
                        command.Parameters.AddWithValue("@AccountNumber", NewClient.AccountNumber);
                        command.Parameters.AddWithValue("@AccountBalance", NewClient.AccountBalance);

                        await command.ExecuteNonQueryAsync();
                    }
                }

                // Redirect to the index page after successful insertion
                return RedirectToPage("/Clients/Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating a new client.");
                ModelState.AddModelError("", "Unable to create client. Please try again later.");
                return Page();
            }
        }
    }

    public class ClientInfo
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string AccountNumber { get; set; }
        public decimal AccountBalance { get; set; }
    }
}
