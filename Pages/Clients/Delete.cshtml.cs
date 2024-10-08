using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace cpsbank.Pages.Clients
{
    public class DeleteModel : PageModel
    {
        private readonly string _connectionString;
        private readonly ILogger<DeleteModel> _logger;

        public DeleteModel(IConfiguration configuration, ILogger<DeleteModel> logger)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _logger = logger;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            // This is just for confirmation, not necessary for actual deletion.
            // You can return the client data for user confirmation here if needed.
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    string sql = "DELETE FROM Clients WHERE ClientID == @ClientID";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@ClientID", id);
                        await command.ExecuteNonQueryAsync();
                    }
                }

                TempData["SuccessMessage"] = "Client deleted successfully!";
                return RedirectToPage("/Clients/Index"); // Redirect to the index page after successful deletion
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting client data.");
                TempData["ErrorMessage"] = "An error occurred while deleting the client. Please try again.";
                return RedirectToPage("/Clients/Index"); // Redirect back on error
            }
        }
    }
}
