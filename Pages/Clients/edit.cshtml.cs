using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Data.SqlClient;

namespace cpsbank.Pages.Clients
{
    public class EditModel : PageModel
    {
        public ClientInfo ClientInfo { get; set; } = new ClientInfo();
        public string ErrorMessage { get; set; }
        public string SuccessMessage { get; set; }

        private readonly ILogger<EditModel> _logger;

        public EditModel(ILogger<EditModel> logger)
        {
            _logger = logger;
        }

        public void OnGet(int? id)
        {
            if (id == null)
            {
                ErrorMessage = "Client ID is required.";
                return;
            }

            try
            {
                string connectionString = "Server=GREG\\SQLEXPRESS01;Database=cpsbank;Trusted_Connection=True;MultipleActiveResultSets=true";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "SELECT * FROM Clients WHERE ClientID = @id";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                ClientInfo.ClientID = reader.GetInt32(0);
                                ClientInfo.FirstName = reader.GetString(1);
                                ClientInfo.LastName = reader.GetString(2);
                                ClientInfo.Email = reader.GetString(3);
                                ClientInfo.PhoneNumber = reader.GetString(4);
                                ClientInfo.AccountNumber = reader.GetString(5);
                                ClientInfo.DateOfBirth = reader.GetDateTime(6);
                                ClientInfo.AccountBalance = reader.GetDecimal(7);
                            }
                            else
                            {
                                ErrorMessage = "Client not found.";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving client data.");
                ErrorMessage = "An error occurred while retrieving client data.";
            }
        }

        public IActionResult OnPost()
        {
            // Update logic here (similar to your existing NewModel code)
            return RedirectToPage("/Clients/Index");
        }
    }
}
