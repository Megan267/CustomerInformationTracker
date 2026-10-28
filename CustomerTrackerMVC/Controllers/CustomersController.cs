using CustomerTrackerMVC.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net;

namespace CustomerTrackerMVC.Controllers
{
    public class CustomersController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public CustomersController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public async Task<IActionResult> Index(CustomerListQuery query)
        {
            if (!ModelState.IsValid)
            {
                query.Sort = CustomerSortField.Name;
            }

            if (query.Page < 1)
            {
                query.Page = 1;
            }

            var apiClient = _httpClientFactory.CreateClient("ApiClient");
            var customers = new CustomerPage();

            try
            {
                customers = await apiClient.GetFromJsonAsync<CustomerPage>("/api/customer?" + query.ToApiQueryString())
                    ?? new CustomerPage();
                query.Page = customers.Page;
            }
            catch (Exception ex) when (IsApiUnavailable(ex))
            {
                ViewData["ApiError"] = "Unable to load customers. Make sure the API is running.";
            }

            var model = new CustomerListViewModel(query, customers);

            if (Request.Headers.XRequestedWith == "XMLHttpRequest")
            {
                return PartialView("_CustomerResults", model);
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Manage(int? id, string? returnUrl)
        {
            ViewData["ReturnUrl"] = BackToList(returnUrl);

            if (id == null)
            {
                return View(new Customer { Address = new Address { Country = "South Africa" } });
            }

            var apiClient = _httpClientFactory.CreateClient("ApiClient");
            HttpResponseMessage response;
            try
            {
                response = await apiClient.GetAsync($"/api/customer/{id}");
            }
            catch (Exception ex) when (IsApiUnavailable(ex))
            {
                TempData["ErrorMessage"] = ApiUnavailableMessage;
                return Redirect(BackToList(returnUrl));
            }

            if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = "Customer not found. It may have been deleted.";
                return Redirect(BackToList(returnUrl));
            }

            var customer = await response.Content.ReadFromJsonAsync<Customer>();
            return View(customer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Manage(Customer customer, string? returnUrl)
        {
            ViewData["ReturnUrl"] = BackToList(returnUrl);

            if (!ModelState.IsValid)
            {
                return View(customer);
            }

            var apiClient = _httpClientFactory.CreateClient("ApiClient");
            var dto = new
            {
                customer.Name,
                Address = new
                {
                    customer.Address.Street,
                    customer.Address.City,
                    customer.Address.Province,
                    customer.Address.PostalCode,
                    customer.Address.Country
                },
                customer.TelephoneNumber,
                customer.ContactPersonName,
                customer.ContactPersonEmail,
                customer.VatNumber
            };

            bool isNew = customer.CustomerId == 0;
            HttpResponseMessage response;
            try
            {
                response = isNew
                    ? await apiClient.PostAsJsonAsync("/api/customer", dto)
                    : await apiClient.PutAsJsonAsync($"/api/customer/{customer.CustomerId}", dto);
            }
            catch (Exception ex) when (IsApiUnavailable(ex))
            {
                ModelState.AddModelError(string.Empty, ApiUnavailableMessage);
                return View(customer);
            }

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = isNew
                    ? $"{customer.Name} was added successfully."
                    : $"{customer.Name} was updated successfully.";
                return Redirect(BackToList(returnUrl));
            }

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                TempData["ErrorMessage"] = "Customer not found. It may have been deleted.";
                return Redirect(BackToList(returnUrl));
            }

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                var problem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
                if (problem != null)
                {
                    foreach (var error in problem.Errors)
                    {
                        foreach (var message in error.Value)
                        {
                            ModelState.AddModelError(error.Key, message);
                        }
                    }
                }
            }

            if (ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, isNew ? "Failed to add customer." : "Failed to update customer.");
            }

            return View(customer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, string? customerName, string? returnUrl, bool confirmed = false)
        {
            if (!confirmed)
            {
                return RedirectToAction(nameof(ConfirmDelete), new { id, returnUrl });
            }

            var apiClient = _httpClientFactory.CreateClient("ApiClient");
            HttpResponseMessage response;
            try
            {
                response = await apiClient.DeleteAsync($"/api/customer/{id}");
            }
            catch (Exception ex) when (IsApiUnavailable(ex))
            {
                TempData["ErrorMessage"] = ApiUnavailableMessage;
                return Redirect(BackToList(returnUrl));
            }

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = $"{customerName} was deleted.";
                TempData["UndoCustomerId"] = id;
                TempData["UndoCustomerName"] = customerName;
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to delete customer. It may already have been deleted.";
            }

            return Redirect(BackToList(returnUrl));
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmDelete(int id, string? returnUrl)
        {
            ViewData["ReturnUrl"] = BackToList(returnUrl);

            var apiClient = _httpClientFactory.CreateClient("ApiClient");
            HttpResponseMessage response;
            try
            {
                response = await apiClient.GetAsync($"/api/customer/{id}");
            }
            catch (Exception ex) when (IsApiUnavailable(ex))
            {
                TempData["ErrorMessage"] = ApiUnavailableMessage;
                return Redirect(BackToList(returnUrl));
            }

            if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = "Customer not found. It may already have been deleted.";
                return Redirect(BackToList(returnUrl));
            }

            var customer = await response.Content.ReadFromJsonAsync<Customer>();
            return View(customer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reinstate(int id, string? customerName, string? returnUrl)
        {
            var apiClient = _httpClientFactory.CreateClient("ApiClient");
            HttpResponseMessage response;
            try
            {
                response = await apiClient.PostAsync($"/api/customer/{id}/reinstate", null);
            }
            catch (Exception ex) when (IsApiUnavailable(ex))
            {
                TempData["ErrorMessage"] = ApiUnavailableMessage;
                return Redirect(BackToList(returnUrl));
            }

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = $"{customerName} was restored.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to restore customer.";
            }

            return Redirect(BackToList(returnUrl));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private const string ApiUnavailableMessage = "The customer service could not be reached. Make sure the API is running and try again.";

        private static bool IsApiUnavailable(Exception ex)
        {
            return ex is HttpRequestException or TaskCanceledException or System.Text.Json.JsonException;
        }

        private string BackToList(string? returnUrl)
        {
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return returnUrl;
            }

            return Url.Action("Index") ?? "/";
        }
    }
}