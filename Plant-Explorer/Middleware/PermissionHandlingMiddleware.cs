using Plant_Explorer.Contract.Repositories.Interface;
using Plant_Explorer.Services.Infrastructure;

namespace Plant_Explorer.Middleware
{
    /// <summary>
    /// 
    /// </summary>
    public class PermissionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<PermissionHandlingMiddleware> _logger;
        private readonly Dictionary<string, Dictionary<string, List<string>>> _rolePermissions;
        private readonly Dictionary<string, List<string>> _excludedUris;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="next"></param>
        /// <param name="logger"></param>
        public PermissionHandlingMiddleware(RequestDelegate next, ILogger<PermissionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
            _excludedUris = new()
            {
                    { "GET", new List<string>() { "/api/plantcharacteristic", "/api/applicationcategory", "/api/characteristiccategory", "/api/Plant", "/api/users" } },

                    { "POST", new List<string>() { "/api/plantcharacteristic", "/api/applicationcategory", "/api/characteristiccategory", "/api/Plant" } },

                    { "PUT", new List<string>() { "/api/plantcharacteristic", "/api/applicationcategory", "/api/characteristiccategory", "/api/Plant" } },

                    { "DELETE", new List<string>() { "/api/plantcharacteristic", "/api/applicationcategory", "/api/characteristiccategory", "/api/Plant" } },
                //    { "POST", new List<string>() { "/api/auth/login", "/api/auth/active-account", "/api/auth/forgot-password", "/api/auth/check-valid-code", "/api/auth/reset-password", "/api/auth/login-google", "/api/auth/create-customer" } },
                //    { "PUT", new List<string>() {  } },
                //    { "PATCH", new List<string>() {  } },
                //    { "DELETE", new List<string>() {  } },
                //    { "CONTROLLER", new List<string>() { "/api/foods", "/api/categoriesfoods", "/api/auth", "/api/menus" } }
            };
            //    _rolePermissions = new()
            //{
            //    {
            //        "Staff", new Dictionary<string, List<string>>()
            //        {
            //            { "GET", new List<string>() { "/api/categoriesfoods/get", "/api/auth/info", "/api/warehouse/all", "/api/warehouse/{wareHouseID}", "/api/orders" } },
            //            { "POST", new List<string>() { "" } },
            //            { "PUT", new List<string>() { "/api/payment/in-place", "/api/orders/order-status-shift" } },
            //            { "PATCH", new List<string>() {  } },
            //            { "DELETE", new List<string>() {  } },
            //            { "CONTROLLER", new List<string>() {  } }
            //        }
            //    },
            //    {
            //        "Customer", new Dictionary<string, List<string>>()
            //        {
            //            { "GET", new List<string>() { "/api/auth/info", "/api/menus", "/api/vouchers/customer", "/api/orders/customer", "/api/orders/cart", "/api/branchs" } },
            //            { "POST", new List<string>() { "/api/auth/change-password", "/api/orders" } },
            //            { "PUT", new List<string>() { "/api/auth/customer", "/api/orders/adjust-item" } },
            //            { "PATCH", new List<string>() {  } },
            //            { "DELETE", new List<string>() { "/api/orders", "/api/orders/delete-item" } },
            //            { "CONTROLLER", new List<string>() { "/api/payment" } }
            //        }
            //    },
            //    {
            //        "Owner", new Dictionary<string, List<string>>()
            //        {
            //            { "GET", new List<string>() { "/api/auth/info", "/api/auth/list-staff-owner-customer" } },
            //            { "POST", new List<string>() { "/api/auth/change-password" } },
            //            { "PUT", new List<string>() {  } },
            //            { "PATCH", new List<string>() {  } },
            //            { "DELETE", new List<string>() { "/api/auth/staff" } },
            //            { "CONTROLLER", new List<string>() {  } }
            //        }
            //    },
            //};

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="unitOfWork"></param>
        public async Task Invoke(HttpContext context, IUnitOfWork unitOfWork)
        {
            if (HasPermission(context))
            {
                await _next(context);
            }
            else
            {
                await Authentication.HandleForbiddenRequest(context);
            }
        }

        private bool HasPermission(HttpContext context)
        {
            string requestUri = context.Request.Path.Value!;
            string requestMethod = context.Request.Method;

            // Skip further checks for non-API endpoints
            if (!requestUri.StartsWith("/api/"))
                return true;

            // 1. Check excluded URIs accessible to everyone regardless of role
            if (_excludedUris.TryGetValue(requestMethod, out var allowedUris))
            {
                // If the URI is part of the excluded URIs for the method, allow access
                if (allowedUris.Any(uri => requestUri.StartsWith(uri, StringComparison.OrdinalIgnoreCase)))
                    return true;
            }

            // 2. Check controller-wide access for excluded URIs (for any method)
            if (_excludedUris.TryGetValue("CONTROLLER", out var controllerUris))
            {
                // Check if the URI starts with any of the controller base URIs
                if (controllerUris.Any(controllerUri => requestUri.StartsWith(controllerUri, StringComparison.OrdinalIgnoreCase)))
                    return true;
            }


            try
            {
                // 3. Get the user's role from the context
                string userRole = Authentication.GetUserRoleFromHttpContext(context);

                // If the user role is "admin", allow access to all endpoints
                if (userRole == "admin")
                    return true;

                // 4. Check role-based permissions for the specific API
                if (_rolePermissions.TryGetValue(userRole, out var methodPermissions))
                {
                    // Check if the method (GET, POST, etc.) exists for the user's role
                    if (methodPermissions.TryGetValue(requestMethod, out var allowedUrisForRole))
                    {
                        // Check if the requestUri starts with any of the allowed URIs
                        if (allowedUrisForRole.Any(uri => requestUri.StartsWith(uri, StringComparison.OrdinalIgnoreCase)))
                            return true;
                    }

                    // 5. Check if the role has controller-wide access
                    if (methodPermissions.TryGetValue("CONTROLLER", out var controllerUrisForRole))
                    {
                        if (controllerUrisForRole.Any(controllerUri => requestUri.StartsWith(controllerUri, StringComparison.OrdinalIgnoreCase)))
                            return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while checking permissions");
                return false;
            }
        }
    }
}
