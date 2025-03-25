using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using ShopEProduction.DTOs;
using ShopEProduction.Models;

namespace ShopEProduction.Controllers
{
    public class UserDashboardController : Controller
    {
        private readonly ShopEproductionContext _context; // Replace with your DbContext

        public UserDashboardController(ShopEproductionContext context)
        {
            _context = context;
        }

        // Action to render the UserDashboard view
        [HttpGet]
        public IActionResult Index() // Default action for /UserDashboard
        {
            var userId = HttpContext.Session.GetString("userId");
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account"); // Adjust to your login action
            }
            return View("UserDashboard"); // Explicitly specify UserDashboard.cshtml
        }

        // Action to fetch dashboard data via AJAX
        [HttpGet]
        public async Task<IActionResult> GetDashboardData(
            string metric = "order",
            string periodType = "week",
            int? year = null,
            int? month = null)
        {
            try
            {
                var userId = HttpContext.Session.GetString("userId");
                if (string.IsNullOrEmpty(userId))
                {
                    return Json(new { success = false, message = "User not logged in." });
                }
                int parsedUserId = int.Parse(userId);

                string sql;
                var parameters = new List<SqlParameter> { new SqlParameter("@UserId", parsedUserId) };

                string selectClause = metric.ToLower() == "order"
                        ? "COUNT(DISTINCT ph.ID) AS OrderCount, CAST(0 AS DECIMAL(18,2)) AS TotalSpent"
                        : "0 AS OrderCount, SUM(CAST(phd.PRICE_AT_PURCHASE AS DECIMAL(18,2))) AS TotalSpent";

                string whereClause = "WHERE ph.USER_ID = @UserId";
                if (year.HasValue)
                {
                    whereClause += " AND DATEPART(YEAR, ph.PURCHASE_DATE) = @Year";
                    parameters.Add(new SqlParameter("@Year", year.Value));
                }
                if (month.HasValue && periodType.ToLower() == "week")
                {
                    whereClause += " AND DATEPART(MONTH, ph.PURCHASE_DATE) = @Month";
                    parameters.Add(new SqlParameter("@Month", month.Value));
                }

                switch (periodType.ToLower())
                {
                    case "year":
                        sql = $@"
                            SELECT 
                                DATEPART(YEAR, ph.PURCHASE_DATE) AS Year,
                                DATEPART(MONTH, ph.PURCHASE_DATE) AS Period,
                                {selectClause}
                            FROM PURCHASE_HISTORY ph
                            LEFT JOIN PURCHASE_HISTORY_DETAILS phd ON ph.ID = phd.HISTORY_ID
                            {whereClause}
                            GROUP BY 
                                DATEPART(YEAR, ph.PURCHASE_DATE),
                                DATEPART(MONTH, ph.PURCHASE_DATE)
                            ORDER BY Year, Period;
                        ";
                        break;

                    case "month":
                        sql = $@"
                            SELECT 
                                DATEPART(YEAR, ph.PURCHASE_DATE) AS Year,
                                DATEPART(WEEK, ph.PURCHASE_DATE) AS Period,
                                {selectClause}
                            FROM PURCHASE_HISTORY ph
                            LEFT JOIN PURCHASE_HISTORY_DETAILS phd ON ph.ID = phd.HISTORY_ID
                            {whereClause}
                            GROUP BY 
                                DATEPART(YEAR, ph.PURCHASE_DATE),
                                DATEPART(WEEK, ph.PURCHASE_DATE)
                            ORDER BY Year, Period;
                        ";
                        break;

                    case "week":
                        sql = $@"
                            SELECT 
                                DATEPART(YEAR, ph.PURCHASE_DATE) AS Year,
                                DATEPART(DAYOFYEAR, ph.PURCHASE_DATE) % 7 + 1 AS Period,
                                {selectClause}
                            FROM PURCHASE_HISTORY ph
                            LEFT JOIN PURCHASE_HISTORY_DETAILS phd ON ph.ID = phd.HISTORY_ID
                            {whereClause}
                            GROUP BY 
                                DATEPART(YEAR, ph.PURCHASE_DATE),
                                DATEPART(DAYOFYEAR, ph.PURCHASE_DATE) % 7 + 1
                            ORDER BY Year, Period;
                        ";
                        break;

                    default:
                        return BadRequest("Invalid period type. Use 'year', 'month', or 'week'.");
                }

                var dashboardData = await _context.Database.SqlQueryRaw<DashboardDataDto>(sql, parameters.ToArray())
                    .ToListAsync();

                return Json(new { success = true, data = dashboardData });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error generating dashboard: " + ex.Message });
            }
        }
    }
}
