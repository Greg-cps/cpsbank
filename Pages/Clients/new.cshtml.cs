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

            // Validate minimum age of 18 years
            if (ClientInfo.DateOfBirth != null)
            {
                int age = DateTime.Now.Year - ClientInfo.DateOfBirth.Value.Year;
                if (ClientInfo.DateOfBirth.Value > DateTime.Now.AddYears(-age)) age--;

                if (age < 18)
                {
                    ModelState.AddModelError(string.Empty, "Client must be at least 18 years old.");
                    return Page();
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Date of birth is required.");
                return Page();
            }

            // Validate that account balance is positive
            if (ClientInfo.AccountBalance <= 0)
            {
                ModelState.AddModelError(string.Empty, "Account balance must be a positive number.");
                return Page();
            }

            // Validate required fields
            if (string.IsNullOrWhiteSpace(ClientInfo.FirstName) ||
                string.IsNullOrWhiteSpace(ClientInfo.LastName) ||
                string.IsNullOrWhiteSpace(ClientInfo.Email) ||
                string.IsNullOrWhiteSpace(ClientInfo.PhoneNumber) ||
                string.IsNullOrWhiteSpace(ClientInfo.AccountNumber))
            {
                ModelState.AddModelError(string.Empty, "All fields are required.");
                return Page();
            }

            try
            {
                string connectionString = "Server=GREG\\SQLEXPRESS01;Database=cpsbank;Trusted_Connection=True;MultipleActiveResultSets=true";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Check for duplicate email or phone number
                    string checkDuplicateSql = "SELECT COUNT(*) FROM Clients WHERE Email = @Email OR PhoneNumber = @PhoneNumber";
                    using (SqlCommand checkCmd = new SqlCommand(checkDuplicateSql, connection))
                    {
                        checkCmd.Parameters.AddWithValue("@Email", ClientInfo.Email);
                        checkCmd.Parameters.AddWithValue("@PhoneNumber", ClientInfo.PhoneNumber);

                        int existingCount = (int)checkCmd.ExecuteScalar();
                        if (existingCount > 0)
                        {
                            ModelState.AddModelError(string.Empty, "A client with the same email or phone number already exists.");
                            return Page();
                        }
                    }

                    // Insert new client record
                    string insertSql = "INSERT INTO Clients (FirstName, LastName, Email, PhoneNumber, AccountNumber, DateOfBirth, AccountBalance) " +
                                       "VALUES (@FirstName, @LastName, @Email, @PhoneNumber, @AccountNumber, @DateOfBirth, @AccountBalance)";
                    using (SqlCommand command = new SqlCommand(insertSql, connection))
                    {
                        command.Parameters.AddWithValue("@FirstName", ClientInfo.FirstName);
                        command.Parameters.AddWithValue("@LastName", ClientInfo.LastName);
                        command.Parameters.AddWithValue("@Email", ClientInfo.Email);
                        command.Parameters.AddWithValue("@PhoneNumber", ClientInfo.PhoneNumber);
                        command.Parameters.AddWithValue("@AccountNumber", ClientInfo.AccountNumber);
                        command.Parameters.AddWithValue("@DateOfBirth", ClientInfo.DateOfBirth);
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
