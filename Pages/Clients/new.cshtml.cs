using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Data.SqlClient;

namespace cpsbank.Pages.Clients
{
    public class NewModel : PageModel
    {
        public ClientInfo ClientInfo { get; set; } = new ClientInfo();
        public string ErrorMessage { get; set; } = "";
        public string SuccessMessage { get; set; } = "";

        private readonly ILogger<NewModel> _logger;

        public NewModel(ILogger<NewModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
            // Page Load logic (if any) goes here
        }

        public IActionResult OnPost()
        {
            // Assign values from the form to ClientInfo properties
            ClientInfo.FirstName = Request.Form["FirstName"];
            ClientInfo.LastName = Request.Form["LastName"];
            ClientInfo.Email = Request.Form["Email"];
            ClientInfo.PhoneNumber = Request.Form["PhoneNumber"];
            ClientInfo.AccountNumber = Request.Form["AccountNumber"];
            ClientInfo.DateOfBirth = DateTime.TryParse(Request.Form["DateOfBirth"], out DateTime dob) ? dob : (DateTime?)null;
            ClientInfo.AccountBalance = decimal.TryParse(Request.Form["AccountBalance"], out decimal balance) ? balance : 0;

            // Validate required fields
            if (string.IsNullOrWhiteSpace(ClientInfo.FirstName) ||
                string.IsNullOrWhiteSpace(ClientInfo.LastName) ||
                string.IsNullOrWhiteSpace(ClientInfo.Email) ||
                string.IsNullOrWhiteSpace(ClientInfo.PhoneNumber) ||
                string.IsNullOrWhiteSpace(ClientInfo.AccountNumber) ||
                ClientInfo.DateOfBirth == null ||
                ClientInfo.AccountBalance < 0)
            {
                ModelState.AddModelError(string.Empty, "All fields are required, and account balance cannot be negative.");
                return Page();
            }

            try
            {
                string connectionString = "Server=GREG\\SQLEXPRESS01;Database=cpsbank;Trusted_Connection=True;MultipleActiveResultSets=true";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "INSERT INTO Clients (FirstName, LastName, Email, PhoneNumber, AccountNumber, DateOfBirth, AccountBalance) " +
                                 "VALUES (@FirstName, @LastName, @Email, @PhoneNumber, @AccountNumber, @DateOfBirth, @AccountBalance)";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@FirstName", ClientInfo.FirstName);
                        command.Parameters.AddWithValue("@LastName", ClientInfo.LastName);
                        command.Parameters.AddWithValue("@Email", ClientInfo.Email);
                        command.Parameters.AddWithValue("@PhoneNumber", ClientInfo.PhoneNumber);
                        command.Parameters.AddWithValue("@AccountNumber", ClientInfo.AccountNumber);
                        command.Parameters.AddWithValue("@AccountBalance", ClientInfo.AccountBalance);

                        command.ExecuteNonQuery();
                    }
                }

                TempData["SuccessMessage"] = "Client created successfully!";
                return RedirectToPage(); // Reload the form
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while saving client data.");
                ModelState.AddModelError(string.Empty, "An error occurred while saving client data. Please try again.");
                return Page();
            }
        }
    }
}
