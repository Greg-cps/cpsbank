using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace cpsbank.Pages.Clients
{
    public class IndexModel : PageModel
    {
        private readonly string _connectionString;
        private readonly ILogger<IndexModel> _logger;

        public List<ClientInfo> ListClients { get; set; } = new List<ClientInfo>();
        public int TotalClients { get; set; }
        public int CurrentPage { get; set; } = 1;
        public const int PageSize = 10; // Number of clients per page
        public string SearchTerm { get; set; } = string.Empty; // Property to hold the search term

        public IndexModel(IConfiguration configuration, ILogger<IndexModel> logger)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _logger = logger;
        }

        public async Task OnGetAsync(int currentPage = 1, string searchTerm = "")
        {
            CurrentPage = currentPage;
            SearchTerm = searchTerm;

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    // Get the total number of clients for pagination, filtering by searchTerm
                    string countSql = "SELECT COUNT(*) FROM Clients WHERE (@SearchTerm = '' OR FirstName LIKE @SearchTerm OR LastName LIKE @SearchTerm OR Email LIKE @SearchTerm)";
                    using (SqlCommand countCommand = new SqlCommand(countSql, connection))
                    {
                        countCommand.Parameters.AddWithValue("@SearchTerm", $"%{searchTerm}%");
                        TotalClients = (int)await countCommand.ExecuteScalarAsync();
                    }

                    // Get the paginated list of clients based on the searchTerm
                    string sql = @"
                        SELECT ClientID, FirstName, LastName, Email, PhoneNumber, DateOfBirth, AccountNumber, AccountBalance
                        FROM Clients
                        WHERE (@SearchTerm = '' OR FirstName LIKE @SearchTerm OR LastName LIKE @SearchTerm OR Email LIKE @SearchTerm)
                        ORDER BY ClientID
                        OFFSET @Offset ROWS
                        FETCH NEXT @PageSize ROWS ONLY";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@SearchTerm", $"%{searchTerm}%");
                        command.Parameters.AddWithValue("@Offset", (CurrentPage - 1) * PageSize);
                        command.Parameters.AddWithValue("@PageSize", PageSize);

                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var clientInfo = new ClientInfo
                                {
                                    ClientID = reader.GetInt32(0),
                                    FirstName = reader.IsDBNull(1) ? null : reader.GetString(1),
                                    LastName = reader.IsDBNull(2) ? null : reader.GetString(2),
                                    Email = reader.IsDBNull(3) ? null : reader.GetString(3),
                                    PhoneNumber = reader.IsDBNull(4) ? null : reader.GetString(4),
                                    DateOfBirth = reader.IsDBNull(5) ? (DateTime?)null : reader.GetDateTime(5),
                                    AccountNumber = reader.IsDBNull(6) ? null : reader.GetString(6),
                                    AccountBalance = reader.IsDBNull(7) ? 0 : reader.GetDecimal(7)
                                };

                                ListClients.Add(clientInfo);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving client data.");
            }
        }
    }

    public class ClientInfo
    {
        public int ClientID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string AccountNumber { get; set; }
        public decimal AccountBalance { get; set; }
    }
}
